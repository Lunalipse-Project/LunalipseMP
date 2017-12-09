using NetEaseHijacker.Types;
using System.ComponentModel;

namespace NewMediaPlayer.controler
{
    class MusicInfo : INotifyPropertyChanged
    {
        public SDetail _sd;
        public string MusicN
        {
            get
            {
                return _sd.name;
            }
        }

        public string artist
        {
            get
            {
                return _sd.ar_name;
            }
        }

        public string ID
        {
            get
            {
                return _sd.id;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
