// -----------------------------------------------------------------------
// <copyright company="Christoph van der Fecht - VDsoft">
// This code can be used in commercial, free and open source projects.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Data.SqlTypes;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using TVDB.Interfaces;

namespace TVDB.Model
{
    /// <summary>
    ///     An episode of a serie.
    /// </summary>
    public class Episode : SeriesElement, IXmlSerializer
    {
        /// <summary>
        ///     Name of the <see cref="CombinedEpisodeNumber" /> property.
        /// </summary>
        private const string CombinedEpisodeNumberName = "CombinedEpisodeNumber";

        /// <summary>
        ///     Name of the <see cref="CombinedSeason" /> property.
        /// </summary>
        private const string CombinedSeasonName = "CombinedSeason";

        /// <summary>
        ///     Name of the <see cref="DVDChapter" /> property.
        /// </summary>
        private const string DVDChapterName = "DVDChapter";

        /// <summary>
        ///     Name of the <see cref="DVDDiscId" /> property.
        /// </summary>
        private const string DVDDiscIdName = "DVDDiscId";

        /// <summary>
        ///     Name of the <see cref="DVDEpisodeNumber" /> property.
        /// </summary>
        private const string DVDEpisodeNumberName = "DVDEpisodeNumber";

        /// <summary>
        ///     Name of the <see cref="DVDSeason" /> property.
        /// </summary>
        private const string DVDSeasonName = "DVDSeason";

        /// <summary>
        ///     Name of the <see cref="Director" /> property.
        /// </summary>
        private const string DirectorName = "Director";

        /// <summary>
        ///     Name of the <see cref="EpImageFlag" /> property.
        /// </summary>
        private const string EpImageFlagName = "EpImageFlag";

        /// <summary>
        ///     Name of the <see cref="Number" /> property.
        /// </summary>
        private const string NumberName = "Number";

        /// <summary>
        ///     Name of the <see cref="GuestStars" /> property.
        /// </summary>
        private const string GuestStarsName = "GuestStars";

        /// <summary>
        ///     Name of the <see cref="ProductionCode" /> property.
        /// </summary>
        private const string ProductionCodeName = "ProductionCode";

        /// <summary>
        ///     Name of the <see cref="Rating" /> property.
        /// </summary>
        private const string RatingName = "Rating";

        /// <summary>
        ///     Name of the <see cref="SeasonNumber" /> property.
        /// </summary>
        private const string SeasonNumberName = "SeasonNumber";

        /// <summary>
        ///     Name of the <see cref="Writer" /> property.
        /// </summary>
        private const string WriterName = "Writer";

        /// <summary>
        ///     Name of the <see cref="AbsoluteNumber" /> property.
        /// </summary>
        private const string AbsoluteNumberName = "AbsoluteNumber";

        /// <summary>
        ///     Name of the <see cref="PictureFilename" /> property.
        /// </summary>
        private const string PictureFilenameName = "PictureFilename";

        /// <summary>
        ///     Name of the <see cref="LastUpdated" /> property.
        /// </summary>
        private const string LastUpdatedName = "LastUpdated";

        /// <summary>
        ///     Name of the <see cref="SeasonId" /> property.
        /// </summary>
        private const string SeasonIdName = "SeasonId";

        /// <summary>
        ///     Name of the <see cref="SeriesId" /> property.
        /// </summary>
        private const string SeriesIdName = "SeriesId";

        /// <summary>
        ///     Name of the <see cref="RatingCount" /> property.
        /// </summary>
        private const string RatingCountName = "RatingCount";

        /// <summary>
        ///     Name of the <see cref="Thumbadded" /> property.
        /// </summary>
        private const string ThumbaddedName = "Thumbadded";

        /// <summary>
        ///     Name of the <see cref="ThumbHeight" /> property.
        /// </summary>
        private const string ThumbHeightName = "ThumbHeight";

        /// <summary>
        ///     Name of the <see cref="ThumbWidth" /> property.
        /// </summary>
        private const string ThumbWidthName = "ThumbWidth";

        /// <summary>
        ///     Name of the <see cref="IsTMSExport" /> property.
        /// </summary>
        private const string IsTMSExportName = "IsTMSExport";

