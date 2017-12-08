using I18N;
using NewMediaPlayer.Dialog;
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

namespace NewMediaPlayer.ui
{
    /// <summary>
    /// Dlg_PN.xaml 的交互逻辑
    /// </summary>
    public partial class Dlg_PN : Page
    {
        LDailog ldd;
        I18NHelper ih;
        PageLang pl;
        public Dlg_PN(string msg, LDailog ld)
        {
            InitializeComponent();
            ldd = ld;
            cnt.Text = msg;
            ih = I18NHelper.INSTANCE;
            pl = ih.GetReferrence("Dia_Tip");
            ok.Content = pl.GetContent("ok");
            no.Content = pl.GetContent("no");
        }

        private void okClk(object sender, RoutedEventArgs e)
        {
            ldd.DialogResult = true;
        }

        private void noClk(object sender, RoutedEventArgs e)
        {
            ldd.DialogResult = false;
        }
    }
}
