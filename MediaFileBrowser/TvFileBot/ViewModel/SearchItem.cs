using System;
using PropertyChanged;
using TVDB.Model;

namespace TvFileBot.ViewModel
{
    [ImplementPropertyChanged]
    public class SearchItem
    {
        public SearchItem(Series s)
        {
            Id = s.Id;
            Title = s.Name;
            Overview = s.Overview;
            FirstAired = s.FirstAired;
            Status = s.Status;
        }

        public int Id { get; set; }

        public string Status { get; set; }

        public DateTime FirstAired { get; set; }

        public string Overview { get; set; }

        public string Title { get; set; }
    }
}