using System;

namespace LunalipseAPI.PlayMode
{
    public class LunalipseCustomMode : Attribute
    {
        /// <summary>
        /// 是否参与并支持国际化。
        /// <para>
        /// 如果设置此项为True，请务必准备好相应的语言对应键值。并在使用<seealso cref="ModeManager.RegistMode(int, string)"/> 注册模式时将描述设置为语言索引键。
        /// </para>
        /// <para>
        /// 设置语言键值可通I18N提供的方法 <seealso cref="I18N.I18NProxy.AddLang(string, string, string)"/> 
        /// 将语言键值添加到主窗口(<seealso cref="I18N.Referrence.MAINWINDOW"/>)
        /// </para>
        /// </summary>
        public bool I18Npresent = false;

        public string PluginID;
    }
}
