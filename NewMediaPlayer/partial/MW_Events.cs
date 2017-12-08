
using LunalipseAPI;
using LunalipseAPI.Configuration;
using LunalipseAPI.Generic;
using LunalipseAPI.LunalipxPlugin;
using LunalipseAPI.PlayMode;
using SHELL = NewMediaPlayer.Shell;
using NewMediaPlayer.Generic;
using NewMediaPlayer.Lunalipx;
using NewMediaPlayer.Sound;
using System;
using System.Windows.Input;
using LunalipseAPI.Graphics;
using System.Windows;
using NewMediaPlayer.Dialog;
using LunalipseAPI.KShortcut;
using NewMediaPlayer.Lyric;
using System.Windows.Media;

namespace NewMediaPlayer
{
    partial class MainWindow
    {
        /// <summary>
        /// Lunalipse 事件总线接入
        /// </summary>
        public void RegEvent()
        {
            MainUIEvent.PlayMusic += (a) =>
            {
                if (a != -1) global.SELECTED_MUSIC = (a - 1);
                Ps_opsc();
                //ModeChang(false);
            };
            MainUIEvent.SetPlayMode += (a) =>
            {
                global.PLAY_MODE = a;
            };
            MainUIEvent.SetVolume += (a) =>
            {
                vol_adj.Value = a;
            };
            MainUIEvent.AdjustVolBit += (s) =>
            {
                vol_adj.Value += s;
            };
            MainUIEvent.SetMuL += (a) =>
            {
                if (a == null) return;
                MusicList.Items.Clear();
                foreach (string s in a)
                {
                    MusicList.Items.Add(s);
                }
            };

            lyrciAsm.StateChange += (x) =>
            {
                Mainbtn.Fill = ps.ChangePlayStatus(x) ? new SolidColorBrush(Color.FromArgb(193, 92, 184, 92)) : new SolidColorBrush(Color.FromArgb(193, 217, 83, 79));
            };

            PH.PluginLoaded += (x) =>
            {
                PH.ENTITIES[x].ENTRY.Initialize();
                LogFile.WriteLog("INFO", "Plugin：" + x + " load successfully.");
                PH.ENTITIES[x].ENTRY.GrabMusicList(al);
                PH.ENTITIES[x].LDrawing?.InitialDraw();
                PH.ENTITIES[x].LPX?.LPxInitialize();
                PH.ENTITIES[x].LMode?.BeingInitialize();

            };
            PH.PluginUnload += (x) =>
            {
                LogFile.WriteLog("INFO", "Plugin：" + x + " unload successfully.");
                PH.ENTITIES[x]?.ENTRY.Destroy();
                PH.ENTITIES[x]?.LDrawing.UIUndraw();
                PH.ENTITIES[x].LMode?.Deinitialize();
            };

            DrawingManager.OnDrawButton += (lt, lb, n) =>
            {
                if (PH.hasUIDrawPrivilege(n))
                {
                    PH.UIDW.DrawButton(this, lb, lt, n);
                    return true;
                }
                else return false;
            };
            DrawingManager.OnDrawLabel += (lt, ll, n) =>
            {
                if (PH.hasUIDrawPrivilege(n))
                {
                    PH.UIDW.DrawLabel(this, ll, lt, n);
                    return true;
                }
                else return false;

            };
            DrawingManager.OnDrawControl += (lt, c, n) =>
            {
                if (PH.hasUIDrawPrivilege(n))
                {
                    PH.UIDW.DrawVanilla(this, lt, c, n);
                    return true;
                }
                else return false;
            };
            DrawingManager.OnUIRelased += (n) =>
            {
                A01: foreach (FrameworkElement fe in outershell.Children)
                {
                    if (fe.Tag != null && fe.Tag.Equals(n))
                    {
                        outershell.Children.Remove(fe);
                        goto A01;
                    }
                }
            };

            LunalipxEx.RCommand += (c, d) =>
            {
                if (!PH.hasLunalipxExPrivilege(d)) return false;
                try
                {
                    if (Command.cmd.IndexOf(c) == -1) Command.cmd.Add(c);
                    else return false;
                    return true;
                }
                catch { return false; }
            };
            LunalipxEx.URCommand += (c) =>
            {
                try
                {
                    Command.cmd.Remove(c);
                    return true;
                }
                catch { return false; }
            };
            LunalipxEx.EScript += (a, d) =>
            {
                if (!PH.hasLunalipxExPrivilege(d)) return;
                Command.SCRIPT_range = a > 6 ? 6 : a;
            };

            ModeManager.RegMODE += (a, b, d) =>
            {
                if (PH.hasAddCMPrivilege(d))
                {
                    CustomModeCollection.collection.Add(new CustomMode { Identification = a, Key = b });
                    Command.MAX_MODE++;
                    return true;
                }
                else return false;
            };
            ModeManager.URegMODE += (a, d) =>
            {
                if (!PH.hasAddCMPrivilege(d)) return;
                foreach (CustomMode cm in CustomModeCollection.collection)
                {
                    if (cm.Identification == a)
                    {
                        CustomModeCollection.collection.Remove(cm);
                        break;
                    }
                }
                Command.MAX_MODE--;
            };

            LunalipseContainer.OpenD += (a, b, c) =>
            {
                try
                {
                    new LDailog(a, b, c, "HEADER").ShowDialog();
                    return true;
                }
                catch
                {
                    return false;
                }
            };

            LunalipseContainer.ODeft += (a, b) =>
            {
                try
                {
                    new LDailog(a, b).ShowDialog();
                    return true;
                }
                catch
                {
                    return false;
                }
            };

            floating.GetCurMusic += () => Playing.Content.ToString();

            KShortcutManager.AddShortCut += (i,k) => global.KsHolder.Add4nRep(i,k);

            GlobalCfgManager.CfgROperation += (k, d) => PH.hasCfgRWPrivilege(d) ? global.__data[k] : null;

            GlobalCfgManager.CfgWOperation += (k, v, d) =>
            {
                if (!PH.hasCfgRWPrivilege(d)) return false;
                if (!global.__data.ContainsKey(k))
                    global.__data.Add(k, v);
                else global.__data[k] = v;
                return true;
            };
            GlobalCfgManager.hKey += (a) =>
            {
                return global.__data.ContainsKey(a);
            };
            duration.AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler((a, b) =>
            {
                draged = true;
            }), true);
            duration.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler((a, b) => {
                if (ps.ISPLAYING)
                {
                    ps.setNewPosition((long)Math.Round(duration.Value));
                    draged = false;
                }
            }), true);

            LI.OnMusicChanged += (a, b, c) =>
            {
                if (a >= 0)
                {
                    MusicList.SelectedIndex = a;
                }
                else if (b.AvailableEx())
                {
                    int i = 0;
                    if ((i = MusicList.Items.IndexOf(b)) != -1)
                    {
                        global.SELECTED_MUSIC = i;
                        MusicList.SelectedIndex = global.SELECTED_MUSIC;
                    }
                    else
                    {
                        SHELL.LPXShell.WriteLine(" [Fatal] Song '{0}' not found.", b);
                    }
                }
                else if (c)
                {
                    Random r = new Random();
                    MusicList.SelectedIndex = global.SELECTED_MUSIC = r.Next(MusicList.Items.Count);
                }
            };

            SHELL.AutoShd.OnShutdownRequested += () =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (ps.ISPLAYING) ps.StopPlay();
                    Close();
                });
            };

            LineSpectrum.gfftd += x =>
            {
                PH.ENTITIES.ForEach((k, v) =>
                {
                    v?.LFFT?.onFFTDataUpdate(x);
                });
            };
        }
    }
}
