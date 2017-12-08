using CSCore.Streams.Effects;
using NewMediaPlayer.Generic;
using NewMediaPlayer.Sound;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NewMediaPlayer
{
    /// <summary>
    /// Equalizer.xaml 的交互逻辑
    /// </summary>
    public partial class Equalizer : Window
    {
        public int[,] Setting = new int[5, 10]
        {
            {0,0,0,0,0,0,0,0,0,0 },
            {6,4,-5,2,3,4,4,5,5,6 },
            {6,4,0,-2,-6,1,4,6,7,9},
            {4,0,1,2,3,4,5,4,3,3},
            {4,4,3,0,-4,-3,2,5,7,7}
        };
        ResourceHolder RH;
        public Equalizer()
        {
            InitializeComponent();
            RH = ResourceHolder.INSTANCE;
            presetting.Items.Clear();
            presetting.Items.Add("原音质");
            presetting.Items.Add("低音");
            presetting.Items.Add("摇滚");
            presetting.Items.Add("人声");
            presetting.Items.Add("方案1");
            for(int i=0;i<global.EQUALIZER_SAVE.Length;i++)
            {
                if (global.EQUALIZER_SAVE[i] != Setting[global.EQUALIZER_SET, i])
                {
                    for (int j = 1; j <= 10; j++)
                    {
                        (this.FindName("p" + j) as Slider).Value = global.EQUALIZER_SAVE[j - 1];
                    }
                    break;
                }
                if(i==9)
                {
                    presetting.SelectedIndex = global.EQUALIZER_SET;
                }
            }
            logo.Source = RH.getImage("LunaCM");
            this.Background = new ImageBrush(RH.getImage("CrEmp"));
            LogFile.WriteLog("INFO", "Lunalipse Equalizer Initialized");
        }

        private void vol_adj_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider adjust = sender as Slider;
            try
            {
                if (PlaySound.equzer != null && adjust != null)
                {
                    double perc = (adjust.Value / (double)adjust.Maximum);
                    var value = (float)(perc * 20);
                    int index = Int32.Parse(adjust.Name.Replace("p", ""));
                    ((Label)this.FindName("l" + index)).Content = (int)Math.Round(e.NewValue);
                    EqualizerFilter ef = PlaySound.equzer.SampleFilters[index];
                    ef.AverageGainDB = value;
                }
            }
            catch(Exception E) { LogFile.WriteLog("ERROR", E.Message); }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void presetting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            global.EQUALIZER_SET = presetting.SelectedIndex;
            for(int i=1;i<=10;i++)
            {
                (this.FindName("p" + i) as Slider).Value = Setting[presetting.SelectedIndex, i-1] * 10;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            for(int i=1;i<=10;i++)
            {
                global.EQUALIZER_SAVE[i - 1] = (int)Math.Round((this.FindName("p" + i) as Slider).Value);
            }
            LogFile.WriteLog("INFO", "Saving Lunalipse Equalizer Setting");
        }

        private void DelayAdj(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                Slider s = sender as Slider;
                switch (s.Name)
                {
                    case "rDly":
                        PlaySound.R = (float)e.NewValue;
                        rc.Text = "R-Channel:{0}".FormateEx(Math.Round(PlaySound.R));
                        break;
                    case "lDly":
                        PlaySound.L = (float)e.NewValue;
                        lc.Text = "L-Channel:{0}".FormateEx(Math.Round(PlaySound.R));
                        break;
                }
            }
            catch { }
        }

        //It's WetDry
        private void wetDart(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                PlaySound.WD = (float)e.NewValue;
                wp.Text = "Wet Percentage:{0}".FormateEx(Math.Round(PlaySound.WD));
            }
            catch { }
        }
    }
}
