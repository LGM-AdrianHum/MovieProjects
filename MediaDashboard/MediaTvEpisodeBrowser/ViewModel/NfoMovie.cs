using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using TMDbLib.Objects.Movies;

namespace MediaTvEpisodeBrowser.ViewModel
{
    /// <remarks />
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory(@"code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", ElementName = "movie", IsNullable = false)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class NfoMovie
    {
        public NfoMovie()
        {
        }




        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "title")]
        public string Title { get; set; }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "originaltitle")]
        public string OriginalTitle { get; set; }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "sorttitle")]
        public string SortTitle { get; set; }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "set")]
        public string CollectionName { get; set; }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "rating")]
        public string Rating { get; set; }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "year")]
        public string Year { get; set; }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "top250")]
        public string Top250 { get; set; }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "votes")]
        public string Votes { get; set; }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "outline")]
        public string Outline { get; set; }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "plot")]
        public string Plot { get; set; }
        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "tagline")]
        public string TagLine { get; set; }
        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "runtime")]
        public string RunTime { get; set; }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "thumb")]
        public string Thumb { get; set; }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "mpaa")]
        public string Mpaa { get; set; }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "playcount")]
        public string PlayCount { get; set; }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "watched")]
        public string Watched { get; set; }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "id")]
        public string id { get; set; }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "filenameandpath")]
        public string FilenameAndPath { get; set; }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "trailer")]
        public string Trailer { get; set; }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "genre")]
        public string Genre { get; set; }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "Credits")]
        public string Credits { get; set; }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "director")]
        public string Director { get; set; }



        [XmlElement("tmdbid", Form = XmlSchemaForm.Unqualified)]
        public int TmDbId { get; set; }

        public void LoadFrom(Movie mvie, string filename = "")
        {
            TmDbId = mvie.Id;
            Title = mvie.Title;
            OriginalTitle = mvie.OriginalTitle;
            SortTitle = mvie.Title;
            CollectionName = mvie.BelongsToCollection?.FirstOrDefault()?.Name;
            Rating = mvie.VoteAverage.ToString("0.00");
            Year = mvie.ReleaseDate?.Year.ToString();
            Top250 = "0";
            Votes = mvie.VoteCount.ToString();
            Outline = mvie.Overview;
            Plot = "";
            TagLine = mvie.Tagline;
            RunTime = $"{mvie.Runtime} min";
            Thumb = $"http://image.tmdb.org/t/p/w154/{mvie.PosterPath}";
            Mpaa = "n/a";
            PlayCount = "0";
            Watched = "false";
            id = mvie.ImdbId;
            FilenameAndPath = filename;
            Trailer = "";

            Credits = "";

            if (mvie.Genres != null && mvie.Genres.Any()) Genre = string.Join(", ", mvie.Genres.Select(x => x));
        }

        public static NfoMovie LoadFile(string fn)
        {
            var sdata = File.ReadAllText(fn);
            if (string.IsNullOrEmpty(sdata)) return null;
            if (sdata.Contains("MovieNfoDataSet"))
            {
                var r = XmlHelper.Deserialize<MovieNfoDataSet>(sdata);
                if (r != null && r.Items.Any())
                {
                    return r.Items.FirstOrDefault();
                }
            }
            var r2 = XmlHelper.Deserialize<NfoMovie>(sdata);
            return r2;
        }

        public void Save(string fn)
        {
            var m = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };

            var sdata = XmlHelper.Serialize(this, m);
            File.WriteAllText(fn, sdata);
        }
    }
}