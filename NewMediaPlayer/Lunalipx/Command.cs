using System.Collections;

namespace NewMediaPlayer.Lunalipx
{
    public class Command
    {
        public static ArrayList cmd = new ArrayList();
        public static int MAX_MODE = 6;
        public static int SCRIPT_range = 6;
        public const int LUNA_PLAYS = 0x0111;
        public const int LUNA_SETEQU = 0x0333;
        public const int LUNA_PASS = 0x0444;
        public const int LUNA_PLAYN = 0x0555;
        public const int LUNA_LLOOP = 0x0FFF;
        public const int LUNA_SHUTDOWN_COM = 0x3FFF;

        public static string DetranslateCMD(int cmd)
        {
            switch(cmd)
            {
                case LUNA_PLAYS:return "luna.play({0},{1})";
                case LUNA_PLAYN: return "luna.playN({0},{1})";
                case LUNA_PASS: return "next()";
                case LUNA_LLOOP: return "lloop()";
                case LUNA_SETEQU: return "luna.eqz({0})";
                case LUNA_SHUTDOWN_COM: return "NightmareMoon()";
                default:return "0x"+cmd.ToString("x4")+" {0}";
            }
        }
    }
}
