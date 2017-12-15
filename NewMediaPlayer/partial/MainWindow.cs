using LunalipseAPI.Generic;
using LunalipseAPI.KShortcut;
using NewMediaPlayer.Generic;
using NewMediaPlayer.Lunalipx;
using NewMediaPlayer.PluginHoster;
using NewMediaPlayer.Shell;
using NewMediaPlayer.ui.Heartbeater;
using NullStudio.Utils.Keyboardhook;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace NewMediaPlayer
{
    //主窗体程序的局部类
    partial class MainWindow
    {
        public bool draged = false;
        Pacer p = Pacer.INSTANCE;

        public void ApplyingUIText()
        {
            savePpt.Content = PL.GetContent("saveCfg");
            setmpath.Content = PL.GetContent("MusicDir");
            programe.Content = PL.GetContent("lpxCfg");
            equzer.Content = PL.GetContent("btnList4");
            loadscript.Content = PL.GetContent("btnList3");
            //Edlpx.Content = PL.GetContent("btnList2");
            netMusic.Content = PL.GetContent("btnList1");
            playmode.Content = PL.GetContent("orderPlay");
        }

        int stored = -1;
        void ModeChang(params int[] mode)
        {

            if (global.PLAY_MODE < Command.MAX_MODE && mode != null)
            {
                global.PLAY_MODE++;
            }
            else if ( mode.Length != 0 && mode[0] < Command.MAX_MODE)
            {
                global.PLAY_MODE = mode[0];
            }
            else global.PLAY_MODE = 0;
            switch (global.PLAY_MODE)
            {
                case 0:
                    playmode.Content = PL.GetContent("orderPlay");
                    break;
                case 1:
                    playmode.Content = PL.GetContent("singleLoop");
                    break;
                case 2:
                    playmode.Content = PL.GetContent("randomPlay");
                    break;
                default:
                    if(global.PLAY_MODE<= Command.SCRIPT_range)
                        playmode.Content = PL.GetContent("program").FormateEx(global.PLAY_MODE - 2);
                    else
                    {
                        CustomMode cm = CustomModeCollection.collection[global.PLAY_MODE - (Command.SCRIPT_range+1)] as CustomMode;
                        foreach (string s in PH.LUNA_MODEEXTEND)
                        {
                            if(PH.ENTITIES[s].modeI18NReq)
                            {
                                playmode.Content = PL.GetContent(cm.Key) ?? "NAN";
                            }
                            else
                            {
                                playmode.Content = cm.Key ?? "NAN";
                            }
                        }
                    }
                    break;
            }
            PH.FireSequence(APIBridge.MODE_C, global.PLAY_MODE);
        }

        public void Ps_opsc()
        {
            AutoShd.ManualCheck();
            if (fs != null)
            {
                fs.Close();
                fs.Dispose();
            }
            MusicList.Dispatcher.Invoke(new Action(() =>
            {
                switch (global.PLAY_MODE)
                {
                    case 0:
                        if (global.SELECTED_MUSIC < MusicList.Items.Count)
                        {
                            global.SELECTED_MUSIC++;
                        }
                        else
                        {
                            global.SELECTED_MUSIC = 0;
                        }
                        loadscript.IsEnabled = false;
                        break;
                    case 2:
                        Random r = new Random();
                        global.SELECTED_MUSIC = r.Next(0, MusicList.Items.Count - 1);
                        loadscript.IsEnabled = false;
                        break;
                    case 1:
                        loadscript.IsEnabled = false;
                        // play cycle. Do nothing.
                        break;
                    default:
                        if(global.PLAY_MODE<=Command.SCRIPT_range)
                        {
                            loadscript.IsEnabled = true;
                            int pnum = global.PLAY_MODE - 2;
                            bool isSuccess = false;
                            if (sp == null)
                            {
                                sp = SyntaxParser.INSTANCE;
                                sp.PROGRAME_COUNTER = 1;
                                isSuccess = sp.Parse(al,pnum);
                            }
                            else if (sp.GetProgrameSeq != pnum)
                            {
                                sp = SyntaxParser.INSTANCE;
                                sp.PROGRAME_COUNTER = 1;
                                isSuccess = sp.Parse(al,pnum);
                            }
                            else isSuccess = true;
                            if (isSuccess && pressed)
                            {
                                sp.PROGRAME_COUNTER = 1;
                                sp.Excute(MusicList);
                                pressed = false;
                            }
                            else if (isSuccess && !pressed)
                            {
                                sp.Excute(MusicList);
                            }
                            else { global.PLAY_MODE = 0; global.SELECTED_MUSIC = 0; }
                            OnExecuteCompletely?.Invoke();
                        }
                        else
                        {
                            CustomMode cm = CustomModeCollection.collection[global.PLAY_MODE - (Command.SCRIPT_range + 1)] as CustomMode;
                            foreach (string s in PH.LUNA_MODEEXTEND)
                            {
                                PH.ENTITIES[s].LMode?.ModeBehavior(ref global.SELECTED_MUSIC, 
                                    cm.Identification
                                    );
                            }
                        }
                        break;

                }
                MusicList.SelectedIndex = global.SELECTED_MUSIC;
            }));
        }
        public void GetList()
        {
            MusicList.Items.Clear();
            if (!global.MUSIC_PATH.AvailableEx()) return;
            if (!global.MUSIC_PATH.DExist(FType.DICT)) return;

            foreach (string fi in Directory.GetFiles(global.MUSIC_PATH))
            {
                string ext = Path.GetExtension(fi);
                if (!ext.Equals(".mp3") && !ext.Equals(".flac") && !ext.Equals(".acc")) continue;
                string _ = Path.GetFileName(fi);
                MusicList.Items.Add(_);
                al.Add(_);
            }

        }

        private void Ps_OnSoundLoadedComplete(double newDuration)
        {
            duration.Dispatcher.Invoke(new Action(() =>
            {
                //Reset the value to 0
                duration.Value = 0;
                //Applying new duration
                duration.Maximum = newDuration;
                Mainbtn.Fill = new SolidColorBrush(Color.FromArgb(193, 217, 83, 79));
                PlayInfo pi = new PlayInfo();
                pi.Name = MusicList.Items[global.SELECTED_MUSIC].ToString();
                pi.musicPath = global.MUSIC_PATH + @"\" + pi.Name;
                pi.TotalDuration = (long)newDuration;
                PH.FireSequence(APIBridge.MUSIC_C, pi);
            }));
        }

        private void Ps_OnProgressUpdated(long newPosition)
        {
            duration.Dispatcher.Invoke(new Action(() =>
            {
                //Update the thumb
                if(!draged) duration.Value = newPosition;
            }));
        }

        bool isPrivate = false;
        public void RegistKeyPress()
        {
            kh = new key_hook();
            kh.KeyDownEvent += (s, e) =>
            {
                foreach(var ks in global.KsHolder)
                {
                    if (e.KeyValue == ks.Value.Subkey && (int)System.Windows.Forms.Control.ModifierKeys == ks.Value.MainKey)
                    {
                        ks.Value.keyStroked.Invoke();
                        break;
                    }
                }
                e.Handled = true;
            };
            kh.KeyUpEvent += (s, e) =>
            {
                foreach (var ks in global.KsHolder)
                {
                    if (e.KeyValue == ks.Value.Subkey && (int)System.Windows.Forms.Control.ModifierKeys == ks.Value.MainKey && ks.Value.KeepListenUntilRelease)
                    {
                        ks.Value.keyReleased.Invoke();
                        break;
                    }
                }
                e.Handled = true;
            };
            kh.Start();
        }

        //进入私密模式
        BlurEffect bf = new BlurEffect();
        private void InitialPrivateMode()
        {
            bf.KernelType = KernelType.Gaussian;
            bf.Radius = 0;
            outershell.Effect = bf;
        }


        public void RegistKeystrokes()
        {
            global.KsHolder.Add(0x0001,new Keystroke()
            {
                Subkey = (int)System.Windows.Forms.Keys.P,
                MainKey = (int)System.Windows.Forms.Keys.Alt,
                keyStroked = new Behavior(() =>
                {
                    if (!isPrivate)
                    {
                        isPrivate = true;
                        bf.BeginAnimation(BlurEffect.RadiusProperty, new DoubleAnimation(0, 15, TimeSpan.FromSeconds(2.5)));
                    }
                    else
                    {
                        isPrivate = false;
                        bf.BeginAnimation(BlurEffect.RadiusProperty, new DoubleAnimation(15, 0, TimeSpan.FromSeconds(2.5)));
                    }
                })
            });

            global.KsHolder.Add(0x0002, new Keystroke()
            {
                Subkey = (int)System.Windows.Forms.Keys.S,
                MainKey = (int)System.Windows.Forms.Keys.Alt,
                keyStroked = new Behavior(() =>
                {
                    Mainbtn.Fill = ps.ChangePlayStatus() ? new SolidColorBrush(Color.FromArgb(193, 92, 184, 92)) : new SolidColorBrush(Color.FromArgb(193, 217, 83, 79));
                })
            });

            global.KsHolder.Add4nRep(0x0003, new Keystroke()
            {
                Subkey = (int)System.Windows.Forms.Keys.X,
                MainKey = (int)System.Windows.Forms.Keys.Alt,
                KeepListenUntilRelease = false,
                keyStroked=new Behavior(() =>
                {
                    if (!global.ALBUM_BG)
                    {
                        if (!changed)
                        {
                            defult = bgHoder.Background = ImB;
                            changed = true;
                        }
                        else
                        {
                            defult = bgHoder.Background = LGB;
                            changed = false;
                        }
                    }
                    
                })
            });

            global.KsHolder.Add4nRep(0x0004, new Keystroke()
            {
                Subkey = (int)System.Windows.Forms.Keys.N,
                MainKey = (int)System.Windows.Forms.Keys.Alt,
                KeepListenUntilRelease = false,
                keyStroked = new Behavior(() =>
                {
                    if (bgHoder.Opacity-0.1 > 0 && bgHoder.Opacity <= 1)
                        bgHoder.Opacity += -0.1; 
                })
            });

            global.KsHolder.Add4nRep(0x0005,new Keystroke()
            {
                Subkey = (int)System.Windows.Forms.Keys.M,
                MainKey = (int)System.Windows.Forms.Keys.Alt,
                KeepListenUntilRelease = false,
                keyStroked = new Behavior(() =>
                {
                    if (bgHoder.Opacity >= 0 && bgHoder.Opacity+0.1 < 1)
                        bgHoder.Opacity += 0.1;
                })
            });
        }

        private void SetUpStroyBoards()
        {
            m_in = Resources["music_in"] as Storyboard;
            m_out = Resources["music_out"] as Storyboard;
            m_out.Completed += (a, b) =>
            {
                Playing.Content = ClearName(MusicList.Items[global.SELECTED_MUSIC].ToString());
                m_in.Begin();
            };
        }

        private void SetupPacer()
        {
            p.Listen(this);
        }

        /// <summary>
        /// make the music name more clear to see
        /// </summary>
        /// <param name="orgname"></param>
        /// <returns></returns>
        string ClearName(string orgname)
        {
            string[] spet = Path.GetFileNameWithoutExtension(orgname).Split('-');
            return spet[spet.Length - 1].Trim();
        }
    }
}
