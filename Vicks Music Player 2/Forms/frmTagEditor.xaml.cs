using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Vicks_Music_Player_2.Forms
{
    /// <summary>
    /// Interaction logic for frmTagEditor.xaml
    /// </summary>
    public partial class TagEditor : Window
    {

        public TagEditor(string FileName)
        {
            InitializeComponent();
            this.filename = FileName;
        }

        #region Declares
        private bool IsOpen;
        public string filename { get; set; }
        public static event EventHandler ReloadTags;
        public static event EventHandler CloseFile;
        public static event EventHandler OpenFile;
        #endregion

        #region LoadData
        private void frmTagEditor_Loaded(object sender, RoutedEventArgs e)
        {
            //checks to see if multiple files are loaded or not
            if (filename.Contains("¥")) { LoadMultiData(); } else { LoadData(); }
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        private void LoadMultiData()
        {
            var FileList = filename.Split('¥');
            ArrayList TrackNum = new ArrayList();
            ArrayList TrackNumTotal = new ArrayList();
            ArrayList Artist = new ArrayList();
            ArrayList Title = new ArrayList();
            ArrayList Album = new ArrayList();
            ArrayList AlbumArtist = new ArrayList();
            ArrayList Year = new ArrayList();
            ArrayList Genre = new ArrayList();
            ArrayList Comment = new ArrayList();
            bool TrackNumBool = true;
            bool TrackNumTotalBool = true;
            bool ArtistBool = true;
            bool TitleBool = true;
            bool AlbumBool = true;
            bool AlbumArtistBool = true;
            bool YearBool = true;
            bool GenreBool = true;
            bool CommentBool = true;
            //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
            if (Path.GetExtension(FileList[0]).ToLower() == ".mp3")
            {
                foreach (var Song in FileList)
                {
                    TagLib.File mTrack = TagLib.File.Create(Song);
                    TagLib.Tag ID32 = mTrack.GetTag(TagLib.TagTypes.Id3v2);

                    //Checks the tags to compare them. If their empty add a bullshit string to avoid errors
                    if (ID32.Track.ToString() == null) { TrackNum.Add(""); } else { TrackNum.Add(ID32.Track.ToString()); }
                    if (ID32.TrackCount.ToString() == null) { TrackNumTotal.Add(""); } else { TrackNumTotal.Add(ID32.TrackCount.ToString()); }
                    if (ID32.FirstPerformer == null) { Artist.Add(""); } else { Artist.Add(ID32.FirstPerformer); }
                    if (ID32.Title == null) { Title.Add(""); } else { Title.Add(ID32.Title); }
                    if (ID32.Album == null) { Album.Add(""); } else { Album.Add(ID32.Album); }
                    if (ID32.FirstAlbumArtist == null) { AlbumArtist.Add(""); } else { AlbumArtist.Add(ID32.FirstAlbumArtist); }
                    if (ID32.Year.ToString() == null) { Year.Add(""); } else { Year.Add(ID32.Year.ToString()); }
                    if (ID32.FirstGenre == null) { Genre.Add(""); } else { Genre.Add(ID32.FirstGenre); }
                    if (ID32.Comment == null) { Comment.Add(""); } else { Comment.Add(ID32.Comment); }
                }
                //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
                //checks track numbers
                for (int i = 1; i < TrackNum.Count; i++)
                {
                    string Check = TrackNum[0].ToString();
                    if (Check != TrackNum[i].ToString()) { TrackNumBool = false; }
                }
                //checks total tracks
                for (int i = 1; i < TrackNumTotal.Count; i++)
                {
                    string Check = TrackNumTotal[0].ToString();
                    if (Check != TrackNumTotal[i].ToString()) { TrackNumTotalBool = false; }
                }
                //checks artist
                for (int i = 1; i < Artist.Count; i++)
                {
                    string Check = Artist[0].ToString();
                    if (Check != Artist[i].ToString()) { ArtistBool = false; }
                }
                //checks title
                for (int i = 1; i < Title.Count; i++)
                {
                    string Check = Title[0].ToString();
                    if (Check != Title[i].ToString()) { TitleBool = false; }
                }
                //checks album
                for (int i = 1; i < Album.Count; i++)
                {
                    string Check = Album[0].ToString();
                    if (Check != Album[i].ToString()) { AlbumBool = false; }
                }
                //checks album artist
                for (int i = 1; i < AlbumArtist.Count; i++)
                {
                    string Check = AlbumArtist[0].ToString();
                    if (Check != AlbumArtist[i].ToString()) { AlbumArtistBool = false; }
                }
                //checks year
                for (int i = 1; i < Year.Count; i++)
                {
                    string Check = Year[0].ToString();
                    if (Check != Year[i].ToString()) { YearBool = false; }
                }
                //checks genre
                for (int i = 1; i < Genre.Count; i++)
                {
                    string Check = Genre[0].ToString();
                    if (Check != Genre[i].ToString()) { GenreBool = false; }
                }
                //checks comment
                for (int i = 1; i < Comment.Count; i++)
                {
                    string Check = Comment[0].ToString();
                    if (Check != Comment[i].ToString()) { CommentBool = false; }
                }
                //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
                //If the values are the same thing display them
                if (TrackNumBool == true) { txtTrackNum.Text = TrackNum[0].ToString(); }
                if (TrackNumTotalBool == true) { txtTrackNumTotal.Text = TrackNumTotal[0].ToString(); }
                if (ArtistBool == true) { txtArtist.Text = Artist[0].ToString(); }
                if (TitleBool == true) { txtTitle.Text = Title[0].ToString(); }
                if (AlbumBool == true) { txtAlbum.Text = Album[0].ToString(); }
                if (AlbumArtistBool == true) { txtAlbumArtist.Text = AlbumArtist[0].ToString(); }
                if (YearBool == true) { txtYear.Text = Year[0].ToString(); }
                if (GenreBool == true) { txtGenre.Text = Genre[0].ToString(); }
                if (CommentBool == true) { txtComment.Text = Comment[0].ToString(); }
            }

            if (Path.GetExtension(filename).ToLower() == ".flac")
            {
                foreach (var Song in FileList)
                {
                    TagLib.File mTrack = TagLib.File.Create(Song);
                    TagLib.Tag FLAC = mTrack.GetTag(TagLib.TagTypes.FlacMetadata);

                    //Checks the tags to compare them. If their empty add a bullshit string to avoid errors
                    if (FLAC.Track.ToString() == null) { TrackNum.Add(""); } else { TrackNum.Add(FLAC.Track.ToString()); }
                    if (FLAC.TrackCount.ToString() == null) { TrackNumTotal.Add(""); } else { TrackNumTotal.Add(FLAC.TrackCount.ToString()); }
                    if (FLAC.FirstPerformer == null) { Artist.Add(""); } else { Artist.Add(FLAC.FirstPerformer); }
                    if (FLAC.Title == null) { Title.Add(""); } else { Title.Add(FLAC.Title); }
                    if (FLAC.Album == null) { Album.Add(""); } else { Album.Add(FLAC.Album); }
                    if (FLAC.FirstAlbumArtist == null) { AlbumArtist.Add(""); } else { AlbumArtist.Add(FLAC.FirstAlbumArtist); }
                    if (FLAC.Year.ToString() == null) { Year.Add(""); } else { Year.Add(FLAC.Year.ToString()); }
                    if (FLAC.FirstGenre == null) { Genre.Add(""); } else { Genre.Add(FLAC.FirstGenre); }
                    if (FLAC.Comment == null) { Comment.Add(""); } else { Comment.Add(FLAC.Comment); }
                }
                //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
                //checks track numbers
                for (int i = 1; i < TrackNum.Count; i++)
                {
                    string Check = TrackNum[0].ToString();
                    if (Check != TrackNum[i].ToString()) { TrackNumBool = false; }
                }
                //checks total tracks
                for (int i = 1; i < TrackNumTotal.Count; i++)
                {
                    string Check = TrackNumTotal[0].ToString();
                    if (Check != TrackNumTotal[i].ToString()) { TrackNumTotalBool = false; }
                }
                //checks artist
                for (int i = 1; i < Artist.Count; i++)
                {
                    string Check = Artist[0].ToString();
                    if (Check != Artist[i].ToString()) { ArtistBool = false; }
                }
                //checks title
                for (int i = 1; i < Title.Count; i++)
                {
                    string Check = Title[0].ToString();
                    if (Check != Title[i].ToString()) { TitleBool = false; }
                }
                //checks album
                for (int i = 1; i < Album.Count; i++)
                {
                    string Check = Album[0].ToString();
                    if (Check != Album[i].ToString()) { AlbumBool = false; }
                }
                //checks album artist
                for (int i = 1; i < AlbumArtist.Count; i++)
                {
                    string Check = AlbumArtist[0].ToString();
                    if (Check != AlbumArtist[i].ToString()) { AlbumArtistBool = false; }
                }
                //checks year
                for (int i = 1; i < Year.Count; i++)
                {
                    string Check = Year[0].ToString();
                    if (Check != Year[i].ToString()) { YearBool = false; }
                }
                //checks genre
                for (int i = 1; i < Genre.Count; i++)
                {
                    string Check = Genre[0].ToString();
                    if (Check != Genre[i].ToString()) { GenreBool = false; }
                }
                //checks comment
                for (int i = 1; i < Comment.Count; i++)
                {
                    string Check = Comment[0].ToString();
                    if (Check != Comment[i].ToString()) { CommentBool = false; }
                }
                //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
                //If the values are the same thing display them
                if (TrackNumBool == true) { txtTrackNum.Text = TrackNum[0].ToString(); }
                if (TrackNumTotalBool == true) { txtTrackNumTotal.Text = TrackNumTotal[0].ToString(); }
                if (ArtistBool == true) { txtArtist.Text = Artist[0].ToString(); }
                if (TitleBool == true) { txtTitle.Text = Title[0].ToString(); }
                if (AlbumBool == true) { txtAlbum.Text = Album[0].ToString(); }
                if (AlbumArtistBool == true) { txtAlbumArtist.Text = AlbumArtist[0].ToString(); }
                if (YearBool == true) { txtYear.Text = Year[0].ToString(); }
                if (GenreBool == true) { txtGenre.Text = Genre[0].ToString(); }
                if (CommentBool == true) { txtComment.Text = Comment[0].ToString(); }
            }
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        private void LoadData()
        {
            if (Path.GetExtension(filename).ToLower() == ".mp3")
            {
                //loads the MP3 tags
                TagLib.File mTrack = TagLib.File.Create(filename);
                TagLib.Tag ID32 = mTrack.GetTag(TagLib.TagTypes.Id3v2);

                txtTrackNum.Text = ID32.Track.ToString();
                txtTrackNumTotal.Text = ID32.TrackCount.ToString();
                txtArtist.Text = ID32.FirstPerformer;
                txtTitle.Text = ID32.Title;
                txtAlbum.Text = ID32.Album;
                txtAlbumArtist.Text = ID32.FirstAlbumArtist;
                txtYear.Text = ID32.Year.ToString();
                txtGenre.Text = ID32.FirstGenre;
                txtComment.Text = ID32.Comment;
            }

            if (Path.GetExtension(filename).ToLower() == ".flac")
            {
                //loads the FLAC tags
                TagLib.File mTrack = TagLib.File.Create(filename);
                TagLib.Tag FLAC = mTrack.GetTag(TagLib.TagTypes.FlacMetadata);

                txtTrackNum.Text = FLAC.Track.ToString();
                txtTrackNumTotal.Text = FLAC.TrackCount.ToString();
                txtArtist.Text = FLAC.FirstPerformer;
                txtTitle.Text = FLAC.Title;
                txtAlbum.Text = FLAC.Album;
                txtAlbumArtist.Text = FLAC.FirstAlbumArtist;
                txtYear.Text = FLAC.Year.ToString();
                txtGenre.Text = FLAC.FirstGenre;
                txtComment.Text = FLAC.Comment;
            }
        }
        #endregion

        #region Save Data
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (this.Title == "Multiple Files")
            {
                var FileList = filename.Split('¥');
                foreach (var Song in FileList)
                {
                    //if the file is in use then close it so we can save it
                    FileInfo file = new FileInfo(Song);
                    if (IsFileLocked(file) == true)
                    {
                        IsOpen = true;
                        CloseFile(this, e);
                        do
                        {
                            //wait for the file to close
                        } while (IsFileLocked(file) == false);
                    }

                    if (Path.GetExtension(Song).ToLower() == ".mp3")
                    {
                        //saves the MP3 tags
                        TagLib.File mTrack = TagLib.File.Create(Song);
                        TagLib.Tag ID32 = mTrack.GetTag(TagLib.TagTypes.Id3v2);

                        if (txtTrackNum.Text != "") { ID32.Track = Convert.ToUInt16(txtTrackNum.Text); }
                        if (txtTrackNumTotal.Text != "") { ID32.TrackCount = Convert.ToUInt16(txtTrackNumTotal.Text); }
                        if (txtArtist.Text != "") { ID32.Performers = new string[] { txtArtist.Text }; }
                        if (txtTitle.Text != "") { ID32.Title = txtTitle.Text; }
                        if (txtAlbum.Text != "") { ID32.Album = txtAlbum.Text; }
                        if (txtAlbumArtist.Text != "") { ID32.AlbumArtists = new string[] { txtAlbumArtist.Text }; }
                        if (txtYear.Text != "") { ID32.Year = Convert.ToUInt16(txtYear.Text); }
                        if (txtGenre.Text != "") { ID32.Genres = new string[] { txtGenre.Text }; }
                        if (txtComment.Text != "") { ID32.Comment = txtComment.Text; }
                        mTrack.Save();
                    }

                    if (Path.GetExtension(Song).ToLower() == ".flac")
                    {
                        //saves the FLAC tags
                        TagLib.File mTrack = TagLib.File.Create(Song);
                        TagLib.Tag FLAC = mTrack.GetTag(TagLib.TagTypes.FlacMetadata);

                        if (txtTrackNum.Text != "") { FLAC.Track = Convert.ToUInt16(txtTrackNum.Text); }
                        if (txtTrackNumTotal.Text != "") { FLAC.TrackCount = Convert.ToUInt16(txtTrackNumTotal.Text); }
                        if (txtArtist.Text != "") { FLAC.Performers = new string[] { txtArtist.Text }; }
                        if (txtTitle.Text != "") { FLAC.Title = txtTitle.Text; }
                        if (txtAlbum.Text != "") { FLAC.Album = txtAlbum.Text; }
                        if (txtAlbumArtist.Text != "") { FLAC.AlbumArtists = new string[] { txtAlbumArtist.Text }; }
                        if (txtYear.Text != "") { FLAC.Year = Convert.ToUInt16(txtYear.Text); }
                        if (txtGenre.Text != "") { FLAC.Genres = new string[] { txtGenre.Text }; }
                        if (txtComment.Text != "") { FLAC.Comment = txtComment.Text; }
                        mTrack.Save();
                    }
                }
                //reopens the file after the save if it was in use
                if (IsOpen == true) { OpenFile(this, e); }

                //update the playlist with the new tags and close the form
                //ReloadTags(this, e);
                this.Close();
            }
            else
            {
                //if the file is in use then close it so we can save it
                FileInfo file = new FileInfo(filename);
                if (IsFileLocked(file) == true)
                {
                    IsOpen = true;
                    CloseFile(this, e);
                    do
                    {
                        //wait for the file to close
                    } while (IsFileLocked(file) == false);
                }

                if (Path.GetExtension(filename).ToLower() == ".mp3")
                {
                    //saves the MP3 tags
                    TagLib.File mTrack = TagLib.File.Create(filename);
                    TagLib.Tag ID32 = mTrack.GetTag(TagLib.TagTypes.Id3v2);

                    //TagLib.Id3v2.Tag ID3Frame = (TagLib.Id3v2.Tag)mTrack.GetTag(TagLib.TagTypes.Id3v2);
                    //TagLib.Id3v2.TextInformationFrame Track_Frame = TagLib.Id3v2.TextInformationFrame.Get(ID3Frame, "TRCK", true);
                    //if (txtTrackNum.Text == "") { txtTrackNum.Text = "0"; }
                    //if (txtTrackNumTotal.Text == "") { txtTrackNum.Text = "0"; }
                    //string TrackSplit = txtTrackNum.Text + "/" + txtTrackNumTotal.Text;
                    //Track_Frame.Text = new string[] { TrackSplit };

                    if (txtTrackNum.Text == "") { ID32.Track = 0; }
                    if (txtTrackNumTotal.Text == "") { ID32.TrackCount = 0; }
                    ID32.Performers = new string[] { txtArtist.Text };
                    ID32.Title = txtTitle.Text;
                    ID32.Album = txtAlbum.Text;
                    ID32.AlbumArtists = new string[] { txtAlbumArtist.Text };
                    if (txtYear.Text == "") { ID32.Year = 0; }
                    ID32.Genres = new string[] { txtGenre.Text };
                    ID32.Comment = txtComment.Text;

                    mTrack.Save();
                }

                if (Path.GetExtension(filename).ToLower() == ".flac")
                {
                    //saves the FLAC tags
                    TagLib.File mTrack = TagLib.File.Create(filename);
                    TagLib.Tag FLAC = mTrack.GetTag(TagLib.TagTypes.FlacMetadata);

                    if (txtTrackNum.Text == "") { FLAC.Track = 0; }
                    if (txtTrackNumTotal.Text == "") { FLAC.TrackCount = 0; }
                    FLAC.Performers = new string[] { txtArtist.Text };
                    FLAC.Title = txtTitle.Text;
                    FLAC.Album = txtAlbum.Text;
                    FLAC.AlbumArtists = new string[] { txtAlbumArtist.Text };
                    if (txtYear.Text == "") { FLAC.Year = 0; }
                    FLAC.Genres = new string[] { txtGenre.Text };
                    FLAC.Comment = txtComment.Text;

                    mTrack.Save();
                }

                //reopens the file after the save if it was in use
                if (IsOpen == true) { OpenFile(this, e); }

                //update the playlist with the new tags and close the form
                ReloadTags(this, e);
                this.Close();
            }
        }
        #endregion

        #region Functions
        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null) { overflowGrid.Visibility = Visibility.Collapsed; }
            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null) { mainPanelBorder.Margin = new Thickness(); }
        }
        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
        #endregion
    }
}
