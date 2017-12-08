#define USE_FILE_I18N
using I18N;
using LpxResource;
using LpxResource.Generic;
using LpxResource.LRTypes;
using NewMediaPlayer.controler;
using NewMediaPlayer.Dialog;
using LPXG = NewMediaPlayer.Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Windows;

/* ====== Resources Index =======
 * R_lpx00.lrss => Images and fonts
 * R_lpx01.lrss => I18N Configurations (Only adopt in published version)
 */

namespace NewMediaPlayer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        LResInput lri;
        I18NHelper I18NH;
        PageLang PL;
        bool tmp_;
        public App()
        {
            //DispatcherUnhandledException += App_DispatcherUnhandledException;
            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ReportException(e.ExceptionObject as Exception);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ReportException(e.Exception);
            e.Handled = true;
        }

        private void CheckAdministrator()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);

            bool runAsAdmin = wp.IsInRole(WindowsBuiltInRole.Administrator);

            if (!runAsAdmin)
            {
                // It is not possible to launch a ClickOnce app as administrator directly,  
                // so instead we launch the app as administrator in a new process.  
                var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);

                // The following properties run the new process as administrator  
                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";

                // Start the new process  
                try
                {
                    Process.Start(processInfo);
                }
                catch (Exception ex)
                {
                    
                }

                // Shut down the current process  
                Environment.Exit(0);
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            lri = new LResInput();
            I18NH = I18NHelper.INSTANCE;
            tmp_ = PropertyHelper.ReadProperty();
            I18NH.LANGUAGES = global.USE_SYS_LANG ? GetSysLang() : global.LANG ?? Languages.CHINESE;
#if !USE_FILE_I18N
            I18NH.parseLangFile(lri.LoadResourceReadOnly(@"Resources\R_lps01.lrss"));
#else
            I18NH.parseLangFile();
#endif
            if (!new LPXG.validationF().Validation()) Environment.Exit(0);
            LogFile.InitializeLogFile();
            LogFile.WriteLog("INFO", "Reading conifg");

            PL = I18NH.GetReferrence("Cfg");

            InitializeResource();

            StartupUri = new Uri("MainWindow.xaml", UriKind.RelativeOrAbsolute);
            if (global.VER == null && tmp_)
            {
                new LDailog(LunalipsContentUI.TIPMESSAGE, PL.GetContent("CfgIncmp"), PL.GetContent("CfgIncmpC")).Show();
                PropertyHelper.SaveProperty();
            }
            else if (tmp_)
            {
                string s = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                if (!global.VER.Equals(s))
                {
                    new LDailog(LunalipsContentUI.TIPMESSAGE, PL.GetContent("CfgOld"), PL.GetContent("CfgOldC").FormateEx(global.VER, s)).Show();
                }
            }
            
        }

        void InitializeResource()
        {
            LPXG.ResourceHolder rh = LPXG.ResourceHolder.INSTANCE;
            LogFile.WriteLog("INFO", "Parsing Resources");
            EventHoster.onErrOcurr += (x, y) =>
            {
                new LDailog(LunalipsContentUI.TIPMESSAGE, PL.GetContent("ferr_h"), PL.GetContent("ferr_c").FormateEx(x.ToString(), y[0])).ShowDialog();
                LogFile.WriteLog("ERROR", y[0]);
                LogFile.Release();
                Environment.Exit(0);
            };
            EventHoster.onAllDBlockRead += (x, y) =>
            {
                LogFile.WriteLog("INFO","Combined reosource chunks with size {0} with final pointer 0x{1}".FormateEx(x.ToString(),y.ToString("x16")));
            };
            ResourceCollection RObj = lri.LoadResourceReadOnly(@"Resources\R_lps00.lrss");
            LogFile.WriteLog("INFO", "Embedding Resources");
            //rh.AddFonts(RObj.getResourceK("Equestria"));
            //rh.AddFonts(RObj.getResourceK("Minecraft"));
            IEnumerator<StreamRes> iers = RObj.getResourceSE("g$");
            while(iers.MoveNext())
            {
                rh.AddImage(iers.Current.rData, iers.Current.fname);
            }
            LogFile.WriteLog("INFO", "Resources Embedded completely.");
        }

        private void ReportException(Exception e)
        {
            LogFile.WriteLog("UNEXPECTED ERROR", e.Message);
            new LDailog(LunalipsContentUI.TIPMESSAGE, PL.GetContent("ferr_h"), PL.GetContent("ferr_c").FormateEx(e.HResult, e.Message)).ShowDialog();
        }
        public Languages GetSysLang()
        {
            switch (System.Globalization.CultureInfo.CurrentUICulture.Name)
            {
                case "zh-CN":
                    return Languages.CHINESE;
                case "en-US":
                    return Languages.ENGLISH;
                case "ru-RU":
                    return Languages.RUSSIAN;
                case "zh-TW":
                    return Languages.TRADITIONAL;
                default:
                    return Languages.ENGLISH;
            }
        }
    }
}
