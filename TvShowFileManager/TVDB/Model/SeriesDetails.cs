// -----------------------------------------------------------------------
// <copyright company="Christoph van der Fecht - VDsoft">
// This code can be used in commercial, free and open source projects.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml;
using PropertyChanged;

namespace TVDB.Model
{
    /// <summary>
    ///     Contains all details for the requested series like Actors, Banners and all episodes of the series.
    /// </summary>
    [ImplementPropertyChanged]
    public class SeriesDetails : IDisposable
    {
        /// <summary>
        ///     The Actors.
        /// </summary>
        private List<Actor> _actors;

        /// <summary>
        ///     Xml document that contains all actors.
        /// </summary>
        private XmlDocument _actorsDoc;

        /// <summary>
        ///     The Banners.
        /// </summary>
        private List<Banner> _banners;

        /// <summary>
        ///     Xml document that contains all banners.
        /// </summary>
        private XmlDocument _bannersDoc;

        /// <summary>
        ///     Path of the extracted files.
        /// </summary>
        private string _extractionPath;

        /// <summary>
        ///     Xml document that contains all details of a series.
        /// </summary>
        private XmlDocument _languageDoc;

        /// <summary>
        ///     The Series.
        /// </summary>
        private Series _series;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SeriesDetails" /> class.
        /// </summary>
        /// <param name="extractionPath">Path of the extracted files.</param>
        /// <param name="language">Language of the series.</param>
        /// <exception cref="System.IO.DirectoryNotFoundException">Occurs when the provided extraction path doesn't exists.</exception>
        /// <exception cref="ArgumentNullException">Occurs when to provided language is null or empty.</exception>
        public SeriesDetails(string extractionPath, string language)
        {
            var dirInfo = new DirectoryInfo(extractionPath);
            if (!dirInfo.Exists)
            {
                throw new DirectoryNotFoundException($"The directory \"{dirInfo.FullName}\" could not be found.");
            }

            if (string.IsNullOrEmpty(language))
            {
                throw new ArgumentNullException(nameof(language), "Provided language must not be null or empty.");
            }

            _extractionPath = extractionPath;
            Language = language;

            // load actors xml.
            _actorsDoc = new XmlDocument();
            _actorsDoc.Load(Path.Combine(_extractionPath, "actors.xml"));

            // load banners xml.
            _bannersDoc = new XmlDocument();
            _bannersDoc.Load(Path.Combine(_extractionPath, "banners.xml"));

            // load series xml.
            _languageDoc = new XmlDocument();
            _languageDoc.Load(Path.Combine(_extractionPath, $"{Language}.xml"));
        }

        /// <summary>
        ///     Gets or sets the Language property.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        ///     Gets or sets the Actors property.
        /// </summary>
        public List<Actor> Actors
        {
            get
            {
                if (_actors == null)
                {
                    DeserializeActors();
                }

                return _actors;
            }

            private set
            {
                if (value != null && value == _actors)
                {
                    return;
                }

                _actors = value;
            }
        }

        /// <summary>
        ///     Gets or sets the Series property.
        /// </summary>
        public Series Series
        {
            get
            {
                if (_series == null)
                {
                    DeserializeSeries();
                }

                return _series;
            }

            private set
            {
                if (value != null && value == _series)
                {
                    return;
                }

                _series = value;
            }
        }

        /// <summary>
        ///     Gets or sets the Banners property.
        /// </summary>
        public List<Banner> Banners
        {
            get
            {
                if (_banners == null)
                {
                    DeserializeBanners();
                }

                return _banners;
            }

            set
            {
                if (value != null && value == _banners)
                {
                    return;
                }

                _banners = value;
            }
        }

        /// <summary>
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     Releases all resources of the class.
        /// </summary>
        public void Dispose()
        {
            _extractionPath = null;
            _actorsDoc = null;
            _bannersDoc = null;
            _languageDoc = null;

            if (Actors != null)
            {
                _actors.Clear();
                _actors = null;
            }

            if (Banners != null)
            {
                _banners.Clear();
                _banners = null;
            }

            if (Series != null)
            {
                _series = null;
            }
        }

        /// <summary>
        ///     Deserializes all actors of the series.
        /// </summary>
        private void DeserializeActors()
        {
            if (_actorsDoc == null || _actorsDoc.ChildNodes.Count == 0) return;

            Actors = new List<Actor>();
            var actorsNode = _actorsDoc.ChildNodes[1];

            foreach (XmlNode currentNode in actorsNode)
            {
                if (!currentNode.Name.Equals("Actor", StringComparison.OrdinalIgnoreCase)) continue;
                var deserializes = new Actor();
                deserializes.Deserialize(currentNode);
                Actors.Add(deserializes);
            }
        }

        /// <summary>
        ///     Deserializes all banners of the series.
        /// </summary>
        private void DeserializeBanners()
        {
            if (_bannersDoc == null || _bannersDoc.ChildNodes.Count == 0) return;

            Banners = new List<Banner>();
            var bannersNode = _bannersDoc.ChildNodes[1];
            foreach (XmlNode currentNode in bannersNode.ChildNodes)
            {
                if (!currentNode.Name.Equals("Banner", StringComparison.OrdinalIgnoreCase)) continue;
                var deserialized = new Banner();
                deserialized.Deserialize(currentNode);
                Banners.Add(deserialized);
            }
        }

        /// <summary>
        ///     Deserializes the series.
        /// </summary>
        private void DeserializeSeries()
        {
            if (_languageDoc == null || _languageDoc.ChildNodes.Count == 0) return;
            Series = new Series();
            if (Actors != null && Actors.Count > 0)
            {
                Series.ActorCollection = new ObservableCollection<Actor>(Actors);
            }

            var dataNode = _languageDoc.ChildNodes[1];

            foreach (XmlNode currentNode in dataNode.ChildNodes)
            {
                if (currentNode.Name.Equals("Episode", StringComparison.OrdinalIgnoreCase))
                {
                    var deserialized = new Episode();
                    deserialized.Deserialize(currentNode);

                    Series.AddEpisode(deserialized);
                }
                else if (currentNode.Name.Equals("Series", StringComparison.OrdinalIgnoreCase))
                {
                    Series.Deserialize(currentNode);
                }
            }
        }
    }
}