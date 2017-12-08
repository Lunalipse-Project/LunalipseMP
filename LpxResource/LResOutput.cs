using LpxResource.Generic;
using LpxResource.LRTypes;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Runtime.InteropServices;

namespace LpxResource
{
    
    public class LResOutput
    {
        
        

        int hs = 0, ns = 0, ds = 0, upr = 2;

        public LResOutput()
        {
            hs = Marshal.SizeOf(typeof(rHeader));
            ns = Marshal.SizeOf(typeof(sNotation));
            ds = Marshal.SizeOf(typeof(dBlock));
        }
        
        /// <summary>
        /// 封装资源至LRSS文件
        /// </summary>
        /// <param name="expath">LRSS导出路径</param>
        /// <param name="ro">资源文件集合</param>
        /// <returns></returns>
        public int Generate(string expath,ResourceCollection ro)
        {
            ro.FileCount = ro.Size;
            try
            {
                using (FileStream fs = new FileStream(expath, FileMode.Create))
                {
                    fs.Write(Utils.s2b(ro.Header, Marshal.SizeOf(ro.Header)), 0, Marshal.SizeOf(ro.Header));
                    long partial = 0;
                    for (int i = 0; i < ro.FileCount; i++)
                    {
                        Resource r = ro.getResourceI(i);
                        __lrssO(r, fs, i,ref partial);
                        EventHoster.IStatusUpdate(EvtType.CURRENT_BYTE, partial);
                    }
                    fs.Flush();
                }
                GC.Collect();
                return ro.FileCount;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return -1;
            }
        }

        /// <summary>
        /// (异步) 封装资源至LRSS文件
        /// </summary>
        /// <param name="expath">LRSS导出路径</param>
        /// <param name="resources">资源文件路径</param>
        /// <param name="_rh">LRSS标头</param>
        /// <returns></returns>
        public async Task<int> GenerateAsync(string expath,rHeader _rh, params string[] resources)
        {
            if (resources == null) throw new ArgumentNullException();
            int total = resources.Length;
            long _tb = 0;
            long totalByte = 0;
            rHeader rh = _rh;
            rh.size = new long[32];
            for (int i = 0; i < resources.Length; i++)
            {
                if (!resources[i].DExist(FType.FILE)) throw new FileNotFoundException();
                if (i > 31) throw new ArgumentOutOfRangeException();
                _tb += rh.size[i] = new FileInfo(resources[i]).Length;
            }
            EventHoster.IStatusUpdate(EvtType.TOTAL_BYTE, _tb += hs + ns * total);
            using (FileStream fs = new FileStream(expath, FileMode.Create))
            {
                rh.file = total;
                await fs.WriteAsync(Utils.s2b(rh, hs), 0, hs);
                int i = 0;
                foreach (string s in resources)
                {
                    byte[] FileCtx;
                    using (FileStream _fs = new FileStream(s, FileMode.Open))
                    {
                        FileCtx = await Utils.st2bAsync(_fs);
                    }
                    Resource r = new Resource()
                    {
                        fname = s.getFName(),
                        fType = s.getRType(),
                        rData = FileCtx
                    };
                    totalByte += await __lrssO_Async(r, fs, i, totalByte);
                    i++;
                    EventHoster.IStatusUpdate(EvtType.CURRENT_BYTE, totalByte);
                }
                fs.Flush();
            }
            return total;
        }

        /// <summary>
        /// (异步) 封装资源至LRSS文件
        /// </summary>
        /// <param name="expath">LRSS导出路径</param>
        /// <param name="ro">资源文件集合</param>
        /// <returns></returns>
        public async Task<int> GenerateAsync(string expath, ResourceCollection ro)
        {
            ro.FileCount = ro.Size;
            try
            {
                using (FileStream fs = new FileStream(expath, FileMode.Create))
                {
                    long totalb = hs;
                    await fs.WriteAsync(Utils.s2b(ro.Header, hs), 0, hs);
                    for (int i = 0; i < ro.FileCount; i++)
                    {
                        Resource r = ro.getResourceI(i);
                        totalb += await __lrssO_Async(r, fs, i, totalb);
                        EventHoster.IStatusUpdate(EvtType.CURRENT_BYTE, totalb);
                    }
                    fs.Flush();
                }
                GC.Collect();
                return ro.FileCount;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return -1;
            }
        }

        public bool AppendHeader(rHeader rh, Stream s)
        {
            try
            {
                s.Seek(0, SeekOrigin.Begin);
                byte[] b = Utils.s2b(rh, hs);
                s.Write(b, 0, hs);
                s.Seek(0, SeekOrigin.Begin);
                return true;
            }
            catch(Exception e)
            {
                EventHoster.IErrOcurr(EErrors.GENERAL, e.Message);
                return false;
            }
        }

