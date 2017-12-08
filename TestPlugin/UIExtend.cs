using System;
using LunalipseAPI.Generic;
using LunalipseAPI.Graphics;
using LunalipseAPI.Graphics.Generic;
using System.Windows.Media;
using System.Windows;
using System.Windows.Forms;

namespace TestPlugin
{
    /// <summary>
    /// Told Lunalipse this is a UI class, which allows you drawing on Lunalipse
    /// This is important , Lunalipse will load this class as normal.
    /// </summary>
    [LunalipseDrawing(PluginID = "LXP_Extend")]
    public class UIExtend : ILunalipseDrawing
    {
        DrawingManager dm = new DrawingManager("LXP_Extend");
        public void InitialDraw()
        {
            LButton LBT = new LButton
            {
                Name = "testB",
                Content = "TEST BUTTON",
                Width = 100,
                Height = 40,
                HContentAligment = Alignment.CENTER,
                VContentAligment = Alignment.CENTER,
                HAligment = Alignment.CENTER,
                VAligment = Alignment.CENTER,
                Margin = new int[] { 20, 50, 20, 50 },
                Visibility = LVisibility.VISIBLE,
                Foreground = Color.FromArgb(255, 255, 255, 255),
                Background = Color.FromArgb(0, 0, 0, 0),
                FontSize = 13
            };
            LBT.ButtonEvent += OnBtn1Clk;
            dm.Luna_DrawButton(LunalipseTarget.MAIN_WINDOW, LBT);
            LLabel LL = new LLabel
            {
                Name = "testL",
                Content = "TEST LABEL",
                Width = 80,
                Height = 40,
                HContentAligment = Alignment.CENTER,
                VContentAligment = Alignment.CENTER,
                HAligment = Alignment.CENTER,
                VAligment = Alignment.CENTER,
                Margin = new int[] { 20, 100, 20, 20 },
                Visibility = LVisibility.VISIBLE,
                Foreground = Color.FromArgb(255, 255, 255, 255),
                Background = Color.FromArgb(0, 0, 0, 0),
                FontSize = 12
            };
            dm.Luna_DrawLabel(LunalipseTarget.MAIN_WINDOW, LL);
        }


        public void OnBtn1Clk(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("I am click. I am");
        }

        public void UIDraw(LunalipseTarget lt)
        {
            
        }

        public void UIUndraw()
        {
            dm.Luna_ReleaseUI();
        }
    }
}
