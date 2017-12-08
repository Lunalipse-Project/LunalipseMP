using LunalipseAPI.Graphics.Generic;
using System;
using System.Windows.Controls;

namespace LunalipseAPI.Graphics
{
    public delegate bool DrawButton(LunalipseTarget lt,LButton lb,string pgName);
    public delegate bool DrawLabel(LunalipseTarget lt, LLabel lb,string pgName);
    public delegate bool DrawControls(LunalipseTarget lt, Control cc,string pgName);
    public delegate void ReleaseUI(string pgName);
    public class DrawingManager
    {
        public static event DrawButton OnDrawButton;
        public static event DrawLabel OnDrawLabel;
        public static event DrawControls OnDrawControl;
        public static event ReleaseUI OnUIRelased;

        private string pgName;
        public DrawingManager(string PluginID)
        {
            pgName = PluginID;
        }


        public bool Luna_DrawButton(LunalipseTarget LT,LButton LB)
        {
            return OnDrawButton(LT, LB, pgName);
            
        }
        public bool Luna_DrawLabel(LunalipseTarget LT, LLabel LB)
        {
            return OnDrawLabel(LT, LB,pgName);
        }

        public bool Luna_DrawMisc(LunalipseTarget lt,Control ctrl)
        {
            return OnDrawControl(lt, ctrl,pgName);
        }

        public void Luna_ReleaseUI()
        {
            OnUIRelased(pgName);
        }
    }
}
