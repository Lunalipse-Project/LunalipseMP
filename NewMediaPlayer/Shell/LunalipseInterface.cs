using LunalipseShell;
using NewMediaPlayer.Sound;
using System;

namespace NewMediaPlayer.Shell
{
    public class LunalipseInterface
    {
        public static volatile LunalipseInterface LI_INSTANCE;
        static readonly object LOCK_CHK = new object();

        int mode = 0, s_mode = 0, tmpInt = 0;
        string ax = ""/*, bx = ""*/;
        public static LunalipseInterface INSTANCE
        {
            get
            {
                if(LI_INSTANCE==null)
                {
                    lock(LOCK_CHK)
                    {
                        if(LI_INSTANCE==null)
                        {
                            LI_INSTANCE = new LunalipseInterface();
                        }
                    }
                }
                return LI_INSTANCE;
            }
        }


        public delegate void ScalingStrategyChange(ScalingStrategy ss);
        public delegate void PlayMusic(int muID,string name,bool isRandom);
        public event ScalingStrategyChange OnScalingStrategyChange;
        public event PlayMusic OnMusicChanged;

        public void ChangeScalingStrategy(ScalingStrategy SS)
        {
            OnScalingStrategyChange?.Invoke(SS);
        }

        public void PlayMusicByMode(int mode,params object[] o)
        {
            switch(mode)
            {
                //Next song
                case 0:
                    OnMusicChanged?.Invoke(++global.SELECTED_MUSIC, "", false);
                    break;
                //Specific Music ID
                case 1:
                    try { OnMusicChanged?.Invoke(Convert.ToInt32(o[0] as string), "", false); } catch
                    {
                        LPXShell.WriteLine(" [Fatal] Invailde arguments of ID.");
                    }
                    break;
                //Specific Music Name
                case 2:
                    OnMusicChanged?.Invoke(-1, Convert.ToString(o[0]), false);
                    break;
                //Play Random
                case 3:
                    OnMusicChanged?.Invoke(-1, "", true);
                    break;
                default:
                    LPXShell.WriteLine(" [Fatal] Mode not defined");
                    break;
            }
        }

        public void ParseCommand(Command cmd)
        {
            if (cmd.Args.Length == 0) ShowHelpHint();
            for(int i=0;i<cmd.Args.Length;i+=2)
            {
                switch (cmd.Args[i])
                {
                    case "-help":
                        ShowHelpHint();
                        return;
                    case "-m":
                        switch(cmd.Args[i+1])
                        {
                            case "FFT":
                                mode = 0;
                                break;
                            case "MPLY":
                                mode = 1;
                                break;
                            case "GEN":
                                mode = 2;
                                break;
                            default:
                                LPXShell.WriteLine(" [Fatal] Mode '{0}' not defined.", cmd.Args[i + 1]);
                                return;
                        }
                        break;
                    case "-pm":
                        switch (cmd.Args[i + 1])
                        {
                            case "LC":
                                s_mode = 0;
                                break;
                            case "ID":
                                s_mode = 1;
                                break;
                            case "SN":
                                s_mode = 2;
                                break;
                            case "RD":
                                s_mode = 3;
                                break;
                            default:
                                LPXShell.WriteLine(" [Fatal] Option '{0}' for argument '-pm' is not defined.", cmd.Args[i + 1]);
                                return;
                        }
                        break;
                    case "-fpm":
                        switch (cmd.Args[i + 1])
                        {
                            case "SS":
                                s_mode = 0;
                                break;
                            case "DT":
                                s_mode = 1;
                                break;
                            case "SY":
                                s_mode = 2;
                                break;
                            default:
                                LPXShell.WriteLine(" [Fatal] Option '{0}' for argument '-fpm' is not defined.", cmd.Args[i + 1]);
                                return;
                        }
                        break;
                    case "-v":
                        ax = cmd.Args[i + 1];
                        break;
                    default:
                        ShowHelpHint();
                        return;
                }
            }
            ExecuteByMode();
        }

