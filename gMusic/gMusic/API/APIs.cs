using System;
using System.Collections.Generic;
using System.Text;

namespace gMusic.API
{
    public class APIs
    {
        public const String MUSIC_SEARCH = "http://music.163.com/api/search/get/";
        public const String MUSIC_LYRIC = "http://music.163.com/api/song/lyric?";
        public const String MUSIC_DETAIL = "http://music.163.com/api/song/detail/?";
        public const String MUSIC_DWLINK = "http://m2.music.126.net/{0}/{1}.{2}";

        public const String P_DETAIL = "%5B{0}%5D";
    }

    public class SearchType
    {
        public const int S_SONG = 1;
        public const int S_ALBUM = 10;
        public const int S_SINGER = 100;
        public const int S_LIST = 1000;
        public const int S_USER = 1002;
    }

    public class EventType
    {
        public const string E_SEARCH = "p001";
        public const string E_LYRIC = "p011";
        public const string E_DETAIL = "p111";
    }
}
