// -----------------------------------------------------------------------
// <copyright company="Christoph van der Fecht - VDsoft">
// This code can be used in commercial, free and open source projects.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using PropertyChanged;
using TVDB.Interfaces;

namespace TVDB.Model
{
    /// <summary>
    ///     Image of a series.
    /// </summary>
    [ImplementPropertyChanged]
    public class Banner : INotifyPropertyChanged, IXmlSerializer
    {
        /// <summary>
        ///     Gets or sets the Id of the banner.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the Path of the image.
        /// </summary>
        public string BannerPath { get; set; }

        /// <summary>
        ///     Gets or sets the Type of the banner.
        /// </summary>
        public BannerTyp Type { get; set; }

        /// <summary>
        ///     Gets or sets the Dimension of the image.
        /// </summary>
        public string Dimension { get; set; }

        /// <summary>
        ///     Gets or sets the Colors of the banner.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        ///     Gets or sets the Language of the banner image.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        ///     Gets or sets the Rating fo the banner.
        /// </summary>
        public double Rating { get; set; }

        /// <summary>
        ///     Gets or sets the Number of ratings.
        /// </summary>
        public int RatingCount { get; set; }

        /// <summary>
        ///     Gets or sets the Series name.
        /// </summary>
        public bool SeriesName { get; set; }

        /// <summary>
        ///     Gets or sets the Path to the thumbnail of the image.
        /// </summary>
        public string ThumbnailPath { get; set; }

        /// <summary>
        ///     Gets or sets the Path to the vignette image.
        /// </summary>
        public string VignettePath { get; set; }

        /// <summary>
        ///     Gets or sets the season.
        /// </summary>
        public int Season { get; set; }

        /// <summary>
        ///     Occures when a property changed its value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Deserializes a banner xml node.
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
        ///             /// Xml document that contains all banners.
        ///             ///
        ///         </summary>
        /// 		private XmlDocument bannersDoc = null;
        /// 		
        /// 		/// <summary>
        ///             /// Initializes a new instance of the DocuClass class.
        ///             ///
        ///         </summary>
        /// 		public DocuClass(string extractionPath)
        /// 		{
        /// 			// load actors xml.
        /// 			this.bannersDoc = new XmlDocument();
        /// 			this.bannersDoc.Load(System.IO.Path.Combine(this.extractionPath, "banners.xml"));
        /// 		}
        /// 		
        /// 		/// <summary>
        ///             /// Deserializes all banners of the series.
        ///             ///
        ///         </summary>
        /// 		public List&#60;Banner&#62; DeserializeBanners()
        /// 		{
        /// 			List&#60;Banner&#62; banners = new List&#60;Banner&#62;();
        /// 			
        /// 			// load the xml docs second child.
        /// 			XmlNode bannersNode = this.bannersDoc.ChildNodes[1];
        /// 			// deserialize all banners.
        /// 			foreach (XmlNode currentNode in bannersNode.ChildNodes)
        /// 			{
        /// 				if (currentNode.Name.Equals("Banner", StringComparison.OrdinalIgnoreCase))
        /// 				{
        /// 					Banner deserialized = new Banner();
        /// 					deserialized.Deserialize(currentNode);
        /// 					banners.Add(deserialized);
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
                throw new ArgumentNullException(nameof(node), "Provided node must not be null.");
            }

            var cultureInfo = CultureInfo.CreateSpecificCulture("en-US");

            foreach (XmlNode currentNode in node.ChildNodes)
            {
                if (currentNode.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                {
                    int result;
                    int.TryParse(currentNode.InnerText, out result);
                    Id = result;
                }
                else if (currentNode.Name.Equals("BannerPath", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        BannerPath = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("BannerType", StringComparison.OrdinalIgnoreCase))
                {
                    BannerTyp currentTyp;
                    Enum.TryParse(currentNode.InnerText, out currentTyp);
                    Type = currentTyp;
                }
                else if (currentNode.Name.Equals("BannerType2", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        Dimension = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("Colors", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        Color = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("Language", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        Language = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("Rating", StringComparison.OrdinalIgnoreCase))
                {
                    double result;
                    double.TryParse(currentNode.InnerText, NumberStyles.Number, cultureInfo, out result);
                    Rating = result;
                }
                else if (currentNode.Name.Equals("RatingCount", StringComparison.OrdinalIgnoreCase))
                {
                    int result;
                    int.TryParse(currentNode.InnerText, out result);
                    RatingCount = result;
                }
                else if (currentNode.Name.Equals("SeriesName", StringComparison.OrdinalIgnoreCase))
                {
                    bool result;
                    bool.TryParse(currentNode.InnerText, out result);
                    SeriesName = result;
                }
                else if (currentNode.Name.Equals("ThumbnailPath", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        ThumbnailPath = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("VignettePath", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        VignettePath = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("Season", StringComparison.OrdinalIgnoreCase))
                {
                    int result;
                    int.TryParse(currentNode.InnerText, out result);
                    Season = result;
                }
            }
        }
    }
}