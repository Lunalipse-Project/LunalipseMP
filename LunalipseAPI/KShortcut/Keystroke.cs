using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunalipseAPI.KShortcut
{
    public delegate void Behavior();
    public class Keystroke
    {
        /// <summary>
        /// 主键，比如像Ctrl，Alt，Shift之类的
        /// </summary>
        public int MainKey;
        /// <summary>
        /// 次建，任意类型
        /// </summary>
        public int Subkey;
        /// <summary>
        /// 是否监听快捷键抬起事件
        /// </summary>
        public bool KeepListenUntilRelease;
        /// <summary>
        /// 快捷键按下时所执行的委托方法。
        /// </summary>
        public Behavior keyStroked;
        /// <summary>
        /// 快捷键抬起时所执行的委托方法。
        /// </summary>
        public Behavior keyReleased;
    }
}
