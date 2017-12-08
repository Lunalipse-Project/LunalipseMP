using gMusic.API;
using gMusic.MusicOL;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace gMusic.util
{
    public class ResultFormatter
    {
        public static IDictionary<int, MusicOvw> MusicOverview(string json)
        {
            try
            {
                JToken jt = JObject.Parse(json)["result"];
                int c = 0;
                IDictionary<int, MusicOvw> dir = new Dictionary<int, MusicOvw>();
                foreach (var v in jt["songs"])
                {
                    MusicOvw movw = new MusicOvw(v["id"].ToString(), v["name"].ToString(), ((v["artists"])[0])["name"].ToString());
                    dir.Add(c, movw);
                    c++;
                }
                return dir;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        public static string getLyrics(string json)
        {
            JToken jt = JObject.Parse(json);
            if (jt["lrc"] == null || jt["nolyric"] != null) return "NO_LYRICS";
            return jt["lrc"]["lyric"].ToString();
        }

        public static MusicDtl getDetail(string json)
        {
            MusicDtl mdl = new MusicDtl();
            try
            {
                
                var jt = (JObject.Parse(json)["songs"])[0];
                mdl.musicN = jt["name"].ToString();
                if(jt["hMusic"] !=null)
                    mdl.H_URL = jt["hMusic"].HasValues ? String.Format(APIs.MUSIC_DWLINK,
                        enMusicId.EncryptMusicId(jt["hMusic"]["dfsId"].ToString()),
                        jt["hMusic"]["dfsId"].ToString(),
                        jt["hMusic"]["extension"].ToString()) : "";

                if(jt["mMusic"]!=null)
                    mdl.M_URL = jt["hMusic"].HasValues ? String.Format(APIs.MUSIC_DWLINK,
                        enMusicId.EncryptMusicId(jt["mMusic"]["dfsId"].ToString()),
                        jt["mMusic"]["dfsId"].ToString(),
                        jt["mMusic"]["extension"].ToString()) : "";

                if(jt["lMusic"]!=null)
                    mdl.L_URL = jt["hMusic"].HasValues ? String.Format(APIs.MUSIC_DWLINK,
                        enMusicId.EncryptMusicId(jt["lMusic"]["dfsId"].ToString()),
                        jt["lMusic"]["dfsId"].ToString(),
                        jt["lMusic"]["extension"].ToString()) : "";

                mdl.SIZE = new long[]
                {
                jt["hMusic"].HasValues?long.Parse(jt["hMusic"]["size"].ToString()):0,
                jt["mMusic"].HasValues?long.Parse(jt["mMusic"]["size"].ToString()):0,
                jt["lMusic"].HasValues?long.Parse(jt["lMusic"]["size"].ToString()):0
                };
                mdl.perlisten = jt["mp3Url"].HasValues?jt["mp3Url"].ToString():mdl.L_URL;
                mdl.picURL = jt["album"]["picUrl"].ToString();
                mdl.AblumName = jt["album"]["name"].ToString();
                mdl.Artist = jt["artists"][0]["name"].ToString(); ;
                mdl.brate = jt["hMusic"].HasValues ? jt["hMusic"]["bitrate"].ToString() : (jt["mMusic"].HasValues ? jt["mMusic"]["bitrate"].ToString() : jt["lMusic"]["bitrate"].ToString());
                return mdl;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
    }
}
