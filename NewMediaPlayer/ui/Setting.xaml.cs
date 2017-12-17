using I18N;
using LpxResource.Generic;
using NewMediaPlayer.controler;
using NewMediaPlayer.Dialog;
using NewMediaPlayer.Generic.Attr;
using NewMediaPlayer.Shell;
using NewMediaPlayer.Sound;
using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace NewMediaPlayer.ui
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Setting : Page
    {
        LDailog ld;
        PageLang PL;
        LunalipseInterface LI;
        public Setting(LDailog _ld)
        {
            InitializeComponent();
            ld = _ld;
            LI = LunalipseInterface.INSTANCE;

            foreach(FieldInfo fi in typeof(global).GetFields(BindingFlags.Static|BindingFlags.Public))
            {
                if (fi.GetCustomAttribute(typeof(ExternSetting)) == null) continue;
                CheckBox cb = FindName(fi.Name) is CheckBox _cb ? _cb : null;
                if (cb == null) continue;
                cb.IsChecked = (bool)fi.GetValue(null);
            }
            consolas.Visibility = global.USE_SHELL ? Visibility.Visible : Visibility.Hidden;

            PL = I18NHelper.INSTANCE.GetReferrence("Setting");
            for (int i = 1; i <= 5; i++)
            {
                string __ctx = PL.GetContent("l{0}".FormateEx(i));
                if (i<4)
                {
                    scaling.Items.Add(__ctx);
                }
                else shape.Items.Add(__ctx);
            }
            shape.SelectedIndex = global.USE_CIRCULAR_SPECT ? 1 : 0;
            

            foreach(var i in I18NHelper.INSTANCE.GetReferrence("common").AllLang)
            {
                lang.Items.Add(i.Value);
            }

            if (global.LANG != null) lang.SelectedIndex = (int)global.LANG;
            else lang.SelectedIndex = 0;

            scaling.SelectedIndex = (int)global.SSTR;
            freq.Text = global.FFT_REF_FRQ.ToString();

            _ld.Title.Content = PL.GetContent("title");
            ApplyLang();

        }

        void ApplyLang()
        {
            for(int i = 1; i <= 7; i++)
            {
                Label l = FindName("t" + i) is Label ? FindName("t" + i) as Label : null;
                if (l == null) continue;
                l.Content = PL.GetContent("T{0}".FormateEx(i));
                for(int j=1; ;j++)
                {
                    object o = FindName("t{0}s{1}".FormateEx(i, j));
                    Label lj = o is Label l_ ? l_ : null;
                    if (lj == null) break;
                    lj.Content = PL.GetContent("T{0}_Sub{1}".FormateEx(i, j));
                }
            }

            success.Content = PL.GetContent("sucess");
            apply.Content = PL.GetContent("apy");
            consolas.Content = PL.GetContent("console");
            plgmana.Content = PL.GetContent("plgm");
        }

        //Events
        

        private void StatusCheck(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            bool status = cb.IsChecked.HasValue ? (bool)cb.IsChecked : IsInitialized;
            typeof(global).GetField(cb.Name, BindingFlags.Static | BindingFlags.Public).SetValue(null, status);
            consolas.Visibility = global.USE_SHELL ? Visibility.Visible : Visibility.Collapsed;
        }

        private void scaling_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ScalingStrategy s;
            switch (scaling.SelectedIndex)
            {
                case 0:
                    s = ScalingStrategy.Decibel;
                    break;
                case 1:
                    s = ScalingStrategy.Linear;
                    break;
                case 2:
                    s = ScalingStrategy.Sqrt;
                    break;
                default:
                    s = ScalingStrategy.Linear;
                    break;
            }
            LI.ChangeScalingStrategy(global.SSTR = s);
        }

        //Apply
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            global.FFT_REF_FRQ = validate();
            bool b;
            if (b = PropertyHelper.SaveProperty())
            {
                success.Content = PL.GetContent("sucess");
            }
            else
            {
                success.Content = PL.GetContent("SaveFT");
            }
            try
            {
                new Thread(() =>
                {
                    Dispatcher.Invoke(() => notify.BeginAnimation(HeightProperty, new DoubleAnimation(0, 30, TimeSpan.FromSeconds(.5))));
                    Thread.Sleep(2500);
                    Dispatcher.Invoke(() => notify.BeginAnimation(HeightProperty, new DoubleAnimation(30, 0, TimeSpan.FromSeconds(.5))));
                }).Start();
            }
            catch { }
            new LDailog(LunalipsContentUI.TIPMESSAGE, PL.GetContent(b? "SaveST": "SaveFT"), PL.GetContent(b? "SaveSTc": "SaveFTc")).ShowDialog();
        }

        public int validate()
        {
            try
            {
                int a = int.Parse(freq.Text);
                if(a<=15&&a>=500)
                {
                    return 45;
                }
                return a;
            }
            catch { return 45; }
        }

        private void LangChange(object sender, SelectionChangedEventArgs e)
        {
            Languages? l = Languages.CHINESE;
            switch(lang.SelectedIndex)
            {
                case 0:
                    l = Languages.CHINESE;
                    break;
                case 1:
                    l = Languages.TRADITIONAL;
                    break;
                case 2:
                    l = Languages.ENGLISH;
                    break;
                case 3:
                    l = Languages.RUSSIAN;
                    break;
            }
            global.LANG = l;
        }

        private void plgM(object sender, RoutedEventArgs e)
        {
            new LDailog(LunalipsContentUI.PLUGIN_MANAGER, "", true).ShowDialog();
        }

        private void OpenCmd(object sender, RoutedEventArgs e)
        {
            new LDailog(LunalipsContentUI.LUNALIPSE_SHELL, "Lunalipse Shell", true).Show();
        }

        private void shapeChanged(object sender, SelectionChangedEventArgs e)
        {
            global.USE_CIRCULAR_SPECT = (shape.SelectedIndex == 1);
        }
    }
}
