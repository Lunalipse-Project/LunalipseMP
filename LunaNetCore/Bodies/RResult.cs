using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LunaNetCore.Bodies.ParasitismBundle;

namespace LunaNetCore.Bodies
{
    /// <summary>
    /// 响应体
    /// </summary>
    public class RResult
    {
        string url;
        HttpMethod Method;
        string resd;
        Action<string,string> rbnd;

        /// <summary>
        /// 构造一个请求返回实例
        /// </summary>
        /// <param name="u">请求URL</param>
        /// <param name="m">请求所使用的方式</param>
        /// <param name="r">服务器响应数据</param>
        /// <param name="r_bnd">寄生方法</param>
        public RResult(string u,HttpMethod m,string r,Action<string, string> r_bnd)
        {
            url = u;
            Method = m;
            resd = r;
            rbnd = r_bnd;
        }

        /// <summary>
        /// 获取请求的URL
        /// </summary>
        public string URL
        {
            get
            {
                return url;
            }
        }

        /// <summary>
        /// 获取请求使用的方法
        /// </summary>
        public HttpMethod RequestMethod
        {
            get
            {
                return Method;
            }
        }

        /// <summary>
        /// 请求的结果
        /// </summary>
        public string ResultData
        {
            get
            {
                return resd;
            }
        }

        public Action<string, string> CallBack
        {
            get { return rbnd; }
        }
    }
}
