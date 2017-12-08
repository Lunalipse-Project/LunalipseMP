namespace LunalipseAPI.LunalipxPlugin
{
    public delegate bool A_Command(string CMD,string id);
    public delegate bool R_Command(string CMD);
    public delegate void E_ScriptC(int maxiumRange, string id);
    public class LunalipxEx
    {
        public static event A_Command RCommand;
        public static event R_Command URCommand;
        public static event E_ScriptC EScript;

        string id;
        public LunalipxEx(string PluginID)
        {
            id = PluginID;
        }

        public string PluginID
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// 注册一个自定义Lunlipx脚本指令到引擎指令集
        /// </summary>
        /// <param name="CMD">您的指令</param>
        /// <returns></returns>
        public bool RegsitCommand(string CMD)
        {
            return RCommand(CMD, id);
        }

        /// <summary>
        /// 卸载自定义指令
        /// <para>
        /// 这通常会在<seealso cref="MainUI.Destroy"/>方法下使用
        /// </para>
        /// </summary>
        /// <param name="CMD">您已经注册过的指令</param>
        /// <returns></returns>
        public bool UnregistCommand(string CMD)
        {
            return URCommand(CMD);
        }

        public void SizingScriptOptionRange(int newRange)
        {
            EScript(newRange, id);
        }
    }
}
