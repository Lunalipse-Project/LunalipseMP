using LunapxCompiler.Generic;
using NewMediaPlayer.Generic;
using NewMediaPlayer.Shell;
using System.Collections.Generic;

namespace NewMediaPlayer.Lunalipx
{
    public static class CompilerExt
    {
        public static string translate2lunapx(this List<LUNALIPS_Expression> L)
        {
            string result = "";
            int c = 0;
            foreach(LUNALIPS_Expression lxe in L)
            {
                string statement = "{0};\n";
                string cmd = Command.DetranslateCMD(lxe.exe_CMD);
                switch(lxe.exe_CMD)
                {
                    case Command.LUNA_PLAYS:
                        cmd = cmd.FormateEx(lxe.songsName,lxe.Vol);
                        break;
                    case Command.LUNA_PLAYN:
                        if (lxe.isRandom)
                        {
                            cmd = cmd.FormateEx("#RAND", lxe.Vol);
                        }
                        else
                        {
                            cmd = cmd.FormateEx(lxe.SongID, lxe.Vol);
                        }
                        break;
                    case Command.LUNA_SETEQU:
                        string _cache = "";
                        if (lxe.equerz == null) break;
                        foreach(int i in lxe.equerz)
                        {
                            _cache += i + ",";
                        }
                        _cache = _cache.Substring(0, _cache.Length - 1);
                        cmd = cmd.FormateEx(_cache);
                        break;
                }
                if(lxe.hasREP)
                {
                    statement = statement.FormateEx("{0}:{1}".FormateEx(cmd, lxe.REP_Times));
                }
                else
                {
                    statement = statement.FormateEx(cmd);
                }
                result += statement;
                c++;
            }
            LPXShell.WriteLine("Decompile done!\n {0} statements decompiled.",c);
            return result;
        }
    }
}
