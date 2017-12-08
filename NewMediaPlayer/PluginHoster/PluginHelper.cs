using I18N;
using LunalipseAPI;
using LunalipseAPI.Configuration;
using LunalipseAPI.Generic;
using LunalipseAPI.Graphics;
using LunalipseAPI.LunalipxPlugin;
using LunalipseAPI.PlayMode;
using NewMediaPlayer.controler;
using NewMediaPlayer.Dialog;
using NewMediaPlayer.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NewMediaPlayer.PluginHoster
{
    public delegate void Loaded(string p1);
    public delegate void UnLoaded(string p2);
    public partial class PluginHelper
    {
        public static volatile PluginHelper _PH_INSTANCE;
        public static readonly object _OBJ = new object();

        public event Loaded PluginLoaded;
        public event UnLoaded PluginUnload;

        PageLang pl;
        IDictionary<string, PluginEntity> _methods;
        IDictionary<string, LunalipsePluginInfo> INFO;
        IDictionary<string, Assembly> asms;
        ArrayList LunapxExtend = new ArrayList();
        ArrayList LpxCustomMode = new ArrayList();

        I18NBridge i18nb;

        public UIDrawable UIDW;

        private IDictionary<string, bool> IsActivated;
        public static PluginHelper INSTANCE
        {
            get
            {
                if (_PH_INSTANCE == null)
                {
                    lock(_OBJ)
                    {
                        if(_PH_INSTANCE == null)
                        {
                            return _PH_INSTANCE = new PluginHelper();
                        }
                    }
                }
                return _PH_INSTANCE;
            }
        }

        public IDictionary<string, LunalipsePluginInfo> Plugins
        {
            get
            {
                return INFO;
            }
        }

        private PluginHelper()
        {
            if (!Directory.Exists("plugins")) Directory.CreateDirectory("plugins");
            pl = I18NHelper.INSTANCE.GetReferrence("plugin");
            INFO = new Dictionary<string, LunalipsePluginInfo>();
            _methods = new Dictionary<string, PluginEntity>();
            IsActivated = new Dictionary<string, bool>();
            asms = new Dictionary<string, Assembly>();
            UIDW = new UIDrawable();
            i18nb = new I18NBridge(this);
        }

        public bool hasCfgRWPrivilege(string id)
        {
            return ConfigIOPrivilege.Contains(id);
        }
        public bool hasUIDrawPrivilege(string id)
        {
            return DrawingUIPrivilege.Contains(id);
        }
        public bool hasAddCMPrivilege(string id)
        {
            return CustomModePrivilege.Contains(id);
        }
        public bool hasLunalipxExPrivilege(string id)
        {
            return LunalipxExtendPrivilege.Contains(id);
        }
        public bool hasNetworkingPrivilege(string id)
        {
            return NetworkingPrivilege.Contains(id);
        }

        public ArrayList LUNAPXEXTEND
        {
            get
            {
                return LunapxExtend;
            }
        }

        public ArrayList LUNA_MODEEXTEND
        {
            get
            {
                return LpxCustomMode;
            }
        }

        public void GetPluginList()
        {
            try
            {
                foreach (string s in Directory.GetFiles("plugins\\"))
                {
                    if (!Path.GetExtension(s).Equals(".lxpg")) continue;
                    string ___p = Path.GetFileNameWithoutExtension(s);
                    asms.Add(___p,Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory+s));
                    foreach (Type t in asms[___p].GetExportedTypes())
                    {
                        Attribute a;
                        if ((a = t.GetCustomAttribute(typeof(LunalipsePluginInfo))) != null)
                        {
                            LunalipsePluginInfo lpi = a as LunalipsePluginInfo;
                            INFO.Add(___p, lpi);
                            IsActivated.Add(___p, false);
                            _methods.Add(___p, null);
                            if (lpi.autoLoad)
                            {
                                _pal.Add("autoLoad");
                                if (global.PLUGIN_SECURITY)
                                {
                                    LogFile.WriteLog("INFO", "Ask for privilege : {0}".FormateEx(pl.GetContent("stiv_p1")));
                                    LDailog ld = new LDailog(LunalipsContentUI.DIA_WITH_YESNO, pl.GetContent("plg_stiv_h"), pl.GetContent("plg_stiv_c").FormateEx(lpi.Name, ___p, pl.GetContent("stiv_p1")));
                                    if (ld.ShowDialog() == true)
                                    {
                                        LoadPlugin(___p);
                                        LogFile.WriteLog("INFO", "Permitted");
                                    }
                                    else LogFile.WriteLog("INFO", "Denied");
                                }
                                else
                                {
                                    LogFile.WriteLog("INFO", "Plugin security has been disabled. Auto permit.");
                                    LoadPlugin(___p);
                                }
                            }
                        }
                        else
                        {
                            GetPluginPrivilege(t);
                        }
                    }
                    ApplyPrevilege(___p);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Source);
            }
        }

        public IDictionary<string, bool> ACTIVATED
        {
            get
            {
                return IsActivated;
            }
        }

        public IDictionary<string, PluginEntity> ENTITIES
        {
            get
            {
                return _methods;
            }
        }

        bool Found;
        public void LoadPlugin(string plugin)
        {
            try
            {
                foreach (Type t in asms[plugin].GetExportedTypes())
                {
                    Attribute abt;
                    if (t.GetInterface("MainUI") != null && t.Name.Equals("LXPLMAIN"))
                    {
                        Found = true;
                        IsActivated[plugin] = true;
                        _methods[plugin] = new PluginEntity();
                        if ((abt = t.GetCustomAttribute(typeof(GlobalConfigPrivilege))) != null)
                        {
                            if (global.PLUGIN_SECURITY)
                            {
                                LogFile.WriteLog("INFO", "Ask for privilege : {0}".FormateEx(pl.GetContent("stiv_p2")));
                                LDailog ld = new LDailog(LunalipsContentUI.DIA_WITH_YESNO, pl.GetContent("plg_stiv_h"), pl.GetContent("plg_stiv_c").FormateEx(INFO[plugin].Name, plugin, pl.GetContent("stiv_p2")));
                                if (ld.ShowDialog() == true)
                                {
                                    ConfigIOPrivilege.Add((abt as GlobalConfigPrivilege).PluginID);
                                    LogFile.WriteLog("INFO", "Permitted");
                                }
                                else LogFile.WriteLog("INFO", "Denied");
                            }
                            else
                            {
                                LogFile.WriteLog("INFO", "Plugin security has been disabled. Auto permit.");
                                ConfigIOPrivilege.Add((abt as GlobalConfigPrivilege).PluginID);
                            }
                        }
                        else if ((abt = t.GetCustomAttribute(typeof(LunalipseNetworking))) != null)
                        {
                            if (global.PLUGIN_SECURITY)
                            {
                                LogFile.WriteLog("INFO", "Ask for privilege : {0}".FormateEx(pl.GetContent("stiv_p3")));
                                LDailog ld = new LDailog(LunalipsContentUI.DIA_WITH_YESNO, pl.GetContent("plg_stiv_h"), pl.GetContent("plg_stiv_c").FormateEx(INFO[plugin].Name, plugin, pl.GetContent("stiv_p3")));
                                if (ld.ShowDialog() == true)
                                {
                                    ConfigIOPrivilege.Add((abt as LunalipseNetworking).PluginID);
                                    _methods[plugin].initialLNC();
                                    LogFile.WriteLog("INFO", "Permitted");
                                }
                                else LogFile.WriteLog("INFO", "Denied");
                            }
                            else
                            {
                                LogFile.WriteLog("INFO", "Plugin security has been disabled. Auto permit.");
                                ConfigIOPrivilege.Add((abt as LunalipseNetworking).PluginID);
                                _methods[plugin].initialLNC();
                            }
                        }
                        ParseFunction(t, plugin);
                        PluginLoaded?.Invoke(plugin);

                        break;
                    }
                }
                if (!Found)
                {
                    new LDailog(controler.LunalipsContentUI.TIPMESSAGE, pl.GetContent("noEntP"), string.Format(pl.GetContent("noEntPc"), plugin + ".lxpg")).ShowDialog();
                }
            }
            catch(Exception e)
            {
                new LDailog(controler.LunalipsContentUI.TIPMESSAGE, pl.GetContent("err"), string.Format(pl.GetContent("errC"), e.Message)).ShowDialog();
            }
        }

        public void Unload(string plugin)
        {
            //asm = null;
            IsActivated[plugin] = false;
            PluginUnload(plugin);
            _methods[plugin] = null;
        }

        private void ParseFunction(Type t_,string plgName)
        {
            _methods[plgName].ENTRY = (MainUI)Activator.CreateInstance(t_);
            foreach(Type __t in asms[plgName].GetExportedTypes())
            {
                Attribute abt = null;
                if (__t.GetInterface("ILunalipseDrawing") != null && (abt= __t.GetCustomAttribute(typeof(LunalipseDrawing))) != null)
                {
                    _methods[plgName].LDrawing = (ILunalipseDrawing)Activator.CreateInstance(__t);
                    if (global.PLUGIN_SECURITY)
                    {
                        LogFile.WriteLog("INFO", "Ask for privilege : {0}".FormateEx(pl.GetContent("stiv_p3")));
                        LDailog ld = new LDailog(LunalipsContentUI.DIA_WITH_YESNO, pl.GetContent("plg_stiv_h"), pl.GetContent("plg_stiv_c").FormateEx(INFO[plgName].Name, plgName, pl.GetContent("stiv_p3")));
                        if (ld.ShowDialog() == true)
                        {
                            DrawingUIPrivilege.Add((abt as LunalipseDrawing).PluginID);
                            LogFile.WriteLog("INFO", "Permitted");
                        }
                        else
                        {
                            LogFile.WriteLog("INFO", "Denied");
                        }
                    }
                    else
                    {
                        LogFile.WriteLog("INFO", "Plugin security has been disabled. Auto permit.");
                        DrawingUIPrivilege.Add((abt as LunalipseDrawing).PluginID);
                    }
                }
                else if(__t.GetInterface("FFT") != null)
                {
                    _methods[plgName].LFFT = (FFT)Activator.CreateInstance(__t);
                }
                else if (__t.GetInterface("ILunalipx") != null && (abt = __t.GetCustomAttribute(typeof(LunalipxExtend))) != null)
                {
                    if(((LunalipxExtend)abt).SupportVersion.Equals(Assembly.GetExecutingAssembly().GetName().Version.ToString()))
                    {
                        LunapxExtend.Add(plgName);
                        _methods[plgName].LPX = (ILunalipx)Activator.CreateInstance(__t);
                        if(global.PLUGIN_SECURITY)
                        {
                            LogFile.WriteLog("INFO", "Ask for privilege : {0}".FormateEx(pl.GetContent("stiv_p5")));
                            LDailog ld = new LDailog(LunalipsContentUI.DIA_WITH_YESNO,
                                pl.GetContent("plg_stiv_h"),
                                pl.GetContent("plg_stiv_c")
                                    .FormateEx(INFO[plgName].Name,
                                    plgName,
                                    pl.GetContent("stiv_p5")));
                            if (ld.ShowDialog() == true)
                            {
                                LunalipxExtendPrivilege.Add((abt as LunalipxExtend).PluginID);
                                LogFile.WriteLog("INFO", "Permitted");
                            }
                            else LogFile.WriteLog("INFO", "Denied");
                        }
                        else
                        {
                            LogFile.WriteLog("INFO", "Plugin security has been disabled. Auto permit.");
                            LunalipxExtendPrivilege.Add((abt as LunalipxExtend).PluginID);
                        }
                    }
                    else
                    {
                        new LDailog(controler.LunalipsContentUI.TIPMESSAGE, pl.GetContent("errP"), string.Format(pl.GetContent("errPC"), plgName)).ShowDialog();
                    }
                }
                else if (__t.GetInterface("IMode") != null && (abt = __t.GetCustomAttribute(typeof(LunalipseCustomMode))) != null)
                {
                    LpxCustomMode.Add(plgName);
                    _methods[plgName].LMode = (IMode)Activator.CreateInstance(__t);
                    _methods[plgName].modeI18NReq = (abt as LunalipseCustomMode).I18Npresent;
                    if (global.PLUGIN_SECURITY)
                    {
                        LogFile.WriteLog("INFO", "Ask for privilege : {0}".FormateEx(pl.GetContent("stiv_p4")));
                        LDailog ld = new LDailog(LunalipsContentUI.DIA_WITH_YESNO,
                            pl.GetContent("plg_stiv_h"),
                            pl.GetContent("plg_stiv_c")
                                .FormateEx(INFO[plgName].Name,
                                    plgName,
                                    pl.GetContent("stiv_p4")));
                        if (ld.ShowDialog() == true)
                        {
                            CustomModePrivilege.Add((abt as LunalipseCustomMode).PluginID);
                            LogFile.WriteLog("INFO", "Permitted");
                        }
                        else LogFile.WriteLog("INFO", "Denied");
                    }
                    else
                    {
                        LogFile.WriteLog("INFO", "Plugin security has been disabled. Auto permit.");
                        CustomModePrivilege.Add((abt as LunalipseCustomMode).PluginID);
                    }
                }
            }
        }

        public void FireSequence(APIBridge s,params object[] a)
        {
            foreach(var i in _methods)
            {
                switch(s)
                {
                    case APIBridge.MUSIC_C:
                        i.Value?.ENTRY.MusicChange(a[0] as PlayInfo);
                        break;
                    case APIBridge.MODE_C:
                        i.Value?.ENTRY.ModeChange(Convert.ToInt32(a[0]));
                        break;
                    case APIBridge.VOL_C:
                        i.Value?.ENTRY.VolumeChange(Convert.ToDouble(a[0]));
                        break;
                    case APIBridge.LUNALIPSE_SHUTDOWN:
                        i.Value?.ENTRY.LunalipseExit();
                        break;
                    
                }
            }
        }
    }
}