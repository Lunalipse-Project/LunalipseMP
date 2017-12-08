using LpxResource.Generic;
using LpxResource.LRTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LpxResource
{
    
    public partial class LResInput
    {
        

        FileStream fs_G;
        int hs = 0, sn = 0, db = 0;
        rHeader rh;
        private int upr = 0;

        public LResInput()
        {
            hs = Marshal.SizeOf(typeof(rHeader));
            sn = Marshal.SizeOf(typeof(sNotation));
            db = Marshal.SizeOf(typeof(dBlock));
        }

        public ResourceCollection LoadResourceReadOnly(string path)
        {
            FileStream fs;
            ResourceCollection ro = new ResourceCollection();
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            }
            catch(Exception e)
            {
                EventHoster.IErrOcurr(EErrors.OPEN_FILE, e.Message);
                return null;
            }
            int hs = Marshal.SizeOf(typeof(rHeader));
            int ns = Marshal.SizeOf(typeof(sNotation));
            int bs = Marshal.SizeOf(typeof(dBlock));
            byte[] tread = new byte[4096];
            //Reset curosr
            fs.Seek(0, SeekOrigin.Begin);
            fs.Read(tread, 0, hs);
            ro.Header = (rHeader)Utils.b2s(tread, typeof(rHeader));

            long tb = hs;
            EventHoster.IStatusUpdate(EvtType.TOTAL_BYTE, fs.Length);

            for (int i=0;i< ro.FileCount;i++)
            {
                fs.Read(tread, 0, ns);
                sNotation sn = (sNotation)Utils.b2s(tread, typeof(sNotation));
                tb += this.sn;
                byte[] t = __rRes(sn, fs, tb);

                Resource r = new Resource()
                {
                    fType = sn.type,
                    fname = sn.name
                };
                r.rData = new byte[sn.flen];
                Array.Copy(t, 0, r.rData, 0, sn.flen);
                ro.AddResource(r);
            }
            fs.Dispose();
            return ro;
        }

        public rHeader LoadResource(string path)
        {
            Dispose();
            try
            {
                fs_G = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            }
            catch (Exception e)
            {
                EventHoster.IErrOcurr(EErrors.OPEN_FILE, e.Message);
                return new rHeader();
            }

            byte[] b = new byte[hs];
            fs_G.Read(b, 0, hs);
            return rh = (rHeader)Utils.b2s(b, typeof(rHeader));
        }

        public sNotation[] AllNotations(rHeader h)
        {
            if (fs_G == null) return null;
            long baseOffset = hs;
            List<sNotation> snL = new List<sNotation>();
            fs_G.Seek(baseOffset, SeekOrigin.Begin);
            for(int i =0;i<h.file;i++)
            {
                byte[] b = new byte[sn];
                fs_G.Read(b, 0, sn);
                sNotation sN = (sNotation)Utils.b2s(b, typeof(sNotation));
                baseOffset += sn + sN.bcount * 1024;
                snL.Add(sN);
                fs_G.Seek(baseOffset, SeekOrigin.Begin);
            }
            fs_G.Seek(0, SeekOrigin.Begin);
            return snL.ToArray();
        }

        public Resource ReadResourceAt(int res_index)
        {
            if (fs_G == null) return null;
            if (res_index > rh.file - 1) return null;

            //Calculate offset in stream
            fs_G.Seek(Utils.CalcStreamOffset(rh, res_index), SeekOrigin.Begin);
            byte[] b = new byte[1028];
            long tb = 0;

            fs_G.Read(b, 0, sn);

            sNotation sN = (sNotation)Utils.b2s(b, typeof(sNotation));
            EventHoster.IStatusUpdate(EvtType.TOTAL_BYTE, sN.bcount * 1024 + this.sn);
            byte[] t = __rRes(sN, fs_G, tb);

            return new Resource()
            {
                fname = sN.name,
                fType = sN.type,
                rData = t
            };
        }

        

        #region Method Extract
        async Task<byte[]> __rResAsync(sNotation sN, Stream st, long totalB)
        {
            IntPtr p_mem;
            byte[] b = new byte[1028];
            try
            {
                p_mem = Marshal.AllocHGlobal(new IntPtr(sN.bcount * 1024));
            }
            catch (OutOfMemoryException oom) { EventHoster.IErrOcurr(EErrors.OUT_OF_MEM, oom.Message); return null; }
            long tspan = getTimeSp(sN.bcount);
            //Store the initial value
            IntPtr p_org = p_mem;
            for (int i = 0; i < sN.bcount; i++)
            {
                await st.ReadAsync(b, 0, db);
                dBlock dB = (dBlock)Utils.b2s(b, typeof(dBlock));
                if (dB.index != sN.index)
                {
                    EventHoster.IErrOcurr(EErrors.PARSE_BLOCK, "Chunk error at 0x" + st.Position.ToString("x16"));
                    return null;
                }
                totalB += b.Length;
                if (i % tspan == 0)
                {
                    EventHoster.IStatusUpdate(EvtType.CURRENT_BYTE, totalB);
                }
                Marshal.Copy(dB.bdat, 0, p_mem + 1024 * i, 1024);
            }
            EventHoster.IStatusUpdate(EvtType.CURRENT_BYTE, totalB);
            EventHoster.IAllDBlockRead(sN.bcount * 1024, p_org.ToInt64());
            byte[] tmp = new byte[sN.flen];
            Marshal.Copy(p_org, tmp, 0, tmp.Length);
            Marshal.FreeHGlobal(p_org);
            return tmp;
        }

        byte[] __rRes(sNotation sN,Stream st, long totalB)
        {
            IntPtr p_mem;
            byte[] b = new byte[1028];
            try
            {
                p_mem = Marshal.AllocHGlobal(new IntPtr(sN.bcount * 1024));
            }
            catch (OutOfMemoryException oom) { EventHoster.IErrOcurr(EErrors.OUT_OF_MEM, oom.Message); return null; }
            long tspan = getTimeSp(sN.bcount);
            //Store the initial value
            IntPtr p_org = p_mem;
            for (int i = 0; i < sN.bcount; i++)
            {
                st.Read(b, 0, db);
                dBlock dB = (dBlock)Utils.b2s(b, typeof(dBlock));
                if (dB.index != sN.index)
                {
                    EventHoster.IErrOcurr(EErrors.PARSE_BLOCK, "Chunk error at 0x" + st.Position.ToString("x16"));
                    return null;
                }
                totalB += b.Length;
                if (i % tspan == 0)
                {
                    EventHoster.IStatusUpdate(EvtType.CURRENT_BYTE, totalB);
                }
                Marshal.Copy(dB.bdat, 0, p_mem + 1024 * i, 1024);
            }
            EventHoster.IStatusUpdate(EvtType.CURRENT_BYTE, totalB);
            EventHoster.IAllDBlockRead(sN.bcount * 1024, p_org.ToInt64());
            byte[] tmp = new byte[sN.flen];
            Marshal.Copy(p_org, tmp, 0, tmp.Length);
            Marshal.FreeHGlobal(p_org);
            return tmp;
        }


        #endregion

        public int FCount
        {
            get
            {
                if (fs_G == null) throw new NullReferenceException();
                return rh.file;
            }
        }

        public void Dispose()
        {
            if (fs_G != null)
            {
                fs_G.Dispose();
            }
        }

        /// <summary>
        /// 获取或设置进度报告间隔因子(>=0)
        /// </summary>
        public int ActiveTSpan
        {
            get
            {
                return upr;
            }
            set
            {
                if (value < 0) upr = 2;
                upr = value;
            }
        }

        public FileStream LrssStream
        {
            get
            {
                return fs_G;
            }
        }

        long getTimeSp(long l)
        {
            double d = l;
            int ur = upr;
            while (--ur > 0 && l >= 2)
            {
                d /= 2;
            }
            return (long)Math.Ceiling(d);
        }
    }
}
