using NewMediaPlayer.controler;
using NewMediaPlayer.Lunalipx;
using NewMediaPlayer.ui;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace NewMediaPlayer.Dialog
{
    public delegate void OnWindowClosing();
    /// <summary>
    /// LDailog.xaml 的交互逻辑
    /// </summary>
    public partial class LDailog : Window
    {
        public static event OnWindowClosing owc;
        public LDailog()
        {
            InitializeComponent();
        }
        public LDailog(LunalipsContentUI lcui,string header,params object[] args) : this()
        {
            Title.Content = header;
            InvokeChangeContent(lcui, null, args);
        }

        public LDailog(LunalipsContentUI lcui, string header,bool needResize, params object[] args) : this()
        {
            Title.Content = header;
            ResizeWindow(lcui, args);
        }

        public LDailog(object CONTENT_INSTANCE,double width,double height,string header) : this()
        {
            Height = height + 55;
            Width = width + 30;
            inner.Content = CONTENT_INSTANCE;
            Title.Content = header;
        }

        public LDailog(string i_,params object[] paras) : this()
        {
            LunalipsContentUI lcui = (LunalipsContentUI)Enum.Parse(typeof(LunalipsContentUI), i_);
            InvokeChangeContent(lcui, null, paras);
        }

        private void ResizeWindow(LunalipsContentUI lcui_, params object[] args_)
        {
            switch (lcui_)
            {
                case LunalipsContentUI.OL_DL_SETTING:
                    OLmusicLib oll = new OLmusicLib(this);
                    Height = oll.Height + 55;
                    Width = oll.Width + 30;
                    InvokeChangeContent(lcui_, oll, args_);
                    break;
                case LunalipsContentUI.MAINSETTING:
                    Setting ST = new Setting(this);
                    Height = ST.Height + 55;
                    Width = ST.Width + 30;
                    InvokeChangeContent(lcui_, ST, args_);
                    break;
                case LunalipsContentUI.PLUGIN_MANAGER:
                    PluginManager pm = new PluginManager(this);
                    Height = pm.Height + 55;
                    Width = pm.Width + 30;
                    InvokeChangeContent(lcui_, pm, args_);
                    break;
                case LunalipsContentUI.PRIVILEGE_VIEWER:
                    PrivilegeViewer pw = new PrivilegeViewer(this, args_[0] as string);
                    Height = pw.Height + 55;
                    Width = pw.Width + 30;
                    InvokeChangeContent(lcui_, pw);
                    break;
                case LunalipsContentUI.GRADIANT_ADJ:
                    colorPicker cp = new colorPicker(this);
                    Height = cp.Height + 55;
                    Width = cp.Width + 30;
                    InvokeChangeContent(lcui_, cp);
                    break;
                case LunalipsContentUI.LUNALIPSE_SHELL:
                    LShell ls = new LShell();
                    Height = ls.Height + 41;
                    Width = ls.Width + 16;
                    InvokeChangeContent(lcui_, ls);
                    break;
            }
        }

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        
        protected void InvokeChangeContent(LunalipsContentUI id,object a, params object[] pArg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                switch (id)
                {
                    case LunalipsContentUI.TIPMESSAGE:
                        inner.Content = new LDlg_Tip(pArg[0] as string, this);
                        break;
                    case LunalipsContentUI.CTRL_JUMP_TO_LINE:
                        PrgCounter pc = new PrgCounter(pArg[0] as SyntaxParser, pArg[1] as MainWindow, this);
                        inner.Content = pc;
                        break;
                    case LunalipsContentUI.OL_DL_SETTING:
                        inner.Content = (a as OLmusicLib);
                        break;
                    case LunalipsContentUI.MAINSETTING:
                        inner.Content = a as Setting;
                        break;
                    case LunalipsContentUI.PLUGIN_MANAGER:
                        inner.Content = a as PluginManager;
                        break;
                    case LunalipsContentUI.DIA_WITH_YESNO:
                        inner.Content = new Dlg_PN(pArg[0] as string,this);
                        break;
                    case LunalipsContentUI.PRIVILEGE_VIEWER:
                        inner.Content = a as PrivilegeViewer;
                        break;
                    case LunalipsContentUI.GRADIANT_ADJ:
                        inner.Content = a as colorPicker;
                        break;
                    case LunalipsContentUI.LUNALIPSE_SHELL:
                        inner.Content = a as LShell;
                        break;
                }
            }));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            owc?.Invoke();
        }
    }
}
