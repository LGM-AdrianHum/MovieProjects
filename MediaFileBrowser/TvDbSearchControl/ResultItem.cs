using System;
using PropertyChanged;
using TVDB.Model;

namespace TvDbSearchControl
{
    [ImplementPropertyChanged]
    public class ResultItem
    {
        public ResultItem()
        {
        }

        public ResultItem(Series series)
        {
            Poster = series.Poster;
            Id = series.SeriesId;
            ShowName = series.Name;
            FirstAired = series.FirstAired;
            if (!string.IsNullOrEmpty(series.Overview))
            {
                OverView = series?.Overview?
                    .Replace("\r\n", "|")
                    .Replace("\r", "|")
                    .Replace("\n", "|");
                if (OverView.Contains("|")) OverView = OverView.Substring(0, OverView.IndexOf('|') - 1);
            }

            Status = (series.Status + "     ").Substring(0, 1).ToLower();
        }


        public string Poster { get; set; }

        public int Id { get; set; }
        public string Status { get; set; }
        public string ShowName { get; set; }
        public DateTime? FirstAired { get; set; }
        public string OverView { get; set; }
    }
}