using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Vicks_Music_Player_2.Classes;

namespace Vicks_Music_Player_2.Forms
{
    /// <summary>
    /// Interaction logic for frmSearch.xaml
    /// </summary>
    public partial class frmSearch : Window
    {
        public ItemCollection lstitems { get; set; }

        public frmSearch(ItemCollection lstItems)
        {
            InitializeComponent();
            this.lstitems = lstItems;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //when typing text search the items list for what your typing
            txtSearch.Text = txtSearch.Text.ToLower();
            lstSearch.Items.Clear();
            for (int i = 0; i <= lstitems.Count - 1; i++)
            {
                string search = lstitems[i].ToString().ToLower();
                if (search.Contains(txtSearch.Text))
                {
                    lstSearch.Items.Add(lstitems[i].ToString());
                }
            }
        }

        private void lstSearch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //goes through the itemlist and gets the index of the song then sends it over to the main form to select and play it
            int Index = 0;
            for (int i = 0; i < lstitems.Count -1; i++)
            {
                if (lstitems[i].ToString() == lstSearch.SelectedItem.ToString())
                {
                    Index = i;
                    break;
                }
            }
            var main = App.Current.MainWindow as MainWindow;
            main.SearchPlay(Index);
            this.Close();
        }

        private void Search_Loaded(object sender, RoutedEventArgs e)
        {
            //sets focus to the textbox on startup
            txtSearch.Focus();
        }
    }
}
