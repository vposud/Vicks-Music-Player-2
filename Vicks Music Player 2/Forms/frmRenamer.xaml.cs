using System.IO;
using System.Windows;

namespace Vicks_Music_Player_2.Forms
{
    /// <summary>
    /// Interaction logic for frmRenamer.xaml
    /// </summary>
    public partial class frmRenamer : Window
    {
        public frmRenamer()
        {
            InitializeComponent();
        }

        private System.Collections.ArrayList FullFileName;

        #region ListBox
        private void lstAlbumArt_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        private void lstAlbumArt_Drop(object sender, DragEventArgs e)
        {
            FullFileName = new System.Collections.ArrayList();
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var file in Directory.GetFiles(files[0]))
            {
                if (Path.GetExtension(file) == ".mp3" || Path.GetExtension(file) == ".flac")
                {
                    //adds the full path
                    FullFileName.Add(file);
                }
            }
            AlbumArt();
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        private void listBox_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        private void listBox_Drop(object sender, DragEventArgs e)
        {
            FullFileName = new System.Collections.ArrayList();
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var file in Directory.GetFiles(files[0]))
            {
                if (Path.GetExtension(file) == ".mp3" || Path.GetExtension(file) == ".flac")
                {
                    //adds the full path
                    FullFileName.Add(file);
                }
            }
            RenameFiles();
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        private void lstAlbumArtRemove_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        private void lstAlbumArtRemove_Drop(object sender, DragEventArgs e)
        {
            FullFileName = new System.Collections.ArrayList();
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var file in Directory.GetFiles(files[0]))
            {
                if (Path.GetExtension(file) == ".mp3" || Path.GetExtension(file) == ".flac")
                {
                    //adds the full path
                    FullFileName.Add(file);
                }
            }
            AlbumArtRemove();
        }
        #endregion

        #region Functions                                                                       
        private void RenameFiles()
        {
            //Renames all files to 00 - Track Title
            foreach (var filename in FullFileName)
            {
                string NewName = "";
                string ext = Path.GetExtension(filename.ToString());
                if (Path.GetExtension(filename.ToString()).ToLower() == ".mp3")
                {
                    //loads the MP3 tags
                    TagLib.File mTrack = TagLib.File.Create(filename.ToString());
                    TagLib.Tag ID32 = mTrack.GetTag(TagLib.TagTypes.Id3v2);

                    NewName = ID32.Track.ToString("00") + " - " + ID32.Title;

                    File.Move(filename.ToString(), Path.GetDirectoryName(filename.ToString()) + "\\" + NewName + ext);
                }

                if (Path.GetExtension(filename.ToString()).ToLower() == ".flac")
                {
                    //loads the FLAC tags
                    TagLib.File mTrack = TagLib.File.Create(filename.ToString());
                    TagLib.Tag FLAC = mTrack.GetTag(TagLib.TagTypes.FlacMetadata);

                    NewName = FLAC.Track.ToString("00") + " - " + FLAC.Title;

                    File.Move(filename.ToString(), Path.GetDirectoryName(filename.ToString()) + "\\" + NewName + ext);
                }
            }
            MessageBox.Show("File Renaming Done!");
            FullFileName.Clear();
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        private void AlbumArt()
        {
            //Gets the album art to save
            string picpath = "";
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.InitialDirectory = Path.GetDirectoryName(FullFileName[0].ToString());
            if (openFileDialog.ShowDialog() == true)
            picpath = openFileDialog.FileName;
            if (picpath == "") { return; }

            foreach (var filename in FullFileName)
            {
                if (Path.GetExtension(filename.ToString()).ToLower() == ".flac" || Path.GetExtension(filename.ToString()).ToLower() == ".mp3")
                {
                    //Saves album art
                    TagLib.File tagFile = TagLib.File.Create(filename.ToString());
                    TagLib.IPicture newArt = new TagLib.Picture(picpath);
                    tagFile.Tag.Pictures = new TagLib.IPicture[1] { newArt };
                    //tagFile.Tag.Pictures = new TagLib.IPicture[0];
                    tagFile.Save();
                }
            }
            MessageBox.Show("Album Art Saved!");
            FullFileName.Clear();
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        private void AlbumArtRemove()
        {
            foreach (var filename in FullFileName)
            {
                if (Path.GetExtension(filename.ToString()).ToLower() == ".flac" || Path.GetExtension(filename.ToString()).ToLower() == ".mp3")
                {
                    //Deletes album art
                    TagLib.File tagFile = TagLib.File.Create(filename.ToString());
                    tagFile.Tag.Pictures = new TagLib.IPicture[0];
                    tagFile.Save();
                }
            }
            MessageBox.Show("Album Art Removed!");
            FullFileName.Clear();
        }
        #endregion

        #region Form Events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listBox.Items.Add("Renamer");
            lstAlbumArt.Items.Add("Add Album Art");
            lstAlbumArtRemove.Items.Add("Remove Album Art");
        }
        #endregion

    }
}
