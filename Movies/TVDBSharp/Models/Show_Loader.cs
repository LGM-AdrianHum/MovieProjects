// Author: Hum, Adrian
// Project: Movies/TVDBSharp/Show_Loader.cs
//
// Created  Date: 2015-10-09  2:40 PM
// Modified Date: 2015-10-09  2:47 PM

#region Using Directives

using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using TVDBSharp.Models.Enums;

#endregion

namespace TVDBSharp.Models
{
    public partial class Show
    {
        public Episode NextEpisode
        {
            get
            {
                if (Episodes == null) return null;
                if (Status == Status.Ended) return null;
                var n = Episodes.FirstOrDefault(x => x.FirstAired.HasValue && x.FirstAired.Value >= DateTime.Now);
                return n;
            }
        }

        public string NextAried
        {
            get
            {
                if (NextEpisode == null) return "<no details>";
                return string.Format("{0} - {1} ({2:dd-MMM-yyyy})", NextEpisode.SeasonEpisode, NextEpisode.Title, NextEpisode.FirstAired);
            }
        }

        /// <exception cref="FileNotFoundException">The file cannot be found. </exception>
        public static Show Load(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return null;

            var serializer = new XmlSerializer(typeof (Show));

            try {
                var reader = new StreamReader(filename);
                var shw = (Show) serializer.Deserialize(reader);
                reader.Close();
                return shw;
            }
            catch (DirectoryNotFoundException directoryNotFoundException) {
                return null;
            }
        }

        public void Save(string filename)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlSerializer serializer = new XmlSerializer(typeof(Show));
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, this);
                stream.Position = 0;
                xmlDocument.Load(stream);
                xmlDocument.Save(filename);
                stream.Close();
            }
        }

    }
}