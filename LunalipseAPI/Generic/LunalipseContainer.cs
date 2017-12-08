using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunalipseAPI.Generic
{
    public delegate bool ODialogue(object i, double h, double w);
    public delegate bool ODefault(string i, params object[] p);
    public class LunalipseContainer
    {
        public static event ODialogue OpenD;
        public static event ODefault ODeft;

        /// <summary>
        /// 显示Lunlipse对话框。
        /// </summary>
        /// <param name="INSTANCE">对话框布局文件，必须为<seealso cref="System.Windows.Controls.Page"/></param>
        /// <param name="hei">该布局的高度</param>
        /// <param name="wei">该布局的宽度</param>
        /// <returns>能否显示</returns>
        public static bool OpenLPXdialogEx(object INSTANCE, double hei, double wei)
        {
            return OpenD(INSTANCE, hei, wei);
        }

        public static bool OpenLPXdialogEx(string ixer, params object[] p_)
        {
            return ODeft(ixer, p_);
        }
    }
}
