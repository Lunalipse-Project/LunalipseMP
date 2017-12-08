using gMusic.API;
using LunaNetCore;
using LunaNetCore.Bodies;
using Newtonsoft;
using Newtonsoft.Json.Linq;

namespace gMusic
{
    public class LunaProxy
    {
        private volatile static LunaProxy _LP_INSTANCE = null;
        private static readonly object LHper = new object();
        IRequestEvent re;
        
        public static LunaProxy INSTANCE(IRequestEvent _re)
        {
            if(_LP_INSTANCE == null)
            {
                lock(LHper)
                {
                    if(_LP_INSTANCE == null)
                    {
                        _LP_INSTANCE = new LunaProxy(_re);
                    }
                }
            }
            return _LP_INSTANCE;
        }
        private LunaProxy(IRequestEvent ire)
        {
            LNetC.InitializeLunaNetCore();
            re = ire;
            LNetC.OnHttpRequesting += (x) =>
            {
                re.HttpRequesting(x);
            };
            LNetC.OnHttpResponded += (x,y) =>
            {
                re.HttpResponded(x,y);

            };
            LNetC.OnAllQueueRequestCompletely += () =>
            {
                re.AllReqCompletely();
            };
            LNetC.OnErrorOccursInDeepLayer += (x) =>
            {
                re.ErrorOccurs(x);
            };
            LNetC.OnHttpTimeOut += () => re.ReqTimeOut();
        }

        public void SearchGetSongs(string searchKeyW,int searchType,int offset)
        {
            RBody rb = new RBody();
            rb.URL = APIs.MUSIC_SEARCH;
            rb.AddParameter("s", searchKeyW);
            rb.AddParameter("limit", "30");
            rb.AddParameter("type", searchType+"");
            rb.AddParameter("offset", offset+"");
            rb.RequestMethod = HttpMethod.POST;
            LNetC.AddRequestBody(rb, EventType.E_SEARCH);
        }

        public void GetLyric(string songid)
        {
            RBody rb = new RBody();
            rb.URL = APIs.MUSIC_LYRIC;
            rb.AddParameter("os", "pc");
            rb.AddParameter("id",  songid);
            rb.AddParameter("lv","-1");
            rb.AddParameter("kv","-1");
            rb.AddParameter("tv","-1");
            rb.RequestMethod = HttpMethod.GET;
            LNetC.AddRequestBody(rb, EventType.E_LYRIC);
        }

        public void GetDetail(string sid)
        {
            RBody rb = new RBody();
            rb.URL = APIs.MUSIC_DETAIL;
            rb.AddParameter("id", sid);
            rb.AddParameter("ids", string.Format(APIs.P_DETAIL, sid));
            rb.RequestMethod = HttpMethod.POST;
            LNetC.AddRequestBody(rb, EventType.E_DETAIL);
        }

        public string GetDownloadLink(string dfsid,string ext)
        {
            string encd = util.enMusicId.EncryptMusicId(dfsid);
            return string.Format(APIs.MUSIC_DWLINK, encd, dfsid, ext);
        }

        public void ReqAsyn()
        {
            LNetC.StartRequestAsyn();
        }

        public static void DEINSTANCE()
        {
            _LP_INSTANCE = null;
        }
    }
}
