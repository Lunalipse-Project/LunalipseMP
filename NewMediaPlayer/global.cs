using I18N;
using NewMediaPlayer.Generic;
using NewMediaPlayer.Sound;
using System.Collections;
using System.Collections.Generic;

namespace NewMediaPlayer
{

    public class global
    {
        public static string MUSIC_PATH = "";
        public static int SELECTED_MUSIC = 0;
        public static float MUSIC_VOLUME = 0.7f;
        public static int EQUALIZER_SET = 0;
        public static int[] EQUALIZER_SAVE = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int PLAY_MODE = 0;    //Max: 5
        public static string DOWNLOAD_SAVE_PATH = "NO_SELECT";
        public static double PRELISTEN_VOLUME = 0.7;    //试听歌曲音量
        public static string VER = "";

        public static bool DISP_LYRIC = true;
        public static bool SHOW_FFT = true;
        public static bool SHOW_MUSIC_NAME = true;
        public static bool SHOW_CUR_DURATION = true;
        public static bool SUPPORT_VDESKTOP = true;
        public static bool PLUGIN_SECURITY = true;
        public static bool LOG_RECORD = true;
        public static bool EXPORT_BIN = true;
        public static bool USE_BIN = true;
        public static bool USE_SYS_LANG = false;
        public static bool USE_CIRCULAR_SPECT = true;
        public static bool USE_SHELL = false;

        public static ScalingStrategy SSTR = ScalingStrategy.Linear;
        public static int FFT_REF_FRQ = 45;

        //Non serialization
        public const int MAGIC_NUMBER_4_LPX = 0x4C55;
        public static bool LrcEditing = false;
        public static string CUR_MUSICN = "";

        public static Languages? LANG = Languages.CHINESE;

        public static Dictionary<int, LunalipseAPI.KShortcut.Keystroke> KsHolder = new Dictionary<int, LunalipseAPI.KShortcut.Keystroke>();

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public static IDictionary<string, object> __data = new Dictionary<string, object>();
    }
}