        public async Task<bool> AppendHeaderAsync(rHeader rh, Stream s)
        {
            try
            {
                s.Seek(0, SeekOrigin.Begin);
                byte[] b = Utils.s2b(rh, hs);
                await s.WriteAsync(b, 0, hs);
                s.Seek(0, SeekOrigin.Begin);
                return true;
            }
            catch (Exception e)
            {
                EventHoster.IErrOcurr(EErrors.GENERAL, e.Message);
                return false;
            }
        }

        public bool AppendResource(Resource r, Stream s, int inx)
        {
            try
            {
                long tb = (long)(Math.Ceiling(r.rData.Length / 1024d) * ds) + ns;
                EventHoster.IStatusUpdate(EvtType.TOTAL_BYTE, tb);
                long d = ns;
                s.Seek(0, SeekOrigin.End);
                __lrssO(r, s, inx, ref d);
                EventHoster.IStatusUpdate(EvtType.CURRENT_BYTE, d);
                s.Seek(0, SeekOrigin.Begin);
                return true;
            }
            catch(Exception e)
            {
                EventHoster.IErrOcurr(EErrors.GENERAL, e.Message);
                return false;
            }
        }

        public async Task<bool> AppendResourceAsync(Resource r, Stream s, int inx)
        {
            try
            {
                long tb = (long)(Math.Ceiling(r.rData.Length / 1024d) * ds) + ns;
                EventHoster.IStatusUpdate(EvtType.TOTAL_BYTE, tb);
                long d = ns;
                s.Seek(0, SeekOrigin.End);
                d = await __lrssO_Async(r, s, inx, d);
                EventHoster.IStatusUpdate(EvtType.CURRENT_BYTE, d);
                s.Seek(0, SeekOrigin.Begin);
                return true;
            }
            catch (Exception e)
            {
                EventHoster.IErrOcurr(EErrors.GENERAL, e.Message);
                return false;
            }
        }


        private void __lrssO(Resource r,Stream fst,int curIndex,ref long curByte)
        {
            sNotation sn = new sNotation();
            sn.index = curIndex;
            sn.type = r.fType;
            sn.name = r.fname;
            sn.flen = r.rData.Length;
            sn.bcount = (int)Math.Ceiling(sn.flen / 1024d);

            long tspan = getTimeSp(sn.bcount);

            long left = sn.flen % 1024L;
            curByte += ns;
            fst.Write(Utils.s2b(sn, ns), 0, ns);
            for (int j = 0; j < sn.bcount; j++)
            {
                dBlock db = new dBlock();
                db.index = curIndex;
                db.bdat = new byte[1024];
                if (j == sn.bcount - 1 && left > 0)
                {
                    Array.Copy(r.rData, 1024 * j, db.bdat, 0, left);
                    curByte += left;
                }
                else
                {
                    Array.Copy(r.rData, 1024 * j, db.bdat, 0, 1024);
                    curByte += 1024;
                }
                if(j% tspan == 0)
                {
                    EventHoster.IStatusUpdate(EvtType.CURRENT_BYTE, curByte);
                }
                fst.Write(Utils.s2b(db, ds), 0, ds);
            }
        }

        private async Task<long> __lrssO_Async(Resource r, Stream fst, int curIndex, long curp)
        {
            sNotation sn = new sNotation();
            sn.index = curIndex;
            sn.type = r.fType;
            sn.name = r.fname;
            sn.flen = r.rData.Length;
            sn.bcount = (int)Math.Ceiling(sn.flen / 1024d);

            long tspan = getTimeSp(sn.bcount);
            long left = sn.flen % 1024L;
            long curByte = ns;
            await fst.WriteAsync(Utils.s2b(sn, ns), 0, ns);
            for (int j = 0; j < sn.bcount; j++)
            {
                dBlock db = new dBlock();
                db.index = curIndex;
                db.bdat = new byte[1024];
                if (j == sn.bcount - 1 && left > 0)
                {
                    Array.Copy(r.rData, 1024 * j, db.bdat, 0, left);
                    curByte += left;
                }
                else
                {
                    Array.Copy(r.rData, 1024 * j, db.bdat, 0, 1024);
                    curByte += 1024;
                }
                if (j % tspan == 0)
                {
                    EventHoster.IStatusUpdate(EvtType.CURRENT_BYTE, curp + curByte);
                }
                await fst.WriteAsync(Utils.s2b(db, ds), 0, ds);
            }
            return curByte;
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
