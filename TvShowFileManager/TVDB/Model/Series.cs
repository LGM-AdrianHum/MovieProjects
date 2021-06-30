// -----------------------------------------------------------------------
// <copyright company="Christoph van der Fecht - VDsoft">
// This code can be used in commercial, free and open source projects.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Globalization;
using System.Xml;
using TVDB.Interfaces;

namespace TVDB.Model
{
    /// <summary>
    ///     A Series with all details and episodes.
    /// </summary>
    public class Series : SeriesElement, IXmlSerializer
    {
        /// <summary>
        ///     Name of the <see cref="Banner" /> property.
        /// </summary>
        private const string BannerName = "Banner";

        /// <summary>
        ///     Name of the <see cref="Zap2ItId" /> property.
        /// </summary>
        private const string Zap2ItIdName = "Zap2ItId";

        /// <summary>
        ///     Name of the <see cref="HasEpisodes" /> property.
        /// </summary>
        private const string HasEpisodesName = "HasEpisodes";

        /// <summary>
        ///     Name of the <see cref="Episodes" /> property.
        /// </summary>
        private const string EpisodesName = "Episodes";

        /// <summary>
        ///     Name of the <see cref="SeriesId" /> property.
        /// </summary>
        private const string SeriesIdName = "SeriesId";

        /// <summary>
        ///     Name of the <see cref="Actorts" /> property.
        /// </summary>
        private const string ActortsName = "Actorts";

        /// <summary>
        ///     Name of the <see cref="AirsDayOfWeel" /> property.
        /// </summary>
        private const string AirsDayOfWeelName = "AirsDayOfWeel";

        /// <summary>
        ///     Name of the <see cref="AirsTime" /> property.
        /// </summary>
        private const string AirsTimeName = "AirsTime";

        /// <summary>
        ///     Name of the <see cref="ContentRating" /> property.
        /// </summary>
        private const string ContentRatingName = "ContentRating";

        /// <summary>
        ///     Name of the <see cref="Genre" /> property.
        /// </summary>
        private const string GenreName = "Genre";

        /// <summary>
        ///     Name of the <see cref="Network" /> property.
        /// </summary>
        private const string NetworkName = "Network";

        /// <summary>
        ///     Name of the <see cref="NetworkId" /> property.
        /// </summary>
        private const string NetworkIdName = "NetworkId";

        /// <summary>
        ///     Name of the <see cref="RatingCount" /> property.
        /// </summary>
        private const string RatingCountName = "RatingCount";

        /// <summary>
        ///     Name of the <see cref="Rating" /> property.
        /// </summary>
        private const string RatingName = "Rating";

        /// <summary>
        ///     Name of the <see cref="Runtime" /> property.
        /// </summary>
        private const string RuntimeName = "Runtime";

        /// <summary>
        ///     Name of the <see cref="Status" /> property.
        /// </summary>
        private const string StatusName = "Status";

        /// <summary>
        ///     Name of the <see cref="AddedDate" /> property.
        /// </summary>
        private const string AddedDateName = "AddedDate";

        /// <summary>
        ///     Name of the <see cref="AddedByUserId" /> property.
        /// </summary>
        private const string AddedByUserIdName = "AddedByUserId";

        /// <summary>
        ///     Name of the <see cref="FanArt" /> property.
        /// </summary>
        private const string FanArtName = "FanArt";

        /// <summary>
        ///     Name of the <see cref="LastUpdated" /> property.
        /// </summary>
        private const string LastUpdatedName = "LastUpdated";

        /// <summary>
        ///     Name of the <see cref="Poster" /> property.
        /// </summary>
        private const string PosterName = "Poster";

        /// <summary>
        ///     Name of the <see cref="TMSWanted" /> property.
        /// </summary>
        private const string TMSWantedName = "TMSWanted";

        /// <summary>
        ///     Name of the <see cref="ActorCollection" /> property.
        /// </summary>
        private const string ActorCollectionName = "ActorCollection";

