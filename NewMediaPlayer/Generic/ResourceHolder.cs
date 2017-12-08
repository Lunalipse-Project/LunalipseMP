using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace NewMediaPlayer.Generic
{
    public class ResourceHolder
    {
        public static volatile ResourceHolder RH_INSTANCE;
        public static readonly object LOCKER = new object();

        public static ResourceHolder INSTANCE
        {
            get
            {
                if(RH_INSTANCE == null)
                {
                    lock(LOCKER)
                    {
                        if (RH_INSTANCE == null) return RH_INSTANCE = new ResourceHolder();
                    }
                }
                return RH_INSTANCE;
            }
        }

        PrivateFontCollection FONTS;
        Dictionary<string, BitmapImage> IMAGES;

        public ResourceHolder()
        {
            FONTS = new PrivateFontCollection();
            IMAGES = new Dictionary<string, BitmapImage>();
        }

        public void AddFonts(byte[] font)
        {
            IntPtr fntp = Marshal.AllocCoTaskMem(font.Length);
            Marshal.Copy(font, 0, fntp, font.Length);
            FONTS.AddMemoryFont(fntp, font.Length);
            Marshal.FreeCoTaskMem(fntp);
        }

        public void AddImage(Stream imgstream,string key)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = imgstream;
            bi.EndInit();
            IMAGES.Add(key, bi);
        }

        public FontFamily getFont(int i)
        {
            if (i >= FONTS.Families.Length) return null;
            return FONTS.Families[i];
        }

        public BitmapImage getImage(string key)
        {
            return IMAGES[key];
        }
    }
}
