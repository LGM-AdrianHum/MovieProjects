// Author: Adrian Hum
// Project: Movies/SearcherViewModel.cs
// 
// Created : 2015-10-03  06:16 
// Modified: 2015-10-03 06:16)

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using PropertyChanged;

namespace Movies
{
    [ImplementPropertyChanged]
    public class SearcherViewModel
    {
        public SearcherViewModel()
        {
            AllTvShows = Directory.GetDirectories(@"\\admin-pc\H\MyTv").Select(x => Path.GetFileName(x)).ToList();
        }

        public FileSearchType FileSearchType { get; set; }

        public string SearchTypeName
        {
            get { return FileSearchType.ToString(); }
        }

        public string OperationsFilePath { get; set; }

        public string OperationsFile
        {
            get { return string.IsNullOrEmpty(OperationsFilePath) ? "" : Path.GetFileName(OperationsFilePath); }
        }

        public List<string> AllTvShows { get; set; }
        public string SelectedTvShow { get; set; }
        public string TvSeriesName { get; set; }

        public Visibility TvEpisodeSelected
        {
            get
            {
                if (!string.IsNullOrEmpty(TvSeriesName)|| FileSearchType==FileSearchType.Undecided) return Visibility.Collapsed;
                
                return (FileSearchType == FileSearchType.TvDb) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}