        /// <summary>
        ///     All actors of the series.
        /// </summary>
        private string actors;

        /// <summary>
        ///     Collection of the actors of the series.
        /// </summary>
        private ObservableCollection<Actor> actorsCollection = new ObservableCollection<Actor>();

        /// <summary>
        ///     Id of the user who added the series.
        /// </summary>
        private int addedByUserId = -1;

        /// <summary>
        ///     Date the series was added to the system.
        /// </summary>
        private DateTime addedDate = (DateTime) SqlDateTime.MinValue;

        /// <summary>
        ///     Day the series is aired.
        /// </summary>
        private string airsDayOfWeek;

        /// <summary>
        ///     Time the series is aired.
        /// </summary>
        private string airsTime;

        /// <summary>
        ///     Path of the banner for the series.
        /// </summary>
        private string bannerPath;

        /// <summary>
        ///     The content rating of the series.
        /// </summary>
        private string contentRating;

        /// <summary>
        ///     Collection of all assigned episodes.
        /// </summary>
        private ObservableCollection<Episode> episodes = new ObservableCollection<Episode>();

        /// <summary>
        ///     Path of the fan art.
        /// </summary>
        private string fanArt;

        /// <summary>
        ///     The genre of the series.
        /// </summary>
        private string genre;

        /// <summary>
        ///     Value indicating whether the series has episodes assigend or not.
        /// </summary>
        private bool hasEpisodes;

        /// <summary>
        ///     Last updated id.
        /// </summary>
        private long lastUpdated = -1;

        /// <summary>
        ///     The network name that aires the series.
        /// </summary>
        private string network;

        /// <summary>
        ///     Id of the network.
        /// </summary>
        private int networkId = -1;

        /// <summary>
        ///     Path of the poster.
        /// </summary>
        private string poster;

        /// <summary>
        ///     Rating fo the series.
        /// </summary>
        private double rating = -1.0;

        /// <summary>
        ///     Count of rates.
        /// </summary>
        private int ratingCount = -1;

        /// <summary>
        ///     Runtime of the series.
        /// </summary>
        private double runtime = -1.0;

        [Key]
        /// <summary>
        /// Series ID.
        /// </summary>
        private int seriesId = -1;

        /// <summary>
        ///     Status of the series.
        /// </summary>
        private string status;

        /// <summary>
        ///     Value indicating whether the tms is wanted for the series or not.
        /// </summary>
        private bool tmsWanted;

        /// <summary>
        ///     Zap2It id of the series.
        /// </summary>
        private string zap2ItId;

        /// <summary>
        ///     Gets or sets the path of the banner for the series.
        /// </summary>
        public string Banner
        {
            get { return bannerPath; }

            set
            {
                if (value == bannerPath)
                {
                    return;
                }

                bannerPath = value;
                RaisePropertyChanged(BannerName);
            }
        }

