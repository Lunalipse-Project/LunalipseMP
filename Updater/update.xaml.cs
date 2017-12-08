using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;

namespace Updater
{
    /// <summary>
    /// update.xaml 的交互逻辑
    /// </summary>
    public partial class update : Page
    {
        const int TARGET_WIDTH = 393;
        const int EACH_STEP = 2;
        DispatcherTimer dt;
        public update()
        {
            InitializeComponent();
            pgb.Width = 0;
        }

        public void doLoop()
        {
            dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(0.001);
            pgb.HorizontalAlignment = HorizontalAlignment.Left;
            bool isdone = false ;
            dt.Tick += (o, e) =>
            {
                if(pgb.Width< TARGET_WIDTH && !isdone)
                {
                    pgb.Width+= EACH_STEP;
                }
                else if(pgb.Width>= TARGET_WIDTH)
                {
                    pgb.HorizontalAlignment = HorizontalAlignment.Right;
                    isdone = true;
                    pgb.Width-= EACH_STEP;
                }
                else if(pgb.Width>0 && isdone)
                {
                    pgb.Width-= EACH_STEP;
                }
                else
                {
                    pgb.HorizontalAlignment = HorizontalAlignment.Left;
                    isdone = false;
                    pgb.Width+= EACH_STEP;
                }
            };
            dt.Start();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            doLoop();
        }
    }
}
