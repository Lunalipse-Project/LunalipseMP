using System;
using System.Windows;
using System.Collections.ObjectModel;
using NewMediaPlayer.controler;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using NewMediaPlayer.ui;
using NewMediaPlayer.Dialog;
using NewMediaPlayer.Generic;
using System.Windows.Media;
using LunaNetCore.Bodies;
using NetEaseHijacker;
using NetEaseHijacker.Types;

namespace NewMediaPlayer
{
    /// <summary>
    /// Music.xaml 的交互逻辑
    /// </summary>
    public partial class Music : Window
    {
        SDetail md;
        ResourceHolder RH;
        SearchType MODE = SearchType.SONGS;
        string MusicN_For_LRC = "";
        Hijack hj;

        string[] MODE_SET = new string[]
        {
            "单    曲",
            "歌    词",
        };
        
        ObservableCollection<MusicInfo> list = new ObservableCollection<MusicInfo>();
        public Music()
        {
            InitializeComponent();
            hj = new Hijack();
            RH = ResourceHolder.INSTANCE;
            logo.Background = new ImageBrush(RH.getImage("LunaCM"));
            bgi.Background = new ImageBrush(RH.getImage("Luna_in_space"));

            hj.E_Responded((par1, par2) =>
            {
                Dispatcher.Invoke(new Action(() => loading.Visibility = Visibility.Hidden));
                switch ((SearchType)Enum.Parse(typeof(SearchType), par1))
                {
                    case SearchType.SONGS:
                        ProccessResultList(par2);
                        break;
                    case SearchType.LYRIC:
                        ShowLyric(par2);
                        break;
                    case SearchType.DETAIL:
                        md = hj.ParseSongDetail(par2.ResultData);
                        InvokeChangeContent(LunalipsContentUI.MUSIC_DETAIL, md);
                        break;
                }
            });

            hj.E_TimeOut(() =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    loading.Visibility = Visibility.Hidden;
                    new LDailog(LunalipsContentUI.TIPMESSAGE, "请求超时", "请求超时，请检查您的网络连接。").ShowDialog();
                }));
            });

            hj.E_Requesting((x) =>
            {
                if (((SearchType)Enum.Parse(typeof(SearchType), x)).Equals(SearchType.SONGS))
                {
                    Dispatcher.Invoke(new Action(() => loading.Visibility = Visibility.Visible));
                }
            });
        }

        private void ShowLyric(RResult rr)
        {
            //InvokeChangeContent(LunalipsContentUI.LYRIC_DISPLY, ResultFormatter.getLyrics(rr.ResultData), MusicN_For_LRC);
        }

        protected void InvokeChangeContent(LunalipsContentUI id, params object[] pArg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (CC.Content != null)
                {
                    CC.BeginAnimation(OpacityProperty, new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1)));
                }
                switch (id)
                {
                    case LunalipsContentUI.MUSIC_DETAIL:
                        CC.Content = new musicDetail(pArg[0] as SDetail);
                        break;
                    case LunalipsContentUI.LYRIC_DISPLY:
                        CC.Content = new LyricPreview(pArg[0] as string, pArg[1] as string);
                        break;
                }
                CC.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1)));
            }));
        }

        private void ProccessResultList(RResult rr)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                list.Clear();
                List<SResult> ls = hj.ParseSongList(rr.ResultData);
                foreach (var b in ls)
                {
                    list.Add(new MusicInfo() { MusicN = b.name, artist = b.artist, ID = b.id });
                }
                music.ItemsSource = list;
            }));
        }

        

        private void EllipseMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch { }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string s = musicName.Text;
            if(MODE== SearchType.SONGS)
            {
                hj.SearchSong(s);
            }
            else
            {
                //Lyric
            }
        }

        private void music_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(music.SelectedIndex!=-1)
            {
                MusicInfo mi = music.SelectedItem as MusicInfo;
                if (MODE!=SearchType.LYRIC)
                {
                    hj.SongDetail(mi.ID);
                    music.SelectedIndex = -1;
                    
                }
                else
                {
                    //Lyric implement
                    MusicN_For_LRC = mi.MusicN.Replace(":", " ");
                }
            }
        }

        private void ModeChange(object sender, RoutedEventArgs e)
        {
            MODE++;
            if (MODE == SearchType.SONGS) MODE = SearchType.LYRIC;
            else MODE = SearchType.SONGS;
            (sender as Button).Content = MODE_SET[MODE == SearchType.SONGS ? 0 : 1];
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }

        private void setting_md(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new LDailog(LunalipsContentUI.OL_DL_SETTING, "配置联网乐库", true).ShowDialog();
        }

        
    }
}
