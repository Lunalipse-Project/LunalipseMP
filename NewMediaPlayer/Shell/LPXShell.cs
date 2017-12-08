using NewMediaPlayer.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMediaPlayer.Shell
{
    public class LPXShell
    {
        public delegate void MsgOut(string msg);
        public delegate void Promte();
        public delegate void Clear();
        public static event MsgOut OnMessageAppend;
        public static event Promte OnPromte;
        public static event Clear OnClear;
        public static void WriteLine(string msg,params object[] args)
        {
            string _msg = args != null ? msg.FormateEx(args) : msg;
            OnMessageAppend?.Invoke("{0}\n".FormateEx(_msg));
        }
        public static void WriteLine()
        {
            OnMessageAppend?.Invoke("\n");
        }
        public static void Write(string msg, params object[] args)
        {
            string _msg = args != null ? msg.FormateEx(args) : msg;
            OnMessageAppend?.Invoke("{0}".FormateEx(_msg));
        }
        public static void EndWriting()
        {
            OnPromte?.Invoke();
        }
        public static void Flush()
        {
            OnClear?.Invoke();
        }
    }
}
