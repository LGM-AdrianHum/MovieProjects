using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EpisodeViewer.Controls;
using PropertyChanged;

namespace EpisodeViewer
{
    [ImplementPropertyChanged]
    public class PutAwayVm
    {
        private VideoDirectoryData _fileVm;

        public PutAwayVm()
        {

            FileVm.SelectionMade += FileVm_SelectionMade;
        }

        private void FileVm_SelectionMade(object sender, EventArgs e)
        {
            for(var i=1; i<FileVm.SelectedVideoItem.SearchKey.Length;i++)
            {
                var m = FileVm.SelectedVideoItem.SearchKey.Substring(0, i);
                var l=TvVm.AllDirectories.Where(x => x.SearchKey.StartsWith(m));
                var any = l.Any();
                if (any) TvVm.SelectedDirectory = l.FirstOrDefault();
                else break;
            }
            MessageBox.Show(TvVm.SelectedDirectory?.Name);
        }

        public TvLibraryData TvVm { get; set; }

        public VideoDirectoryData FileVm
        {
            get { return _fileVm; }
            set { _fileVm = value; }
        }
    }
}
