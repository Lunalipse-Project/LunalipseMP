using LpxResource.Generic;
using NewMediaPlayer.Dialog;
using NewMediaPlayer.Sound;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace NewMediaPlayer.Lyric
{
    //States: True->Play , False->Pause
    public delegate void SetPlayBackState(bool state);
    /// <summary>
    /// lyrciAsm.xaml 的交互逻辑
    /// </summary>
    public partial class lyrciAsm : Page
    {
        public static event SetPlayBackState StateChange;
        bool isEditing = false;
        double CurTimeSec = 0d;
        double executedLrc = 0d;
        bool jump = false;
        int idx = -1;
        Action<double> TIMEJUMP;
        public lyrciAsm(Action act, Action<double>  TJ)
        {
            InitializeComponent();
            TIMEJUMP = TJ;
            global.LrcEditing = true;
            PlaySound.OnDurationChanged += (x, y) =>
            {
                CurTimeSec = x;
                if (x < executedLrc) idx = -1;
                for (; idx < lrclist.Items.Count-1; idx++)
                {
                    if (idx < lrclist.Items.Count)
                        if (x < Math.Round((lrclist.Items[idx + 1] as LyricUnit).OffsetTSd)) break;
                    LyricUnit lu = lrclist.Items[idx+1] as LyricUnit;
                    executedLrc = Math.Round(lu.OffsetTSd);
                    if (executedLrc == Math.Round(x))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            lrclist.Focus();
                            lrclist.SelectedIndex = idx+1;
                        });
                        break;
                    }
                }
            };
            LyricDecompiler.INSTANCE.OnLyricPerpeared += () =>
            {
                ReflashLRC();
            };
            LDailog.owc += () =>
            {
                global.LrcEditing = false;
                act();
            };
            ReflashLRC();
        }

        void ReflashLRC()
        {
            foreach (var i in LyricDecompiler.INSTANCE.Lyrics)
            {
                LyricUnit lu = new LyricUnit(i.Key);
                if (i.Value.Contains("|"))
                {
                    string[] str = i.Value.Split('|');
                    lu.Lyric = str[0];
                    lu.SubLyric = str[1];
                }
                else lu.Lyric = i.Value;
                lrclist.Items.Add(lu);
            }
        }
        private void node(object sender, RoutedEventArgs e)
        {
            if(isEditing)
            {
                nodes.Content = "追加标记点";
                LyricUnit lu = new LyricUnit(CurTimeSec, Lrc.Text, subLrc.Text);
                lrclist.Items.Add(lu);
                Lrc.Text = subLrc.Text = "";
                StateChange(true);
                isEditing = false;
            }
            else
            {
                nodes.Content = "完     成";
                TimeSpan ts = TimeSpan.FromSeconds(CurTimeSec);
                offset.Content = ts.ToString(@"mm\:ss\.fff");
                StateChange(false);
                isEditing = true;
            }
        }

        private void lrclist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LyricUnit lu = (LyricUnit)lrclist.SelectedItem;
            if (lu != null)
            {
                offset.Content = lu.Offset;
                Lrc.Text = lu.LyricEditing;
                subLrc.Text = lu.SubLyric;
            }
            //if(jump)
            //{
            //    TIMEJUMP(lu.OffsetTSd);
            //}
        }

        private void save(object sender, RoutedEventArgs e)
        {
            string s = "";
            for(int i=0;i<lrclist.Items.Count;i++)
            {
                LyricUnit lu = lrclist.Items[i] as LyricUnit;
                s += "[{0}]{1}|{2}\n".FormateEx(lu.Offset, lu.LyricEditing, lu.SubLyric);
            }
            using (FileStream fs = new FileStream("{0}//Lyrics//{1}.lrc".FormateEx(global.MUSIC_PATH, global.CUR_MUSICN), FileMode.Create))
            {
                byte[] b = Encoding.UTF8.GetBytes(s);
                fs.Write(b, 0, b.Length);
            }
        }



        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.LeftCtrl)
            {
                jump = true;
            }
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
            {
                jump = false;
            }
        }
    }
}
