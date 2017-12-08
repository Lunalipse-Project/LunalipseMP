using LunalipseShell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewMediaPlayer.Shell
{
    public delegate void Shutdown();
    public class AutoShd
    {
        static int mode = 0, igs = 0, sys = 0;
        static DateTime dt;
        static bool keepCheck = false, hasSet = false, nextDay = false;
        static Thread listenT;

        public static event Shutdown OnShutdownRequested;
        public static void ParseCommand(Command cmd)
        {
            if (cmd.Args.Length <= 0)
            {
                ShowHelpHint();
                return;
            }
            for(int i=0;i<cmd.Args.Length;i+=2)
            {
                switch(cmd.Args[i])
                {
                    case "-help":
                        ShowHelpHint();
                        break;
                    case "-m":
                        switch (cmd.Args[i + 1])
                        {
                            case "TSD":
                                mode = 0;
                                break;
                            case "CLS":
                                mode = 1;
                                break;
                            default:
                                LPXShell.WriteLine("[Fatal] Mode '{0}' not defined.", cmd.Args[i]);
                                return;
                        }
                        break;
                    case "-t":
                        if(!DateTime.TryParseExact(cmd.Args[i + 1],"HH:mm:ss",CultureInfo.InvariantCulture,DateTimeStyles.None, out dt)
                            && !DateTime.TryParseExact(cmd.Args[i + 1], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            LPXShell.WriteLine("[Fatal] Unable to solve time from '{0}'.", cmd.Args[i]);
                            hasSet = false;
                            return;
                        }
                        hasSet = true;
                        break;
                    case "-igsong":
                        switch(cmd.Args[i+1])
                        {
                            case "y":
                                igs = 0;
                                break;
                            case "n":
                                igs = 1;
                                break;
                            default:
                                LPXShell.WriteLine("[Fatal] Option '{0}' not defined.", cmd.Args[i]);
                                return;
                        }
                        break;
                    case "-sys":
                        switch (cmd.Args[i + 1])
                        {
                            case "n":
                                sys = 0;
                                break;
                            case "s":
                                sys = 1;
                                break;
                            case "h":
                                sys = 2;
                                break;
                            default:
                                LPXShell.WriteLine("[Fatal] Option '{0}' not defined.", cmd.Args[i]);
                                return;
                        }
                        break;
                    case "-nxtd":
                        nextDay = true;
                        break;

                }
            }
            RunByMode();
        }

        public static void RunByMode()
        {
            if(nextDay)
            {
                dt = dt.AddDays(1);
            }
            switch (mode)
            {
                case 0:
                    if (!hasSet)
                    {
                        LPXShell.WriteLine(" [ERROR] No time is specified. Use argument '-t' to specified a time.");
                        return;
                    }
                    hasSet = false;
                    keepCheck = true;
                    if (igs == 0)
                    {
                        ListenPer20s();
                    }
                    LPXShell.WriteLine(" [INFO] Lunalipse {0}will be shut down at {1} {2}.",
                        sys==0?"":"and Windows ",
                        dt.ToString(),
                        igs==0?"immediately":"");
                    break;
                case 1:
                    listenT?.Abort();
                    keepCheck = hasSet = false;
                    LPXShell.WriteLine(" [INFO] Plan has been canceled.");
                    break;
            }
        }

        private static void ListenPer20s()
        {
            listenT = new Thread(new ThreadStart(() =>
            {
                while(keepCheck)
                {
                    if(DateTime.Compare(DateTime.Now, dt) >= 0)
                    {
                        shutd();
                    }
                    Thread.Sleep(5000);
                }
            }));
            listenT.Start();
        }

        public static void ManualCheck()
        {
            if (igs == 0) return;
            if (DateTime.Compare(DateTime.Now, dt) >= 0)
            {
                shutd();
            }
        }

        static void shutd()
        {
            if (sys >= 1)
            {
#if DEBUG
                Process.Start("shutdown.exe", sys == 1 ? "-s -f -t 10" : "-h");
#else
                Console.WriteLine("shutdown.exe {0}",sys == 1 ? "-s -f -t 10" : "-h");
#endif
            }
            keepCheck = hasSet = false;
            OnShutdownRequested?.Invoke();
        }

        private static void ShowHelpHint()
        {
            LPXShell.WriteLine(" Lunalipse Lullaby Tool (LLT)   [Version 1.0.0.0]");
            LPXShell.WriteLine(" The tool provide basic functionality for your lullabies playing.");
            LPXShell.WriteLine();
            LPXShell.WriteLine("    Arguments:");
            LPXShell.WriteLine("        {0,-15} - {1}", "-m <T_SD|CLS>", "set the mode of LLT");
            LPXShell.WriteLine("           {0,-10} - {1}", "T_SD", "Shutdown timer");
            LPXShell.WriteLine("           {0,-10} - {1}", "CLS", "Clear Setting");
            LPXShell.WriteLine("        {0,-15} - {1}", "-t", "Time to shutdown.(Must be in the form of HH:mm:ss)");
            LPXShell.WriteLine("        {0,-15} - {1}", "-igsong <y|n>", "Ignore the song.");
            LPXShell.WriteLine("           {0,-10} - {1}", "y", "This will shutdown the Lunalipse or system at exact time even song in playing");
            LPXShell.WriteLine("           {0,-10} - {1}", "n", "If specified time is up. Wait until the current song finished its playing");
            LPXShell.WriteLine("        {0,-15} - {1}", "-sys <n|s|h>", "System shutdown option");
            LPXShell.WriteLine("           {0,-10} - {1}", "n", "Just shutdown Lunalipse.");
            LPXShell.WriteLine("           {0,-10} - {1}", "s", "Shutdown the whole Windows OS");
            LPXShell.WriteLine("           {0,-10} - {1}", "h", "Make the system into hibernation status.(Not shutdown)"); 
            LPXShell.WriteLine("        {0,-15} - {1}", "-nxtd", "Remainding Lunalipse the time is in next day.");
        }
    }
}
