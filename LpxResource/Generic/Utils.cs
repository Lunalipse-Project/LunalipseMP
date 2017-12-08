using LpxResource.LRTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LpxResource.Generic
{
    public static class Utils
    {
        static string[] suffixs = new string[] {
            "B",
            "KB",
            "MB",
            "GB",
            "TB"
        };

        /// <summary>
        /// 按照给定参数格式化字符串
        /// </summary>
        /// <param name="target">被格式化的字符串</param>
        /// <param name="s">代入参数</param>
        /// <returns></returns>
        public static string FormateEx(this string target, params object[] s)
        {
            return string.Format(target, s);
        }

        /// <summary>
        /// 判断字符串是否有效（不为null或者空字符）
        /// </summary>
        /// <param name="target">目标字符串</param>
        /// <returns></returns>
        public static bool AvailableEx(this string target)
        {
            return !(String.IsNullOrEmpty(target) || String.IsNullOrWhiteSpace(target));
        }

        /// <summary>
        /// 判断字符串所表示的文件/目录路径是否有效
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ft">判断类型（文件还是目录）</param>
        /// <returns></returns>
        public static bool DExist(this string path, FType ft)
        {
            if (ft == FType.FILE)
                return File.Exists(path);
            else
                return Directory.Exists(path);
        }

        /// <summary>
        /// 获取字符串所表示的文件后缀名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string getRType(this string path)
        {
            return Path.GetExtension(path).Replace(".","");
        }

        /// <summary>
        /// 获取字符串所表示的文件名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string getFName(this string target)
        {
            return Path.GetFileNameWithoutExtension(target);
        }

        /// <summary>
        /// 将长整型作为字节数换算为其他等价的数据容量单位
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string ToStroage(this long target)
        {
            double b = target;
            int i = 1;
            if(target<1024)
                return "{0}{1}".FormateEx(b, suffixs[0]);
            while ((b /= 1024) >= 1024 * i)
            {
                i++;
            }
            return i > suffixs.Length - 1 ? "NAN" : "{0:N2}{1}".FormateEx(b, suffixs[i]);
        }

        /// <summary>
        /// 字节数组转化为数据结构体
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="type">数据结构类型</param>
        /// <returns></returns>
        public static object b2s(byte[] bytes, Type type)
        {
            int size = Marshal.SizeOf(type);
            if (size > bytes.Length)
            {
                return null;
            }
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, structPtr, size);
            object obj = Marshal.PtrToStructure(structPtr, type);
            Marshal.FreeHGlobal(structPtr);
            return obj;
        }

        /// <summary>
        /// 数据结构体转化为字节数组
        /// </summary>
        /// <param name="size">结构体长度</param>
        /// <param name="structObj">数据结构类型</param>
        /// <returns></returns>
        public static byte[] s2b(object structObj, int size)
        {
            byte[] bytes = new byte[size];
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structObj, structPtr, false);
            Marshal.Copy(structPtr, bytes, 0, size);
            Marshal.FreeHGlobal(structPtr);
            return bytes;

        }

        /// <summary>
        /// 数据流转化为字节数组
        /// </summary>
        /// <param name="s">数据流</param>
        /// <returns></returns>
        public static byte[] st2b(Stream s)
        {
            byte[] b = new byte[s.Length];
            s.Seek(0, SeekOrigin.Begin);
            s.Read(b, 0, b.Length);
            s.Seek(0, SeekOrigin.Begin);
            return b;
        }

        /// <summary>
        /// (异步) 数据流转化为字节数组
        /// </summary>
        /// <param name="s">数据流</param>
        /// <returns></returns>
        public static async Task<byte[]> st2bAsync(Stream s)
        {
            byte[] b = new byte[s.Length];
            s.Seek(0, SeekOrigin.Begin);
            await s.ReadAsync(b, 0, b.Length);
            s.Seek(0, SeekOrigin.Begin);
            return b;
        }

        /// <summary>
        /// 根据给定的LRSS标头和内部封装资源索引计算出该资源的流内偏移地址
        /// </summary>
        /// <param name="rh">LRSS标头</param>
        /// <param name="inx">内部封装资源索引</param>
        /// <returns></returns>
        public static long CalcStreamOffset(rHeader rh,int inx)
        {
            int sn = Marshal.SizeOf(typeof(sNotation));
            int db = Marshal.SizeOf(typeof(dBlock));
            long offsetStream = Marshal.SizeOf(typeof(rHeader));
            for (int i = 0; i < inx; i++)
            {
                if (rh.size[i] == 0) continue;
                offsetStream += sn;
                int btaken = (int)Math.Ceiling(rh.size[i] / 1024d);
                offsetStream += db * btaken;
            }
            return offsetStream;
        }

        public static bool getResourceByPath(string path,ref Resource res)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    byte[] b = new byte[fs.Length];
                    fs.Read(b, 0, b.Length);
                    res.fname = Path.GetFileNameWithoutExtension(path);
                    res.fType = Path.GetExtension(path).Replace(".", "");
                    res.rData = b;
                }
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public static async Task<Resource> getResourceByPathAsync(string path)
        {
            try
            {
                Resource res = new Resource();
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    byte[] b = new byte[fs.Length];
                    await fs.ReadAsync(b, 0, b.Length);
                    res.fname = Path.GetFileNameWithoutExtension(path);
                    res.fType = Path.GetExtension(path).Replace(".", "");
                    res.rData = b;
                }
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }

    public enum FType
    {
        FILE,
        DICT
    }
}