        /// <summary>
        ///     Name of the <see cref="IsTMSReviewBlurry" /> property.
        /// </summary>
        private const string IsTMSReviewBlurryName = "IsTMSReviewBlurry";

        /// <summary>
        ///     Name of the <see cref="TMSReviewById" /> property.
        /// </summary>
        private const string TMSReviewByIdName = "TMSReviewById";

        /// <summary>
        ///     Name of the <see cref="IsTMSReviewDark" /> property.
        /// </summary>
        private const string IsTMSReviewDarkName = "IsTMSReviewDark";

        /// <summary>
        ///     Name of the <see cref="TMSReviewDate" /> property.
        /// </summary>
        private const string TMSReviewDateName = "TMSReviewDate";

        /// <summary>
        ///     Name of the <see cref="TMSReviewLogoId" /> property.
        /// </summary>
        private const string TMSReviewLogoIdName = "TMSReviewLogoId";

        /// <summary>
        ///     Name of the <see cref="TMSReviewOther" /> property.
        /// </summary>
        private const string TMSReviewOtherName = "TMSReviewOther";

        /// <summary>
        ///     Name of the <see cref="IsTMSReviewUnsure" /> property.
        /// </summary>
        private const string IsTMSReviewUnsureName = "IsTMSReviewUnsure";

        /// <summary>
        ///     Absolute number of the episode.
        /// </summary>
        private int absoluteNumber = -1;

        /// <summary>
        ///     Airs after season name.
        /// </summary>
        private int airsAfterSeason = -1;

        /// <summary>
        ///     Number or episode this one airs before.
        /// </summary>
        private int airsBeforeEpisode = -1;

        /// <summary>
        ///     Number of the season this episode airs before.
        /// </summary>
        private int airsBeforeSeason = -1;

        /// <summary>
        ///     The combined episde number.
        /// </summary>
        private double combinedEpisodeNumber = -1;

        /// <summary>
        ///     The combined season number.
        /// </summary>
        private int combinedSeason = -1;

        /// <summary>
        ///     The director fo the series.
        /// </summary>
        private string director;

        /// <summary>
        ///     Chapter number of the dvd.
        /// </summary>
        private int dvdChapter = -1;

        /// <summary>
        ///     Id of the dvd disk.
        /// </summary>
        private int dvdDiskId = -1;

        /// <summary>
        ///     The episode number on the dvd.
        /// </summary>
        private double dvdEpisodeNumber = -1;

        /// <summary>
        ///     Season number of the dvd.
        /// </summary>
        private int dvdSeason = -1;

        /// <summary>
        ///     The ep image flag.
        /// </summary>
        private int epImageFlag = -1;

        /// <summary>
        ///     Names of any guest stars that appeared in this episode.
        /// </summary>
        private string guestStars;

        /// <summary>
        ///     Value inidcating whether the episode is a tms export or not.
        /// </summary>
        private bool isTMSExport;

        /// <summary>
        ///     Value indicating whether the tms review is blurry or not.
        /// </summary>
        private bool isTMSReviewBlurry;

        /// <summary>
        ///     Value indicating whether the tms review is dark or not.
        /// </summary>
        private bool isTMSReviewDark;

        /// <summary>
        ///     Value indicating whether the tms review is unsure or not.
        /// </summary>
        private bool isTMSReviewUnsure;

        /// <summary>
        ///     Id of the last update.
        /// </summary>
        private long lastUpdated = -1;

        /// <summary>
        ///     The number of the episode.
        /// </summary>
        private int number = -1;

        /// <summary>
        ///     Path and name of the picture.
        /// </summary>
        private string pictureFilename;

        /// <summary>
        ///     The production code of the episode.
        /// </summary>
        private int productionCode = -1;

        /// <summary>
        ///     Rating of the episode.
        /// </summary>
        private double rating = -1;

        /// <summary>
        ///     The rating count of the episode.
        /// </summary>
        private int ratingCount = -1;

        /// <summary>
        ///     Id of the season.
        /// </summary>
        private int seasonId = -1;

        /// <summary>
        ///     Number of the season the episode belongs to.
        /// </summary>
        private int seasonNumber = -1;

