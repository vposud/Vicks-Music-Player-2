using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Vicks_Music_Player_2.Classes
{
    class Playlist
    {
        private string sFileName;
        private string sTitle;
        private Brush sBrush;

        public Playlist(string strFileName, string strTitle, Brush strBrush)
        {
            this.sFileName = strFileName;
            this.sTitle = strTitle;
            this.sBrush = strBrush;
        }

        public string FileName
        {
            get { return this.sFileName; }
        }

        public string Title
        {
            get { return this.sTitle; }
        }

        public Brush BrushColor
        {
            get { return this.sBrush; }
        }

        public override string ToString()
        {
            return this.sTitle;
        }

        public string GetFileName()
        {
            return this.sFileName;
        }
    }
}