using CSCore.Streams.Effects;
using LunapxCompiler;
using LunapxCompiler.Generic;
using NewMediaPlayer.Generic;
using NewMediaPlayer.PluginHoster;
using NewMediaPlayer.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using static LunapxCompiler.Generic.Structures;

namespace NewMediaPlayer.Lunalipx
{
    public partial class SyntaxParser
    {
        public static volatile SyntaxParser SP_INSTANCE;
        public static readonly object LOCKER_CHKER = new object();
        PluginHelper PH;
        NewSyntax ns;
        ArrayList commands = new ArrayList();
        string fname;
        int programe = 0;
        //循环次数计数器
        int REP_COUNTER = 0;
        //程序指针累计器
        int PRG_COUNTER = 0;

        bool randomLIST = false,has_random_modifier=false;

        Random r = new Random();

        //int SAVED_PTR = 0;
        //REP(COMMAND(TIMES))
        public SyntaxParser()
        {
            PH = PluginHelper.INSTANCE;
            ns = new NewSyntax();
            global.KsHolder.Add4nRep(0xFF00, new LunalipseAPI.KShortcut.Keystroke()
            {
                MainKey = (int)System.Windows.Forms.Keys.Alt,
                Subkey = (int)System.Windows.Forms.Keys.B,
                KeepListenUntilRelease = false,
                keyStroked = new LunalipseAPI.KShortcut.Behavior(() =>
                {
                    if (has_random_modifier) randomLIST = randomLIST ? false : true;
                })
            });
        }

        public static SyntaxParser INSTANCE
        {
            get
            {
                if(SP_INSTANCE == null)
                {
                    lock (LOCKER_CHKER)
                    {
                        if (SP_INSTANCE == null) SP_INSTANCE = new SyntaxParser();
                    }
                }
                return SP_INSTANCE;
            }
        }

        public int GetProgrameSeq
        {
            get
            {
                return programe;
            }
        }

        public bool Parse(ArrayList LB_,int prg)
        {
            programe = prg;
            has_random_modifier = randomLIST = false;
            List<LUNALIPS_Expression> _tmp;
            if((_tmp= ReadBIN().commands)!= null)
            {
                commands = ArrayList.Adapter(_tmp);
                return true;
            }
            commands.Clear();
            if (!ReadScript(programe,"")) return false;
            perCompile(LB_,false);
            if(randomLIST)
            {
                perCompile(LB_, true);
                foreach (LUNALIPS_Expression le in commands)
                {
                    if (le.exe_CMD == Command.LUNA_PASS)
                    {
                        le.exe_CMD = Command.LUNA_PLAYS;
                    }
                }
            }
            exportBIN();
            return true;
        }

