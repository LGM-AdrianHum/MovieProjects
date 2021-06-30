using System;
using System.Collections.Generic;
using System.Windows.Forms;
using XbmcJson;
using System.Threading;
using System.Drawing;
using System.IO;

namespace XBMC_Remote
{
    public partial class MainView : Form
    {
        XbmcConnection Xbmc;
        System.Windows.Forms.Timer UpdateTimerOne = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer UpdateTimerThree = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer UpdateTimerTen = new System.Windows.Forms.Timer();
        string CurrentSong = "";
        string Ip, Port, User, Password;
        // Used to differentiate between user changing value and RefreshNowPlaying() changing value.
        bool TrackbarSlide = false;

        public MainView()
        {
            InitializeComponent();
        }

        public void RefreshNowPlaying()
        {
            TBarVolume.Value = Xbmc.Control.GetVolume();

            if (Xbmc.Player.IsAudioPlayerActive())
            {
                Song NowPlaying = Xbmc.AudioPlaylist.GetCurrentItem(new string[] {"title", "artist", "album"} );

                if (NowPlaying.Title != CurrentSong)
                {
                    CurrentSong = NowPlaying.Title;
                    TBNowPlayingSong.Text = NowPlaying.Title;
                    TBNowPlayingArtist.Text = NowPlaying.Artist;
                    TBNowPlayingAlbum.Text = NowPlaying.Album;
                    
                    System.Drawing.Size size = new System.Drawing.Size(32,32);

                    if (NowPlaying.Thumbnail == "")
                    {
                        PBNowPlaying.Image = Image.FromFile(Environment.CurrentDirectory + @"\Data\" + "music-icon.jpg");
                    }
                    else
                    {
                        PBNowPlaying.Image = Xbmc.Files.GetImageFromThumbnail(NowPlaying.Thumbnail);
                    }
                }

                TBNowPlayingRuntime.Text = Xbmc.AudioPlayer.GetTimeFormatted();

                TrackbarSlide = true;
                TBarProgress.Value = (int)Xbmc.AudioPlayer.GetPercentagePlayed();
                TrackbarSlide = false;
             }
            else if (Xbmc.Player.IsVideoPlayerActive())
            {
                PlaylistItem NowPlaying = Xbmc.VideoPlaylist.GetCurrentItemAllFields();

                if (NowPlaying.Thumbnail == "")
                {
                    PBNowPlaying.Image = Image.FromFile(Environment.CurrentDirectory + @"\Data\" + "music-icon.jpg");
                }
                else
                {
                    PBNowPlaying.Image = Xbmc.Files.GetImageFromThumbnail(NowPlaying.Thumbnail);
                }

                TBNowPlayingRuntime.Text = Xbmc.VideoPlayer.GetTimeFormatted();
                TrackbarSlide = true;
                TBarProgress.Value = (int)Xbmc.VideoPlayer.GetPercentagePlayed();
                TrackbarSlide = false;
            }
        }

        public void RefreshSystemStatsThree()
        {
            TBFreeMemory.Text = Xbmc.System_.GetFreeMemory();
            TBFPS.Text = Xbmc.System_.GetFPS().ToString();
        }

        public void RefreshSystemStatsTen()
        {

            TBResolution.Text = Xbmc.System_.GetScreenResolution();
            TBBuild.Text = Xbmc.System_.GetBuildVersion();
            TBUPT.Text = Xbmc.System_.GetUptime();
        }

        public void RefreshMovies()
        {
            DGVMovies.Rows.Clear();

            foreach (Movie m in Xbmc.VideoLibrary.GetMovies())
            {
                DataGridViewRow NewRow = new DataGridViewRow();
                NewRow.CreateCells(DGVMovies, new object[] {m.Label});
                NewRow.Tag = m._id;
                DGVMovies.Rows.Add(NewRow);
            }
        }

        public void RefreshTvShows()
        {
            foreach(TvShow tv in Xbmc.VideoLibrary.GetTvShows())
            {
                TreeNode TvShowNode = new TreeNode(tv.Label);
                TvShowNode.Tag = tv._id;
                TVTvShows.Nodes.Add(TvShowNode);

                foreach(Season sea in Xbmc.VideoLibrary.GetSeasons(tv._id, new string[] {"season"}, null, null, null, null))
                {
                    TreeNode SeasonNode = new TreeNode(sea.Label);
                    SeasonNode.Tag = sea._Season;
                    TvShowNode.Nodes.Add(SeasonNode);

                    foreach(Episode ep in Xbmc.VideoLibrary.GetEpisodes(tv._id, sea._Season, new string[] {"episodeid"}, null, null, null, null))
                    {
                        TreeNode EpisodeNode = new TreeNode(ep.Label);
                        EpisodeNode.Tag = ep._id;
                        SeasonNode.Nodes.Add(EpisodeNode);
                    }
                }
            }

            if (TVTvShows.Nodes.Count >= 1)
            {
                TVTvShows.SelectedNode = TVTvShows.Nodes[0];
            }
        }

        public void RefreshMusic()
        {
            foreach (Artist ar in Xbmc.AudioLibrary.GetArtists())
            {
                TreeNode ArtistNode = new TreeNode(ar.Label);
                ArtistNode.Tag = ar._id;
                TVMusic.Nodes.Add(ArtistNode);

                foreach (Album al in Xbmc.AudioLibrary.GetAlbumsByArtist(ar._id))
                {
                    TreeNode AlbumNode = new TreeNode(al.Label);
                    AlbumNode.Tag = al._id;
                    ArtistNode.Nodes.Add(AlbumNode);

                    foreach (Song s in Xbmc.AudioLibrary.GetSongsByAlbum(al._id))
                    {
                        TreeNode SongNode = new TreeNode(s.Label);
                        SongNode.Tag = s._id;
                        AlbumNode.Nodes.Add(SongNode);
                    }
                }
            }
        }

        public void RefreshAudioPlaylist()
        {
            if (Xbmc.Status.IsConnected)
            {
                DGVMusicPlaylist.Rows.Clear();

                foreach (Song i in Xbmc.AudioPlaylist.GetItems())
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    NewRow.CreateCells(DGVMovies, new object[] { i.Label });
                    DGVMusicPlaylist.Rows.Add(NewRow);
                }
            }
        }

        void UpdateTimerTen_Tick(Object myObject, EventArgs myEventArgs)
        {
            UpdateTimerTen.Stop();
            if (Xbmc.Status.IsConnected)
            {
                RefreshSystemStatsTen();
            }
            UpdateTimerTen.Enabled = true;
        }

        void UpdateTimerThree_Tick(Object myObject, EventArgs myEventArgs)
        {
            UpdateTimerThree.Stop();
            if (Xbmc.Status.IsConnected)
            {
                RefreshSystemStatsThree();  
            }
            UpdateTimerThree.Enabled = true;
        }

        void UpdateTimerOne_Tick(Object myObject, EventArgs myEventArgs)
        {
            UpdateTimerOne.Stop();
            if (Xbmc.Status.IsConnected)
            {
                RefreshNowPlaying();  
            }
            UpdateTimerOne.Enabled = true;
        }

        private void MainView_Load(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + "settings.ini"))
            {
                Ip = sr.ReadLine();
                Port = sr.ReadLine();
                User = sr.ReadLine();
                Password = sr.ReadLine();
            }

