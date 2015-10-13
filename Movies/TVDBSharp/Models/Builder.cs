// Author: Adrian Hum
// Project: TVDBSharp/Builder.cs
// 
// Created : 2015-03-06  16:06 
// Modified: 2015-04-10 16:05)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using TVDBSharp.Models.DAO;
using TVDBSharp.Models.Enums;
using TVDBSharp.Utilities;

namespace TVDBSharp.Models {
    /// <summary>
    ///     Provides builder classes for complex entities.
    /// </summary>
    public class Builder {
        private const string UriPrefix = "http://thetvdb.com/banners/";
        private readonly IDataProvider _dataProvider;

        /// <summary>
        ///     Initializes a new Builder object with the given <see cref="IDataProvider" />.
        /// </summary>
        /// <param name="dataProvider">The DataProvider used to retrieve XML responses.</param>
        public Builder(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        /// <summary>
        ///     Builds a show object from the given show ID.
        /// </summary>
        /// <param name="showId">ID of the show to serialize into a <see cref="Show" /> object.</param>
        /// <returns>Returns the Show object.</returns>
        public Show BuildShow(int showId)
        {
            var builder = new ShowBuilder(_dataProvider.GetShow(showId));
            return builder.GetResult();
        }

        public Episode BuildEpisode(int episodeId, string lang)
        {
            var builder = new EpisodeBuilder(_dataProvider.GetEpisode(episodeId, lang).Descendants("Episode").First());
            return builder.GetResult();
        }

        public Updates BuildUpdates(Interval interval)
        {
            var builder = new UpdatesBuilder(_dataProvider.GetUpdates(interval));
            return builder.GetResult();
        }

        /// <summary>
        ///     Returns a list of <see cref="Show" /> objects that match the given query.
        /// </summary>
        /// <param name="query">Query the search is performed with.</param>
        /// <param name="results">Maximal amount of shows the resultset should return.</param>
        /// <returns>Returns a list of show objects.</returns>
        public List<Show> Search(string query, int results)
        {
            var shows = new List<Show>(results);
            var doc = _dataProvider.Search(query);

            shows.AddRange(from element in doc.Descendants("Series").Take(results)
                select int.Parse(element.GetXmlData("seriesid"))
                into id
                select _dataProvider.GetShow(id)
                into response
                select new ShowBuilder(response).GetResult());

            return shows;
        }

        private static Uri GetBannerUri(string uriSuffix)
        {
            return new Uri(UriPrefix + uriSuffix, UriKind.Absolute);
        }

        private class ShowBuilder {
            private readonly Show _show;

            public ShowBuilder(XDocument doc)
            {
                _show = new Show
                {
                    Id = int.Parse(doc.GetSeriesData("id")),
                    ImdbId = doc.GetSeriesData("IMDB_ID"),
                    Name = doc.GetSeriesData("SeriesName"),
                    Language = doc.GetSeriesData("Language"),
                    Network = doc.GetSeriesData("Network"),
                    Description = doc.GetSeriesData("Overview"),
                    Rating = string.IsNullOrWhiteSpace(doc.GetSeriesData("Rating"))
                        ? (double?) null
                        : Convert.ToDouble(doc.GetSeriesData("Rating"),
                            CultureInfo.InvariantCulture),
                    RatingCount = string.IsNullOrWhiteSpace(doc.GetSeriesData("RatingCount"))
                        ? 0
                        : Convert.ToInt32(doc.GetSeriesData("RatingCount")),
                    Runtime = string.IsNullOrWhiteSpace(doc.GetSeriesData("Runtime"))
                        ? (int?) null
                        : Convert.ToInt32(doc.GetSeriesData("Runtime")),
                    Banner = GetBannerUri(doc.GetSeriesData("banner")),
                    Fanart = GetBannerUri(doc.GetSeriesData("fanart")),
                    LastUpdated = string.IsNullOrWhiteSpace(doc.GetSeriesData("lastupdated"))
                        ? (long?) null
                        : Convert.ToInt64(doc.GetSeriesData("lastupdated")),
                    Poster = GetBannerUri(doc.GetSeriesData("poster")),
                    Zap2ItID = doc.GetSeriesData("zap2it_id"),
                    FirstAired = string.IsNullOrWhiteSpace(doc.GetSeriesData("FirstAired"))
                        ? (DateTime?) null
                        : Utils.ParseDate(doc.GetSeriesData("FirstAired")),
                    AirTime = string.IsNullOrWhiteSpace(doc.GetSeriesData("Airs_Time"))
                        ? (TimeSpan?) null
                        : Utils.ParseTime(doc.GetSeriesData("Airs_Time")),
                    AirDay = string.IsNullOrWhiteSpace(doc.GetSeriesData("Airs_DayOfWeek"))
                        ? (Frequency?) null
                        : (Frequency) Enum.Parse(typeof (Frequency), doc.GetSeriesData("Airs_DayOfWeek")),
                    Status = string.IsNullOrWhiteSpace(doc.GetSeriesData("Status"))
                        ? Status.Unknown
                        : (Status) Enum.Parse(typeof (Status), doc.GetSeriesData("Status")),
                    ContentRating = Utils.GetContentRating(doc.GetSeriesData("ContentRating")),
                    Genres = new List<string>(doc.GetSeriesData("Genre")
                        .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries)),
                    Actors = new List<string>(doc.GetSeriesData("Actors")
                        .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries)),
                    Episodes = new EpisodesBuilder(doc).BuildEpisodes()
                };
            }

            public Show GetResult()
            {
                return _show;
            }
        }

        public class EpisodeBuilder {
            private readonly Episode _episode;