        private void ExecuteByMode()
        {
            switch(mode)
            {
                case 0:
                    switch (s_mode)
                    {
                        case 0:
                            ScalingStrategy ss;
                            if(!Enum.TryParse(ax, out ss))
                            {
                                LPXShell.WriteLine(" [Fatal] Illegal scaling strategy '{0}'", ax);
                                return;
                            }
                            OnScalingStrategyChange(ss);
                            break;
                        case 1:
                            int delay = 0;
                            if(!int.TryParse(ax,out delay))
                            {
                                LPXShell.WriteLine(" [Fatal] Illegal delay timespan '{0}'", ax);
                                return;
                            }
                            if(delay>500||delay<15)
                            {
                                LPXShell.WriteLine(" [ERROR] For best performance. Lunalipse do not accept any value below 15ms or above 500ms.");
                                return;
                            }
                            global.FFT_REF_FRQ = delay;
                            break;
                        case 2:
                            global.USE_CIRCULAR_SPECT = !global.USE_CIRCULAR_SPECT;
                            break;
                    }
                    break;
                case 1:
                    PlayMusicByMode(s_mode, ax);
                    break;
            }
        }

        private void ShowHelpHint()
        {
            LPXShell.WriteLine(" Lunalipse Kernel Shell Execution Evironment.   [Version:1.0.0.0]");
            LPXShell.WriteLine(" The Interface that allows you interactive with Lunalipse kernel via Lunalipse Shell.");
            LPXShell.WriteLine();
            LPXShell.WriteLine("    Arguments: ");
            LPXShell.WriteLine("       {0,-15} - {1}", "-m <FFT|MPLY|GEN>","Set the mode.");
            LPXShell.WriteLine("          {0,-12} > {1}", "FFT", "Settings of sound visualizer");
            LPXShell.WriteLine("          {0,-12} > {1}", "MPLY", "Play the music in the list");
            LPXShell.WriteLine("          {0,-12} > {1}", "GEN", "General Settings");
            LPXShell.WriteLine("       {0,-15} - {1}", "-pm <LC|ID|SN|RD>", "[MPLY]  Set the way that how the Lunalipse play the music.");
            LPXShell.WriteLine("          {0,-12} > {1}", "LC", "List Cycling");
            LPXShell.WriteLine("          {0,-12} > {1}", "ID", "Play by song ID");
            LPXShell.WriteLine("          {0,-12} > {1}", "SN", "Play by song name");
            LPXShell.WriteLine("          {0,-12} > {1}", "RD", "Play Randomly");
            LPXShell.WriteLine("       {0,-15} - {1}", "-fpm <SS|DT|SY>", "[FFT] Set the properties that you want to change in visualizer.");
            LPXShell.WriteLine("          {0,-12} > {1}", "SS", "Float transformation scaling strategy.");
            LPXShell.WriteLine("          {0,-12} > {1}", "DT", "Drawing delay of spectrum.");
            LPXShell.WriteLine("          {0,-12} > {1}", "SY", "Switch the FFT performance style.");
            LPXShell.WriteLine("       {0,-15} - {1}", "-v <VALUE>", "[ALL] Set the value to meet the need of command");
            LPXShell.WriteLine();
            LPXShell.WriteLine("    Value definition of argument '-v':");
            LPXShell.WriteLine("       Mode FFT:");
            LPXShell.WriteLine("          Option SS:");
            LPXShell.WriteLine("             {0,-10} > {1}", "Decibel", "Scaling by decibel");
            LPXShell.WriteLine("             {0,-10} > {1}", "Linear", "Scaling by power");
            LPXShell.WriteLine("             {0,-10} > {1}", "Sqrt", "Scaling by the square root");
            LPXShell.WriteLine("          Option DT:");
            LPXShell.WriteLine("             {0,-10} > {1}", "<Integer>", "Millisecond. Any Integer within the sensible range");
            LPXShell.WriteLine("       Mode MPLY:");
            LPXShell.WriteLine("          Option ID:");
            LPXShell.WriteLine("             {0,-10} > {1}", "<Integer>", "Sequence number of song in list");
            LPXShell.WriteLine("          Option SN:");
            LPXShell.WriteLine("             {0,-10} > {1}", "<String>", "song's name (wrap with DOUBLE quotes )");
        }
    }
}
