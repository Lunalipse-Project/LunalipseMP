using LpxResource.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LpxResource.LRTypes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct rHeader
    {
        public int magic;
        public int file;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string sig;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public long[] size;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct sNotation
    {
        public int index;
        public long flen;
        public int bcount;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string name;
        [MarshalAs(UnmanagedType.ByValTStr,SizeConst = 8)]
        public string type;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct dBlock
    {
        public int index;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public byte[] bdat;
    }

    public class Resource
    {
        public string fType;
        public string fname;
        public byte[] rData;

        public override string ToString()
        {
            return "File name: {0}\nFile Type: {1}\nFile Size: {2}"
                    .FormateEx(fname, fType, rData.LongLength.ToStroage());
        }
    }

    public class StreamRes
    {
        public string fname;
        public string ftype;
        public Stream rData;
        public override string ToString()
        {
            return "File name: {0}\nFile Type: {1}\nFile Size: {2}"
                    .FormateEx(fname, ftype, rData.Length.ToStroage());
        }
    }
}
