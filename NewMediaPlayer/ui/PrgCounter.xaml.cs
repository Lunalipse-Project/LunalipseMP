using I18N;
using NewMediaPlayer.Dialog;
using NewMediaPlayer.Lunalipx;
using System.Windows;
using System.Windows.Controls;

namespace NewMediaPlayer.ui
{
    /// <summary>
    /// PrgCounter.xaml 的交互逻辑
    /// </summary>
    public partial class PrgCounter : Page
    {
        SyntaxParser _sp;
        MainWindow _mw;
        LDailog _ld;
        I18NHelper ih;
        PageLang PL;
        public PrgCounter(SyntaxParser sp,MainWindow mw,LDailog ld)
        {
            InitializeComponent();
            ih = I18NHelper.INSTANCE;
            PL = ih.GetReferrence("scriptCfg");
            p1.Content = PL.GetContent("curPtr");
            p2.Content = PL.GetContent("newPtr");
            delay.Content = PL.GetContent("apyCfgD");
            imm.Content = PL.GetContent("apyCfgN");
            ld.Title.Content = PL.GetContent("title");
            _sp = sp;
            _mw = mw;
            _ld = ld;
            counter.Content = sp.PROGRAME_COUNTER;
            mw.OnExecuteCompletely += () =>
            {
                counter.Content = sp.PROGRAME_COUNTER;
            };
        }


        //Immidately called
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int _P = 0;
            warning.Visibility = Visibility.Hidden;
            try
            {
                _P = int.Parse(n_cnter.Text);
                _sp.PROGRAME_COUNTER = _P;
                //Applying Change
                _mw.Ps_opsc();
                _ld.Close();
            }
            catch
            {
                warning.Visibility = Visibility.Visible;
            } 
        }

        private void delay_Click(object sender, RoutedEventArgs e)
        {
            int _P = 0;
            warning.Visibility = Visibility.Hidden;
            try
            {
                _P = int.Parse(n_cnter.Text);
                _sp.PROGRAME_COUNTER = _P;
                _ld.Close();
            }
            catch
            {
                warning.Visibility = Visibility.Visible;
            }
        }
    }
}
