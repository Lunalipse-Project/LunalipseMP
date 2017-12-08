using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NewMediaPlayer.Generic
{
    class U32
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx,
        int cy, uint uFlags);
        [DllImport("user32", EntryPoint = "SetWindowLong")]
        public static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

        [DllImport("user32", EntryPoint = "GetWindowLong")]
        public static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

        public const int WS_EX_TRANSPARENT = 0x20;
        public const int GWL_EXSTYLE = (-20);
        public const int SE_SHUTDOWN_PRIVILEGE = 0x13;
    }
}
