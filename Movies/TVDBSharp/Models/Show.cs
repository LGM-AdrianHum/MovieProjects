// Author: Adrian Hum
// Project: TVDBSharp/Show.cs
// 
// Created : 2015-03-06  16:06 
// Modified: 2015-04-10 16:05)

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TVDBSharp.Models.Enums;

namespace TVDBSharp.Models {
    /// <summary>
    ///     Entity describing a show.
    /// </summary>
    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public partial class Show {
        /// <summary>
        ///     Unique identifier used by IMDb.
        /// </summary>
        public string ImdbId { get; set; }

        /// <summary>
        ///     Unique identifier used by TvDb and TVDBSharp.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     List of all actors in the show.
        /// </summary>
        public List<string> Actors { get; set; }

        /// <summary>
        ///     Day of the week when the show airs.
        /// </summary>
        public Frequency? AirDay { get; set; }

        /// <summary>
        ///     Time of the day when the show airs.
        /// </summary>
        public TimeSpan? AirTime { get; set; }

        /// <summary>
        ///     Rating of the content provided by an official organ.
        /// </summary>
        public ContentRating ContentRating { get; set; }

        /// <summary>
        ///     The date the show aired for the first time.
        /// </summary>
        public DateTime? FirstAired { get; set; }

        /// <summary>
        ///     A list of genres the show is associated with.
        /// </summary>
        public List<string> Genres { get; set; }

        /// <summary>
        ///     Main language of the show.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        ///     Network that broadcasts the show.
        /// </summary>
        public string Network { get; set; }

        /// <summary>
        ///     A short overview of the show.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Average rating as shown on IMDb.
        /// </summary>
        public double? Rating { get; set; }

        /// <summary>
        ///     Amount of votes cast.
        /// </summary>
        public int RatingCount { get; set; }

        /// <summary>
        ///     Let me know if you find out what this is.
        /// </summary>
        public int? Runtime { get; set; }

        /// <summary>
        ///     Name of the show.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Current status of the show.
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        ///     Link to the banner image.
        /// </summary>
        public XmlUri Banner { get; set; }

        /// <summary>
        ///     Link to a fanart image.
        /// </summary>
        public XmlUri Fanart { get; set; }

        /// <summary>
        ///     Timestamp of the latest update.
        /// </summary>
        public long? LastUpdated { get; set; }

        /// <summary>
        ///     Let me know if you find out what this is.
        /// </summary>
        public XmlUri Poster { get; set; }

        /// <summary>
        ///     No clue
        /// </summary>
        public string Zap2ItID { get; set; }

        /// <summary>
        ///     A list of all episodes associated with this show.
        /// </summary>
        public List<Episode> Episodes { get; set; }
    }
}