            if (Ip == null || Port == null)
            {
                Settings set = new Settings();
                set.ShowDialog();
            }
            else
            {
                Xbmc = new XbmcConnection(Ip, Convert.ToInt32(Port), User, Password);

                UpdateTimerOne.Interval = 1000;
                UpdateTimerThree.Interval = 3000;
                UpdateTimerTen.Interval = 10000;
                UpdateTimerOne.Tick += new EventHandler(UpdateTimerOne_Tick);
                UpdateTimerThree.Tick += new EventHandler(UpdateTimerThree_Tick);
                UpdateTimerTen.Tick += new EventHandler(UpdateTimerTen_Tick);
                UpdateTimerOne.Start();
                UpdateTimerThree.Start();
                UpdateTimerTen.Start();

             if (Xbmc.Status.IsConnected)
               {
                    TBarVolume.Value = Xbmc.Control.GetVolume();
                    RefreshMovies();
                    RefreshTvShows();
                    RefreshMusic();
                    RefreshAudioPlaylist();
               }
            }
        }
          
        private void DGVMovies_SelectionChanged(object sender, EventArgs e)
        {
            Movie m = Xbmc.VideoLibrary.GetMoviesAllFields(null, null, DGVMovies.CurrentRow.Index, DGVMovies.CurrentRow.Index + 1)[0];
            TBMoviesDirector.Text = m.Director;
            TBMoviesWriter.Text = m.Writer;
            TBMoviesStudio.Text = m.Studio;
            TBMoviesGenre.Text = m.Genre;
            TBMoviesYear.Text = m.Year.ToString();
            TBMoviesRuntime.Text = m.Runtime;
            TBMoviesPlot.Text = m.Plot;
            PBMoviesArt.Image = Xbmc.Files.GetImageFromThumbnail(m.Thumbnail);
        }

        private void BTNPlayPause_Click(object sender, EventArgs e)
        {
            if(Xbmc.Player.IsAudioPlayerActive())
                Xbmc.AudioPlayer.PlayPause();

            if (Xbmc.Player.IsVideoPlayerActive())
                Xbmc.VideoPlayer.PlayPause();
        }

        private void BTNStop_Click(object sender, EventArgs e)
        {
            if (Xbmc.Player.IsAudioPlayerActive())
            {
                Xbmc.AudioPlayer.Stop();
                DGVMusicPlaylist.Rows.Clear();
            }

            if (Xbmc.Player.IsVideoPlayerActive())
                Xbmc.VideoPlayer.Stop();
        }

