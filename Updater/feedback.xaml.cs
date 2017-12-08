using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Updater
{
    /// <summary>
    /// feedback.xaml 的交互逻辑
    /// </summary>
    public partial class feedback : Page
    {
        public feedback()
        {
            InitializeComponent();
        }

        private void scoreC(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(sc!=null)
            {
                sc.Content = e.NewValue;
            }
        }
    }
}
