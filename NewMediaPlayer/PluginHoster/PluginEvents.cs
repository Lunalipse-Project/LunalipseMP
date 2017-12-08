using LunalipseAPI.Networking;
using LunaNetCore;
using R_LNC = LunaNetCore.Bodies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewMediaPlayer.Generic;

namespace NewMediaPlayer.PluginHoster
{
    internal class PluginEvents
    {
        PluginHelper PH;
        public PluginEvents(PluginHelper ph)
        {
            PH = ph;
        }

        public void SetupEvent()
        {
            SetupNetworkEvents();
        }

        private void SetupNetworkEvents()
        {
            LunaNetwork.RBodyAdded += (x, y, z) =>
            {
                if (PH.hasNetworkingPrivilege(z))
                {
                    PH.ENTITIES[z].LNC?.AddRequestBody(rb2RB(x), y);
                }
                return false;
            };
            LunaNetwork.RBodyGet += (x, y) =>
            {
                if (PH.hasNetworkingPrivilege(y))
                {
                    return RB2rb(PH.ENTITIES[y].LNC?.GetBody(x));
                }
                return null;
            };
            LunaNetwork.RBodyRemoved += (x, y) =>
            {
                if (PH.hasNetworkingPrivilege(y))
                {
                    PH.ENTITIES[y].LNC?.RemoveBody(x);
                }
                return false;
            };
            LunaNetwork.StartReqQue += (x) =>
            {
                if (PH.hasNetworkingPrivilege(x))
                {
                    PH.ENTITIES[x].LNC?.StartRequestAsyn();
                }
                return false;
            };
        }

        private R_LNC.RBody rb2RB(RequestBody rb)
        {
            R_LNC.RBody RB = new R_LNC.RBody()
            {
                BodyBundle = rb.BodyBundle,
                RequestCookie = rb.RequestCookie,
                RequestMethod = hm2HM(rb.RequestMethod),
                URL = rb.URL
            };
            rb.RequestParameter.ForEach((x, y) =>
            {
                RB.AddParameter(x, y);
            });
            return RB;
        }

        private RequestBody RB2rb(R_LNC.RBody RB)
        {
            RequestBody rb = new RequestBody()
            {
                BodyBundle = RB.BodyBundle,
                RequestCookie = RB.RequestCookie,
                RequestMethod = HM2hm(RB.RequestMethod),
                URL = RB.URL
            };
            RB.RequestParameter.ForEach((x, y) =>
            {
                rb.AddParameter(x, y);
            });
            return rb;
        }

        private R_LNC.HttpMethod hm2HM(HttpMethod hm)
        {
            if (hm == HttpMethod.GET) return R_LNC.HttpMethod.GET;
            return R_LNC.HttpMethod.POST;
        }

        private HttpMethod HM2hm(R_LNC.HttpMethod hm)
        {
            if (hm == R_LNC.HttpMethod.GET) return HttpMethod.GET;
            return HttpMethod.POST;
        }
    }
}
