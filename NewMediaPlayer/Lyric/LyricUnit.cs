using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMediaPlayer.Lyric
{
    class LyricUnit : INotifyPropertyChanged
    {
        private string lyric;
        private string subLyric;
        private TimeSpan offset;

        public LyricUnit(double ts) : this(TimeSpan.FromSeconds(ts), "", "") { }

        public LyricUnit(TimeSpan OfTs, string lrc,string slrc)
        {
            lyric = lrc;
            offset = OfTs;
            subLyric = slrc;
        }

        public LyricUnit(double sec,string lrc, string slrc) : this(TimeSpan.FromSeconds(sec), lrc, slrc) { }
        
        public String Lyric
        {
            get
            {
                return lyric.Length >= 15 ? lyric.Substring(0, 14) + " ..." : lyric;
            }
            set
            {
                lyric = value;
            }
        }

        public string LyricEditing
        {
            get
            {
                return lyric;
            }
        }

        public string SubLyric
        {
            get
            {
                return subLyric;
            }
            set
            {
                subLyric = value;
            }
        }

        public String Offset
        {
            get
            {
                return offset.ToString(@"mm\:ss\.fff");
            }
        }

        public TimeSpan OffsetTS
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
            }
        }

        public double OffsetTSd
        {
            get
            {
                return offset.TotalSeconds;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
