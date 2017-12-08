using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunalipseAPI.LunalipxPlugin
{
    public interface ILunalipx
    {
        /// <summary>
        /// Lunlipx脚本拓展程序初始化函数。
        /// <para>
        /// 请配合<seealso cref="LunalipxEx.RegsitCommand(string)"/> 在此初始化函数下注册您的自定义指令
        /// </para>
        /// </summary>
        void LPxInitialize();
        /// <summary>
        /// 将自定义指令分类筛选器
        /// <para>
        /// 用于将字符形式的指令翻译为Lunalipx脚本引擎所能理解的整形数据
        /// </para>
        /// </summary>
        /// <param name="Method"></param>
        /// <returns></returns>
        int LunalipxMethod(string Method);
        /// <summary>
        /// 当自定义指令被宿主程序执行时，该方法用于实现该指令的具体功能
        /// </summary>
        /// <param name="command">执行的指令</param>
        /// <param name="PGCounter">Lunalipx脚本程序指针</param>
        /// <param name="SELMusic">目前选中的歌曲</param>
        /// <returns>一个布尔值，如果该功能改变了程序指针，则应返回True，反之为False</returns>
        bool LunalipxBehavior(int command,ref int PGCounter,ref int SELMusic);
    }
}
