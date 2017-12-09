using NetEaseHijacker;
using NetEaseHijacker.Types;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using NewMediaPlayer.Generic;
using System.Threading.Tasks;

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
                            string[] tmp = ChoosenUrl.Split('.');
                            
                            Dispatcher.Invoke(() => { prev.Source = new Uri(ChoosenUrl); fomate.Content = ext = tmp[tmp.Length - 1]; });
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
                    t = Utils.SizeCalc(x);
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
                    status.Content = "正在下载：" + Utils.SizeCalc(x) + " / " + t;
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
                //fomate.Content = ext = s[s.Length - 1].ToUpperInvariant();
                hsize.Content = Utils.SizeCalc(md.sizes[0]);
                msize.Content = Utils.SizeCalc(md.sizes[1]);
                lsize.Content = Utils.SizeCalc(md.sizes[2]);
                Duration.Content = TimeSpan.FromMilliseconds(md.duration).ToString(@"hh\:mm\:ss");
            }));
        }

        private void DownloadSongs(object sender, RoutedEventArgs e)
        {
            if (md == null) return;
            Button b = sender as Button;
            int br = 0;
            long bv = 0;
            switch (b.Name)
            {
                case "l":
                    bv = md.sizes[2];
                    br = md.bitrate[2];
                    break;
                case "m":
                    bv = md.sizes[1];
                    br = md.bitrate[1];
                    break;
                case "h":
                    bv = md.sizes[0];
                    br = md.bitrate[0];
                    break;
            }
            Task.Run(() =>
            {
                hj.DownloadURL(md.id, br.ToString()).Wait();
                RunDownload(ChoosenUrl, bv);
            });
        }

        public void RunDownload(string _u, long a)
        {
            Console.WriteLine(_u);
            Thread t = new Thread(new ThreadStart(() =>
            {
                der.DownloadFile(_u, String.Format(global.DOWNLOAD_SAVE_PATH + "/{0}.{1}", md.name, ext.ToLowerInvariant()), a);
            }));
            t.Start();
        }
    }
}
