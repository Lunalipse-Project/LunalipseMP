﻿using I18N;
using LpxResource.Generic;
using NewMediaPlayer.controler;
using NewMediaPlayer.Dialog;
using NewMediaPlayer.Shell;
using NewMediaPlayer.Sound;
using System;
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

            fft.IsChecked = global.SHOW_FFT;
            name.IsChecked = global.SHOW_MUSIC_NAME;
            durtion.IsChecked = global.SHOW_CUR_DURATION;
            vdesk.IsEnabled = Environment.OSVersion.Version.Major == 10;
            vdesk.IsChecked = global.SUPPORT_VDESKTOP;
            diplrc.IsChecked = global.DISP_LYRIC;
            plgsec.IsChecked = global.PLUGIN_SECURITY;
            enableLog.IsChecked = global.LOG_RECORD;
            expbin.IsChecked = global.EXPORT_BIN;
            usebin.IsChecked = global.USE_BIN;
            syslang.IsChecked = global.USE_SYS_LANG;
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
            t1.Content = PL.GetContent("T1");
            t1s1.Content = PL.GetContent("T1_Sub1");
            t1s2.Content = PL.GetContent("T1_Sub2");
            t1s3.Content = PL.GetContent("T1_Sub3");
            t1s4.Content = PL.GetContent("T1_Sub4");
            t1s5.Content = PL.GetContent("T1_Sub5");

            t2.Content = PL.GetContent("T2");
            t2s1.Content = PL.GetContent("T2_Sub1");
            t2s2.Content = PL.GetContent("T2_Sub2");
            t2s3.Content = PL.GetContent("T2_Sub3");

            t3.Content = PL.GetContent("T3");
            t3s1.Content = PL.GetContent("T3_Sub1");
            t3s2.Content = PL.GetContent("T3_Sub2");

            t4.Content = PL.GetContent("T4");
            t4s1.Content = PL.GetContent("T4_Sub1");

            t5.Content = PL.GetContent("T5");
            t5s1.Content = PL.GetContent("T5_Sub1");
            t5s2.Content = PL.GetContent("T5_Sub2");

            t6.Content = PL.GetContent("T6");
            t6s1.Content = PL.GetContent("T6_Sub1");
            t6s2.Content = PL.GetContent("T6_Sub2");

            

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
            switch (cb.Name)
            {
                case "fft":
                    global.SHOW_FFT = status;
                    break;
                case "name":
                    global.SHOW_MUSIC_NAME = status;
                    break;
                case "durtion":
                    global.SHOW_CUR_DURATION = status;
                    break;
                case "vdesk":
                    global.SUPPORT_VDESKTOP = status;
                    break;
                case "diplrc":
                    global.DISP_LYRIC = status;
                    break;
                case "plgsec":
                    global.PLUGIN_SECURITY = status;
                    break;
                case "enableLog":
                    global.LOG_RECORD = status;
                    break;
                case "expbin":
                    global.EXPORT_BIN = status;
                    break;
                case "usebin":
                    global.USE_BIN = status;
                    break;
                case "console":
                    global.USE_SHELL = status;
                    consolas.Visibility = global.USE_SHELL ? Visibility.Visible : Visibility.Collapsed;
                    break;
                case "syslang":
                    global.USE_SYS_LANG = status;
                    break;
            }
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