using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using LunapxCompiler.Generic;

namespace LunapxCompiler
{
    public class lpxCompiler
    {
        Structures.lpxHeader lpxh;
        List<Structures.lpxExpression> exps;
        public lpxCompiler(int sftID)
        {
            lpxh = new Structures.lpxHeader();
            exps = new List<Structures.lpxExpression>();
            lpxh.id = sftID;
            lpxh.type = Structures.lpxType;
        }

        public void SetExpression(ArrayList _lxpe)
        {
            exps.Clear();
            for(int i=0;i<_lxpe.Count; i++)
            {
                LUNALIPS_Expression s = _lxpe[i] as LUNALIPS_Expression;
                Structures.lpxExpression l = new Structures.lpxExpression();
                l.equerz = s.equerz;
                l.exe_CMD = s.exe_CMD;
                l.hasREP = s.hasREP;
                l.isRandom = s.isRandom;
                l.isShutdownREQ = s.isShutdownREQ;
                l.REP_Times = s.REP_Times;
                l.SongID = s.SongID;
                l.songsName = s.songsName;
                l.Vol = s.Vol;
                exps.Add(l);
            };
        }

        public void CompileTo(string path)
        {
            lpxh.cmds = exps.Count;
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                fs.Write(Utils.s2b(lpxh, Marshal.SizeOf(lpxh)), 0, Marshal.SizeOf(lpxh));
                exps.ForEach((s) =>
                {
                    fs.Write(Utils.s2b(s, Marshal.SizeOf(s)), 0, Marshal.SizeOf(s));
                });
                fs.Flush();
            }
        }
    }
}
