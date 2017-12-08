using NewMediaPlayer.Generic;
using NewMediaPlayer.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NewMediaPlayer.ui
{
    /// <summary>
    /// LShell.xaml 的交互逻辑
    /// </summary>
    public partial class LShell : Page
    {
        LunalipseShellFilter lsf;
        public LShell()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
            lsf = new LunalipseShellFilter(console);
            
            LPXShell.OnMessageAppend += (m) =>
            {
                console.Text += m;
            };
            LPXShell.OnPromte += () =>
            {
                console.InsertNewPrompt();
            };
            LPXShell.OnClear += () =>
            {
                console.Text = "";
            };
            DrawStartLogo();
            console.CommandEntered += (o, cmd) =>
            {
                lsf.Filtering(cmd.Command);
                console.InsertNewPrompt();
                console.ScrollToEnd();
            };

        }

        void RegCommand()
        {
            console.RegisteredCommands.Add("lunalipse");
            console.Help_Descriptor.Add("lunalipse", "Lunalipse Music Player kernel.Basic music play support.");
            console.RegisteredCommands.Add("lpxcplr");
            console.Help_Descriptor.Add("lpxcplr", "Support for Lunalipse Behavior Script (LBS) (de)compiler");
            console.RegisteredCommands.Add("autosd");
            console.Help_Descriptor.Add("autosd", "Shutdown the player and system after a specific time or a song");
            console.RegisteredCommands.Add("cfgtl");
            console.Help_Descriptor.Add("cfgtl", "[Experimental] Configuration file tool.");
        }

        void DrawStartLogo()
        {
            new Thread(new ThreadStart(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    LPXShell.Flush();
                    LPXShell.WriteLine(
                               "\n                     @???  ???\n" +
                               "                   ?       ~#                                   \n" +
                               "                 $       ?#                                     \n" +
                               "                ?       ?                                       \n" +
                               "               &       ??\n" +
                               "              @        ?#                                       \n" +
                               "              &         @          #                            \n" +
                               "              ?         ?#       ? ?                            \n" +
                               "              ?           ?$@@$~?  ?\n" +
                               "              ??                   @                            \n" +
                               "               ~                 ??\n" +
                               "                ?                @                              \n" +
                               "                 &             ?\n" +
                               "                   $?       ??\n" +
                               "                        #             \n" +
                               "                                                                ");
                    LPXShell.WriteLine("[INFO] Initializing Lunalipse Shell Environment......");
                });
                Thread.Sleep(2500);
                Dispatcher.Invoke(() =>
                {
                    console.Prompt = "\nLunalipseShell> ";
                    LPXShell.Flush();
                    console.Text += "Lunalipse Music Player [Version: {0}]\n".FormateEx(Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    console.Text += "(c) 2017 Lunaixsky. All rights reserved\n\n";
                    console.Text += "Use the command 'help' to see available commands.\n\n";
                    RegCommand();
                    LPXShell.EndWriting();
                });
            })).Start();
        }
    }
}
