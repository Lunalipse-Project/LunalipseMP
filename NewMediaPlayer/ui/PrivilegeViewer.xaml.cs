using I18N;
using NewMediaPlayer.Dialog;
using NewMediaPlayer.Generic;
using NewMediaPlayer.PluginHoster;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace NewMediaPlayer.ui
{
    /// <summary>
    /// PrivilegeViewer.xaml 的交互逻辑
    /// </summary>
    public partial class PrivilegeViewer : Page
    {
        ObservableCollection<PrivilegeHoster> list = new ObservableCollection<PrivilegeHoster>();
        PluginHelper PH;
        PageLang PL;
        public PrivilegeViewer(LDailog ld, string plgName)
        {
            InitializeComponent();
            PH = PluginHelper.INSTANCE;
            PL = I18NHelper.INSTANCE.GetReferrence("PrivilegeViewer");
            ld.Title.Content = PL.GetContent("title");
            desc_.Text = PL.GetContent("desc").FormateEx(plgName);
            Console.WriteLine(plgName);
            foreach(string p in PH.GetPrivilegeByName(plgName))
            {
                list.Add(new PrivilegeHoster()
                {
                    Privilege = PL.GetContent(p),
                    PTag = p
                });
                Console.WriteLine(p);
            }
            prive.ItemsSource = list;
        }

        private void prive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tag = (prive.SelectedItem as PrivilegeHoster).PTag;
            desc.Text = PL.GetContent(tag + "_desc");
        }
    }
}
