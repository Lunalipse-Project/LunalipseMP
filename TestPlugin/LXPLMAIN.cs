using LunalipseAPI;
using LunalipseAPI.Generic;
using System.Collections;
using System.Windows.Forms;
using System;
using LunalipseAPI.KShortcut;
using LunalipseAPI.Configuration;

namespace TestPlugin
{
    //Entry Point
    [LunalipsePluginInfo(Name ="Lunalipse Extension",
        Description ="Extended the basic function of Lunalipse", 
        Team ="Canterlot Computing Research Center",
        Version ="1.0.2.0",
        Author = "Lunaixsky",
        autoLoad = true)]
    [GlobalConfigPrivilege(PluginID = "LXP_Extend")]
    [LunalipseNetworking(PluginID = "LXP_Extend")]
    public class LXPLMAIN : MainUI
    {
        GlobalCfgManager gcm = new GlobalCfgManager("LXP_Extend");
        MainUIEvent muie = new MainUIEvent();
        Form1 f = new Form1();
        public void Destroy()
        {
            
        }

        public void GrabMusicList(ArrayList ml)
        {
            if (!gcm.HasConfigField("musicL"))
            {
                gcm.WriteConfig("musicL", ml);
            }
            else
            {
                muie.Invoke(Setter.SET_MUSICL, gcm.ReadConfig("musicL"));
            }
        }

        public void Initialize()
        {
            RegKS();
        }

        public void LunalipseExit()
        {
            
        }

        public void ModeChange(int mode)
        {
           // MessageBox.Show("Mode Changed : " + mode);
        }

        public void MusicChange(PlayInfo pi)
        {
            
        }

        public void VolumeChange(double vol)
        {
            
        }

        public void RegKS()
        {
            KShortcutManager.AddKeyShortcut(new Keystroke()
            {
                MainKey = (int)Keys.Control,
                Subkey = (int)Keys.W,
                keyStroked = new Behavior(() =>
                {
                    muie.Invoke(Setter.INCS_VOL);
                }),
            });
            KShortcutManager.AddKeyShortcut(new Keystroke()
            {
                MainKey = (int)Keys.Control,
                Subkey = (int)Keys.Q,
                keyStroked = new Behavior(() =>
                {
                    muie.Invoke(Setter.DECS_VOL);
                })
            });
            //KShortcutManager.AddKeyShortcut(new Keystroke()
            //{
            //    MainKey = (int)System.Windows.Forms.Keys.Control,
            //    Subkey = (int)System.Windows.Forms.Keys.S,
            //    keyStroked = new Behavior(() =>
            //    {
            //        f.Show();
            //    }),
            //    keyReleased = new Behavior(() =>
            //    {
            //        f.Hide();
            //    }),
            //    KeepListenUntilRelease = true
            //});
        }
    }
}
