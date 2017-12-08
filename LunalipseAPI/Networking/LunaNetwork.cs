using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunalipseAPI.Networking
{
    public class LunaNetwork : LncEventBus
    {
        public static event A_RBody RBodyAdded;
        public static event R_RBody RBodyRemoved;
        public static event G_RBody RBodyGet;
        public static event S_REQ_QUE StartReqQue;
        public static event E_ERROR OnErrorOccur;

        string pid;

        public LunaNetwork(string PID,out bool success)
        {
            pid = PID;
            if (pid == "" || pid == null)
                success = false;
            success = true;
       }

        public bool AddRequestBody(RequestBody rb,string id)
        {
            return RBodyAdded(rb, id, pid);
        }

        public bool RemoveRequestBody(string id)
        {
            return RBodyRemoved(id, pid);
        }

        public RequestBody GetRequestBody(string id)
        {
            return RBodyGet(id, pid);
        }

        public bool StartRequestQueue()
        {
            return StartReqQue(pid);
        }
    }
}