        /// <summary>
        ///     Gets or sets the Zap2It id of the series.
        /// </summary>
        public string Zap2ItId
        {
            get { return zap2ItId; }

            set
            {
                if (value == zap2ItId)
                {
                    return;
                }

                zap2ItId = value;
                RaisePropertyChanged(Zap2ItIdName);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the series has episodes assigend or not.
        /// </summary>
        public bool HasEpisodes
        {
            get { return hasEpisodes; }

            set
            {
                if (value == hasEpisodes)
                {
                    return;
                }

                hasEpisodes = value;
                RaisePropertyChanged(HasEpisodesName);
            }
        }

        /// <summary>
        ///     Gets or sets a collection of all assigned episodes.
        /// </summary>
        public ObservableCollection<Episode> Episodes
        {
            get { return episodes; }

            set
            {
                if (value == episodes)
                {
                    return;
                }

                episodes = value;
                RaisePropertyChanged(EpisodesName);
            }
        }

        /// <summary>
        ///     Gets or sets the series id.
        /// </summary>
        public int SeriesId
        {
            get { return seriesId; }

            set
            {
                if (value == seriesId)
                {
                    return;
                }

                seriesId = value;
                RaisePropertyChanged(SeriesIdName);
            }
        }

        /// <summary>
        ///     Gets or sets all actors of the series.
        /// </summary>
        public string Actorts
        {
            get { return actors; }

            set
            {
                if (value == actors)
                {
                    return;
                }

                actors = value;
                RaisePropertyChanged(ActortsName);
            }
        }

        /// <summary>
        ///     Gets or sets the day the series is aired.
        /// </summary>
        public string AirsDayOfWeel
        {
            get { return airsDayOfWeek; }

            set
            {
                if (value == airsDayOfWeek)
                {
                    return;
                }

                airsDayOfWeek = value;
                RaisePropertyChanged(AirsDayOfWeelName);
            }
        }

        /// <summary>
        ///     Gets or sets the time the series is aired.
        /// </summary>
        public string AirsTime
        {
            get { return airsTime; }

            set
            {
                if (value == airsTime)
                {
                    return;
                }

                airsTime = value;
                RaisePropertyChanged(AirsTimeName);
            }
        }

        /// <summary>
        ///     Gets or sets the content rating of the series.
        /// </summary>
        public string ContentRating
        {
            get { return contentRating; }

            set
            {
                if (value == contentRating)
                {
                    return;
                }

                contentRating = value;
                RaisePropertyChanged(ContentRatingName);
            }
        }

        /// <summary>
        ///     Gets or sets the genre of the series.
        /// </summary>
        public string Genre
        {
            get { return genre; }

            set
            {
                if (value == genre)
                {
                    return;
                }

                genre = value;
                RaisePropertyChanged(GenreName);
            }
        }

        /// <summary>
        ///     Gets or sets the network name that aires the series.
        /// </summary>
        public string Network
        {
            get { return network; }

            set
            {
                if (value == network)
                {
                    return;
                }

                network = value;
                RaisePropertyChanged(NetworkName);
            }
        }

        /// <summary>
        ///     Gets or sets the id of the network.
        /// </summary>
        public int NetworkId
        {
            get { return networkId; }

            set
            {
                if (value == networkId)
                {
                    return;
                }

                networkId = value;
                RaisePropertyChanged(NetworkIdName);
            }
        }

        /// <summary>
        ///     Gets or sets the count of rates.
        /// </summary>
        public int RatingCount
        {
            get { return ratingCount; }

            set
            {
                if (value == ratingCount)
                {
                    return;
                }

                ratingCount = value;
                RaisePropertyChanged(RatingCountName);
            }
        }

        /// <summary>
        ///     Gets or sets the rating fo the series.
        /// </summary>
        public double Rating
        {
            get { return rating; }

            set
            {
                if (value == rating)
                {
                    return;
                }

                rating = value;
                RaisePropertyChanged(RatingName);
            }
        }

        /// <summary>
        ///     Gets or sets the runtime of the series.
        /// </summary>
        public double Runtime
        {
            get { return runtime; }

            set
            {
                if (value == runtime)
                {
                    return;
                }

                runtime = value;
                RaisePropertyChanged(RuntimeName);
            }
        }

        /// <summary>
        ///     Gets or sets the status of the series.
        /// </summary>
        public string Status
        {
            get { return status; }

            set
            {
                if (value == status)
                {
                    return;
                }

                status = value;
                RaisePropertyChanged(StatusName);
            }
        }

        /// <summary>
        ///     Gets or sets the date the series was added to the system.
        /// </summary>
        public DateTime AddedDate
        {
            get { return addedDate; }

            set
            {
                if (value == addedDate)
                {
                    return;
                }

                addedDate = value;
                RaisePropertyChanged(AddedDateName);
            }
        }

        /// <summary>
        ///     Gets or sets the id of the user who added the series.
        /// </summary>
        public int AddedByUserId
        {
            get { return addedByUserId; }

            set
            {
                if (value == addedByUserId)
                {
                    return;
                }

                addedByUserId = value;
                RaisePropertyChanged(AddedByUserIdName);
            }
        }

        /// <summary>
        ///     Gets or sets the path of the fan art.
        /// </summary>
        public string FanArt
        {
            get { return fanArt; }

            set
            {
                if (value == fanArt)
                {
                    return;
                }

                fanArt = value;
                RaisePropertyChanged(FanArtName);
            }
        }

        /// <summary>
        ///     Gets or sets the last updated id.
        /// </summary>
        public long LastUpdated
        {
            get { return lastUpdated; }

            set
            {
                if (value == lastUpdated)
                {
                    return;
                }

                lastUpdated = value;
                RaisePropertyChanged(LastUpdatedName);
            }
        }

        /// <summary>
        ///     Gets or sets the path of the poster.
        /// </summary>
        public string Poster
        {
            get { return poster; }

            set
            {
                if (value == poster)
                {
                    return;
                }

                poster = value;
                RaisePropertyChanged(PosterName);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the tms is wanted for the series or not.
        /// </summary>
        public bool TMSWanted
        {
            get { return tmsWanted; }

            set
            {
                if (value == tmsWanted)
                {
                    return;
                }

                tmsWanted = value;
                RaisePropertyChanged(TMSWantedName);
            }
        }

        /// <summary>
        ///     Gets or sets a collection of the actors of the series.
        /// </summary>
        public ObservableCollection<Actor> ActorCollection
        {
            get { return actorsCollection; }

            set
            {
                if (value == actorsCollection)
                {
                    return;
                }

                actorsCollection = value;
                RaisePropertyChanged(ActorCollectionName);
            }
        }

        /// <summary>
        ///     Deserializes the provided xml node.
        /// </summary>
        /// <param name="node">Node to deserialize.</param>
        /// <exception cref="ArgumentNullException">Occurs when the provided <see cref="System.Xml.XmlNode" /> is null.</exception>
        /// <example>
        ///     This example shows how to use the deserialize method.
        ///     <code>
        /// namespace Docunamespace
        /// {
        /// 	/// <summary>
        ///             /// Class for the docu.
        ///             ///
        ///         </summary>
        /// 	class DocuClass
        /// 	{
        /// 		/// <summary>
        ///             /// Xml document that contains all details of a series.
        ///             ///
        ///         </summary>
        /// 		private XmlDocument languageDoc = null;
        /// 		
        /// 		/// <summary>
        ///             /// Initializes a new instance of the DocuClass class.
        ///             ///
        ///         </summary>
        /// 		public DocuClass(string extractionPath)
        /// 		{
        /// 			// load series xml.
        /// 			this.languageDoc = new XmlDocument();
        /// 			this.languageDoc.Load(System.IO.Path.Combine(this.extractionPath, string.Format("{0}.xml", this.Language)));
        /// 		}
        /// 		
        /// 		/// <summary>
        ///             /// Deserializes all actors of the series.
        ///             ///
        ///         </summary>
        /// 		public Series DeserializeSeries()
        /// 		{
        /// 			Series series = new Series();
        /// 			
        /// 			// load the xml docs second child.
        /// 			XmlNode dataNode = this.languageDoc.ChildNodes[1];
        /// 			// deserialize all episodes and series details.
        /// 			foreach (XmlNode currentNode in dataNode.ChildNodes)
        /// 			{
        /// 				if (currentNode.Name.Equals("Episode", StringComparison.OrdinalIgnoreCase))
        /// 				{
        /// 					Episode deserialized = new Episode();
        /// 					deserialized.Deserialize(currentNode);
        /// 					series.AddEpisode(deserialized);
        /// 					continue;
        /// 				}
        /// 				else if (currentNode.Name.Equals("Series", StringComparison.OrdinalIgnoreCase))
        /// 				{
        /// 					series.Deserialize(currentNode);
        /// 					continue;
        /// 				}
        /// 			}
        /// 		}
        /// 	}
        /// }
        /// </code>
        /// </example>
        public void Deserialize(XmlNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node", "Provided node must not be null.");
            }

            var cultureInfo = CultureInfo.CreateSpecificCulture("en-US");

            foreach (XmlNode currentNode in node.ChildNodes)
            {
                if (currentNode.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                {
                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    Id = result;
                }
                else if (currentNode.Name.Equals("SeriesID", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    SeriesId = result;
                }
                else if (currentNode.Name.Equals("Language", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        Language = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("SeriesName", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        Name = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("banner", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        Banner = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("Overview", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        Overview = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("FirstAired", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    FirstAired = DateTime.Parse(currentNode.InnerText);
                }
                else if (currentNode.Name.Equals("IMDB_ID", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    IMDBId = currentNode.InnerText;
                }
                else if (currentNode.Name.Equals("zap2it_id", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    Zap2ItId = currentNode.InnerText;
                }
                else if (currentNode.Name.Equals("Actors", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        Actorts = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("Airs_DayOfWeek", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    AirsDayOfWeel = currentNode.InnerText;
                }
                else if (currentNode.Name.Equals("Airs_Time", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    AirsTime = currentNode.InnerText;
                }
                else if (currentNode.Name.Equals("ContentRating", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        ContentRating = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("Genre", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    Genre = currentNode.InnerText;
                }
                else if (currentNode.Name.Equals("Network", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    Network = currentNode.InnerText;
                }
                else if (currentNode.Name.Equals("NetworkID", StringComparison.OrdinalIgnoreCase))
                {
                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    NetworkId = result;
                }
                else if (currentNode.Name.Equals("Rating", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    var result = -1.0;
                    double.TryParse(currentNode.InnerText, NumberStyles.Number, cultureInfo, out result);
                    Rating = result;
                }
                else if (currentNode.Name.Equals("RatingCount", StringComparison.OrdinalIgnoreCase))
                {
                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    RatingCount = result;
                }
                else if (currentNode.Name.Equals("Runtime", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    var result = -1.0;
                    double.TryParse(currentNode.InnerText, NumberStyles.Number, cultureInfo, out result);
                    Runtime = result;
                }
                else if (currentNode.Name.Equals("Status", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    Status = currentNode.InnerText;
                }
                else if (currentNode.Name.Equals("added", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    AddedDate = DateTime.Parse(currentNode.InnerText);
                }
                else if (currentNode.Name.Equals("addedBy", StringComparison.OrdinalIgnoreCase))
                {
                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    AddedByUserId = result;
                }
                else if (currentNode.Name.Equals("fanart", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        FanArt = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("lastupdated", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    long result = -1;
                    long.TryParse(currentNode.InnerText, out result);
                    LastUpdated = result;
                }
                else if (currentNode.Name.Equals("poster", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        Poster = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("tms_wanted", StringComparison.OrdinalIgnoreCase))
                {
                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);

                    TMSWanted = result > 0 ? true : false;
                }
            }

            Initialize();
        }

        /// <summary>
        ///     Adds the provided episode to the series.
        /// </summary>
        /// <param name="toAdd">Episode to add.</param>
        /// <exception cref="ArgumentNullException">Occurs when the provided <see cref="Episode" /> is null.</exception>
        public void AddEpisode(Episode toAdd)
        {
            if (toAdd == null)
            {
                throw new ArgumentNullException("toAdd", "Episode to add must not be null.");
            }

            Episodes.Add(toAdd);
        }

        /// <summary>
        ///     Initializes the properties.
        /// </summary>
        private void Initialize()
        {
            if (Episodes.Count > 0)
            {
                HasEpisodes = true;
            }

            Actorts = PrepareText(actors);
            Genre = PrepareText(genre);
        }
    }
}