using LpxResource.Generic;
using LpxResource.LRTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LpxResource
{
    public partial class LResInput
    {
        public async Task<ResourceCollection> LoadResourceReadOnlyAsync(string path)
        {
            FileStream fs;
            ResourceCollection ro = new ResourceCollection();
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            }
            catch (Exception e)
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
            await fs.ReadAsync(tread, 0, hs);
            EventHoster.IStatusUpdate(EvtType.TOTAL_BYTE, fs.Length);
            long tb = hs;
            ro.Header = (rHeader)Utils.b2s(tread, typeof(rHeader));
            for (int i = 0; i < ro.FileCount; i++)
            {
                await fs.ReadAsync(tread, 0, ns);
                sNotation sn = (sNotation)Utils.b2s(tread, typeof(sNotation));
                tb += this.sn;
                byte[] t = await __rResAsync(sn, fs, tb);

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

        public async Task<Resource> ReadResourceAsyncAt(int res_index)
        {
            if (fs_G == null) return null;
            if (res_index > rh.file - 1) return null;
            //Calculate offset in stream
            fs_G.Seek(Utils.CalcStreamOffset(rh, res_index), SeekOrigin.Begin);

            long tb = sn;
            byte[] b = new byte[1028];
            await fs_G.ReadAsync(b, 0, sn);
            sNotation sN = (sNotation)Utils.b2s(b, typeof(sNotation));
            EventHoster.IStatusUpdate(EvtType.TOTAL_BYTE, sN.bcount * 1024 + sn);
            byte[] t = await __rResAsync(sN, fs_G, tb);

            //Reset stream pointer
            fs_G.Seek(0, SeekOrigin.Begin);
            return new Resource()
            {
                fname = sN.name,
                fType = sN.type,
                rData = t
            };
        }

        public async Task<sNotation[]> AllNotationsAsync(rHeader h)
        {
            if (fs_G == null) return null;
            long baseOffset = hs;
            List<sNotation> snL = new List<sNotation>();
            fs_G.Seek(baseOffset, SeekOrigin.Begin);
            for (int i = 0; i < h.file; i++)
            {
                byte[] b = new byte[sn];
                await fs_G.ReadAsync(b, 0, sn);
                sNotation sN = (sNotation)Utils.b2s(b, typeof(sNotation));
                baseOffset += sn + sN.bcount * db;
                snL.Add(sN);
                fs_G.Seek(baseOffset, SeekOrigin.Begin);
            }
            fs_G.Seek(0, SeekOrigin.Begin);
            return snL.ToArray();
        }
    }
}
