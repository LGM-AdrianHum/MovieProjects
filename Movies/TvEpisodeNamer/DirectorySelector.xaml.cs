// Author: Adrian Hum
// Project: Movies/DirectorySelector.xaml.cs
// 
// Created : 2015-10-02  23:11 
// Modified: 2015-10-02 23:14)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TvEpisodeNamer
{
    /// <summary>
    ///     Interaction logic for DirectorySelector.xaml
    /// </summary>
    public partial class DirectorySelector : INotifyPropertyChanged
    {
        public DirectorySelector()
        {
            InitializeComponent();
        }

        public List<FileFolder> CurrentBase { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!DirectoryBox.IsFocused) return;
            SetWorkingDirectory();
        }

        public void SetWorkingDirectory()
        {
            var r = DirectoryBox.Text;
            if (DirectoryBox.SelectionStart > 0) r = DirectoryBox.Text.Substring(0, DirectoryBox.SelectionStart);

            var brs = lastvalidpath(r);
            var restpattern = r.Substring(brs.Length);
            if (brs.Length > 0)
            {
                CurrentBase =
                    Directory.GetDirectories(brs, restpattern + "*.*")
                        .Union(Directory.GetFiles(brs, restpattern + "*.*"))
                        .OrderBy(x => x)
                        .Select(x => new FileFolder(x))
                        .ToList();

                OnPropertyChanged("CurrentBase");
                DirectoryListBox.ScrollIntoView(DirectoryListBox.SelectedItem);
            }
            else CurrentBase = null;
        }

        public string lastvalidpath(string s)
        {
            if (s.Length < 3) return "";
            var br = s.LastIndexOf("\\", StringComparison.Ordinal);
            if (br <= 0) return "";
            var brs = s.Substring(0, br + 1);

            if (brs.StartsWith(@"\\") && brs.Length > 3)
            {
                if (CheckPath(brs))
                {
                    return brs;
                }
            }
            else if (Directory.Exists(brs))
                return brs;
            return "";
        }

        protected bool CheckPath(string s)
        {
            var uri = new Uri(s);
            if (uri.Segments.Length == 0 || string.IsNullOrWhiteSpace(uri.Host))
                return false;
            if (uri.Segments.Length >= 2)
            {
                return Directory.Exists(s);
            }
            return false;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void DirectoryBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                if (DirectoryBox.Text.EndsWith("\\"))
                {
                    if (DirectoryBox.Text.Length == 3) return;
                    var l = DirectoryBox.Text.Substring(0, DirectoryBox.Text.Length - 1);
                    var q = l.LastIndexOf("\\", StringComparison.Ordinal) + 1;
                    DirectoryBox.Text = l;
                    DirectoryBox.SelectionStart = q;
                    DirectoryBox.SelectionLength = DirectoryBox.Text.Length - q;
                    e.Handled = true;
                }
                DirectoryBox.Focus();
            }

            if (e.Key == Key.Down)
            {
                DirectoryListBox.SelectedIndex++;
                DirectoryListBox.Focus();
                //var l = DirectoryListBox.SelectedValue as FileFolder;
                //if (l == null) return;
                //if(l.Img=="folder") DirectoryBox.Text = l.FullName;
            }
            if (e.Key == Key.Enter)
            {
                if (Directory.Exists(DirectoryBox.Text))
                {
                    SetWorkingDirectory();
                }
            }
        }

        private void DirectoryListBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                if (DirectoryBox.Text.EndsWith("\\"))
                {
                    if (DirectoryBox.Text.Length == 3) return;
                    var l = DirectoryBox.Text.Substring(0, DirectoryBox.Text.Length - 1);
                    var q = l.LastIndexOf("\\", StringComparison.Ordinal) + 1;
                    DirectoryBox.Text = l;
                    DirectoryBox.SelectionStart = q;
                    DirectoryBox.SelectionLength = DirectoryBox.Text.Length - q;
                    e.Handled = true;
                }
                DirectoryBox.Focus();
            }

            if (e.Key == Key.Enter)
            {
                var l = DirectoryListBox.SelectedValue as FileFolder;
                if (l == null) return;
                if (Directory.Exists(l.FullName))
                {
                    DirectoryBox.Text = l.FullName + "\\";
                    DirectoryBox.Focus();
                    DirectoryBox.SelectionStart = DirectoryBox.Text.Length;
                    SetWorkingDirectory();
                }
                else if (File.Exists(l.FullName))
                {
                    if (FileSelected != null) FileSelected(new FileSelectedEventArgs(l.FullName));
                    
                }
            }
        }

        public event FileSelectedEventDelegate FileSelected;

        private void DirectoryListBox_OnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var l = DirectoryListBox.SelectedValue as FileFolder;
            if (l == null) return;
            if (Directory.Exists(l.FullName))
            {
                DirectoryBox.Text = l.FullName + "\\";
                DirectoryBox.Focus();
                DirectoryBox.SelectionStart = DirectoryBox.Text.Length;
                SetWorkingDirectory();
            }
            else if (File.Exists(l.FullName))
            {
                if (FileSelected != null) FileSelected(new FileSelectedEventArgs(l.FullName));

            }
        }
    }

    public delegate void FileSelectedEventDelegate(FileSelectedEventArgs args);

    public class FileSelectedEventArgs : EventArgs {
        public FileSelectedEventArgs(string filefolder)
        {
            FileFolderName = filefolder;
            
        }
        public string FileFolderName { get; private set; }
    }


    
}