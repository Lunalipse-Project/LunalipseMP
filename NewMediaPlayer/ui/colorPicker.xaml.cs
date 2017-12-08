using I18N;
using NewMediaPlayer.Dialog;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NewMediaPlayer.ui
{
    public delegate void SetGradient(LinearGradientBrush lb);
    /// <summary>
    /// colorPicker.xaml 的交互逻辑
    /// </summary>
    public partial class colorPicker : Page
    {
        public static event SetGradient onGradientSet;
        LinearGradientBrush lgb;
        Color R,L;
        PageLang PL;
        int curS = 0;
        public colorPicker(LDailog ld)
        {
            InitializeComponent();
            PL = I18NHelper.INSTANCE.GetReferrence("colorP");
            lgb = new LinearGradientBrush();
            lgb.StartPoint = new Point(0, 0.5);
            lgb.EndPoint = new Point(1, 0.5);
            R = Color.FromRgb(255, 255, 255);
            L = Color.FromRgb(0, 0, 0);
            lgb.GradientStops.Add(new GradientStop(L,0));//0
            lgb.GradientStops.Add(new GradientStop(R,1));//1

            ld.Title.Content = PL.GetContent("title");
            a.Content = PL.GetContent("a");
            r.Content = PL.GetContent("r");
            g.Content = PL.GetContent("g");
            b.Content = PL.GetContent("b");
            Bapply.Content = PL.GetContent("ap");

        }

        private void delay_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            switch(b.Name)
            {
                case "left":
                    _as.Value = L.A;
                    rs.Value = L.R;
                    gs.Value = L.G;
                    bs.Value = L.B;
                    curS = 0;
                    break;
                case "right":
                    _as.Value = R.A;
                    rs.Value = R.R;
                    gs.Value = R.G;
                    bs.Value = R.B;
                    curS = 1;
                    break;
            }
        }

        private void offsetChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                lgb.GradientStops[1].Offset = offsetS.Value;
            }
            catch { }
        }

        private void apply(object sender, RoutedEventArgs e)
        {
            LinearGradientBrush _lgb = lgb.Clone() as LinearGradientBrush;
            _lgb.StartPoint = new Point(0.5, 0);
            _lgb.EndPoint = new Point(0.5, 1);
            onGradientSet(_lgb);
        }

        private void colorc(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (curS == 0)
                {
                    R = Color.FromArgb((byte)(int)_as.Value, (byte)(int)rs.Value, (byte)(int)gs.Value, (byte)(int)bs.Value);
                    lgb.GradientStops[curS].Color = R;
                }
                else
                {
                    L = Color.FromArgb((byte)(int)_as.Value, (byte)(int)rs.Value, (byte)(int)gs.Value, (byte)(int)bs.Value);
                    lgb.GradientStops[curS].Color = L;
                }
                preview.Fill = lgb;
                a_v.Content = (int)_as.Value;
                r_v.Content = (int)rs.Value;
                g_v.Content = (int)gs.Value;
                b_v.Content = (int)bs.Value;
            }
            catch { }
        }
    }
}
