using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunalipseAPI.Configuration
{
    public delegate bool WCFGopt(string key,object value,string id);
    public delegate bool HKey(string key);
    public delegate object RCFGopt(string key, string id);
    public class GlobalCfgManager
    {
        public static event WCFGopt CfgWOperation;
        public static event RCFGopt CfgROperation;
        public static event HKey hKey;
        private string PluginID;

        /// <summary>
        /// 初始化全局配置文件助手
        /// </summary>
        /// <param name="LID">插件标识符</param>
        public GlobalCfgManager(string LID)
        {
            PluginID = LID;
        }
        public bool WriteConfig(string Key,object Value)
        {
            return CfgWOperation(Key, Value,PluginID);
        }

        public object ReadConfig(string Key)
        {
            return CfgROperation(Key, PluginID);
        }

        public bool HasConfigField(string key)
        {
            return hKey(key);
        }
    }

    /// <summary>
    /// 申请配置文件读写权限。
    /// </summary>
    public class GlobalConfigPrivilege : Attribute
    {
        public string PluginID;
    }
}
