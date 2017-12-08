using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace gMusic.util
{
    public class enMusicId
    {
        public static string EncryptMusicId(string id)
        {
            byte[] b = Encoding.ASCII.GetBytes("3go8&$8*3*3h0k(2)2");
            byte[] b2 = Encoding.ASCII.GetBytes(id);
            for (int i = 0; i < b2.Length; i++)
            {
                b2[i] = (byte)(b2[i] ^ b[i % b.Length]);
            }
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] x = md5.ComputeHash(b2);
            string y = Convert.ToBase64String(x);
            y.Substring(0, y.Length - 1);
            y = y.Replace('/', '_');
            y = y.Replace('+', '-');
            return y;
        }
    }
}
