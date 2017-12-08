using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunapxCompiler.Generic
{
    
    /// <summary>
    /// Lunalipx脚本语句节点
    /// </summary>
    [Serializable]
    public class LUNALIPS_Expression
    {
        public bool hasREP = false;
        public bool isRandom = false;
        public bool isShutdownREQ = false;
        public int REP_Times = 0;
        public int exe_CMD = 0;
        public int SongID = 0;
        public int[] equerz;
        public string songsName = "";
        public double Vol = 70d;
    }
}
