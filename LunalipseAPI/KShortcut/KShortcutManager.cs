using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunalipseAPI.KShortcut
{
    public delegate bool ASC(int i, Keystroke k);
    public class KShortcutManager
    {
        public static event ASC AddShortCut;
        public static bool AddKeyShortcut(int id,Keystroke ks)
        {
            return AddShortCut(id,ks);
        }
    }
}
