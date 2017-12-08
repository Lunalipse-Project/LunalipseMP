using LunaNetCore.Bodies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gMusic.API
{
    public interface IRequestEvent
    {
        void HttpRequesting(string par1);
        void HttpResponded(string par1, RResult par2);
        void AllReqCompletely();
        void ErrorOccurs(Exception e);
        void ReqTimeOut();
    }
}
