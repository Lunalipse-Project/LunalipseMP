using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.IO;
using NewMediaPlayer.Generic;
using System.Windows.Media.Imaging;

namespace NewMediaPlayer.ui.Performance
{
    class BgManager
    {
        ResourceHolder rh;
        string folder;
        public int MAX { get; set; }

        public BgManager()
        {
            rh = ResourceHolder.INSTANCE;
            MAX = 10;
        }
        public BgManager(string p) : this()
        {
            if (!Directory.Exists(p)) Directory.CreateDirectory(p);
            AddImgsFromFolder(p);
        }

        public BgManager(string p,string format) : this()
        {
            if (Directory.Exists(p)) Directory.CreateDirectory(p);
            AddImgsFromFolder(p, format);
        }
        public void AddImgsFromFolder(string path, string supportFormat = ".png|.jpg")
        {
            string[] s = supportFormat.Split('|');
            int c = 0;
            foreach(string fi in Directory.GetFiles(path))
            {
                if (c >= MAX) break;
                if (s.Contains(Path.GetExtension(fi)))
                {
                    rh.AddImage(new BitmapImage(new Uri(fi)), "bg" + c);
                }
                c++;
            }
        }

        public BitmapImage GetBackground(int id)
        {
            if (!rh.Exist("bg" + id)) return null;
            return rh.getImage("bg" + id);
        }
    }
}
