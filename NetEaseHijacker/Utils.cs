using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NetEaseHijacker
{
    static class Utils
    {

        public static string GetEncodedParams(string src)
        {
            return HttpUtility.UrlEncode(
                                enc(
                                    enc(src, NeParams.PrimaryKey), 
                                    NeParams.RandomString
                                ), Encoding.UTF8);
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
            return Convert.ToBase64String(edata);

        }

        public static string FormatE(this string p, params object[] s)
        {
            return string.Format(p, s);
        }
    }
}
