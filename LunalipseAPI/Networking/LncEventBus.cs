using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunalipseAPI.Networking
{
    public class LncEventBus
    {
        public delegate bool A_RBody(RequestBody rb, string RID,string PID);
        public delegate bool R_RBody(string RID, string PID);
        public delegate RequestBody G_RBody(string ID, string PID);
        public delegate bool S_REQ_QUE(string PID);
        public delegate void E_ERROR(Exception e);
        
    }
}