        bool ReadScript(int sqnum,string p)
        {
            int ix = 0;
            string line, alltxt = "";
            string path = "Scripts/prg" + sqnum + ".lunapx";
            fname = Path.GetFileName(p.AvailableEx() ? p : path);
            using (StreamReader sr = new StreamReader(p.AvailableEx() ? p : path))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    alltxt += Regex.Replace(line, "(!!)+.*","")+"\n";
                }
                LogFile.WriteLog("INFO", "Load " + ix + " lines in " + fname);
            }
            alltxt = Regex.Replace(alltxt, @"\!\-(.|\n)*?\-\!", "");
            alltxt = Regex.Replace(alltxt, "[\t\n]", "");
            foreach (string v in alltxt.Split(';'))
            {
                ix++;
                try
                {
                    if (v.AvailableEx())
                    {
                        string[] arg = v.Trim().Split(' ');
                        if (v.StartsWith("load"))
                        {
                            if (arg.Length < 2) continue;
                            else
                            {
                                int seq=0;
                                if (!int.TryParse(arg[arg.Length - 1].Replace("#Program", ""), out seq))
                                {
                                    throw new LPXException("不正确的脚本常量：{0}".FormateEx(arg[arg.Length - 1]), "参数错误");
                                }
                                if (sqnum == seq) throw new LPXException("非法的调用：{0}".FormateEx(v), "探测到死循环");
                                if (!ReadScript(seq,"")) return false;
                            }
                        }
                        else if(v.StartsWith("#pragma"))
                        {
                            if (arg.Length < 2) continue;
                            try
                            {
                                switch ((PRAGMA)Enum.Parse(typeof(PRAGMA), arg[1]))
                                {
                                    case PRAGMA.L_RANDOM:
                                        //has_random_modifier = true;
                                        randomLIST = true;
                                        break;
                                    case PRAGMA.L_LINEAR:
                                        randomLIST = false;
                                        break;
                                    case PRAGMA.L_RL_SWITCHABLE:
                                        has_random_modifier = true;
                                        randomLIST = true;
                                        break;

                                }
                            }
                            catch(ArgumentException)
                            {
                                throw new LPXException("Lunalipx编译器常量：{0}未定义。".FormateEx(arg[1]), "参数错误");
                            }
                        }
                        else commands.Add(ns.ParseStatement(v));
                    }
                }
                catch (LPXException e)
                {
                    Error ee = new Error(e);
                    LogFile.WriteLog("ERROR", e.Message);
                    ee.ShowDialog();
                    return false;
                }
            }
            return true;
        }

        public bool ParseCSPT(ArrayList LB_, string path)
        {
            commands.Clear();
            if (!ReadScript(0, path)) return false;
            perCompile(LB_,true);
            foreach (LUNALIPS_Expression le in commands)
            {
                if (le.exe_CMD == Command.LUNA_PASS)
                {
                    le.exe_CMD = Command.LUNA_PLAYS;
                }
            }
            return true;
        }

        private int EXTEND_FUNC(string c)
        {
            foreach(string s in PH.LUNAPXEXTEND)
            {
                int a;
                if ((a = PH.ENTITIES[s].LPX.LunalipxMethod(c)) != 0x0000)
                {
                    return a;
                }
                else continue;
            }
            return 0x0000;
        }

        private void perCompile(ArrayList LB_,bool crossPlatform)
        {
            int tmp_sel_m = global.SELECTED_MUSIC;
            float tmp_vol_m = global.MUSIC_VOLUME;
            int[] tmp_eqz = global.EQUALIZER_SAVE;
            global.SELECTED_MUSIC = 0;
            foreach (var v in commands)
            {
                LUNALIPS_Expression le;
                switch((le= (v as LUNALIPS_Expression)).exe_CMD)
                {
                    case Command.LUNA_PLAYN:
                        global.SELECTED_MUSIC = le.SongID;
                        break;
                    case Command.LUNA_PLAYS:
                        int a = LB_.IndexOf(le.songsName);
                        global.SELECTED_MUSIC = a == -1 ? global.SELECTED_MUSIC + 1 :a;
                        break;
                    case Command.LUNA_PASS:
                        LUNALIPS_Expression l = (v as LUNALIPS_Expression);
                        if(crossPlatform)
                        {
                            l.songsName = LB_[++global.SELECTED_MUSIC] as string;
                        }
                        else
                        {
                            l.SongID = ++global.SELECTED_MUSIC;
                        }
                        break;
                }
            }
            PRG_COUNTER = 0;
            global.SELECTED_MUSIC = tmp_sel_m;
            global.MUSIC_VOLUME = tmp_vol_m;
            global.EQUALIZER_SAVE = tmp_eqz;
        }

        Random R = new Random();
        public void Excute(ListBox LB)
        {
            
            bool HAS_SELF_INCR = false;
            LUNALIPS_Expression exp = commands[PRG_COUNTER] as LUNALIPS_Expression;
            if(exp.exe_CMD==Command.LUNA_LLOOP)
            {
                //重置指针
                PRG_COUNTER = 0;
                //重新应用指针
                exp = commands[PRG_COUNTER] as LUNALIPS_Expression;
            }
            if(PRG_COUNTER>=commands.Count)
            {
                global.PLAY_MODE = 0;
                global.SELECTED_MUSIC = 1;
                return;
            }
            switch(exp.exe_CMD)
            {
                case Command.LUNA_PLAYS:
                    int test = LB.Items.IndexOf(exp.songsName);
                    if(test==-1)
                    {
                        LogFile.WriteLog("ERROR", "LUNAPX - Music " + exp.songsName + " not found. Check the spelling or file existing.");
                        LogFile.WriteLog("INFO", "LUNAPX - Move to next instruction");
                        PRG_COUNTER++;
                        Excute(LB);
                    }
                    else
                    {
                        global.SELECTED_MUSIC = test;
                    }
                    global.MUSIC_VOLUME = (float)exp.Vol*0.01f;
                    break;
                case Command.LUNA_PASS:
                    /*if (global.SELECTED_MUSIC == LB.Items.Count - 1)
                    {
                        global.SELECTED_MUSIC = 0;
                    }
                    else
                    {
                        global.SELECTED_MUSIC++;
                    }*/
                    global.SELECTED_MUSIC = (commands[PRG_COUNTER] as LUNALIPS_Expression).SongID;
                    break;
                case Command.LUNA_PLAYN:
                    if(exp.isRandom) global.SELECTED_MUSIC = R.Next(LB.Items.Count);
                    else global.SELECTED_MUSIC = exp.SongID;
                    global.MUSIC_VOLUME = (float)exp.Vol * 0.01f;
                    break;
                case Command.LUNA_SETEQU:
                    for(int i=0;i<10;i++)
                    {
                        appliedEquzer(i, exp.equerz[i]);
                    }
                    global.EQUALIZER_SAVE = exp.equerz;
                    PRG_COUNTER++;
                    Excute(LB);
                    break;
                case Command.LUNA_SHUTDOWN_COM:
                    callNightmareMoon();
                    break;
                default:
                    foreach (string s in PH.LUNAPXEXTEND)
                    {
                        HAS_SELF_INCR = PH.ENTITIES[s].LPX.LunalipxBehavior(exp.exe_CMD, ref PRG_COUNTER, ref global.SELECTED_MUSIC);
                    }
                    if (!HAS_SELF_INCR)
                    {
                        PRG_COUNTER = randomLIST ? r.Next(0, commands.Count - 1) : PRG_COUNTER + 1;
                        Excute(LB);
                    }
                    break;
            }
            if (exp.hasREP)
            {
                REP_COUNTER++;
                if (REP_COUNTER >= exp.REP_Times)
                {
                    REP_COUNTER = 0;
                    PRG_COUNTER = randomLIST ? r.Next(0, commands.Count - 1) : PRG_COUNTER + 1;
                }
            }
            else
            {
                if(PRG_COUNTER==0)
                {
                    PRG_COUNTER = randomLIST ? r.Next(0, commands.Count - 1) : PRG_COUNTER + 1;
                }
                else if((commands[PRG_COUNTER - 1] as LUNALIPS_Expression).exe_CMD != Command.LUNA_SETEQU)
                {
                    PRG_COUNTER = randomLIST ? r.Next(0, commands.Count - 1) : PRG_COUNTER + 1;
                }
            }
        }

        private void callNightmareMoon()
        {
            //Console.WriteLine("I am Nightmare Moon!");
            Process.Start("shutdown.exe", "-s -t 00");
        }

        private void appliedEquzer(int inx,int val)
        {
            if (PlaySound.equzer != null)
            {
                double perc = ((double)val / 100d);
                var value = (float)(perc * 20);
                EqualizerFilter ef = PlaySound.equzer.SampleFilters[inx];
                ef.AverageGainDB = value;
            }
        }

        public int PROGRAME_COUNTER
        {
            get
            {
                return PRG_COUNTER;
            }
            set
            {
                if (value < 1) return;
                else PRG_COUNTER = value - 1;
            }
        }

        private bool exportBIN()
        {
            if (!global.EXPORT_BIN) return false;
            if (!"Scripts/bin".DExist(FType.DICT)) Directory.CreateDirectory("Scripts/bin");
            if (("Scripts/bin/prg" + programe + "._lpx").DExist(FType.FILE))
            {
                return false;
            }
            try
            {
                lpxCompiler lc = new lpxCompiler(global.MAGIC_NUMBER_4_LPX);
                lc.SetExpression(commands);
                lc.CompileTo("Scripts/bin/prg" + programe + "._lpx");
                return true;
            }
            catch(Exception e)
            {
                LogFile.WriteLog("ERROR", e.Message);
                return false;
            }
        }

        private LunalipxDecomp ReadBIN()
        {
            if (!global.USE_BIN) return new LunalipxDecomp();
            if (!("Scripts/bin/prg" + programe + "._lpx").DExist(FType.FILE))
            {
                return new LunalipxDecomp();
            }
            try
            {
                lpxDecompiler ld = new lpxDecompiler(global.MAGIC_NUMBER_4_LPX);
                LunalipxDecomp lp = new LunalipxDecomp();
                ld.Decompile("Scripts/bin/prg" + programe + "._lpx");
                lp.commands = ld.CMDList;
                lp.MagicNumber = global.MAGIC_NUMBER_4_LPX;
                return lp;
            }
            catch(Exception e)
            {
                LogFile.WriteLog("ERROR", e.Message);
                return new LunalipxDecomp();
            }
        }

        
        ~SyntaxParser()
        { }
    }
}
