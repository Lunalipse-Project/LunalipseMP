using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LunapxCompiler.Generic
{
    public static class Structures
    {
        public const int lpxType = 0x2D3F;
        [StructLayout(LayoutKind.Sequential)]
        public struct lpxExpression
        {
            public bool hasREP;
            public bool isRandom;
            public bool isShutdownREQ;
            public int REP_Times;
            public int exe_CMD;
            public int SongID;
            [MarshalAs(UnmanagedType.ByValArray,SizeConst = 10)]
            public int[] equerz;
            [MarshalAs(UnmanagedType.ByValTStr,SizeConst = 128)]
            public string songsName;
            public double Vol;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct lpxHeader
        {
            public int type;
            public int cmds;
            public int id;
        }

        public struct LunalipxDecomp
        {
            public int MagicNumber;
            public List<LUNALIPS_Expression> commands;
        }

        public static LUNALIPS_Expression ToExpression(this lpxExpression lxe)
        {
            return new LUNALIPS_Expression()
            {
                equerz = lxe.equerz,
                hasREP = lxe.hasREP,
                isRandom = lxe.isRandom,
                exe_CMD = lxe.exe_CMD,
                isShutdownREQ = lxe.isShutdownREQ,
                REP_Times = lxe.REP_Times,
                songsName = lxe.songsName,
                SongID = lxe.SongID,
                Vol = lxe.Vol
            };
        }
    }
}
