using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaNetCore.Bodies
{
    /// <summary>
    /// 请求执行完成的回调（寄生）方法
    /// </summary>
    public class ParasitismBundle
    {
        /// <summary>
        /// 请求完成回调方法。
        /// </summary>
        /// <param name="rest">请求结果</param>
        public delegate void Bundle(string rest);
    }
}
