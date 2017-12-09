using LunaNetCore.Bodies;
using NetEaseHijacker.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetEaseHijacker
{
    public class Hijack
    {
        Raw r;
        public Hijack()
        {
            r = new Raw();
        }

        public void E_AllComplete(Action e)
        {
            r.Event_AllComplete(() => e?.Invoke());
        }

        public void E_Requesting(Action<string> e)
        {
            r.Event_Requesting((x) => e?.Invoke(x));
        }

        public void E_Responded(Action<string,RResult> e)
        {
            r.Event_Responded((x, y) => e?.Invoke(x, y));
        }

        public void E_TimeOut(Action e)
        {
            r.Event_TimeOut(() => e?.Invoke());
        }

        public async Task SearchSong(string keyw, int limit = 8)
        {
            await r.Get(SearchType.SONGS, keyw, limit.ToString());
        }

        public async Task SongDetail(string id)
        {
            await r.Get(SearchType.DETAIL, id);
        }

        public async Task DownloadURL(string id, string bitRate)
        {
            await r.Get(SearchType.DOWNLOAD, id, bitRate);
        }

        public List<SResult> ParseSongList(string result)
        {
            try
            {
                JObject jo = JObject.Parse(result);
                List<SResult> lsr = new List<SResult>();
                foreach (var v in jo["result"]["songs"])
                {
                    SResult sr = new SResult();
                    sr.id = v["id"].ToString();
                    sr.name = v["name"].ToString();
                    sr.artist = v["artists"][0]["name"].ToString();
                    lsr.Add(sr);
                }
                return lsr;
            }
            catch
            {
                return null;
            }
        }

        public SDetail ParseSongDetail(string result)
        {
            try
            {
                JObject jo = JObject.Parse(result);
                if (!jo["songs"].HasValues) return null;
                JToken jt= jo["songs"][0];
                SDetail sd = new SDetail();
                sd.id = jt["id"].ToString();
                sd.name = jt["name"].ToString();
                sd.al_pic = jt["al"]["picUrl"].ToString();
                sd.ar_name = jt["ar"][0]["name"].ToString();
                sd.al_name = jt["al"]["name"].ToString();
                int i = 0;

                foreach(char c in "lmh")
                {
                    string c_ = c.ToString();
                    if(jt[c_].HasValues)
                    {
                        sd.sizes[i] = Convert.ToInt64(jt[c_]["size"].ToString());
                        sd.bitrate[i] = Convert.ToInt32(jt[c_]["br"].ToString());
                        i++;
                    }
                }
                return sd;
            }
            catch
            {
                return null;
            }
        }

        public string ParseDownloadURL(string result)
        {
            try
            {
                JToken jt = JObject.Parse(result)["data"][0];
                return jt["url"] != null ? jt["url"].ToString() : "";
            }
            catch
            {
                return "";
            }
        }
    }
}
