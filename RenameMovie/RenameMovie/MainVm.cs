using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Relays;
using RenameMovie.Annotations;

namespace RenameMovie
{
    public class MainVm : INotifyPropertyChanged
    {
        public MainVm()
        {
            FocusFolder = @"Y:\P\_Lib_Movies_Sorter";
            RefreshFolder = new Relays.RelayCommand(DoRefreshFolder, o => true);
            MoveFocus = new Relays.RelayCommand(DoMoveFocus, o => true);
            FolderArrayPoint = 0;
        }

        private void DoMoveFocus(object obj)
        {
            var o = obj.ToString().ToLower();
            switch (o)
            {
                case "+":
                    {
                        FolderArrayPoint++;
                        break;
                    }
                case "-":
                    {
                        FolderArrayPoint--;
                        break;
                    }
                case "0":
                    {
                        FolderArrayPoint = 0;
                        break;

                    }
                case "9":
                    {
                        FolderArrayPoint = FolderArray.Count - 1;
                        break;
                    }
                case "r":
                    {
                        FolderArray = Directory.GetDirectories(FocusFolder).Select(Path.GetFileName).ToList();
                        break;
                    }
            }

            if (FolderArrayPoint >= FolderArray.Count) FolderArrayPoint = FolderArray.Count - 1;
            if (FolderArrayPoint < 0) FolderArrayPoint = 0;
            FocusDirectory = FolderArray[FolderArrayPoint];
            OnPropertyChanged(nameof(FocusDirectory));
            OnPropertyChanged(nameof(FolderArray));
            OnPropertyChanged(nameof(FolderArrayPoint));
        }

        public RelayCommand MoveFocus { get; set; }


        private void DoRefreshFolder(object obj)
        {

        }

        public List<string> FolderArray { get; set; }
        public int FolderArrayPoint { get; set; }
        public string FocusDirectory { get; set; }
        public RelayCommand RefreshFolder { get; set; }

        public string FocusFolder { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
