using Ini;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Vicks_Music_Player_2.Classes;

namespace Vicks_Music_Player_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// Playlist playlistdata = (Playlist)lstMain.SelectedItem;
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Declares
        int CurrentIndex, intRandomSong;
        double CurrentVolume;
        string MediaState, isShuffle;
        private TimeSpan CurrentPos;
        private Uri CurrentFile;
        private System.Timers.Timer CurrentTime = new System.Timers.Timer(500);
        IniFile ini = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\config.ini");
        ObservableCollection<Playlist> PlayListItems = new ObservableCollection<Playlist>();
        #endregion

        #region lstMain
        private void lstMain_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        private void lstMain_Drop(object sender, DragEventArgs e)
        {
            //goes through each file and adds its it to the listbox
            if (e.Data.GetData(DataFormats.FileDrop) != null)
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                    if (File.Exists(file))
                    {
                        AddSongToList(file);
                    }
                    else if (Directory.Exists(file))
                    {
                        AddDirectoryToList(file);
                    }
            }
            else
            {
                //allows for moving songs up and down the playlist
                ListBox parent = sender as ListBox;
                Playlist data = e.Data.GetData(typeof(Playlist)) as Playlist;
                Playlist objectToPlaceBefore = GetObjectDataFromPoint(parent, e.GetPosition(parent)) as Playlist;
                if (data != null && objectToPlaceBefore != null)
                {
                    int index = PlayListItems.IndexOf(objectToPlaceBefore);
                    int lastindex = PlayListItems.IndexOf(data);
                    if (lastindex != index)
                    {
                        //only remove and add if the song was draged and moved
                        PlayListItems.Remove(data);
                        PlayListItems.Insert(index, data);
                        lstMain.SelectedItem = data;
                    }
                    else
                    {
                        //if no song was moved then just select the item
                        lstMain.SelectedItem = data;
                    }

                    if (CurrentIndex == lastindex)
                    {
                        //if the currently playing song is playing then change the currentindex to the new location
                        CurrentIndex = index;
                    }
                }
            }
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        private void lstMain_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            StartPlay();
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        private void lstMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //scrolls to the newely selected item
            lstMain.ScrollIntoView(lstMain.SelectedItem);
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        private void lstMain_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //allows for songs to move up and down as long as the mose isnt ddl clicke or the shift key is being pressed
            if (e.ClickCount == 1 && Keyboard.IsKeyDown(Key.LeftShift) == false)
            {
                ListBox parent = sender as ListBox;
                Playlist data = GetObjectDataFromPoint(parent, e.GetPosition(parent)) as Playlist;
                if (data != null)
                {
                    DragDrop.DoDragDrop(parent, data, DragDropEffects.Move);
                }
            }
        }
        #endregion

        #region Lstmain Context Menu
        private void lstmainEdit_Click(object sender, RoutedEventArgs e)
        {
            if (lstMain.SelectedIndex <= -1) { return; };

            if (lstMain.SelectedItems.Count <= 1)
            {
                //one file selected
                Forms.TagEditor tageditor = new Forms.TagEditor(PlayListItems[lstMain.SelectedIndex].FileName);
                tageditor.Title = PlayListItems[lstMain.SelectedIndex].FileName;
                tageditor.Show();
            }
            else
            {
                //multiple files selected
                string FileList = "";
                foreach (var Song in lstMain.SelectedItems)
                {
                    FileList += PlayListItems[lstMain.Items.IndexOf(Song)].FileName + "¥";
                }
                FileList = FileList.TrimEnd('¥');
                Forms.TagEditor tageditor = new Forms.TagEditor(FileList);
                tageditor.Title = "Multiple Files";
                tageditor.Show();
            }
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        private void lstmainRemove_Click(object sender, RoutedEventArgs e)
        {
            //removes multiple items from the listbox
            if (lstMain.SelectedIndex <= -1) { return; };
            System.Collections.ArrayList Remove = new System.Collections.ArrayList();

            if (lstMain.SelectedItems.Count > 1)
            {
                foreach (var song in lstMain.SelectedItems)
                {
                    Playlist playlistdata = (Playlist)song;
                    Remove.Add(playlistdata);
                }
                foreach (var item in Remove)
                {
                    Playlist playlistdata = (Playlist)item;
                    PlayListItems.Remove(playlistdata);
                }
            }
            else
            {
                //removes a single item
                PlayListItems.RemoveAt(lstMain.SelectedIndex);
            }
        }
        #endregion

        #region Form Events
        private void frmMain_Loaded(object sender, RoutedEventArgs e)
        {
            //adds handlers
            CurrentTime.Elapsed += CurrentTime_Elapsed;
            Forms.TagEditor.ReloadTags += TagEditor_ReloadTags;
            Forms.TagEditor.CloseFile += TagEditor_CloseFile;
            Forms.TagEditor.OpenFile += TagEditor_OpenFile;

            //sets the item source
            lstMain.ItemsSource = PlayListItems;

            //if opened with a file then earse the playlist and play that file
            String[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length >= 2)
            {
                AddSongToList(arguments[1]);
                lstMain.SelectedIndex = 0;
                StartPlay();
            }
            else
            {
                //Loads the last songs
                var database = File.ReadAllLines(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\LastSongs.dat");
                foreach (var song in database)
                {
                    var displayfilelocation = song.Split('¥');
                    PlayListItems.Add(new Playlist(displayfilelocation[0], displayfilelocation[1], Brushes.White));
                }
                //sets the album art image to default
                imageAlbumArt.Source = new BitmapImage(new Uri("Icons/Images/No_Album.jpg", UriKind.Relative));
            }

            //sets the value of current index to -1 to not cause error on first playing file
            CurrentIndex = -1;

            //loads settings
            isShuffle = ini.IniReadValue("Settings", "Shuffle");
            chkShuffle.IsChecked = Convert.ToBoolean(isShuffle);
            CurrentVolume = 1;

            //Sets hotkeys
            HotKey SkipFoward = new HotKey(Key.W, KeyModifier.Alt, HotkeySkipFoward);
            HotKey SkipBackward = new HotKey(Key.Q, KeyModifier.Alt, HotkeySkipBackward);
            //HotKey EditTag = new HotKey(Key.T, KeyModifier.Ctrl, HotkeyEditTag);
            HotKey Search = new HotKey(Key.A, KeyModifier.Alt, HotkeySearch);
        }

        private void frmMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Saves the last songs in the playlist
            using (StreamWriter SW = new StreamWriter(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\LastSongs.dat"))
            {
                foreach (var song in PlayListItems)
                {
                    SW.WriteLine(song.FileName + "¥" + song.ToString());
                }
                SW.Close();
            }

            //stops the player and the timer
            CurrentTime.Stop();
            Player.Stop();

            //save settings when closing the form
            ini.IniWriteValue("Settings", "Shuffle", chkShuffle.IsChecked.Value.ToString());

            //closes all windows with the program
            App.Current.Shutdown();
        }

        private void imageAlbumArt_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //opens up the folder of the currently playing file
            if (CurrentIndex <= -1) { return; }
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                System.Diagnostics.Process.Start(Path.GetDirectoryName(PlayListItems[CurrentIndex].FileName));
            }
        }
        #endregion

        #region Slider
        private void posSlider_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            //Stops the timer when the slider is clicked on
            if (e.LeftButton == MouseButtonState.Pressed) { CurrentTime.Stop(); }
        }

        private void posSlider_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Player.NaturalDuration.HasTimeSpan == true)
            {
                //Moves the song to the postion the slider was drag or clicked on
                TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(posSlider.Value));
                Player.Position = ts;
                lblTime.Content = Player.Position.ToString("mm':'ss") + " - " + Player.NaturalDuration.TimeSpan.ToString("mm':'ss");
                posSlider.SelectionEnd = Player.Position.TotalSeconds;
                CurrentTime.Start();
            }
        }
        #endregion

        #region Functions
        private static object GetObjectDataFromPoint(ListBox source, Point point)
        {
            UIElement element = source.InputHitTest(point) as UIElement;
            if (element != null)
            {
                object data = DependencyProperty.UnsetValue;
                while (data == DependencyProperty.UnsetValue)
                {
                    data = source.ItemContainerGenerator.ItemFromContainer(element);
                    if (data == DependencyProperty.UnsetValue)
                        element = VisualTreeHelper.GetParent(element) as UIElement;
                    if (element == source)
                        return null;
                }
                if (data != DependencyProperty.UnsetValue)
                    return data;
            }

            return null;
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        public void SearchPlay(int Index)
        {
            //plays the song seleted from the search form
            lstMain.SelectedIndex = Index;
            StartPlay();
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        public void SingleInstance(string filename)
        {
            //when opening a file when the music player is already open play the new file
            PlayListItems.Clear();
            AddSongToList(filename);
            lstMain.SelectedIndex = 0;
            StartPlay();
            this.Activate();
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        private void TagEditor_OpenFile(object sender, EventArgs e)
        {
            //reopens the file after saving the tag
            Player.Source = CurrentFile;
            Player.Position = CurrentPos;
            Player.Play();
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        private void TagEditor_CloseFile(object sender, EventArgs e)
        {
            //closes file thats playing so we can save the tags
            CurrentPos = Player.Position;
            CurrentFile = Player.Source;
            Player.Stop();
            Player.Close();
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        private void TagEditor_ReloadTags(object sender, EventArgs e)
        {
            //reloads tags on newly saved items from the tag editor window
            int index = lstMain.SelectedIndex;
            string filename = PlayListItems[index].FileName;
            PlayListItems.RemoveAt(index);

            if (Path.GetExtension(filename).ToLower() == ".mp3")
            {
                //adds the track to the listbox only for MP3's
                TagLib.File mTrack = TagLib.File.Create(filename);
                TagLib.Tag ID32 = mTrack.GetTag(TagLib.TagTypes.Id3v2);

                string time = mTrack.Properties.Duration.ToString().Substring(mTrack.Properties.Duration.ToString().IndexOf(":") + 1, 5);
                string DisplayName = "";
                //if the track has no metadata then add just the filename
                if (ID32.FirstPerformer == null || ID32.Title == null)
                {
                    DisplayName = Path.GetFileName(filename).Substring(0, Path.GetFileName(filename).Length - 4) + " [" + time + "]";
                }
                else
                {
                    DisplayName = ID32.FirstPerformer + " - " + ID32.Title + " [" + time + "]";
                }
                //If the file was playing then make it green and select it otherwise make it white
                if (index == CurrentIndex)
                {
                    PlayListItems.Insert(index, new Playlist(filename, DisplayName, Brushes.PaleGreen));
                    lstMain.SelectedIndex = CurrentIndex;
                }
                else
                {
                    PlayListItems.Insert(index, new Playlist(filename, DisplayName, Brushes.White));
                    lstMain.SelectedIndex = index;
                }

            }
            if (Path.GetExtension(filename).ToLower() == ".flac")
            {
                //adds the track to the listbox only for FLAC's
                TagLib.File mTrack = TagLib.File.Create(filename);
                TagLib.Tag FLAC = mTrack.GetTag(TagLib.TagTypes.FlacMetadata);

                string time = mTrack.Properties.Duration.ToString().Substring(mTrack.Properties.Duration.ToString().IndexOf(":") + 1, 5);
                string DisplayName = "";
                //if the track has no metadata then add just the filename
                if (FLAC.FirstPerformer == null || FLAC.Title == null)
                {
                    DisplayName = Path.GetFileName(filename).Substring(0, Path.GetFileName(filename).Length - 4) + " [" + time + "]";
                }
                else
                {
                    DisplayName = FLAC.FirstPerformer + " - " + FLAC.Title + " [" + time + "]";
                }
                //If the file was playing then make it green and select it otherwise make it white
                if (index == CurrentIndex)
                {
                    PlayListItems.Insert(index, new Playlist(filename, DisplayName, Brushes.PaleGreen));
                    lstMain.SelectedIndex = CurrentIndex;
                }
                else
                {
                    PlayListItems.Insert(index, new Playlist(filename, DisplayName, Brushes.White));
                    lstMain.SelectedIndex = index;
                }
            }
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        private void RandomSong()
        {
            //gets a random number for shuffle no more then the max of the list box
            Random rnd = new Random();
            intRandomSong = rnd.Next(lstMain.Items.Count);
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        private void GetTrackMeta(string filename)
        {
            if (Path.GetExtension(filename).ToLower() == ".mp3")
            {
                //adds meta info for MP3's for the labels
                TagLib.File mTrack = TagLib.File.Create(filename);
                TagLib.Tag ID32 = mTrack.GetTag(TagLib.TagTypes.Id3v2);

                lblArtist.Inlines.Clear();
                lblArtist.Inlines.Add(new Run("Artist: ") { Foreground = Brushes.White });
                lblArtist.Inlines.Add(new Run(Convert.ToString(ID32.FirstPerformer)) { Foreground = Brushes.PaleGreen });
                lblTitle.Inlines.Clear();
                lblTitle.Inlines.Add(new Run("Title: ") { Foreground = Brushes.White });
                lblTitle.Inlines.Add(new Run(Convert.ToString(ID32.Title)) { Foreground = Brushes.PaleGreen });
                lblAlbum.Inlines.Clear();
                lblAlbum.Inlines.Add(new Run("Album: ") { Foreground = Brushes.White });
                lblAlbum.Inlines.Add(new Run(Convert.ToString(ID32.Album)) { Foreground = Brushes.PaleGreen });
                lblYear.Inlines.Clear();
                lblYear.Inlines.Add(new Run("Year: ") { Foreground = Brushes.White });
                lblYear.Inlines.Add(new Run(Convert.ToString(ID32.Year)) { Foreground = Brushes.PaleGreen });
                lblTrackNum.Inlines.Clear();
                lblTrackNum.Inlines.Add(new Run("Track #: ") { Foreground = Brushes.White });
                lblTrackNum.Inlines.Add(new Run(Convert.ToString(ID32.Track)) { Foreground = Brushes.PaleGreen });

                //album art if any
                if (mTrack.Tag.Pictures.Length >= 1)
                {
                    MemoryStream ms = new MemoryStream(mTrack.Tag.Pictures[0].Data.Data);
                    imageAlbumArt.Source = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                }
                else
                {
                    imageAlbumArt.Source = new BitmapImage(new Uri("Icons/Images/No_Album.jpg", UriKind.Relative));
                }

                //sets the title of the player to the title of the song
                this.Title = ID32.FirstPerformer + " - " + ID32.Title;
            }
            if (Path.GetExtension(filename).ToLower() == ".flac")
            {
                //adds meta info for FLAC's for the labels
                TagLib.File mTrack = TagLib.File.Create(filename);
                TagLib.Tag FLAC = mTrack.GetTag(TagLib.TagTypes.FlacMetadata);

                lblArtist.Inlines.Clear();
                lblArtist.Inlines.Add(new Run("Artist: ") { Foreground = Brushes.White });
                lblArtist.Inlines.Add(new Run(Convert.ToString(FLAC.FirstPerformer)) { Foreground = Brushes.PaleGreen });
                lblTitle.Inlines.Clear();
                lblTitle.Inlines.Add(new Run("Title: ") { Foreground = Brushes.White });
                lblTitle.Inlines.Add(new Run(Convert.ToString(FLAC.Title)) { Foreground = Brushes.PaleGreen });
                lblAlbum.Inlines.Clear();
                lblAlbum.Inlines.Add(new Run("Album: ") { Foreground = Brushes.White });
                lblAlbum.Inlines.Add(new Run(Convert.ToString(FLAC.Album)) { Foreground = Brushes.PaleGreen });
                lblYear.Inlines.Clear();
                lblYear.Inlines.Add(new Run("Year: ") { Foreground = Brushes.White });
                lblYear.Inlines.Add(new Run(Convert.ToString(FLAC.Year)) { Foreground = Brushes.PaleGreen });
                lblTrackNum.Inlines.Clear();
                lblTrackNum.Inlines.Add(new Run("Track #: ") { Foreground = Brushes.White });
                lblTrackNum.Inlines.Add(new Run(Convert.ToString(FLAC.Track)) { Foreground = Brushes.PaleGreen });

                //album art if any
                if (mTrack.Tag.Pictures.Length >= 1)
                {
                    MemoryStream ms = new MemoryStream(mTrack.Tag.Pictures[0].Data.Data);
                    imageAlbumArt.Source = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                }
                else
                {
                    imageAlbumArt.Source = new BitmapImage(new Uri("Icons/Images/No_Album.jpg", UriKind.Relative));
                }

                //sets the title of the player to the title of the song
                this.Title = FLAC.FirstPerformer + " - " + FLAC.Title;
            }

            if (Path.GetExtension(filename).ToLower() == ".wav")
            {
                //wav files dont have metadata so no album art
                imageAlbumArt.Source = new BitmapImage(new Uri("Icons/Images/No_Album.jpg", UriKind.Relative));

                //sets the title of the main window
                TagLib.File mTrack = TagLib.File.Create(filename);
                string time = mTrack.Properties.Duration.ToString().Substring(mTrack.Properties.Duration.ToString().IndexOf(":") + 1, 5);
                this.Title = Path.GetFileName(filename).Substring(0, Path.GetFileName(filename).Length - 4) + " [" + time + "]";
            }
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        private void GetTrackInfo(string filename)
        {
            //gets information on the track to display in the label
            TagLib.File mTrack = TagLib.File.Create(filename);
            //lblTrackProp.Content = mTrack.Properties.AudioBitrate + "K, " + mTrack.Properties.AudioSampleRate + "KhZ, " + mTrack.Properties.AudioChannels + " Channels, " + Path.GetExtension(filename).Substring(1).ToUpper();
            this.Title += " / " + mTrack.Properties.AudioBitrate + "kbps";
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        private void AddDirectoryToList(string dirN)
        {
            System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            string[] fNames = null;
            string[] fDirs = new string[1];
            string sS = null;
            string sN = null;
            string[] sPattern = { "*.mp3", "*.flac", "*.wav" };

            fDirs = Directory.GetDirectories(dirN);
            foreach (string sN_loopVariable in fDirs)
            {
                sN = sN_loopVariable;
                AddDirectoryToList(sN);
            }

            foreach (string sS_loopVariable in sPattern)
            {
                sS = sS_loopVariable;
                fNames = Directory.GetFiles(dirN, sS);
                foreach (string sN_loopVariable in fNames)
                {
                    sN = sN_loopVariable;
                    AddSongToList(sN);
                }
            }
            System.Windows.Input.Mouse.OverrideCursor = null;
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        private void AddSongToList(string filename)
        {
            if (Path.GetExtension(filename).ToLower() == ".mp3")
            {
                //adds the track to the listbox only for MP3's
                TagLib.File mTrack = TagLib.File.Create(filename);
                TagLib.Tag ID32 = mTrack.GetTag(TagLib.TagTypes.Id3v2);

                string time = mTrack.Properties.Duration.ToString().Substring(mTrack.Properties.Duration.ToString().IndexOf(":") + 1, 5);
                string DisplayName = "";
                //if the track has no metadata then add just the filename
                if (ID32.FirstPerformer == null || ID32.Title == null)
                {
                    DisplayName = Path.GetFileName(filename).Substring(0, Path.GetFileName(filename).Length - 4) + " [" + time + "]";
                }
                else
                {
                    DisplayName = ID32.FirstPerformer + " - " + ID32.Title + " [" + time + "]";
                }


                PlayListItems.Add(new Playlist(filename, DisplayName, Brushes.White));

            }
            if (Path.GetExtension(filename).ToLower() == ".flac")
            {
                //adds the track to the listbox only for FLAC's
                TagLib.File mTrack = TagLib.File.Create(filename);
                TagLib.Tag FLAC = mTrack.GetTag(TagLib.TagTypes.FlacMetadata);

                string time = mTrack.Properties.Duration.ToString().Substring(mTrack.Properties.Duration.ToString().IndexOf(":") + 1, 5);
                string DisplayName = "";
                //if the track has no metadata then add just the filename
                if (FLAC.FirstPerformer == null || FLAC.Title == null)
                {
                    DisplayName = Path.GetFileName(filename).Substring(0, Path.GetFileName(filename).Length - 4) + " [" + time + "]";
                }
                else
                {
                    DisplayName = FLAC.FirstPerformer + " - " + FLAC.Title + " [" + time + "]";
                }

                PlayListItems.Add(new Playlist(filename, DisplayName, Brushes.White));
            }

            if (Path.GetExtension(filename).ToLower() == ".wav")
            {
                TagLib.File mTrack = TagLib.File.Create(filename);

                string time = mTrack.Properties.Duration.ToString().Substring(mTrack.Properties.Duration.ToString().IndexOf(":") + 1, 5);
                string DisplayName = Path.GetFileName(filename).Substring(0, Path.GetFileName(filename).Length - 4) + " [" + time + "]"; ;

                PlayListItems.Add(new Playlist(filename, DisplayName, Brushes.White));
            }
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        private void StartPlay()
        {
            //if no item is selected dont run code
            if (lstMain.SelectedIndex <= -1) { return; };

            //changes the color of the last playing item
            Playlist playlistdata = (Playlist)lstMain.SelectedItem;
            if (CurrentIndex != -1 && lstMain.Items.Count > 1 && CurrentIndex != lstMain.SelectedIndex)
            {
                playlistdata = (Playlist)lstMain.Items[CurrentIndex];
                PlayListItems.RemoveAt(CurrentIndex);
                PlayListItems.Insert(CurrentIndex, new Playlist(playlistdata.FileName, playlistdata.Title, Brushes.White));
                CurrentTime.Stop();
            }

            //changes the color of the currently playing item
            playlistdata = (Playlist)lstMain.SelectedItem;
            CurrentIndex = lstMain.SelectedIndex;
            PlayListItems.RemoveAt(CurrentIndex);
            PlayListItems.Insert(CurrentIndex, new Playlist(playlistdata.FileName, playlistdata.Title, Brushes.PaleGreen));
            lstMain.SelectedIndex = CurrentIndex;

            //gets track metadata
            GetTrackMeta(playlistdata.FileName);

            //gets track info
            GetTrackInfo(playlistdata.FileName);

            //sets the media state to playing
            MediaState = "Play";

            //changes the play button to show a pause icon
            BitmapImage image = new BitmapImage(new Uri("/Icons/Media/pause.png", UriKind.Relative));
            imgPlay.Source = image;

            //starts the timer to see the current position of the file
            lblTime.Content = "00:00 - 00:00";
            CurrentTime.Stop();
            CurrentTime.Start();

            //gets the currently selected file to play and plays it
            Uri Source = new Uri(playlistdata.GetFileName());
            Player.Source = (Source);
            Player.Play();
        }
        #endregion

        #region Media Events
        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            //sets the media volume
            System.Threading.Thread.Sleep(1);
            Player.Volume = .9;
            System.Threading.Thread.Sleep(1);
            Player.Volume = CurrentVolume;
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            btnNext_Click(sender, e);
        }
        private void CurrentTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //displays the current timer positions
            this.Dispatcher.Invoke((Action)(() =>
            {
                if (Player.NaturalDuration.HasTimeSpan == true)
                {
                    lblTime.Content = Player.Position.ToString("mm':'ss") + " - " + Player.NaturalDuration.TimeSpan.ToString("mm':'ss");
                    posSlider.Maximum = Player.NaturalDuration.TimeSpan.TotalSeconds;
                    posSlider.Value = Player.Position.TotalSeconds;
                    posSlider.SelectionEnd = Player.Position.TotalSeconds;
                }
            }));
        }
        #endregion

        #region Menu Functions
        private void menuRename_Click(object sender, RoutedEventArgs e)
        {
            Forms.frmRenamer renamer = new Forms.frmRenamer();
            renamer.Show();
        }

        private void menuClear_Click(object sender, RoutedEventArgs e)
        {
            PlayListItems.Clear();
        }

        private void menuLibrary_Click(object sender, RoutedEventArgs e)
        {
            PlayListItems.Clear();
            LoadLibrary();
        }

        private void chkShuffle_Click(object sender, RoutedEventArgs e)
        {
            //sets the shuffle state on/off
            isShuffle = chkShuffle.IsChecked.Value.ToString();
        }

        private void menuVol100_Click(object sender, RoutedEventArgs e)
        {
            //sets the player volume
            CurrentVolume = 1;
            Player.Volume = 1;
        }

        private void menuVol90_Click(object sender, RoutedEventArgs e)
        {
            //sets the player volume
            CurrentVolume = .9;
            Player.Volume = .9;
        }

        private void menuVol80_Click(object sender, RoutedEventArgs e)
        {
            //sets the player volume
            CurrentVolume = .8;
            Player.Volume = .8;
        }

        private void menuVol70_Click(object sender, RoutedEventArgs e)
        {
            //sets the player volume
            CurrentVolume = .7;
            Player.Volume = .7;
        }

        private void menuVol60_Click(object sender, RoutedEventArgs e)
        {
            //sets the player volume
            CurrentVolume = .6;
            Player.Volume = .6;
        }

        private void menuVol50_Click(object sender, RoutedEventArgs e)
        {
            //sets the player volume
            CurrentVolume = .5;
            Player.Volume = .5;
        }

        private void menuVol40_Click(object sender, RoutedEventArgs e)
        {
            //sets the player volume
            CurrentVolume = .4;
            Player.Volume = .4;
        }

        private void menuVol30_Click(object sender, RoutedEventArgs e)
        {
            //sets the player volume
            CurrentVolume = .3;
            Player.Volume = .3;
        }

        private void menuVol20_Click(object sender, RoutedEventArgs e)
        {
            //sets the player volume
            CurrentVolume = .2;
            Player.Volume = .2;
        }

        private void menuVol10_Click(object sender, RoutedEventArgs e)
        {
            //sets the player volume
            CurrentVolume = .1;
            Player.Volume = .1;
        }
        #endregion

        #region ToolBar Events
        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness();
            }
        }

        private void btnPlay_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (MediaState == "Play")
            {
                BitmapImage image = new BitmapImage(new Uri("/Icons/Media/pause_hover.png", UriKind.Relative));
                imgPlay.Source = image;
            }
            else if (MediaState == null || MediaState == "Stop")
            {
                BitmapImage image = new BitmapImage(new Uri("/Icons/Media/pause_hover.png", UriKind.Relative));
                imgPlay.Source = image;
            }
            else
            {
                BitmapImage image = new BitmapImage(new Uri("/Icons/Media/play_hover.png", UriKind.Relative));
                imgPlay.Source = image;
            }
        }

        private void btnPlay_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (MediaState == "Play")
            {
                BitmapImage image = new BitmapImage(new Uri("/Icons/Media/pause.png", UriKind.Relative));
                imgPlay.Source = image;
            }
            else if (MediaState == null || MediaState == "Stop")
            {
                BitmapImage image = new BitmapImage(new Uri("/Icons/Media/pause.png", UriKind.Relative));
                imgPlay.Source = image;
            }
            else
            {
                BitmapImage image = new BitmapImage(new Uri("/Icons/Media/play.png", UriKind.Relative));
                imgPlay.Source = image;
            }
        }

        private void btnPlay_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MediaState == "Play")
            {
                BitmapImage image = new BitmapImage(new Uri("/Icons/Media/pause_pressed.png", UriKind.Relative));
                imgPlay.Source = image;
            }
            else
            {
                BitmapImage image = new BitmapImage(new Uri("/Icons/Media/play_pressed.png", UriKind.Relative));
                imgPlay.Source = image;
            }

        }

        private void btnPlay_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MediaState == "Play")
            {
                BitmapImage image = new BitmapImage(new Uri("/Icons/Media/play_hover.png", UriKind.Relative));
                imgPlay.Source = image;
            }
            else
            {
                BitmapImage image = new BitmapImage(new Uri("/Icons/Media/pause_hover.png", UriKind.Relative));
                imgPlay.Source = image;
            }

        }

        private void btnStop_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri("/Icons/Media/Stop_hover.png", UriKind.Relative));
            imgStop.Source = image;
        }

        private void btnStop_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri("/Icons/Media/Stop.png", UriKind.Relative));
            imgStop.Source = image;
        }

        private void btnStop_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri("/Icons/Media/Stop_pressed.png", UriKind.Relative));
            imgStop.Source = image;
        }

        private void btnStop_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri("/Icons/Media/Stop_hover.png", UriKind.Relative));
            imgStop.Source = image;
        }

        private void btnBack_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri("/Icons/Media/Back_hover.png", UriKind.Relative));
            imgBack.Source = image;
        }

        private void btnBack_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri("/Icons/Media/Back.png", UriKind.Relative));
            imgBack.Source = image;
        }

        private void btnBack_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri("/Icons/Media/Back_pressed.png", UriKind.Relative));
            imgBack.Source = image;
        }

        private void btnBack_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri("/Icons/Media/Back_hover.png", UriKind.Relative));
            imgBack.Source = image;
        }

        private void btnNext_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri("/Icons/Media/Next_hover.png", UriKind.Relative));
            imgNext.Source = image;
        }

        private void btnNext_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri("/Icons/Media/Next.png", UriKind.Relative));
            imgNext.Source = image;
        }

        private void btnNext_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri("/Icons/Media/Next_pressed.png", UriKind.Relative));
            imgNext.Source = image;
        }

        private void btnNext_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri("/Icons/Media/Next_hover.png", UriKind.Relative));
            imgNext.Source = image;
        }
        #endregion

        #region Media Buttons
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (lstMain.Items.Count <= 0) { return; }

            //unpause the player
            if (MediaState == "Pause")
            {
                Player.Play();
                CurrentTime.Start();
                MediaState = "Play";
            }
            else
            {
                //pause the player
                Player.Pause();
                CurrentTime.Stop();
                MediaState = "Pause";
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            Player.Stop();
            Player.Close();
            CurrentTime.Stop();
            MediaState = "Stop";
            lblTime.Content = "00:00 - 00:00";
            posSlider.Value = 0;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (lstMain.Items.Count <= 0) { return; }

            if (CurrentIndex == 0)
            {
                lstMain.SelectedIndex = lstMain.Items.Count - 1;
            }
            else
            {
                lstMain.SelectedIndex = CurrentIndex - 1;
                StartPlay();
            }

            //start playing
            StartPlay();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (lstMain.Items.Count <= 0) { return; }

            //if shuffle is on then pick a random number and play that song otherwise run the normal code
            if (isShuffle == "True")
            {
                RandomSong();
                lstMain.SelectedIndex = intRandomSong;
                StartPlay();
            }
            else
            {
                if (CurrentIndex == lstMain.Items.Count - 1)
                {
                    //if its the last song in the list stop playing
                    btnStop_Click(sender, e);
                }
                else
                {
                    lstMain.SelectedIndex = CurrentIndex + 1;
                    StartPlay();
                }
            }
        }
        #endregion

        #region Library
        private void SortLibrary()
        {
            //adds all the playlist items to a list
            List<String> LibrarySort = new List<String>();
            foreach (var song in PlayListItems)
            {
                LibrarySort.Add(song.FileName + "¥" + song.ToString());
            }
            //sorts the list
            LibrarySort.Sort();

            //clears and reloads the list
            PlayListItems.Clear();
            foreach (var song in LibrarySort)
            {
                var displayfilelocation = song.Split('¥');
                PlayListItems.Add(new Playlist(displayfilelocation[0], displayfilelocation[1], Brushes.White));
                //adds 
            }

            //saves the new database file
            using (StreamWriter SW = new StreamWriter(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Database.dat"))
            {
                foreach (var song in LibrarySort)
                {
                    SW.WriteLine(song);
                }
                SW.Close();
            }
        }

        private void LoadLibrary()
        {
            //if there is no database create one
            if (File.Exists(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Database.dat") == false)
            {
                AddDirectoryToList(ini.IniReadValue("Settings", "Library"));
                using (StreamWriter SW = new StreamWriter(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Database.dat"))
                {
                    foreach (var song in PlayListItems)
                    {
                        SW.WriteLine(song.FileName + "¥" + song.ToString());
                    }
                    SW.Close();
                }
            }
            //if there is a database just update it then add all the items to the list using the database
            else
            {
                //first clear out all missing files from the database
                var database = File.ReadAllLines(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Database.dat");
                List<String> databaselist = new List<String>();
                //adds the database file to a list
                foreach (var song in database)
                {
                    databaselist.Add(song);
                }
                //goes through the database to check if each file exist if not remove it from the list
                List<String> DatabaseRemove = new List<String>();
                for (int i = 0; i < databaselist.Count; i++)
                {
                    var filename = databaselist[i].Split('¥');
                    if (File.Exists(filename[0]) == false)
                    {
                        DatabaseRemove.Add(databaselist[i]);
                    }
                }
                foreach (var item in DatabaseRemove)
                {
                    databaselist.Remove(item);
                }

                //goes through all the files in the library folder, if the file isn't in the database then add it to the database list
                var AllFiles = DirSearch(ini.IniReadValue("Settings", "Library"));
                //splits the database to file location and display name
                List<String> databaselistsplit = new List<String>();
                foreach (var item in databaselist)
                {
                    var filelocation = item.Split('¥');
                    databaselistsplit.Add(filelocation[0]);
                }
                //checks if the files are in the database if not add them to the main list box
                foreach (var song in AllFiles)
                {
                    if (databaselistsplit.Contains(song) == false)
                    {
                        AddSongToList(song);
                    }
                }

                //now add all the files from the database
                foreach (var song in databaselist)
                {
                    var displayfilelocation = song.Split('¥');
                    PlayListItems.Add(new Playlist(displayfilelocation[0], displayfilelocation[1], Brushes.White));
                }

                //sorts the library and then saves it
                SortLibrary();
            }
        }

        private List<String> DirSearch(string sDir)
        {
            List<String> files = new List<String>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(DirSearch(d));
                }
            }
            catch (System.Exception excpt)
            {
                MessageBox.Show(excpt.Message);
            }

            return files;
        }
        #endregion

        #region Hotkeys
        private void HotkeySearch(HotKey hotKey)
        {
            Forms.frmSearch search = new Forms.frmSearch(lstMain.Items);
            search.Show();
        }

        private void HotkeySkipFoward(HotKey hotKey)
        {
            Player.Position += TimeSpan.FromSeconds(5);
        }

        private void HotkeySkipBackward(HotKey hotKey)
        {
            Player.Position -= TimeSpan.FromSeconds(5);
        }
        private void HotkeyEditTag(HotKey hotKey)
        {
            if (lstMain.SelectedIndex <= -1) { return; };
            Forms.TagEditor tageditor = new Forms.TagEditor(PlayListItems[lstMain.SelectedIndex].FileName);
            tageditor.Title = PlayListItems[lstMain.SelectedIndex].FileName;
            tageditor.Show();
        }
        #endregion

    }
}