using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HashValidator
{
    public class FEntrance
    {
        MD5 md5;
        MD5Export md5e;
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr memcmp(byte[] b1, byte[] b2, IntPtr count);
        public FEntrance()
        {
            md5 = new MD5CryptoServiceProvider();
            md5e = new MD5Export();
        }

        public void Calculate(string fileExtendsion, string path)
        {
            string[] f = Directory.GetFiles(path);
            foreach (string s in f)
            {
                if (Path.GetExtension(s).Equals(fileExtendsion))
                {
                    using (FileStream fs = new FileStream(s, FileMode.Open))
                    {
                        md5e.AddMD5(Path.GetFileName(s), md5.ComputeHash(fs));
                    }
                }
            }
        }

        public bool Export(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, md5e);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public MD5Export getExport(string pathOfMD5F)
        {
            try
            {
                using (FileStream fs = new FileStream(pathOfMD5F, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    return bf.Deserialize(fs) as MD5Export;
                }
            }
            catch
            {
                return null;
            }
        }

        public STATUS ValidateMD5(string pathOfMD5F, string p)
        {
            try
            {
                using (FileStream fs = new FileStream(pathOfMD5F, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    try
                    {
                        md5e = bf.Deserialize(fs) as MD5Export;
                    }
                    catch (SerializationException)
                    {
                        return new STATUS() { Success = false, scode = 1 };
                    }
                    catch (SecurityException)
                    {
                        return new STATUS() { Success = false, scode = 1 };
                    }
                }
                foreach (var o in md5e.MD5Dic)
                {
                    if (!File.Exists(p + o.Key)) return new STATUS() { Success = false, scode = 2 }; ;
                    using (FileStream fs = new FileStream(p + o.Key, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        if (MemoryCompare(md5.ComputeHash(fs), o.Value) != 0)
                        {
#if DEBUG
                            Console.WriteLine("{0,-100}{1}", p + o.Key, "Failure");
#endif
                            return new STATUS() { Success = false, scode = 2 }; ;
                        }
                    }
                }
                return new STATUS() { Success = true, scode = 0 };
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e.StackTrace);
#endif
                return new STATUS() { Success = false, scode = 2 };
            }
        }

        static int MemoryCompare(byte[] b1, byte[] b2)
        {
            IntPtr retval = memcmp(b1, b2, new IntPtr(b1.Length));
            return retval.ToInt32();
        }
    }
    public class STATUS
    {
        public bool Success;
        public int scode = 0;
    }
}
