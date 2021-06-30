using System.Xml.Serialization;

namespace MediaTvEpisodeBrowser.ViewModel
{
    public class MovieNfoDataSet
    {
        private NfoMovie[] _itemsField;

        /// <remarks />
        [XmlElement("movie")]
        public NfoMovie[] Items
        {
            get { return _itemsField; }
            set { _itemsField = value; }
        }
    }
}