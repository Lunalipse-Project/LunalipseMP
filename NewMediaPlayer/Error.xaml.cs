using NewMediaPlayer.Lunalipx;
using System.Windows;

namespace NewMediaPlayer
{
    /// <summary>
    /// Error.xaml 的交互逻辑
    /// </summary>
    public partial class Error : Window
    {
        public Error(LPXException msg)
        {
            InitializeComponent();
            emsg.Content = msg.Type;
            code.Content = msg.Message;
        }
    }
}