        /// <summary>
        ///     Id of the series.
        /// </summary>
        private int seriesId = -1;

        /// <summary>
        ///     Thumbadded id.
        /// </summary>
        private int thumbadded = -1;

        /// <summary>
        ///     The height of the thumbimage.
        /// </summary>
        private int thumbHeight = -1;

        /// <summary>
        ///     Width of the thumb image.
        /// </summary>
        private int thumbWidth = -1;

        /// <summary>
        ///     Id of the user who made the tms review.
        /// </summary>
        private int tmsReviewById = -1;

        /// <summary>
        ///     Date the tms review was made.
        /// </summary>
        private DateTime? tmsReviewDate;

        /// <summary>
        ///     Id of the tms review logo.
        /// </summary>
        private int tmsReviewLogoId = -1;

        /// <summary>
        ///     Tms review other.
        /// </summary>
        private int tmsReviewOther = -1;

        /// <summary>
        ///     Name of the writer of the episode.
        /// </summary>
        private string writer;

        /// <summary>
        ///     Gets or sets the combined episde number.
        /// </summary>
        public double CombinedEpisodeNumber
        {
            get { return combinedEpisodeNumber; }

            set
            {
                if (value == combinedEpisodeNumber)
                {
                    return;
                }

                combinedEpisodeNumber = value;
                RaisePropertyChanged(CombinedEpisodeNumberName);
            }
        }

        /// <summary>
        ///     Gets or sets the combined season number.
        /// </summary>
        public int CombinedSeason
        {
            get { return combinedSeason; }

            set
            {
                if (value == combinedSeason)
                {
                    return;
                }

                combinedSeason = value;
                RaisePropertyChanged(CombinedSeasonName);
            }
        }

        /// <summary>
        ///     Gets or sets the chapter number of the dvd.
        /// </summary>
        public int DVDChapter
        {
            get { return dvdChapter; }

            set
            {
                if (value == dvdChapter)
                {
                    return;
                }

                dvdChapter = value;
                RaisePropertyChanged(DVDChapterName);
            }
        }

        /// <summary>
        ///     Gets or sets the id of the dvd disk.
        /// </summary>
        public int DVDDiscId
        {
            get { return dvdDiskId; }

            set
            {
                if (value == dvdDiskId)
                {
                    return;
                }

                dvdDiskId = value;
                RaisePropertyChanged(DVDDiscIdName);
            }
        }

        /// <summary>
        ///     Gets or sets the episode number on the dvd.
        /// </summary>
        public double DVDEpisodeNumber
        {
            get { return dvdEpisodeNumber; }

            set
            {
                if (value == dvdEpisodeNumber)
                {
                    return;
                }

                dvdEpisodeNumber = value;
                RaisePropertyChanged(DVDEpisodeNumberName);
            }
        }

        /// <summary>
        ///     Gets or sets the season number of the dvd.
        /// </summary>
        public int DVDSeason
        {
            get { return dvdSeason; }

            set
            {
                if (value == dvdSeason)
                {
                    return;
                }

                dvdSeason = value;
                RaisePropertyChanged(DVDSeasonName);
            }
        }

        /// <summary>
        ///     Gets or sets the director fo the series.
        /// </summary>
        public string Director
        {
            get { return director; }

            set
            {
                if (value == director)
                {
                    return;
                }

                director = value;
                RaisePropertyChanged(DirectorName);
            }
        }

        /// <summary>
        ///     Gets or sets the ep image flag.
        /// </summary>
        public int EpImageFlag
        {
            get { return epImageFlag; }

            set
            {
                if (value == epImageFlag)
                {
                    return;
                }

                epImageFlag = value;
                RaisePropertyChanged(EpImageFlagName);
            }
        }

        /// <summary>
        ///     Gets or sets the number of the episode.
        /// </summary>
        public int Number
        {
            get { return number; }

            set
            {
                if (value == number)
                {
                    return;
                }

                number = value;
                RaisePropertyChanged(NumberName);
            }
        }

        /// <summary>
        ///     Gets or sets the names of any guest stars that appeared in this episode.
        /// </summary>
        public string GuestStars
        {
            get { return guestStars; }

            set
            {
                if (value == guestStars)
                {
                    return;
                }

                guestStars = value;
                RaisePropertyChanged(GuestStarsName);
            }
        }

