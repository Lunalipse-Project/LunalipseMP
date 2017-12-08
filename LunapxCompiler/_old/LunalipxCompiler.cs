using LunapxCompiler.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunapxCompiler._old
{
    public class LunalipxCompiler
    {
        List<byte> compiled;

        const byte SPLIT_FLAG = 0xFF;
        const byte INT_BEGIN = 0xFE;
        const byte INT_ARRAY_BEGIN = 0xFD;
        const byte STRING_BEGIN = 0xFC;

        public LunalipxCompiler(int MAGIC_NUM)
        {
            compiled = new List<byte>();
            byte[] mgnum = new byte[4];
            Utils.int2bin(MAGIC_NUM, ref mgnum);
            foreach(byte b in mgnum)
            {
                compiled.Add(b);
            }
            compiled.Add(SPLIT_FLAG);
        }

        public bool Compile(ArrayList LPXE,string expath)
        {
            if (LPXE == null) return false;
            for(int i=0;i<LPXE.Count;i++)
            {
                LUNALIPS_Expression lxe = LPXE[i] as LUNALIPS_Expression;
                compiled.Add(Convert.ToByte(lxe.hasREP));
                compiled.Add(Convert.ToByte(lxe.isRandom));
                compiled.Add(Convert.ToByte(lxe.isShutdownREQ));
                compiled.Add(INT_BEGIN);
                byte[] intb = new byte[4];
                Utils.int2bin(lxe.REP_Times,ref intb);
                AddBytes(intb);
                Utils.int2bin(lxe.exe_CMD, ref intb);
                AddBytes(intb);
                Utils.int2bin(lxe.SongID, ref intb);
                AddBytes(intb);
                compiled.Add(INT_ARRAY_BEGIN);
                if(lxe.equerz!=null)
                {
                    foreach (int j in lxe.equerz)
                    {
                        Utils.int2bin(j, ref intb);
                        AddBytes(intb);
                    }
                }
                compiled.Add(STRING_BEGIN);
                AddBytes(Encoding.UTF8.GetBytes(lxe.songsName));
                compiled.Add(SPLIT_FLAG);
            }
            byte[] final = compiled.ToArray();
            try
            {
                using (FileStream fs = new FileStream(expath, FileMode.Create))
                {
                    fs.Write(final, 0, final.Length);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void AddBytes(byte[] b)
        {
            foreach (byte xb in b) compiled.Add(xb);
        }
    }

    
}
