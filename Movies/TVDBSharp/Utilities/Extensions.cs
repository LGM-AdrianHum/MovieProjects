﻿// Author: Adrian Hum
// Project: TVDBSharp/Extensions.cs
// 
// Created : 2015-03-06  16:06 
// Modified: 2015-04-10 16:05)

using System.Xml.Linq;
using System.Xml.Schema;

namespace TVDBSharp.Utilities {
    /// <summary>
    ///     Extension methods used to simplify data extraction.
    /// </summary>
    public static class Extensions {
        /// <summary>
        ///     Retrieves a value from an XML tree representing a show.
        /// </summary>
        /// <param name="doc">XML tree representing a show.</param>
        /// <param name="element">Name of the element with the data.</param>
        /// <returns>Returns the value corresponding to the given element name.</returns>
        /// <exception cref="XmlSchemaException">Thrown when the element doesn't exist or the XML tree is incorrect.</exception>
        public static string GetSeriesData(this XDocument doc, string element)
        {
            var root = doc.Element("Data");
            if (root == null) throw new XmlSchemaException("Could not find element <Data>");
            var xElement = root.Element("Series");
            if (xElement == null) throw new XmlSchemaException("Could not find element <Series>");
            var result = xElement.Element(element);
            if (result == null) throw new XmlSchemaException("Could not find element <" + element + ">");
            return result.Value;
        }

        /// <summary>
        ///     Retrieves a value from an XML tree.
        /// </summary>
        /// <param name="xmlObject">The given XML (sub)tree.</param>
        /// <param name="element">Name of the element with the data.</param>
        /// <returns>Returns the value corresponding to the given element name;</returns>
        /// <exception cref="XmlSchemaException">Thrown when the element doesn't exist.</exception>
        public static string GetXmlData(this XElement xmlObject, string element)
        {
            var result = xmlObject.Element(element);

            return result != null ? result.Value : null;

            // Removed in favor of returning a null value
            // This will allow us to catch a non-existing tag with the null-coalescing operator
            // Never trust the XML provider.

            //throw new XmlSchemaException("Element <" + element + "> could not be found.");
        }
    }
}