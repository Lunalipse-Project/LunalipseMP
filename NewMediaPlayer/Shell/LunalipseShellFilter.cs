using LunalipseShell;
using NewMediaPlayer.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NewMediaPlayer.Shell
{
    class LunalipseShellFilter
    {
        LSTerminal lst;
        ConfigShell CS;
        LunalipseInterface LI;
        public LunalipseShellFilter(LSTerminal _lst)
        {
            lst = _lst;
            CS = new ConfigShell();
            LI = LunalipseInterface.INSTANCE;
        }
        public void Filtering(Command cmd)
        {
            switch(cmd.Name)
            {
                case "help":
                    LPXShell.WriteLine("  [*WARNING*] Lunalipse Shell are still under experiment and debugging.\n" +
                                "              It may cause some unnecessary loss to your computer without using correctly.");
                    lst.RegisteredCommands.ForEach(c => {
                        LPXShell.WriteLine("    {0,-10} - {1}".FormateEx(c, lst.Help_Descriptor[c]));
                    });
                    break;
                case "lpxcplr":
                    Lunalipx.SyntaxParser.INSTANCE.SP_CMD_Executor(cmd);
                    break;
                case "cls":
                    LPXShell.Flush();
                    LPXShell.WriteLine("Lunalipse Music Player [Version: {0}]",Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    LPXShell.WriteLine("(c) 2017 Lunaixsky. All rights reserved\n");
                    LPXShell.WriteLine("Use the command 'help' to see available commands.\n");
                    break;
                case "cfgtl":
                    CS.ConfigShellCmd(cmd);
                    break;
                case "lunalipse":
                    LI.ParseCommand(cmd);
                    break;
                case "autosd":
                    AutoShd.ParseCommand(cmd);
                    break;
                default:
                    LPXShell.WriteLine("Unknow command. Check for spelling or use 'help' to see available commands.\n");
                    break;
            }
        }
    }
}
