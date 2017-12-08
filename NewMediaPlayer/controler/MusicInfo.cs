using System.ComponentModel;

namespace NewMediaPlayer.controler
{
    class MusicInfo : INotifyPropertyChanged
    {
        string MusicName;
        string Artist;
        string _id;
        public string MusicN
        {
            get
            {
                return MusicName;
            }
            set
            {
                MusicName = value;
            }
        }

        public string artist
        {
            get
            {
                return Artist;
            }
            set
            {
                Artist = value;
            }
        }

        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
