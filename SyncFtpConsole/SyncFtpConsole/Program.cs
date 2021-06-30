using System;
using NLog;

namespace SyncFtpConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("\r\n");
            var bg = Console.BackgroundColor;
            Console.BackgroundColor=ConsoleColor.DarkBlue;
            Console.WriteLine(@"                                                      ");
            Console.WriteLine(@"   ________________________________________________   ");
            Console.WriteLine(@"  |   ____                     _____ _____ ____    |  ");
            Console.WriteLine(@"  |  / ___| _   _ _ __   ___  |  ___|_   _|  _ \   |  ");
            Console.WriteLine(@"  |  \___ \| | | | '_ \ / __| | |_    | | | |_) |  |  ");
            Console.WriteLine(@"  |   ___) | |_| | | | | (__  |  _|   | | |  __/   |  ");
            Console.WriteLine(@"  |  |____/ \__, |_| |_|\___| |_|_    |_| |_|      |  ");
            Console.WriteLine(@"  |    ____                      _                 |  ");
            Console.WriteLine(@"  |   / ___|___  _ __  ___  ___ | | ___            |  ");
            Console.WriteLine(@"  |  | |   / _ \| '_ \/ __|/ _ \| |/ _ \           |  ");
            Console.WriteLine(@"  |  | |__| (_) | | | \__ \ (_) | |  __/           |  ");
            Console.WriteLine(@"  |   \____\___/|_| |_|___/\___/|_|\___|           |  ");
            Console.WriteLine(@"  |________________________________________________|  ");
            Console.WriteLine(@"                                                      ");
            Console.WriteLine("\r\n\r\n");
            Console.BackgroundColor = bg;
            
            Console.Title = "SyncFTP Console : " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss");

            var logger = LogManager.GetLogger("SyncFTPConsole");

            logger.Info($"Total Execution Time {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}");
            var now = DateTime.Now;
            Scanners.Begin();
            var then = DateTime.Now.Subtract(now).TotalSeconds;
            logger.Info($"Total Execution Time {then}");
            Console.WriteLine($"Total Execution Time {then}");
            try
            {
                Console.Write("Process Complete...");
                Reader.ReadLine(3000);
            }
            catch (TimeoutException)
            {
                Console.WriteLine("   Ok.");
            }
            Console.Clear();
        }
    }
}