using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using PropertyChanged;
using Relays;

namespace EpisodeViewer.ViewModel
{
    [ImplementPropertyChanged]
    public class FilePresenter
    {
        public FilePresenter(FileInfo fi, string showname)
        {
            Fi = fi;
            EpiPresent = new List<EpisodePresenter>();
            ShowName = showname;
            Rename = new RelayCommand(DoRename, o => true);
        }

        public FileInfo Fi { get; set; }

        public RelayCommand Rename { get; set; }

        public string ShowName { get; set; }

        public List<EpisodePresenter> EpiPresent { get; set; }
        public string Name => Fi.Name;
        public string RealName { get; private set; }

        public string SeasEpsNum { get; private set; }

        public string SizeS => Fi.Length.ToBytes();

        public SolidColorBrush CorrectNameColor { get; set; }

        public bool CorrectName { get; set; }
        public bool InCorrectName => !CorrectName;

        private void DoRename(object obj)
        {
            try
            {
                var m = RealName;
                if (Fi.Name.ToLower() == RealName) m = "_" + RealName;
                if (Fi.DirectoryName != null)
                {
                    var mm = Path.Combine(Fi.DirectoryName, m);
                    Fi.MoveTo(mm);
                    if (m != RealName)
                    {
                        if (Fi.DirectoryName != null)
                        {
                            mm = Path.Combine(Fi.DirectoryName, RealName);
                            Fi.MoveTo(mm);
                        }
                    }
                    Fi = new FileInfo(mm);
                }
                CorrectName = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Add(EpisodePresenter ve)
        {
            EpiPresent.Add(ve);
            var seas = EpiPresent.FirstOrDefault()?.SeasonNumber.ToString("00");
            var eps = EpiPresent.Select(x => x.EpisodeNumber.ToString("00")).ToArray();
            var epn = EpiPresent.Select(x => x.Name.ToCleanString()).ToArray();
            SeasEpsNum = $"S{seas}E{string.Join("E", eps)}";
            RealName =
                $"{ShowName.ToCleanString()} - S{seas}E{string.Join("E", eps)} - {string.Join(", ", epn)}{Fi.Extension.ToLower()}";

            CorrectName = RealName == Fi.Name;

            CorrectNameColor = CorrectName ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}