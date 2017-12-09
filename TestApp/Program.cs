using LpxResource;
using LpxResource.Generic;
using LpxResource.LRTypes;
using LunaNetCore;
using LunaNetCore.Bodies;
using LunapxCompiler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    class Program
    {
        static char[] c = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        /*static void _Main(string[] args)
        {
            #region A
            /*LunalipxCompiler lc = new LunalipxCompiler(0x34EF);
            //ArrayList a = new ArrayList();
            //a.Add(new LUNALIPS_Expression
            //{
            //    isRandom = true,
            //    REP_Times = 3,
            //    exe_CMD = 0x00FF,
            //    SongID = 334,
            //    songsName = "Alexandrov Ensemble - 我的莫斯科.mp3"
            //});
            //a.Add(new LUNALIPS_Expression
            //{
            //    isRandom = true,
            //    REP_Times = 6,
            //    exe_CMD = 0xF22F,
            //    SongID = 322,
            //    songsName = "Children of the Night.mp3"
            //});
            //lc.Compile(ReadBIN().Commands, "out.lpx");
            //bool iss=false;
            //LunalipxDecomp ld = new LunalipxDecomp();
            //LunalipxDecompiler lde = new LunalipxDecompiler("out.lpx", ref iss);
            //lde.DecompileLPX(ref ld);
            //foreach(LUNALIPS_Expression lxe in ld.commands)
            //{
            //    Console.WriteLine("=========");
            //    Console.WriteLine("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}", lxe.hasREP, lxe.isRandom, lxe.isShutdownREQ, lxe.REP_Times, lxe.exe_CMD, lxe.SongID, lxe.songsName);
            //}
            //InportFromLpsRes();
            //ExportToLpsRes();
            //ImportAtAsync(0);
            //Console.WriteLine(System.Globalization.CultureInfo.CurrentUICulture.Name);
            //Console.WriteLine(new TimeSpan(23443212).ToString(@"mm\:ss\.fff"));
            //Console.WriteLine("{0},{1}", Marshal.SizeOf(typeof(dBlock)) - 1024, sizeof(int));
            Console.WriteLine("done");
#endregion
            
            Console.ReadKey();
        }*/

        static void Main(string[] args)
        {
            string l1 = enc("{\"s\":\"Daniel Ingram\",\"type\":\"1\",\"offset\":\"0\",\"total\":\"true\",\"limit\":\"30\",\"csrf_token\":\"\"}", "0CoJUm6Qyw8W8jud");
            Console.WriteLine();
            string rstr = "9QHGy1GtOmUBkN1c";
            LNetC lnc = new LNetC();
            lnc.OnHttpResponded += (x, y) =>
            {
                Console.WriteLine(x+"|"+y.ResultData);
            };
            lnc.OnHttpTimeOut += () =>
            {
                Console.WriteLine("err");
            };
            lnc.OnAllQueueRequestCompletely += () => Console.WriteLine("OK");
            RBody r = new RBody()
            {
                URL = "http://music.163.com/weapi/cloudsearch/get/web?csrf_token=",
                RequestMethod = HttpMethod.POST
            };

            r.AddParameter("params", System.Web.HttpUtility.UrlEncode(enc(l1, rstr), Encoding.UTF8));
            r.AddParameter("encSecKey", "d123fac38adb3e3e326b672b73a865584f40dbb515c33c4b12d7092bf0a22695820742ed91a47b129bc4a7b62ee2166cb0a1b963c834a9807f4addd2bc9556e2c71f1fc90a9a0d158459494c36ca1887f2c6aa868d70e01c6c00ecfe14aac966a854c43730fe70ceec15e37211bd3c21940dec2b62d77f9cab5856d96fdc5e9a");
            lnc.AddRequestBody(r, "e1");
            //Console.WriteLine(rsa("00e0b509f6259df8642dbc35662901477df22677ec152b5ff68ace615bb7b725152b3ab17a876aea8a5aa76d2e417629ec4ee341f56135fccf695280104e0312ecbda92557c93870114af6c9d05c4f7f0c3685b7a46bee255932575cce10b424d813cfe4875d3e82047b97ddef52741d546b8e289dc6935b3ece0462db0a22b8e7",
            //    "010001",
            //    "9QHGy1GtOmUBkN1c"));
            Console.WriteLine();
            lnc.StartRequestAsyn().Wait();
            Console.ReadKey();
        }

        static string RandomString(int len)
        {
            string res = "";
            Random r = new Random();
            for (int i = 0; i < len; i++)
            {
                res += c[r.Next(c.Length)];
            }
            return res;
        }

        static string enc(string src, string key)
        {
            byte[] data = Encoding.UTF8.GetBytes(src);
            byte[] skey = Encoding.UTF8.GetBytes(key);
            RijndaelManaged rm = new RijndaelManaged();
            rm.Key = skey;
            rm.IV = Encoding.UTF8.GetBytes("0102030405060708");
            rm.Mode = CipherMode.CBC;
            rm.Padding = PaddingMode.PKCS7;
            ICryptoTransform ct = rm.CreateEncryptor();
            byte[] edata = ct.TransformFinalBlock(data, 0, data.Length);
            string s = Convert.ToBase64String(edata);
            return s;

        }

        public static string rsa(string key,string expo,string content)
        {
            string publickey = @"<RSAKeyValue><Modulus>"+Convert.ToBase64String(Encoding.UTF8.GetBytes(key))+ "</Modulus><Exponent>"+expo+"</Exponent></RSAKeyValue>";
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            
            byte[] cipherbytes;
            rsa.FromXmlString(publickey);
            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);
            return Convert.ToBase64String(cipherbytes);
        }

