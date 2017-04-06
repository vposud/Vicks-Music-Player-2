using System;
using System.Windows;
using Vicks_Music_Player_2.Classes;
using System.IO;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Windows.Controls;
using Ini;
using System.Collections.Generic;
using System.Windows.Input;

namespace Vicks_Music_Player_2.Forms
{
    /// <summary>
    /// Interaction logic for frmTagEditor.xaml
    /// </summary>
    public partial class TagEditor : Window
    {
        public string filename { get; set; }

        public TagEditor(string FileName)
        {
            InitializeComponent();
            this.filename = FileName;
        }

        private void frmTagEditor_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Path.GetExtension(filename).ToLower() == ".mp3")
            {
                //saves the MP3 tags
                TagLib.File mTrack = TagLib.File.Create(filename);
                TagLib.Tag ID32 = mTrack.GetTag(TagLib.TagTypes.Id3v2);

                ID32.Track = Convert.ToUInt16(txtTrackNum.Text);
                ID32.TrackCount = Convert.ToUInt16(txtTrackNum.Text);
                ID32.Performers = new string[] { txtArtist.Text };
                ID32.Title = txtTitle.Text;
                ID32.Album = txtAlbum.Text;
                ID32.AlbumArtists = new string[] { txtAlbumArtist.Text };
                ID32.Year = Convert.ToUInt16(txtYear.Text);
                ID32.Genres = new string[] { txtGenre.Text };
                ID32.Comment = txtComment.Text;

                mTrack.Save();

            }

            if (Path.GetExtension(filename).ToLower() == ".flac")
            {
                //saves the FLAC tags
                TagLib.File mTrack = TagLib.File.Create(filename);
                TagLib.Tag FLAC = mTrack.GetTag(TagLib.TagTypes.FlacMetadata);

                FLAC.Track = Convert.ToUInt16(txtTrackNum.Text);
                FLAC.TrackCount = Convert.ToUInt16(txtTrackNum.Text);
                FLAC.Performers = new string[] { txtArtist.Text };
                FLAC.Title = txtTitle.Text;
                FLAC.Album = txtAlbum.Text;
                FLAC.AlbumArtists = new string[] { txtAlbumArtist.Text };
                FLAC.Year = Convert.ToUInt16(txtYear.Text);
                FLAC.Genres = new string[] { txtGenre.Text };
                FLAC.Comment = txtComment.Text;

                mTrack.Save();
            }

            this.Close();
        }
    }
}
