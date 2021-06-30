using PropertyChanged;

namespace TvFileBot.ViewModel
{
    [ImplementPropertyChanged]
    public class ParseValue
    {
        public string Movie { get; set; }
        public string Year { get; set; }
        public string Resolution { get; set; }
        public string Format { get; set; }
        public string AdditionalText { get; set; }
    }
}