#if DEBUG
        private static BINARY ReadBIN()
        {
            //try
            //{
                BINARY b;
                using (FileStream fs = new FileStream("prg1._lpx", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    b = bf.Deserialize(fs) as BINARY;
                }
                return b;
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.StackTrace);
            //    return null;
            //}
        }

        static async void ExportToLpsRes()
        {
            ResourceCollection rob = new ResourceCollection(0x4455, "ABXSCSVWECDSMM");
            LResOutput lro = new LResOutput();
            long t = 0;
            EventHoster.onStatusUpdate += (x,y) =>
            {
                switch(x)
                {
                    case EvtType.CURRENT_BYTE:
                        Console.WriteLine("Complete {0}%", Math.Round((double)y / (double)t * 100d).ToString());
                        break;
                    case EvtType.TOTAL_BYTE:
                        t = y;
                        break;
                }
            };
            rob.addResource(@"F:\Daniel Ingram - A True, True Friend.mp3",null);
            rob.addResourceBash(@"D:\NewMediaPlayer\res");
            rob.addResource(@"C:\Users\Lunaixsky\Downloads\AcrobatXIPro.zip", null);
            lro.ActiveTSpan = 5;
            t = rob.EstLrssLength;
            Console.WriteLine(rob.getResourceI(0));
            await lro.GenerateAsync("R_l_x.lrss", rob);
        }

        private static async void ImportAtAsync(int v)
        {
            LResInput lri = new LResInput();
            EventHoster.onErrOcurr += (x, y) =>
            {
                Console.WriteLine("Error occurs [Type = {0} , Message = {1}]", x, y[0]);
                
            };
            ResourceCollection ro = await lri.LoadResourceReadOnlyAsync(@"D:\NewMediaPlayer\NewMediaPlayer\bin\Debug\Resources\R_lps00.lrss");
            if (ro == null) return;
            IEnumerator<Resource> rt = ro.getResourceEAll("(.*)");
            while(rt.MoveNext())
            {
                Console.WriteLine("{0}\n================\n", rt.Current);
            }
            //using (var f = File.Create(String.Format("{0}.{1}", r.fname, r.fType)))
            //{
            //    f.Write(r.rData, 0, r.rData.Length);
            //}
        }

        static void InportFromLpsRes()
        {
            LResInput lri = new LResInput();
            EventHoster.onErrOcurr += (x,y) =>
            {
                Console.WriteLine("An Exception occurs , message {0}", y == null ? "" : y[0]);
            };

            ResourceCollection rob = lri.LoadResourceReadOnly("R_lps.lrss");
            foreach(long l in rob.Header.size)
            {
                Console.WriteLine("  {0}B", l);
            }
            IEnumerator<StreamRes> iers = rob.getResourceSE("g$");
            while (iers.MoveNext())
            {
                Console.WriteLine(iers.Current.fname);
                //using (var f = File.Create(iers.Current.fname))
                //{
                //    iers.Current.rData.Seek(0, SeekOrigin.Begin);
                //    iers.Current.rData.CopyTo(f);
                //}
            }
        }
#endif
    }

    [Serializable]
    class BINARY
    {
        ArrayList _cmds;
        public BINARY(ArrayList _c)
        {
            _cmds = _c;
        }
        public ArrayList Commands
        {
            get
            {
                return _cmds;
            }
        }
    }
}
