using I18N;
using NewMediaPlayer.Dialog;
using System.Windows;
using System.Windows.Controls;

namespace NewMediaPlayer.ui
{
    /// <summary>
    /// LDlg_Tip.xaml 的交互逻辑
    /// </summary>
    public partial class LDlg_Tip : Page
    {
        LDailog ldd;
        I18NHelper ih;
        PageLang pl;
        public LDlg_Tip(string msg,LDailog ld)
        {
            InitializeComponent();
            ldd = ld;
            cnt.Text = msg;
            ih = I18NHelper.INSTANCE;
            pl = ih.GetReferrence("Dia_Tip");
            ok.Content = pl.GetContent("ok");
        }

        private void okClk(object sender, RoutedEventArgs e)
        {
            ldd.Close();
        }
    }
}
