using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using NewMediaPlayer.Sound;
using WindowsDesktop;
using NewMediaPlayer.ui;
using System.Windows.Media.Animation;
using NewMediaPlayer.Generic;

namespace NewMediaPlayer
{
    public delegate string GMusicName();
    /// <summary>
    /// floating.xaml 的交互逻辑
    /// </summary>
    public partial class floating : Window
    {
        ui.Heartbeater.Beater bt;
        public static event GMusicName GetCurMusic;
        public floating()
        {
            InitializeComponent();
            AllowsTransparency = true;
            this.Width = SystemParameters.WorkArea.Width;
            lrcdpL.Width = Width; ;
            this.Topmost = true;
            
            this.SourceInitialized += delegate
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                uint extendedStyle = U32.GetWindowLong(hwnd, U32.GWL_EXSTYLE);
                U32.SetWindowLong(hwnd, U32.GWL_EXSTYLE, extendedStyle | U32.WS_EX_TRANSPARENT);
            };

            bt = new ui.Heartbeater.Beater(0x0002, () => {
                floating f = new floating();
                f.ShowInTaskbar = false;
                f.Left = SystemParameters.WorkArea.Width - f.Width;
                f.Top = SystemParameters.WorkArea.Height - f.Height;
                f.Topmost = true;
                f.Show();
            });
            bt.startBeating();

            name.Content = global.SHOW_MUSIC_NAME ? GetCurMusic?.Invoke() : "";

            InitializeFFT.OnSpectrumDrawnComplete += (isu) =>
            {
                if(!global.USE_CIRCULAR_SPECT)
                {
                    fftContainer_ft.Source = isu;
                }
                else fftContainer_ft.Source = null;
            };
            Lyric.LyricDecompiler.INSTANCE.LyricNotFoundTrigger += () =>
            {
                lrcdpL.Content = secLrc.Content = "";
            };
            Lyric.LyricsDisplay.OnLryicMatched += (lrc) =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    if (lrc.Contains("|"))
                    {
                        string[] lrcs = lrc.Split('|');
                        lrcdpL.Content = global.DISP_LYRIC ? lrcs[0] : "";
                        secLrc.Content = global.DISP_LYRIC ? lrcs[1] : "";
                    }
                    else
                    {
                        lrcdpL.Content = global.DISP_LYRIC ? lrc : "";
                        secLrc.Content = "";
                    }

                }));
            };
            MainWindow.OnMusicChanged += (Mname) =>
            {
                name.Content = global.SHOW_MUSIC_NAME? Mname:"";
            };
            PlaySound.OnDurationChanged += (a, b) =>
            {
                duration.Dispatcher.Invoke(new Action(() =>
                {
                    duration.Content = global.SHOW_CUR_DURATION ? b : "";
                    if (!global.SHOW_MUSIC_NAME && name.Visibility == Visibility.Visible)
                    {
                        name.Visibility = Visibility.Hidden;
                    }
                    else if(global.SHOW_MUSIC_NAME && name.Visibility != Visibility.Visible)
                    {
                        name.Visibility = Visibility.Visible;
                    }
                }));
            };
            VirtualDesktop.CurrentChanged += (x, y) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if(global.SUPPORT_VDESKTOP)
                    {
                        var hld = new WindowInteropHelper(this).Handle;
                        if (hld!=IntPtr.Zero&&!VirtualDesktopHelper.IsCurrentVirtualDesktop(hld))
                        {
                            this.MoveToDesktop(VirtualDesktop.Current);
                        }
                    }
                });
            };
            colorPicker.onGradientSet += (a) =>
            {
                lrcdpL.Foreground = secLrc.Foreground = a;
            };
        }

        private void Grid_LostFocus(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window w = (Window)sender;
            w.Topmost = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            bt.kill();
        }
    }
}
