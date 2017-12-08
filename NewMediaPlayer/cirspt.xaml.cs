using NewMediaPlayer.Generic;
using NewMediaPlayer.Sound;
using NewMediaPlayer.ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WindowsDesktop;

namespace NewMediaPlayer
{
    /// <summary>
    /// cirspt.xaml 的交互逻辑
    /// </summary>
    public partial class cirspt : Window
    {
        ui.Heartbeater.Beater b;
        public cirspt()
        {
            InitializeComponent();
            AllowsTransparency = true;
            SourceInitialized += delegate
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                uint extendedStyle = U32.GetWindowLong(hwnd, U32.GWL_EXSTYLE);
                U32.SetWindowLong(hwnd, U32.GWL_EXSTYLE, extendedStyle | U32.WS_EX_TRANSPARENT);

                U32.SetWindowPos(hwnd, 1, 0, 0, 0, 0, U32.SE_SHUTDOWN_PRIVILEGE);
            };

            b = new ui.Heartbeater.Beater(0x0001, () =>
            {
                cirspt csr = new cirspt();
                csr.ShowInTaskbar = false;
                csr.Show();
            });
            b.startBeating();

            InitializeFFT.OnSpectrumDrawnComplete += (isu) =>
            {
                if (global.USE_CIRCULAR_SPECT)
                {
                    cfft.Source = isu;
                }
                else cfft.Source = null;
            };

            VirtualDesktop.CurrentChanged += (x, y) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (global.SUPPORT_VDESKTOP)
                    {
                        var hld = new WindowInteropHelper(this).Handle;
                        if (hld != IntPtr.Zero&& !VirtualDesktopHelper.IsCurrentVirtualDesktop(hld))
                        {
                            this.MoveToDesktop(VirtualDesktop.Current);
                        }
                    }
                });
            };
            
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            U32.SetWindowPos(hwnd, 1, 0, 0, 0, 0, U32.SE_SHUTDOWN_PRIVILEGE);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            b.kill();
        }
    }
}
