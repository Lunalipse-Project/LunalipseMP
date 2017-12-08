using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunalipseAPI.PlayMode
{
    public delegate bool RMode(int value,string ModeDes,string ID);
    public delegate void UrMode(int value, string ID);
    public class ModeManager
    {
        public static event RMode RegMODE;
        public static event UrMode URegMODE;

        public string ID;
        public ModeManager(string PluginID)
        {
            ID = PluginID;
        }
        /// <summary>
        /// 注册一个自定义模式
        /// </summary>
        /// <param name="ModeIdentification">模式标识符</param>
        /// <param name="ModeDescription">模式描述字段</param>
        /// <returns>是否成功</returns>
        public bool RegistMode(int ModeIdentification,string ModeDescription)
        {
            return RegMODE(ModeIdentification, ModeDescription, ID);
        }

        public void UnregistMode(int ModeIdentification)
        {
            URegMODE(ModeIdentification, ID);
        }
    }
}
