using NewMediaPlayer.controler;
using NewMediaPlayer.Dialog;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace NewMediaPlayer.ui
{
    /// <summary>
    /// LyricPreview.xaml 的交互逻辑
    /// </summary>
    public partial class LyricPreview : Page
    {
        string lrc,n;
        Regex rx;
        string[] lrcs;
        public LyricPreview(string lrc,string MusicN)
        {
            n = MusicN;
            rx = new Regex(@"\[([0-9.:]*)\]+(.*)", RegexOptions.Compiled);
            InitializeComponent();
            mn.Content = n;
            lyrics.Text = "";
            this.lrc = lrc;
            if(lrc.Equals("NO_LYRICS"))
            {
                lyrics.Text = "未找到对应歌词。";
                save.IsEnabled = false;
                return;
            }
            lrcs = this.lrc.Split('\n');
            int offset = 0;
            for(int i=0;i<6;i++)
            {
                if (i > lrcs.Length - 1) break;
                while (isNonLRC(lrcs[i + offset])) offset++;
                if (i+offset > lrcs.Length - 1) break;
                lyrics.Text += removeTimeSpan(lrcs[i + offset]) + "\n";
                
            }
            save.IsEnabled = true;
        }

        public bool isNonLRC(string line)
        {
            if (line.StartsWith("[ti:"))
            {
                return true;
            }
            else if (line.StartsWith("[ar:"))
            {
                return true;
            }
            else if (line.StartsWith("[al:"))
            {
                return true;
            }
            else if (line.StartsWith("[by:"))
            {
                return true;
            }
            else if (line.StartsWith("[offset:"))
            {
                return true;
            }
            else if (String.IsNullOrEmpty(line)) return true;
            return false;
        }

        public string removeTimeSpan(string _Lrc)
        {
            if (!rx.IsMatch(_Lrc)) return _Lrc;
            MatchCollection mc = rx.Matches(_Lrc);
            return mc[0].Groups[2].Value;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string lrcF = global.DOWNLOAD_SAVE_PATH + @"\Lyrics";
            if (!Directory.Exists(lrcF)) Directory.CreateDirectory(lrcF);
            using (StreamWriter sw = new StreamWriter(lrcF + @"\" + n + ".lrc", false, Encoding.UTF8))
            {
                foreach(string s in lrcs)
                {
                    if (!String.IsNullOrEmpty(s))
                    {
                        if(!removeTimeSpan(s).Equals(""))
                        {
                            sw.WriteLine(s);
                        }
                    }
                }
            }
            LDailog ld = new LDailog(LunalipsContentUI.TIPMESSAGE, "储存成功！", "歌词写入完成。\n保存至：" + lrcF + "/" + n + ".lrc");
            ld.ShowDialog();
        }
    }
}