            public EpisodeBuilder(XElement episodeNode)
            {
                _episode = new Episode
                {
                    Id = int.Parse(episodeNode.GetXmlData("id")),
                    Title = episodeNode.GetXmlData("EpisodeName"),
                    Description = episodeNode.GetXmlData("Overview"),
                    EpisodeNumber = int.Parse(episodeNode.GetXmlData("EpisodeNumber")),
                    Director = episodeNode.GetXmlData("Director"),
                    EpisodeImage = GetBannerUri(episodeNode.GetXmlData("filename")),
                    FirstAired =
                        string.IsNullOrWhiteSpace(episodeNode.GetXmlData("FirstAired"))
                            ? (DateTime?) null
                            : Utils.ParseDate(episodeNode.GetXmlData("FirstAired")),
                    GuestStars =
                        new List<string>(episodeNode.GetXmlData("GuestStars")
                            .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries)),
                    ImdbId = episodeNode.GetXmlData("IMDB_ID"),
                    Language = episodeNode.GetXmlData("Language"),
                    LastUpdated =
                        string.IsNullOrWhiteSpace(episodeNode.GetXmlData("lastupdated"))
                            ? 0L
                            : Convert.ToInt64(episodeNode.GetXmlData("lastupdated")),
                    Rating =
                        string.IsNullOrWhiteSpace(episodeNode.GetXmlData("Rating"))
                            ? (double?) null
                            : Convert.ToDouble(episodeNode.GetXmlData("Rating"),
                                CultureInfo.InvariantCulture),
                    RatingCount =
                        string.IsNullOrWhiteSpace(episodeNode.GetXmlData("RatingCount"))
                            ? 0
                            : Convert.ToInt32(episodeNode.GetXmlData("RatingCount")),
                    SeasonId = int.Parse(episodeNode.GetXmlData("seasonid")),
                    SeasonNumber = int.Parse(episodeNode.GetXmlData("SeasonNumber")),
                    SeriesId = int.Parse(episodeNode.GetXmlData("seriesid")),
                    ThumbHeight =
                        string.IsNullOrWhiteSpace(episodeNode.GetXmlData("thumb_height"))
                            ? (int?) null
                            : Convert.ToInt32(episodeNode.GetXmlData("thumb_height")),
                    ThumbWidth =
                        string.IsNullOrWhiteSpace(episodeNode.GetXmlData("thumb_width"))
                            ? (int?) null
                            : Convert.ToInt32(episodeNode.GetXmlData("thumb_width")),
                    TmsExport = episodeNode.GetXmlData("tms_export"),
                    Writers =
                        new List<string>(episodeNode.GetXmlData("Writer")
                            .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries))
                };
            }

            public Episode GetResult()
            {
                return _episode;
            }
        }

        private class EpisodesBuilder {
            private readonly XDocument _doc;

            public EpisodesBuilder(XDocument doc)
            {
                _doc = doc;
            }

            public List<Episode> BuildEpisodes()
            {
                return
                    _doc.Descendants("Episode")
                        .Select(episodeNode => new EpisodeBuilder(episodeNode).GetResult())
                        .ToList();
            }
        }

        public class UpdatesBuilder {
            private readonly Updates _updates;

            public UpdatesBuilder(XDocument doc)
            {
                if (doc.Root == null) return;
                _updates = new Updates
                {
                    Time = int.Parse(doc.Root.Attribute("time").Value),
                    UpdatedSeries = doc.Root.Elements("Series")
                        .Select(elt =>
                        {
                            var xElement = elt.Element("time");
                            var element = elt.Element("id");
                            if (element == null) return null;
                            return xElement != null
                                ? new UpdatedSerie
                                {
                                    Id = int.Parse(element.Value),
                                    Time = int.Parse(xElement.Value)
                                }
                                : null;
                        })
                        .ToList(),
                    UpdatedEpisodes = doc.Root.Elements("Episode")
                        .Select(elt =>
                        {
                            var xElement1 = elt.Element("id");
                            var xElement = elt.Element("Series");
                            if (xElement == null) return null;
                            var element = elt.Element("time");
                            if (element == null) return null;
                            return xElement1 != null
                                ? new UpdatedEpisode
                                {
                                    Id = int.Parse(xElement1.Value),
                                    SerieId = int.Parse(xElement.Value),
                                    Time = int.Parse(element.Value)
                                }
                                : null;
                        })
                        .ToList(),
                    UpdatedBanners = doc.Root.Elements("Banner")
                        .Select(elt =>
                        {
                            var elSeries = elt.Element("Series");
                            var elFormat = elt.Element("format");
                            if (elFormat == null) return null;
                            var elPath = elt.Element("path");
                            if (elPath == null) return null;
                            var elType = elt.Element("type");
                            if (elType == null) return null;
                            var elSeasonNum = elt.Element("SeasonNumber");
                            if (elSeasonNum == null) return null;
                            var elTime = elt.Element("time");
                            if (elTime == null) return null;
                            return elSeries != null
                                ? new UpdatedBanner
                                {
                                    SerieId = int.Parse(elSeries.Value),
                                    Format = elFormat.Value,
                                    Language =
                                        elt.Elements("language").Select(n => n.Value).FirstOrDefault() ?? string.Empty,
                                    Path = elPath.Value,
                                    Type = elType.Value,
                                    SeasonNumber = elt.Elements("SeasonNumber").Any()
                                        ? int.Parse(elSeasonNum.Value)
                                        : (int?) null,
                                    Time = int.Parse(elTime.Value)
                                }
                                : null;
                        })
                        .ToList()
                };
            }

            public Updates GetResult()
            {
                return _updates;
            }
        }
    }
}