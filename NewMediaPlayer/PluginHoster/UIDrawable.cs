using LunalipseAPI.Graphics;
using LunalipseAPI.Graphics.Generic;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NewMediaPlayer.PluginHoster
{
    public class UIDrawable
    {
        //IDictionary<string, PluginDrawable> Elements;

        public UIDrawable()
        { 
            //Elements = new Dictionary<string, PluginDrawable>();
        }
        public void DrawButton(object target, LButton lb, LunalipseTarget lt, string plgName)
        {
            Button b = new Button
            {
                Name = lb.Name,
                Width = lb.Width,
                Height = lb.Height,
                Content = lb.Content,
                Margin = new System.Windows.Thickness(
                            lb.Margin[0],
                            lb.Margin[1],
                            lb.Margin[2],
                            lb.Margin[3]),
                VerticalAlignment = VertialTrans(lb.VAligment),
                HorizontalAlignment = HorizonTrans(lb.HAligment),
                VerticalContentAlignment = VertialTrans(lb.VContentAligment),
                HorizontalContentAlignment = HorizonTrans(lb.HContentAligment),
                Visibility = VisibleTrans(lb.Visibility),
                Foreground = new SolidColorBrush(lb.Foreground),
                Background = new SolidColorBrush(lb.Background),
                Tag = plgName
            };
            b.FontStyle = lb.Italic ? FontStyles.Italic : FontStyles.Normal;
            b.FontWeight = lb.Bold ? FontWeights.Bold : FontWeights.Normal;
            b.Click += new RoutedEventHandler(lb.ButtonEvent);

            AddTo(target, lt, b);
        }

        public void DrawLabel(object target, LLabel lb, LunalipseTarget lt, string plgName)
        {
            Label b = new Label
            {
                Name = lb.Name,
                Width = lb.Width,
                Height = lb.Height,
                Content = lb.Content,
                Margin = new System.Windows.Thickness(
                            lb.Margin[0],
                            lb.Margin[1],
                            lb.Margin[2],
                            lb.Margin[3]),
                VerticalAlignment = VertialTrans(lb.VAligment),
                HorizontalAlignment = HorizonTrans(lb.HAligment),
                VerticalContentAlignment = VertialTrans(lb.VContentAligment),
                HorizontalContentAlignment = HorizonTrans(lb.HContentAligment),
                Visibility = VisibleTrans(lb.Visibility),
                Foreground = new SolidColorBrush(lb.Foreground),
                Background = new SolidColorBrush(lb.Background),
                Tag = plgName
            };
            b.FontStyle = lb.Italic ? FontStyles.Italic : FontStyles.Normal;
            b.FontWeight = lb.Bold ? FontWeights.Bold : FontWeights.Normal;
            AddTo(target, lt, b);
        }

        public void DrawVanilla(object target,LunalipseTarget lt,Control c, string plgName)
        {
            c.Tag = plgName;
            AddTo(target, lt, c);
        }

        private void AddTo(params object[] a)
        {
            switch (a[1] as LunalipseTarget?)
            {
                case LunalipseTarget.MAIN_WINDOW:
                    MainWindow mw = a[0] as MainWindow;
                    mw.outershell.Children.Add(a[2] as Control);
                    Console.WriteLine((a[2] as Control).Tag as string);
                    break;
            }
            //Elements.Add((a[2] as Control).Name, new PluginDrawable { _E = a[2] as Control, hostForm = a[1] as LunalipseTarget? });
        }

        public VerticalAlignment VertialTrans(Alignment al)
        {
            switch (al)
            {
                case Alignment.CENTER: return VerticalAlignment.Center;
                case Alignment.BOTTOM: return VerticalAlignment.Bottom;
                case Alignment.TOP: return VerticalAlignment.Top;
                case Alignment.STRECH: return VerticalAlignment.Stretch;
            }
            return VerticalAlignment.Center;
        }
        public HorizontalAlignment HorizonTrans(Alignment al)
        {
            switch (al)
            {
                case Alignment.CENTER: return HorizontalAlignment.Center;
                case Alignment.LEFT: return HorizontalAlignment.Left;
                case Alignment.RIGHT: return HorizontalAlignment.Right;
                case Alignment.STRECH: return HorizontalAlignment.Stretch;
            }
            return HorizontalAlignment.Center;
        }
        
        public Visibility VisibleTrans(LVisibility lv)
        {
            switch (lv)
            {
                case LVisibility.VISIBLE:return Visibility.Visible;
                case LVisibility.HIDDEN:return Visibility.Hidden;
            }
            return Visibility.Visible;
        }
    }

    public class PluginDrawable
    {
        public UIElement _E;
        public LunalipseTarget? hostForm;
    }
}
