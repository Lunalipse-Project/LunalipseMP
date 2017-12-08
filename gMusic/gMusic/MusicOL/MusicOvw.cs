using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gMusic.MusicOL
{
    public class MusicOvw
    {
        string id;
        string musicN;
        string artist;
        public MusicOvw(string p1, string p2, string p3)
        {
            id = p1;
            musicN = p2;
            artist = p3;
        }

        public string MusicName
        {
            get
            {
                return musicN;
            }
        }

        public string ID
        {
            get
            {
                return id;
            }
        }

        public string Artist
        {
            get
            {
                return artist;
            }
        }
    }
}
