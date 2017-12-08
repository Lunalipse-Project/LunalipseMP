using NewMediaPlayer.Dialog;
using System;
using System.Windows;
using System.Windows.Controls;

namespace NewMediaPlayer.ui
{
    /// <summary>
    /// OLmusicLib.xaml 的交互逻辑
    /// </summary>
    public partial class OLmusicLib : Page
    {
        string musp;
        double vol;
        LDailog ldd;
        public OLmusicLib(LDailog ld)
        {
            InitializeComponent();
            ldd = ld;
            dlsp.Text = musp = global.DOWNLOAD_SAVE_PATH;
            lrcp.Text = musp + @"\Lyrics";
            vol_adj.Value = vol = global.PRELISTEN_VOLUME;
        }

        private void vdj(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(perc!=null) perc.Content = Decimal.Round(new decimal(e.NewValue), 1) * 100 + "%";
            vol = e.NewValue;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var f_music = new System.Windows.Forms.FolderBrowserDialog();
            if (f_music.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (f_music.SelectedPath != "")
                {
                    dlsp.Text= musp = f_music.SelectedPath;
                    lrcp.Text = musp + @"\Lyrics";
                }
            }
        }

        private void apply_Click(object sender, RoutedEventArgs e)
        {
            global.DOWNLOAD_SAVE_PATH = musp;
            global.PRELISTEN_VOLUME = vol;
            ldd.Close();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            ldd.Close();
        }
    }
}
