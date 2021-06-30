// -----------------------------------------------------------------------
// <copyright company="Christoph van der Fecht - VDsoft">
// This code can be used in commercial, free and open source projects.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using PropertyChanged;

namespace TVDB.Model
{
    /// <summary>
    ///     Base for the elements of a series.
    /// </summary>
    [ImplementPropertyChanged]
    public abstract class SeriesElement : INotifyPropertyChanged
    {
        /// <summary>
        ///     Name of the <see cref="Id" /> property.
        /// </summary>
        private const string IdName = "Id";

        /// <summary>
        ///     Name of the <see cref="Name" /> property.
        /// </summary>
        private const string NameName = "Name";

        /// <summary>
        ///     Name of the <see cref="Language" /> property.
        /// </summary>
        private const string LanguageName = "Language";

        /// <summary>
        ///     Name of the <see cref="Overview" /> property.
        /// </summary>
        private const string OverviewName = "Overview";

        /// <summary>
        ///     Name of the <see cref="FirstAired" /> property.
        /// </summary>
        private const string FirstAiredName = "FirstAired";

        /// <summary>
        ///     Name of the <see cref="IMDBId" /> property.
        /// </summary>
        private const string IMDBIdName = "IMDBId";

        /// <summary>
        ///     Date the series was first aired.
        /// </summary>
        private DateTime? _firstAired;

        /// <summary>
        ///     Id of the element.
        /// </summary>
        private int id;

        /// <summary>
        ///     IMDB ID fo the series.
        /// </summary>
        private string imdbId;

        /// <summary>
        ///     Language of the element.
        /// </summary>
        private string language;

        /// <summary>
        ///     Name of the element.
        /// </summary>
        private string name;

        /// <summary>
        ///     The overview of the series.
        /// </summary>
        private string overview;

        /// <summary>
        ///     Gets or sets the id of the element.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the name of the element.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the language of the element.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        ///     Gets or sets the overview of the series.
        /// </summary>
        public string Overview { get; set; }

        /// <summary>
        ///     Gets or sets the date the series was first aired.
        /// </summary>
        public DateTime FirstAired
        {
            get { return _firstAired ?? (DateTime) SqlDateTime.MinValue; }

            set
            {
                if (_firstAired != null && value == _firstAired.Value) return;
                _firstAired = value;
                RaisePropertyChanged(nameof(FirstAired));
            }
        }

        /// <summary>
        ///     Gets or sets the IMDB ID fo the series.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string IMDBId { get; set; }

        /// <summary>
        ///     Occurs when a property changes its value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        ///     Raises the <see cref="PropertyChanged" /> event.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed its value.</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Removes all "|" inside the provided text.
        /// </summary>
        /// <param name="text">Text to prepare.</param>
        /// <returns>Clean text.</returns>
        protected string PrepareText(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;

            string result;

            if (text.Contains("|")) result = text.Replace("|", ", ");
            else return text;

            if (result.StartsWith(", ")) result = result.Remove(0, 1).Trim();

            if (result.EndsWith(","))
                result = result.Remove(result.LastIndexOf(",", StringComparison.Ordinal), 1).Trim();

            return result;
        }
    }
}