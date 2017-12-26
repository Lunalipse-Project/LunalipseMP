using NewMediaPlayer.controler;
using NewMediaPlayer.Dialog;
using I18N;
using NewMediaPlayer.Lunalipx;
using NewMediaPlayer.Lyric;
using NewMediaPlayer.PluginHoster;
using NewMediaPlayer.Sound;
using NullStudio.Utils.Keyboardhook;
using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using NewMediaPlayer.Generic;
using System.Windows.Media.Imaging;
using System.Threading;
using NewMediaPlayer.Shell;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using NewMediaPlayer.ui.Performance;

namespace NewMediaPlayer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public PlaySound ps;
        bool pressed = false;
        LyricsDisplay ldip;
        FileStream fs;
        SyntaxParser sp = null;
        key_hook kh;
        I18NHelper I18NH;
        PageLang PL;
        PluginHelper PH;
        LunalipseInterface LI;
        ResourceHolder RH;
        BgManager bgM;

        LinearGradientBrush LGB;
        ImageBrush ImB;
        Brush defult;
        Storyboard m_in, m_out;

        bool changed = false, isfirst = true;

        public delegate void MusicChanged(string name);
        public delegate void ExecuteCompletely();
        public static event MusicChanged OnMusicChanged;
        public event ExecuteCompletely OnExecuteCompletely;

        public static ArrayList al = new ArrayList();
        public MainWindow()
        {
            InitializeComponent();
            PH = PluginHelper.INSTANCE;
            RH = ResourceHolder.INSTANCE;
            bgM = new BgManager(AppDomain.CurrentDomain.BaseDirectory+"/background");
            LI = LunalipseInterface.INSTANCE;
            LogFile.WriteLog("INFO", "Initialize Lunalipse sound component completely.");
            ps = new Sound.PlaySound(Dur, curr, fftContainer, new System.Drawing.Size(512, 512));
            ldip = new LyricsDisplay();
            //Events
            LogFile.WriteLog("INFO", "Registering Events.");
            ps.OnPlaySoundComplete += Ps_opsc;
            ps.OnProgressUpdated += Ps_OnProgressUpdated;
            ps.OnSoundLoadedComplete += Ps_OnSoundLoadedComplete;
            PlaySound.OnDurationChanged += (dur, formated) =>
            {
                if(ldip!=null)
                {
                    ldip.IsMeetLrcBlock(dur);
                }
            };
            LogFile.WriteLog("INFO", "Initialize Lunalipse FFT spectrum component completely.");
            InitializeFFT.OnSpectrumDrawnComplete += (isu) =>
            {
                if(!global.USE_CIRCULAR_SPECT)
                {
                    fftContainer.Source = isu;
                }else fftContainer.Source = null;
            };
            if (!"i18n".DExist(FType.DICT)) Directory.CreateDirectory("i18n");

            I18NH = I18NHelper.INSTANCE;
            PL = I18NH.GetReferrence("MainWindow");
            /*----Check the Property-----*/
            vol_adj.Value = global.MUSIC_VOLUME;
            
            /*----Check the Folder availability ----*/
            if(!global.MUSIC_PATH.AvailableEx())
            {
                if(!(global.MUSIC_PATH+"/Lyrics").DExist(FType.DICT))
                {
                    Directory.CreateDirectory(global.MUSIC_PATH + "/Lyrics");
                    LogFile.WriteLog("INFO", "No Lyrics folder present. Create automatically");
                }
            }
            else LogFile.WriteLog("WARNING", "Music path not set yet.");
            SetupPacer();
            RegistKeystrokes();
            InitialPrivateMode();
            RegistKeyPress();
            GetList();
            ApplyingUIText();
            InitialRes();
            SetUpStroyBoards();
            RegEvent();
            PH.GetPluginList();
            
        }

        public void InitialRes()
        {
            LGB = new LinearGradientBrush();
            BlurEffect be = new BlurEffect();
            LGB.StartPoint = new Point(1, 0);
            LGB.EndPoint = new Point(0, 1);
            LGB.GradientStops.Add(new GradientStop(Color.FromArgb(255, 5, 33, 80),0));
            LGB.GradientStops.Add(new GradientStop(Color.FromArgb(255, 52, 55, 136), 0.509));
            LGB.GradientStops.Add(new GradientStop(Color.FromArgb(255, 105, 80, 198), 1));
            ImB = new ImageBrush();
            ImB.ImageSource = RH.getImage("ChoNig");
            //ImB.Opacity = 0.5;
            ImB.Stretch = Stretch.Fill;
            defult = bgHoder.Background;
            Logo.Background = new ImageBrush(RH.getImage("LunaCM"));
            feedback.Fill = new ImageBrush(RH.getImage("button"));
            be.Radius = 100;
            be.KernelType = KernelType.Gaussian;
        }


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try { DragMove(); } catch { };
        }

     
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var f_music = new System.Windows.Forms.FolderBrowserDialog();
            if(f_music.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                global.MUSIC_PATH = f_music.SelectedPath;
                if(global.MUSIC_PATH.AvailableEx())
                {
                    GetList();
                }
            }
        }

        private void MusicList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MusicList.SelectedIndex == -1)
            {
                return;
            }
            global.SELECTED_MUSIC = MusicList.SelectedIndex;
            isfirst = false;
            m_out.Begin();
            try
            {
                string s;
                if (fs != null) fs.Close();
                if(!(s=(global.MUSIC_PATH + @"\" + MusicList.SelectedItem.ToString())).DExist(FType.FILE))
                {
                    new LDailog(LunalipsContentUI.TIPMESSAGE, PL.GetContent("GErr"), PL.GetContent("GE_C2").FormateEx(MusicList.SelectedItem.ToString(), global.MUSIC_PATH)).ShowDialog();
                    return;
                }
                string noExtension = global.CUR_MUSICN = Path.GetFileNameWithoutExtension(s);
                LogFile.WriteLog("INFO", "Now playing : " + noExtension);
                fs = new FileStream(global.MUSIC_PATH + @"\" + MusicList.SelectedItem.ToString(), FileMode.Open, FileAccess.Read);
                //Playing.Content = noExtension;
                OnMusicChanged(noExtension);

                LyricDecompiler.INSTANCE.LoadLyric(global.MUSIC_PATH + @"\Lyrics\" + noExtension + ".lrc");
                ldip.LrcParse(LyricDecompiler.INSTANCE.DecompilerLyrics());

                ps.PlayIt(fs, Path.GetExtension(global.MUSIC_PATH + @"\" + MusicList.SelectedItem.ToString()));
                MusicList.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace);
                LogFile.WriteLog("ERROR", ex.Message);
            }
        }

        

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (fs != null) fs.Close();
            ps.ISO_DISPOSE();
            LogFile.Release();
        }

        private void savePpt_Click(object sender, RoutedEventArgs e)
        {
            if(PropertyHelper.SaveProperty())
            {
                new LDailog(LunalipsContentUI.TIPMESSAGE, PL.GetContent("GS"), PL.GetContent("GS_c1")).ShowDialog();
            }
        }

        private void EllipseMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
            
        }

        private void vol_adj_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(ps!=null)ps.setVolume((float)e.NewValue);
            vol.Content = Math.Round(e.NewValue * 100d) + "%";
            global.MUSIC_VOLUME = (float)e.NewValue;
            PH?.FireSequence(APIBridge.VOL_C, global.MUSIC_VOLUME);
        }

        private void duration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }

        private void Ellipse_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if(isfirst)
            {
                pressed = true;
                isfirst = false;
                global.SELECTED_MUSIC = -1;
                Ps_opsc();
            }
            else Mainbtn.Fill = ps.ChangePlayStatus()?new SolidColorBrush(Color.FromArgb(193,92,184,92)):new SolidColorBrush(Color.FromArgb(193, 217, 83, 79));
        }

        private void duration_MouseEnter(object sender, MouseEventArgs e)
        {
            //isCallByUser = true;
        }

        private void duration_MouseLeave(object sender, MouseEventArgs e)
        {
            //isCallByUser = false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Equalizer eqz = new Equalizer();
            eqz.ShowInTaskbar = false;
            eqz.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            kh.Stop();
            Application.Current.Shutdown();
        }

        private void _Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ModeChang();
        }

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //editor ed = new editor();
            //ed.Show();
            new LDailog(LunalipsContentUI.GRADIANT_ADJ, "2333", true).ShowDialog();
        }

        private void netMusic_Click(object sender, RoutedEventArgs e)
        {
            Music m = new Music();
            m.ShowInTaskbar = false;
            m.Show();
            //ModeChang();
            //lyrciAsm la = new lyrciAsm(()=>ModeChang(1),(v)=>
            //{
            //    //duration
            //    //draged = true;
            //    //ps.setNewPosition((long)Math.Round(v));
            //    //draged = false;
            //});
            //new LDailog(la, la.Width, la.Height, "LYRIC_EDITING").Show();
        }

        private void programe_Click(object sender, RoutedEventArgs e)
        {
            if (sp == null)
            {
                new LDailog(LunalipsContentUI.TIPMESSAGE, PL.GetContent("GErr"), PL.GetContent("GE_C1")).ShowDialog();
                return;
            }
            LDailog ld = new LDailog(LunalipsContentUI.CTRL_JUMP_TO_LINE, "", sp, this);
            ld.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(1000);
                Dispatcher.Invoke(new Action(() =>
                {
                    InvalidateVisual();
                }));
            })).Start();
            (Resources["TitleFloat"] as Storyboard).Begin();
        }

        private void Lpsetting(object sender, MouseButtonEventArgs e)
        {
            new LDailog(LunalipsContentUI.MAINSETTING, "Lunalipse全局设置",true).ShowDialog();
        }

        
    }
}