        /// <summary>
        ///     Gets or sets the production code of the episode.
        /// </summary>
        public int ProductionCode
        {
            get { return productionCode; }

            set
            {
                if (value == productionCode)
                {
                    return;
                }

                productionCode = value;
                RaisePropertyChanged(ProductionCodeName);
            }
        }

        /// <summary>
        ///     Gets or sets the rating of the episode.
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
        /// 
        /// </summary>
        [XmlIgnore]
        public string SeasonEpisode => $"S{SeasonNumber.ToString("00")}E{Number.ToString("00")}";
        /// <summary>
        ///     Gets or sets the number of the season the episode belongs to.
        /// </summary>
        public int SeasonNumber
        {
            get { return seasonNumber; }

            set
            {
                if (value == seasonNumber)
                {
                    return;
                }

                seasonNumber = value;
                RaisePropertyChanged(SeasonNumberName);
            }
        }

        /// <summary>
        ///     Gets or sets the name of the writer of the episode.
        /// </summary>
        public string Writer
        {
            get { return writer; }

            set
            {
                if (value == writer)
                {
                    return;
                }

                writer = value;
                RaisePropertyChanged(WriterName);
            }
        }

        /// <summary>
        ///     Gets or sets the absolute number of the episode.
        /// </summary>
        public int AbsoluteNumber
        {
            get { return absoluteNumber; }

            set
            {
                if (value == absoluteNumber)
                {
                    return;
                }

                absoluteNumber = value;
                RaisePropertyChanged(AbsoluteNumberName);
            }
        }

        /// <summary>
        ///     Gets or sets the path and name of the picture.
        /// </summary>
        public string PictureFilename
        {
            get { return pictureFilename; }

            set
            {
                if (value == pictureFilename)
                {
                    return;
                }

                pictureFilename = value;
                RaisePropertyChanged(PictureFilenameName);
            }
        }

        /// <summary>
        ///     Gets or sets the id of the last update.
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
        ///     Gets or sets the id of the season..
        /// </summary>
        public int SeasonId
        {
            get { return seasonId; }

            set
            {
                if (value == seasonId)
                {
                    return;
                }

                seasonId = value;
                RaisePropertyChanged(SeasonIdName);
            }
        }

        /// <summary>
        ///     Gets or sets the id of the series.
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
        ///     Gets or sets the rating count of the episode.
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
        ///     Gets or sets the thumbadded id.
        /// </summary>
        public int Thumbadded
        {
            get { return thumbadded; }

            set
            {
                if (value == thumbadded)
                {
                    return;
                }

                thumbadded = value;
                RaisePropertyChanged(ThumbaddedName);
            }
        }

        /// <summary>
        ///     Gets or sets the height of the thumbimage.
        /// </summary>
        public int ThumbHeight
        {
            get { return thumbHeight; }

            set
            {
                if (value == thumbHeight)
                {
                    return;
                }

                thumbHeight = value;
                RaisePropertyChanged(ThumbHeightName);
            }
        }

        /// <summary>
        ///     Gets or sets the width of the thumb image.
        /// </summary>
        public int ThumbWidth
        {
            get { return thumbWidth; }

            set
            {
                if (value == thumbWidth)
                {
                    return;
                }

                thumbWidth = value;
                RaisePropertyChanged(ThumbWidthName);
            }
        }

