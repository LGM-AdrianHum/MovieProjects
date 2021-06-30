using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Text;

namespace ReadKodiDatabases
{
    public class TransferData
    {
        private static SQLiteConnection _mSqlitecon;
        private static SqlConnection _mSqlcon;

        private static List<KeyValuePair<string, string>> GetSqLiteSchema(string tablename)
        {
            using (var cmd = new SQLiteCommand("PRAGMA table_info(" + tablename + ");", _mSqlitecon))
            {
                var table = new DataTable();

                try
                {
                    var adp = new SQLiteDataAdapter(cmd);
                    adp.Fill(table);
                    var res = new List<KeyValuePair<string, string>>();
                    for (var i = 0; i < table.Rows.Count; i++)
                    {
                        var key = table.Rows[i]["name"].ToString();
                        var value = table.Rows[i]["type"].ToString();
                        var kvp = new KeyValuePair<string, string>(key, value);

                        res.Add(kvp);
                    }
                    return res;
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            return null;
        }

        private static void Transfer(string tablename, List<KeyValuePair<string, string>> schema)
        {
            using (var cmd = new SQLiteCommand("select * from " + tablename, _mSqlitecon))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var sql = new StringBuilder();
                        sql.Append("insert into " + tablename + " (");
                        var first = true;
                        foreach (var column in schema)
                        {
                            if (first)
                                first = false;
                            else
                                sql.Append(",");
                            sql.Append("[" + column.Key + "]");
                        }
                        sql.Append(") Values(");
                        first = true;
                        foreach (var column in schema)
                        {
                            if (first)
                                first = false;
                            else
                                sql.Append(",");
                            sql.Append("@");
                            sql.Append(column.Key);
                        }
                        sql.Append(");");
                        try
                        {
                            using (var sqlcmd = new SqlCommand(sql.ToString(), _mSqlcon))
                            {
                                foreach (var column in schema)
                                {
                                    sqlcmd.Parameters.AddWithValue("@" + column.Key, reader[column.Key]);
                                }
                                var count = sqlcmd.ExecuteNonQuery();
                                if (count == 0)
                                    throw new Exception("Unable to insert row!");
                            }
                        }
                        catch (Exception exception)
                        {
                            var message = exception.Message;
                            var idx = message.IndexOf("Violation of PRIMARY KEY", StringComparison.Ordinal);
                            if (idx < 0)
                                throw;
                        }
                    }
                }
            }
        }

        private static bool SqlTableExists(string tablename)
        {
            using (var cmd = new SqlCommand("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '" + tablename + "'", _mSqlcon))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                        return true;
                }
            }
            return false;
        }


        private static string ReplaceCaseInsensitive(string str, string oldValue, string newValue)
        {
            var retval = str;
            // find the first occurence of oldValue
            var pos = retval.IndexOf(oldValue, StringComparison.InvariantCultureIgnoreCase);

            while (pos > -1)
            {
                // remove oldValue from the string
                retval = retval.Remove(pos, oldValue.Length);

                // insert newValue in its place
                retval = retval.Insert(pos, newValue);

                // check if oldValue is found further down
                var prevPos = pos + newValue.Length;
                pos = retval.IndexOf(oldValue, prevPos, StringComparison.InvariantCultureIgnoreCase);
            }

            return retval;
        }

        public void DoWork()
        {
            // Connect to SQLite and SQL Server database
            //Data Source=(local);Initial Catalog=TvMovieData;Integrated Security=True
            _mSqlitecon = new SQLiteConnection(@"Data Source=C:\Users\Gen\Desktop\MyVideos99.db;");
            _mSqlitecon.Open();
            _mSqlcon = new SqlConnection("Data Source=localhost;Initial Catalog=SqliteImport;Integrated Security=True;MultipleActiveResultSets=True");
            _mSqlcon.Open();
            var sql = "SELECT * FROM sqlite_master WHERE type='table'";
            var command = new SQLiteCommand(sql, _mSqlitecon);
            var reader = command.ExecuteReader();
            // ReSharper disable once UnusedVariable
            var tables = new List<string>();
            // Loop through all tables
            while (reader.Read())
            {
                var tablename = reader["name"].ToString();
                var sqlstr = reader["sql"].ToString();
                
                if(sqlstr.ToUpper().Contains("DOUBLE")) Console.WriteLine("STOPPER");
                // Only create and import table if it does not exist
                if (!SqlTableExists(tablename))
                {
                    Console.WriteLine(@"Creating table: " + tablename);

                    // Vi retter SQLite SQL til M$ SQL Server
                    sqlstr = ReplaceCaseInsensitive(sqlstr, " BOOL", " bit");
                    sqlstr = ReplaceCaseInsensitive(sqlstr, " BLOB", " varbinary(max)"); // Note, maks 2 GB i varbinary(max) kolonner
                    sqlstr = ReplaceCaseInsensitive(sqlstr, " VARCHAR", " nvarchar");
                    sqlstr = ReplaceCaseInsensitive(sqlstr, " nvarchar,", " nvarchar(max),");
                    sqlstr = ReplaceCaseInsensitive(sqlstr, " nvarchar\r", " nvarchar(max)\r"); // Case windiows
                    sqlstr = ReplaceCaseInsensitive(sqlstr, " nvarchar\n", " nvarchar(max)\n"); // Case linux
                    sqlstr = ReplaceCaseInsensitive(sqlstr, " INTEGER", " int");
                    sqlstr = ReplaceCaseInsensitive(sqlstr, " DOUBLE", " float");
                    sqlstr = ReplaceCaseInsensitive(sqlstr, " TEXT", " nvarchar(max)");
                    var sqlcmd = new SqlCommand(sqlstr, _mSqlcon);
                    sqlcmd.ExecuteNonQuery();
                    sqlcmd.Dispose();
                    var columns = GetSqLiteSchema(tablename);
                    // Copy all rows to MS SQL Server
                    Transfer(tablename, columns);
                }
                else
                    Console.WriteLine(@"Table already exists: " + tablename);
            }
        }
    }
}