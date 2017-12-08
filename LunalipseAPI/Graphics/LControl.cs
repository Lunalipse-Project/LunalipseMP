using LunalipseAPI.Graphics.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LunalipseAPI.Graphics
{
    public class LControl
    {
        public string Name, Content;
        public int Height=20, Width=20;
        Alignment v_align, v_CTX_align,h_align,h_CTX_align;
        int[] margin;
        LVisibility v;
        int fontsize = 15;
        bool isItalic, isBold;
        public Color Foreground = Color.FromArgb(255,2,5,3), Background = Color.FromArgb(0, 2, 5, 3);

        public Alignment VAligment
        {
            get
            {
                return v_align;
            }
            set
            {
                v_align = value;
            }
        }
        public Alignment HAligment
        {
            get
            {
                return h_align;
            }
            set
            {
                h_align = value;
            }
        }

        public Alignment VContentAligment
        {
            get
            {
                return v_CTX_align;
            }
            set
            {
                v_CTX_align = value;
            }
        }

        public Alignment HContentAligment
        {
            get
            {
                return h_CTX_align;
            }
            set
            {
                h_CTX_align = value;
            }
        }


        public int[] Margin
        {
            get
            {
                return margin;
            }
            set
            {
                margin = value;
            }
        }

        public LVisibility Visibility
        {
            get
            {
                return v;
            }
            set
            {
                v = value;
            }
        }

        public int FontSize
        {
            get
            {
                return fontsize;
            }
            set
            {
                fontsize = value;
            }
        }

        public bool Italic
        {
            get
            {
                return isItalic;
            }
            set
            {
                isItalic = value;
            }
        }

        public bool Bold
        {
            get
            {
                return isBold;
            }
            set
            {
                isBold = value;
            }
        }
    }
}
