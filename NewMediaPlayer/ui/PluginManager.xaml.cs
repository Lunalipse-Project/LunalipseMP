using I18N;
using LunalipseAPI.Generic;
using NewMediaPlayer.Dialog;
using NewMediaPlayer.Generic;
using NewMediaPlayer.PluginHoster;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace NewMediaPlayer.ui
{
    /// <summary>
    /// PluginManager.xaml 的交互逻辑
    /// </summary>
    public partial class PluginManager : Page
    {
        ObservableCollection<PluginInfo> list = new ObservableCollection<PluginInfo>();
        PluginHelper PH;
        PageLang PL;
        string name;
        public PluginManager(LDailog ldg)
        {
            InitializeComponent();
            PL = I18NHelper.INSTANCE.GetReferrence("PluginManager");
            PH = PluginHelper.INSTANCE;
            foreach(var lpi in PH.Plugins)
            {
                PluginInfo PI = new PluginInfo();
                PI.PLGName = lpi.Key;
                if (PH.ACTIVATED[PI.PLGName]) PI.PLGActivated = System.Windows.Visibility.Visible;
                else PI.PLGActivated = System.Windows.Visibility.Hidden;
                list.Add(PI);
            }
            PLGDISP.ItemsSource = list;

            ldg.Title.Content = PL.GetContent("title");

            author.Content = PL.GetContent("author");
            team.Content = PL.GetContent("team");
            desc.Content = PL.GetContent("desc");
            privilege.Content = PL.GetContent("priv");
            ver.Content = PL.GetContent("ver");

            load.Content = PL.GetContent("load");

            PH.PluginLoaded += (x) =>
            {
                (PLGDISP.SelectedItem as PluginInfo).PLGActivated = System.Windows.Visibility.Visible;
            };
            PH.PluginUnload += (x) =>
            {
                (PLGDISP.SelectedItem as PluginInfo).PLGActivated = System.Windows.Visibility.Hidden;
            };
            PLGDISP.SelectedIndex = PLGDISP.Items.Count != 0 ? 0 : -1;
        }

        private void PluginChanged(object sender, SelectionChangedEventArgs e)
        {
            PluginInfo pi = PLGDISP.SelectedItem as PluginInfo;
            LunalipsePluginInfo lpi = PH.Plugins[pi.PLGName];
            name = pi.PLGName;
            PluginName.Content = lpi.Name;
            author_v.Content = lpi.Author;
            team_v.Content = lpi.Team;
            ver_v.Content = lpi.Version;
            desc_v.Text = lpi.Description;
            load.Content = PL.GetContent(PH.ACTIVATED[pi.PLGName] ? "unload" : "load");
            actv.Foreground = PH.ACTIVATED[pi.PLGName] ? new SolidColorBrush(Color.FromArgb(225, 92, 184, 92)) : new SolidColorBrush(Color.FromArgb(225, 255, 35, 0));
            actv.Content = PL.GetContent("hint_" + (PH.ACTIVATED[pi.PLGName] ? "a" : "u"));
        }

        private void load_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(PH.ACTIVATED[name]==false)
            {
                PH.LoadPlugin(name);
                load.Content = PL.GetContent("unload");
                actv.Foreground = new SolidColorBrush(Color.FromArgb(225, 92, 184, 92));
                actv.Content = PL.GetContent("hint_a");
            }
            else
            {
                PH.Unload(name);
                load.Content = PL.GetContent("load");
                actv.Foreground = new SolidColorBrush(Color.FromArgb(225, 255, 35, 0));
                actv.Content = PL.GetContent("hint_u");
            }
        }

        private void priv(object sender, System.Windows.RoutedEventArgs e)
        {
            if(name.AvailableEx())
            {
                new LDailog(controler.LunalipsContentUI.PRIVILEGE_VIEWER, "", true, name).ShowDialog();
            }
        }
    }
}
