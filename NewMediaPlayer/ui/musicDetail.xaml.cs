using gMusic.MusicOL;
using gMusic.util;
using NetEaseHijacker;
using NetEaseHijacker.Types;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using NewMediaPlayer.Generic;

namespace NewMediaPlayer.ui
{
    /// <summary>
    /// musicDetail.xaml 的交互逻辑
    /// </summary>
    public partial class musicDetail : Page
    {
        Downloader der;
        SDetail md;
        string ChoosenUrl = "";
        bool firstOpen = false;
        Hijack hj;
        string ext;
        public musicDetail(SDetail _md)
        {
            InitializeComponent();
            prev.LoadedBehavior = MediaState.Play;
            prev.UnloadedBehavior = MediaState.Stop;
            hj = new Hijack();
            hj.E_TimeOut(() => MessageBox.Show("无网络连接", "错误", MessageBoxButton.OK, MessageBoxImage.Error));
            hj.E_Responded((x, y) =>
            {
                switch ((SearchType)Enum.Parse(typeof(SearchType), x))
                {
                    case SearchType.DOWNLOAD:
                        if(!firstOpen)
                        {
                            ChoosenUrl = hj.ParseDownloadURL(y.ResultData);
                            prev.Source = new Uri(ChoosenUrl);
                            firstOpen = true;
                        }
                        else
                        {
                            ChoosenUrl = hj.ParseDownloadURL(y.ResultData);
                        }
                        break;
                }
            });
            prev.Volume = global.PRELISTEN_VOLUME;
            md = _md;
            RegistEvent();
            ProccessDetail();
            hj.DownloadURL(md.id, md.bitrate.Possible((x) => x != 0).ToString());
            
        }

        public void RegistEvent()
        {
            der = new Downloader();
            double total = 0;
            string t = "";
            double prec = 0;
            der.OnDataSetup += (x) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    t = sizeCalc.Calc(x);
                    status.Content = "正在下载：0MB / " + t;
                    total = x;
                    prgs.Maximum = x;
                }));
            };
            der.OnDownloadFinish += (x, y) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (!x)
                    {
                        status.Content = "";
                        prgV.Content = "当前没有任务";
                        prgs.Value = 0;
                    }
                    else
                    {
                        LogFile.WriteLog("ERROR", y.Message);
                        Console.WriteLine(y.ToString());
                    }
                }));
            };
            der.OnTaskUpdate += (x) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    prec = x / total * 100d;
                    status.Content = "正在下载：" + sizeCalc.Calc(x) + " / " + t;
                    prgV.Content = decimal.Round(new decimal(prec), 1).ToString() + "%";
                    prgs.Value = x;
                }));
            };
        }

        public void ProccessDetail()
        {
            if (md == null) return;
            Dispatcher.Invoke(new Action(() =>
            {
                name.Content = md.name;
                album.Source = new BitmapImage(new Uri(md.al_pic));
                singer.Content = md.ar_name;
                bandName.Content = md.al_name;
                string[] s = md.L_URL.Split('.');
                fomate.Content = ext = s[s.Length - 1].ToUpperInvariant();
                hsize.Content = sizeCalc.Calc(long.Parse(md.SIZE[0].ToString()));
                msize.Content = sizeCalc.Calc(long.Parse(md.SIZE[1].ToString()));
                lsize.Content = sizeCalc.Calc(long.Parse(md.SIZE[2].ToString()));
            }));
        }

        private void DownloadSongs(object sender, RoutedEventArgs e)
        {
            if (md == null) return;
            Button b = sender as Button;
            string u = "";
            long bv = 0;
            switch (b.Name)
            {
                case "l":
                    u = md.L_URL;
                    bv = md.SIZE[2];
                    break;
                case "m":
                    u = md.M_URL;
                    bv = md.SIZE[1];
                    break;
                case "h":
                    u = md.H_URL;
                    bv = md.SIZE[0];
                    break;
            }
            RunDownload(u, bv);
        }

        public void RunDownload(string _u, long a)
        {
            Console.WriteLine(_u);
            Thread t = new Thread(new ThreadStart(() =>
            {
                der.DownloadFile(_u, String.Format(global.DOWNLOAD_SAVE_PATH + "/{0}.{1}", md.musicN.Replace(':', ' '), ext.ToLowerInvariant()), a);
            }));
            t.Start();
        }
    }
}
