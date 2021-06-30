// -----------------------------------------------------------------------
// <copyright company="Christoph van der Fecht - VDsoft">
// This code can be used in commercial, free and open source projects.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Xml;
using TVDB.Interfaces;

namespace TVDB.Model
{
    /// <summary>
    ///     An Actor.
    /// </summary>
    public class Actor : INotifyPropertyChanged, IXmlSerializer, IComparable<Actor>
    {
        /// <summary>
        ///     Name of the <see cref="Id" /> property.
        /// </summary>
        private const string IdName = "Id";

        /// <summary>
        ///     Name of the <see cref="Role" /> property.
        /// </summary>
        private const string RoleName = "Role";

        /// <summary>
        ///     Gets or sets the id of the actor.
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        ///     Gets or sets the path of the actors image.
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        ///     Gets or sets the real name of the actor.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the role the actor is playing.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        ///     Gets or sets the number the actors are sorted.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// </summary>
        public static string RoleName1 => RoleName;

        /// <summary>
        ///     Compares the <see cref="SortOrder" /> property of the provided actor and this.
        /// </summary>
        /// <param name="other">Actor to compare.</param>
        /// <returns>Sort indicator.</returns>
        public int CompareTo(Actor other)
        {
            if (other.SortOrder < SortOrder) return -1;
            return other.SortOrder > SortOrder ? 1 : 0;
        }

        /// <summary>
        ///     Occurs when a property changes its value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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
        ///             /// Xml document that contains all actors.
        ///             ///
        ///         </summary>
        /// 		private XmlDocument actorsDoc = null;
        /// 		
        /// 		/// <summary>
        ///             /// Initializes a new instance of the DocuClass class.
        ///             ///
        ///         </summary>
        /// 		public DocuClass(string extractionPath)
        /// 		{
        /// 			// load actors xml.
        /// 			this.actorsDoc = new XmlDocument();
        /// 			this.actorsDoc.Load(System.IO.Path.Combine(this.extractionPath, "actors.xml"));
        /// 		}
        /// 		
        /// 		/// <summary>
        ///             /// Deserializes all actors of the series.
        ///             ///
        ///         </summary>
        /// 		public List&#60;Actor&#62; DeserializeActors()
        /// 		{
        /// 			List&#60;Actor&#62; actors = new List&#60;Actor&#62;();
        /// 			
        /// 			// load the xml docs second child.
        /// 			XmlNode actorsNode = this.actorsDoc.ChildNodes[1];
        /// 			// deserialize all actors.
        /// 			foreach (XmlNode currentNode in actorsNode)
        /// 			{
        /// 				if (currentNode.Name.Equals("Actor", StringComparison.OrdinalIgnoreCase))
        /// 				{
        /// 					Actor deserializes = new Actor();
        /// 					deserializes.Deserialize(currentNode);
        /// 					actors.Add(deserializes);
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

            foreach (XmlNode currentNode in node.ChildNodes)
            {
                if (currentNode.Name.Equals(IdName, StringComparison.InvariantCultureIgnoreCase))
                {
                    int result;
                    int.TryParse(currentNode.InnerText, out result);
                    Id = result;
                }
                else if (currentNode.Name.Equals("Image", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        ImagePath = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("Name", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        Name = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("Role", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerText))
                    {
                        Role = currentNode.InnerText;
                    }
                }
                else if (currentNode.Name.Equals("SortOrder", StringComparison.InvariantCultureIgnoreCase))
                {
                    int result;
                    int.TryParse(currentNode.InnerText, out result);
                    SortOrder = result;
                }
            }
        }
    }
}