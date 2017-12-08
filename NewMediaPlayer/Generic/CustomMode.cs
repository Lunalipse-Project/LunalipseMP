using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMediaPlayer.Generic
{
    class CustomMode
    {
        /// <summary>
        /// 自定义模式的识别码
        /// </summary>
        public int Identification;
        /// <summary>
        /// 自定义模式的名称
        /// <para>
        /// 如果使用特性 <seealso cref="LunalipseAPI.PlayMode.LunalipseCustomMode"/> 设置了 I18Npresent 为True的话，则为语言键值。
        /// </para>
        /// </summary>
        public string Key;
    }

    class CustomModeCollection
    {
        public static ArrayList collection = new ArrayList();
    }
}