        /// <summary>
        ///     Gets or sets a value inidcating whether the episode is a tms export or not.
        /// </summary>
        public bool IsTMSExport
        {
            get { return isTMSExport; }

            set
            {
                if (value == isTMSExport)
                {
                    return;
                }

                isTMSExport = value;
                RaisePropertyChanged(IsTMSExportName);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the tms review is blurry or not.
        /// </summary>
        public bool IsTMSReviewBlurry
        {
            get { return isTMSReviewBlurry; }

            set
            {
                if (value == isTMSReviewBlurry)
                {
                    return;
                }

                isTMSReviewBlurry = value;
                RaisePropertyChanged(IsTMSReviewBlurryName);
            }
        }

        /// <summary>
        ///     Gets or sets the id of the user who made the tms review.
        /// </summary>
        public int TMSReviewById
        {
            get { return tmsReviewById; }

            set
            {
                if (value == tmsReviewById)
                {
                    return;
                }

                tmsReviewById = value;
                RaisePropertyChanged(TMSReviewByIdName);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the tms review is dark or not.
        /// </summary>
        public bool IsTMSReviewDark
        {
            get { return isTMSReviewDark; }

            set
            {
                if (value == isTMSReviewDark)
                {
                    return;
                }

                isTMSReviewDark = value;
                RaisePropertyChanged(IsTMSReviewDarkName);
            }
        }

        /// <summary>
        ///     Gets or sets the date the tms review was made.
        /// </summary>
        public DateTime TMSReviewDate
        {
            get
            {
                var reviewDate = tmsReviewDate;
                if (reviewDate != null) return reviewDate.Value;
                return (DateTime) SqlDateTime.MinValue;
            }

            set
            {
                if (value == tmsReviewDate)
                {
                    return;
                }

                tmsReviewDate = value;
                RaisePropertyChanged(TMSReviewDateName);
            }
        }

        /// <summary>
        ///     Gets or sets the id of the tms review logo.
        /// </summary>
        public int TMSReviewLogoId
        {
            get { return tmsReviewLogoId; }

            set
            {
                if (value == tmsReviewLogoId)
                {
                    return;
                }

                tmsReviewLogoId = value;
                RaisePropertyChanged(TMSReviewLogoIdName);
            }
        }

        /// <summary>
        ///     Gets or sets the tms review other value.
        /// </summary>
        public int TMSReviewOther
        {
            get { return tmsReviewOther; }

            set
            {
                if (value == tmsReviewOther)
                {
                    return;
                }

                tmsReviewOther = value;
                RaisePropertyChanged(TMSReviewOtherName);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the tms review is unsure or not.
        /// </summary>
        public bool IsTMSReviewUnsure
        {
            get { return isTMSReviewUnsure; }

            set
            {
                if (value == isTMSReviewUnsure)
                {
                    return;
                }

                isTMSReviewUnsure = value;
                RaisePropertyChanged(IsTMSReviewUnsureName);
            }
        }

        /// <summary>
        ///     Gets or sets the name of the season this episode airs after.
        /// </summary>
        public int AirsAfterSeason
        {
            get { return airsAfterSeason; }
            set
            {
                if (value == airsAfterSeason)
                {
                    return;
                }

                airsAfterSeason = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the number of hte season this episode airs before.
        /// </summary>
        public int AirsBeforeSeason
        {
            get { return airsBeforeSeason; }
            set
            {
                if (value == airsBeforeSeason)
                {
                    return;
                }

                airsBeforeSeason = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the number of episode this one airs before.
        /// </summary>
        public int AirsBeforeEpisode
        {
            get { return airsBeforeEpisode; }
            set
            {
                if (value == airsBeforeEpisode)
                {
                    return;
                }

                airsBeforeEpisode = value;
                RaisePropertyChanged();
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
        ///             /// Deserializes the series.
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
                else if (currentNode.Name.Equals("Combined_episodenumber", StringComparison.OrdinalIgnoreCase))
                {
                    var result = -1.0;
                    double.TryParse(currentNode.InnerText, NumberStyles.Number, cultureInfo, out result);
                    CombinedEpisodeNumber = result;
                }
                else if (currentNode.Name.Equals("Combined_season", StringComparison.OrdinalIgnoreCase))
                {
                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    CombinedSeason = result;
                }
                else if (currentNode.Name.Equals("DVD_chapter", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    DVDChapter = result;
                }
                else if (currentNode.Name.Equals("DVD_discid", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    DVDDiscId = result;
                }
                else if (currentNode.Name.Equals("DVD_episodenumber", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    double result = -1;
                    double.TryParse(currentNode.InnerText, NumberStyles.Number, cultureInfo, out result);
                    DVDEpisodeNumber = result;
                }
                else if (currentNode.Name.Equals("DVD_season", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    DVDSeason = result;
                }
                else if (currentNode.Name.Equals("Director", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        Director = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("EpImgFlag", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    EpImageFlag = result;
                }
                else if (currentNode.Name.Equals("EpisodeName", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        Name = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("EpisodeNumber", StringComparison.OrdinalIgnoreCase))
                {
                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    Number = result;
                }
                else if (currentNode.Name.Equals("FirstAired", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    FirstAired = DateTime.Parse(currentNode.InnerText);
                }
                else if (currentNode.Name.Equals("GuestStars", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        GuestStars = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("IMDB_ID", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    IMDBId = currentNode.InnerText;
                }
                else if (currentNode.Name.Equals("Language", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        Language = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("Overview", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        Overview = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("ProductionCode", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    ProductionCode = result;
                }
                else if (currentNode.Name.Equals("Rating", StringComparison.OrdinalIgnoreCase))
                {
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
                else if (currentNode.Name.Equals("SeasonNumber", StringComparison.OrdinalIgnoreCase))
                {
                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    SeasonNumber = result;
                }
                else if (currentNode.Name.Equals("Writer", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        Writer = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("absolute_number", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    AbsoluteNumber = result;
                }
                else if (currentNode.Name.Equals("filename", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        PictureFilename = currentNode.InnerText;
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
                else if (currentNode.Name.Equals("seasonid", StringComparison.OrdinalIgnoreCase))
                {
                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    SeasonId = result;
                }
                else if (currentNode.Name.Equals("seriesid", StringComparison.OrdinalIgnoreCase))
                {
                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    SeriesId = result;
                }
                else if (currentNode.Name.Equals("thumb_added", StringComparison.OrdinalIgnoreCase))
                {
                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    Thumbadded = result;
                }
                else if (currentNode.Name.Equals("thumb_height", StringComparison.OrdinalIgnoreCase))
                {
                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    ThumbHeight = result;
                }
                else if (currentNode.Name.Equals("thumb_width", StringComparison.OrdinalIgnoreCase))
                {
                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    ThumbWidth = result;
                }
                else if (currentNode.Name.Equals("tms_export", StringComparison.OrdinalIgnoreCase))
                {
                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    IsTMSExport = result > 0 ? true : false;
                }
                else if (currentNode.Name.Equals("tms_review_blurry", StringComparison.OrdinalIgnoreCase))
                {
                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    IsTMSReviewBlurry = result > 0 ? true : false;
                }
                else if (currentNode.Name.Equals("tms_review_by", StringComparison.OrdinalIgnoreCase))
                {
                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    TMSReviewById = result;
                }
                else if (currentNode.Name.Equals("tms_review_dark", StringComparison.OrdinalIgnoreCase))
                {
                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    IsTMSReviewDark = result > 0 ? true : false;
                }
                else if (currentNode.Name.Equals("tms_review_date", StringComparison.OrdinalIgnoreCase))
                {
                    var result = (DateTime) SqlDateTime.MinValue;
                    DateTime.TryParse(currentNode.InnerText, cultureInfo, DateTimeStyles.AssumeUniversal, out result);
                    TMSReviewDate = result;
                }
                else if (currentNode.Name.Equals("tms_review_logo", StringComparison.OrdinalIgnoreCase))
                {
                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    TMSReviewLogoId = result;
                }
                else if (currentNode.Name.Equals("tms_review_other", StringComparison.OrdinalIgnoreCase))
                {
                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    TMSReviewOther = result;
                }
                else if (currentNode.Name.Equals("tms_review_unsure", StringComparison.OrdinalIgnoreCase))
                {
                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    IsTMSReviewUnsure = result > 0 ? true : false;
                }
                else if (currentNode.Name.Equals("airsafter_season", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    AirsAfterSeason = result;
                }
                else if (currentNode.Name.Equals("airsbefore_season", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    AirsBeforeSeason = result;
                }
                else if (currentNode.Name.Equals("airsbefore_episode", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        continue;
                    }

                    var result = -1;
                    int.TryParse(currentNode.InnerText, out result);
                    AirsBeforeEpisode = result;
                }
            }

            Initialize();
        }

        /// <summary>
        ///     Initializes the properties of the <see cref="Episode" />.
        /// </summary>
        private void Initialize()
        {
            GuestStars = PrepareText(guestStars);

            Writer = PrepareText(writer);
        }
    }
}