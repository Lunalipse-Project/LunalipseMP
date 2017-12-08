using System;
using System.Collections.Generic;

namespace FileSecurity
{
    [Serializable]
    public class MD5Export
    {
        IDictionary<string, byte[]> F_MD5;
        public MD5Export()
        {
            F_MD5 = new Dictionary<string, byte[]>();
        }
        public bool AddMD5(string file,byte[] md5)
        {
            if (F_MD5.ContainsKey("file")) return false;
            F_MD5.Add(file, md5);
            return true;
        }
        public byte[] GetMD5(string key)
        {
            return F_MD5[key];
        }
        public IDictionary<string, byte[]> MD5Dic
        {
            get
            {
                return F_MD5;
            }
        }
    }
}