        private void BTNSkipPrevious_Click(object sender, EventArgs e)
        {
            if (Xbmc.Player.IsAudioPlayerActive())
                Xbmc.AudioPlayer.SkipPrevious();

            if (Xbmc.Player.IsVideoPlayerActive())
                Xbmc.VideoPlayer.SkipPrevious();
        }

        private void BTNSkipNext_Click(object sender, EventArgs e)
        {
            if (Xbmc.Player.IsAudioPlayerActive())
                Xbmc.AudioPlayer.SkipNext();

            if (Xbmc.Player.IsVideoPlayerActive())
                Xbmc.VideoPlayer.SkipNext();
        }

        private void BTNRewind_Click(object sender, EventArgs e)
        {
            if (Xbmc.Player.IsAudioPlayerActive())
                Xbmc.AudioPlayer.Rewind();

            if (Xbmc.Player.IsVideoPlayerActive())
                Xbmc.VideoPlayer.Rewind();
        }

        private void BTNFastForward_Click(object sender, EventArgs e)
        {
            if (Xbmc.Player.IsAudioPlayerActive())
                Xbmc.AudioPlayer.Forward();

            if (Xbmc.Player.IsVideoPlayerActive())
                Xbmc.VideoPlayer.Forward();
        }

        private void BTNRecord_Click(object sender, EventArgs e)
        {
            if (Xbmc.Player.IsAudioPlayerActive())
                Xbmc.AudioPlayer.Record();
        }

        private void TVMusic_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                //Artist
                Xbmc.Control.PlayArtist((int)e.Node.Tag);
            }
            else if (e.Node.Level == 1)
            {
                //Album
                Xbmc.Control.PlayAlbum((int)e.Node.Tag);

            }
            else if (e.Node.Level == 2)
            {
                //Song
                Xbmc.Control.PlaySong((int)e.Node.Tag);
            }

            RefreshAudioPlaylist();
        }

        private void DGVMovies_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Xbmc.Control.PlayMovie((int)DGVMovies.CurrentRow.Tag);
        }

        private void TVTvShows_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                // Show
                TvShow tv = Xbmc.VideoLibrary.GetTvShowsAllFields(null, null, e.Node.Index, e.Node.Index + 1)[0];
                TBTvShowsPlot.Text = tv.Plot;
                PBTvShowsArt.SizeMode = PictureBoxSizeMode.Zoom;
                PBTvShowsArt.Image = Xbmc.Files.GetImageFromThumbnail(tv.Thumbnail);
            }

            if (e.Node.Level == 1)
            {
                //Season
                Season sea = Xbmc.VideoLibrary.GetSeasonsAllFields((int)e.Node.Parent.Tag, null, null, e.Node.Index, e.Node.Index + 1)[0];

                if (sea.Thumbnail != "")
                {
                    PBTvShowsArt.SizeMode = PictureBoxSizeMode.Zoom;
                    PBTvShowsArt.Image = Xbmc.Files.GetImageFromThumbnail(sea.Thumbnail);
                }
                else
                {
                    TvShow tv = Xbmc.VideoLibrary.GetTvShows(null, null, null, e.Node.Parent.Index, e.Node.Parent.Index + 1)[0];
                    PBTvShowsArt.SizeMode = PictureBoxSizeMode.Zoom;
                    PBTvShowsArt.Image = Xbmc.Files.GetImageFromThumbnail(tv.Thumbnail);
                }
            }

            if (e.Node.Level == 2)
            {
                //Episode
                Episode ep = Xbmc.VideoLibrary.GetEpisodesAllFields((int)e.Node.Parent.Parent.Tag, (int)e.Node.Parent.Tag, null, null, e.Node.Index, e.Node.Index + 1)[0];
                TBTvShowsPlot.Text = ep.Plot;
                PBTvShowsArt.SizeMode = PictureBoxSizeMode.Zoom;
                PBTvShowsArt.Image = Xbmc.Files.GetImageFromThumbnail(ep.Thumbnail);
            }

        }

        private void TVTvShows_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level == 2)
            {
                //Episode
                Xbmc.Control.PlayEpisode((int)e.Node.Parent.Parent.Tag, (int)e.Node.Parent.Tag, (int)e.Node.Tag);
            }
        }

        private void DGVMusicPlaylist_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Xbmc.AudioPlaylist.Play(DGVMusicPlaylist.CurrentRow.Index);
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            Settings set = new Settings();
            set.ShowDialog();
        }

        private void TBarVolume_ValueChanged(object sender, EventArgs e)
        {
            Xbmc.Control.SetVolume(TBarVolume.Value);
        }

        private void TBarProgress_ValueChanged(object sender, EventArgs e)
        {
            if(!TrackbarSlide)
            if(Xbmc.Player.IsAudioPlayerActive())
                Xbmc.AudioPlayer.SeekPercentage(TBarProgress.Value);
            else if (Xbmc.Player.IsVideoPlayerActive())
                Xbmc.VideoPlayer.SeekPercentage(TBarProgress.Value);
        }
    }
}
