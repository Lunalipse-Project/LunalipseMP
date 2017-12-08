using LpxResource.LRTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LpxResource.Generic
{
    /// <summary>
    /// 资源文件集合
    /// </summary>
    public class ResourceCollection
    {
        List<Resource> lR;
        rHeader rhR;
        int ix = 0;

        /// <summary>
        /// 资源集合参数初始化
        /// </summary>
        /// <param name="magicNumber">写入LpsRes的幻数</param>
        /// <param name="signature">写入LpsRes的签名（最多16个ASCII字符）</param>
        /// <exception cref="ArgumentException"></exception>
        public ResourceCollection(int magicNumber, string signature)
        {
            rhR = new rHeader();
            rhR.magic = magicNumber;
            rhR.size = new long[32];
            if (signature.Length > 16) throw new ArgumentException();
            rhR.sig = signature;
            lR = new List<Resource>();
        }

        public ResourceCollection()
        {
            rhR = new rHeader();
            lR = new List<Resource>();
            rhR.size = new long[32];
        }


        /// <summary>
        /// 添加至资源
        /// </summary>
        /// <param name="res">资源字节</param>
        /// <param name="extension">最多8字符资源类型描述（推荐资源的后缀名）</param>
        /// <param name="key">资源索引</param>
        /// <exception cref="ArgumentException"></exception>
        public bool addResource(byte[] res, string extension, string key)
        {
            if (extension.Length > 8) throw new ArgumentException("length larger than 8");
            if (key.Length > 128 || !key.AvailableEx()) throw new ArgumentException("Error key");
            if (!Regist2Header(res.Length)) return false;
            lR.Add(new Resource()
            {
                fType = extension,
                fname = key,
                rData = res
            });
            return true;
        }

        /// <summary>
        /// 添加至资源
        /// </summary>
        /// <param name="res">目标资源文件的路径</param>
        /// <param name="key">资源索引（可选，设置为null或空字符即采用目标文件名称作为索引）</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public bool addResource(string res, string key)
        {
            if (!res.DExist(FType.FILE)) throw new FileNotFoundException();
            string s = res.getRType();
            string fn = !key.AvailableEx() ? res.getFName() : key;
            if (fn.Length > 128) throw new ArgumentException("length larger than 128");
            if (s.Length > 8) throw new ArgumentException("length larger than 8");
            using (FileStream fs = new FileStream(res, FileMode.Open))
            {
                if (!Regist2Header(fs.Length)) return false;
                byte[] b = new byte[fs.Length];
                fs.Read(b, 0, b.Length);
                lR.Add(new Resource()
                {
                    fType = s,
                    fname = fn,
                    rData = b
                });
                return true;
            }
        }

        /// <summary>
        /// 添加至资源
        /// </summary>
        /// <param name="res">资源数据流</param>
        /// /// <param name="extension">最多8字符资源类型描述（推荐资源的后缀名）</param>
        /// <exception cref="ArgumentException"></exception>
        public bool addResource(Stream res, string extension, string key)
        {
            if (extension.Length > 8) throw new ArgumentException("length larger than 8");
            if (key.Length > 128 || !key.AvailableEx()) throw new ArgumentException("Error key");
            if (!Regist2Header(res.Length)) return false;
            lR.Add(new Resource()
            {
                fType = extension,
                fname = key,
                rData = Utils.st2b(res)
            });
            return true;
        }

        /// <summary>
        /// 批量添加至资源
        /// <para>注意，每个LRSS文件最多只能将32个文件进行封装</para>
        /// </summary>
        /// <param name="directory">文件的父级目录</param>
        public void addResourceBash(string directory)
        {
            if (!directory.DExist(FType.DICT)) throw new FileNotFoundException();
            foreach (string res in Directory.GetFiles(directory))
            {
                string s = res.getRType();
                if (s.Length > 8) throw new ArgumentException("length larger than 8");
                using (FileStream fs = new FileStream(res, FileMode.Open))
                {
                    if (!Regist2Header(fs.Length)) break;
                    byte[] b = new byte[fs.Length];
                    fs.Read(b, 0, b.Length);
                    lR.Add(new Resource()
                    {
                        fType = s,
                        fname = res.getFName(),
                        rData = b
                    });
                }
            }
        }

        //异步执行的方法
        #region Asynchronized
        public async Task<bool> addResourceAsync(string res, string key)
        {
            if (!res.DExist(FType.FILE)) throw new FileNotFoundException();
            string s = res.getRType();
            string fn = !key.AvailableEx() ? res.getFName() : key;
            if (fn.Length > 128) throw new ArgumentException("length larger than 128");
            if (s.Length > 8) throw new ArgumentException("length larger than 8");
            using (FileStream fs = new FileStream(res, FileMode.Open))
            {
                if (!Regist2Header(fs.Length)) return false;
                byte[] b = new byte[fs.Length];
                await fs.ReadAsync(b, 0, b.Length);
                lR.Add(new Resource()
                {
                    fType = s,
                    fname = fn,
                    rData = b
                });
                return true;
            }
        }
        public async Task<bool> addResourceAsync(Stream res, string extension, string key)
        {
            if (extension.Length > 8) throw new ArgumentException("length larger than 8");
            if (key.Length > 128 || !key.AvailableEx()) throw new ArgumentException("Error key");
            if (!Regist2Header(res.Length)) return false;
            lR.Add(new Resource()
            {
                fType = extension,
                fname = key,
                rData = await Utils.st2bAsync(res)
            });
            return true;
        }

        public async Task addResourceBashAsync(string directory)
        {
            if (!directory.DExist(FType.DICT)) throw new FileNotFoundException();
            foreach (string res in Directory.GetFiles(directory))
            {
                string s = res.getRType();
                if (s.Length > 8) throw new ArgumentException("length larger than 8");
                using (FileStream fs = new FileStream(res, FileMode.Open))
                {
                    if (!Regist2Header(fs.Length)) break;
                    byte[] b = new byte[fs.Length];
                    await fs.ReadAsync(b, 0, b.Length);
                    lR.Add(new Resource()
                    {
                        fType = s,
                        fname = res.getFName(),
                        rData = b
                    });
                }
            }
        }

        #endregion

        //封装的方法
        #region Encapsulation Methods
        public void AddResource(Resource r)
        {
            lR.Add(r);
        }

        public void RemoveResource(Resource i)
        {
            lR.Remove(i);
        }

        public void RemoveResourceAt(int i)
        {
            lR.RemoveAt(i);
        }

        public void ClearAll()
        {
            lR.Clear();
            GC.Collect();
        }

        public int Size
        {
            get
            {
                return lR.Count;
            }
        }

        /// <summary>
        /// 获取或设置Lps资源封装文件的魔数（幻数）
        /// </summary>
        public int Magic
        {
            get
            {
                return rhR.magic;
            }
            set
            {
                rhR.magic = value;
            }
        }

        /// <summary>
        /// 获取或设置Lps资源封装文件的签名
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public string Signature
        {
            get
            {
                return rhR.sig;
            }
            set
            {
                if (value.Length > 16) throw new ArgumentException();
                rhR.sig = value;
            }
        }

        /// <summary>
        /// 获取或设置封装文件个数
        /// </summary>
        public int FileCount
        {
            get
            {
                return rhR.file;
            }
            set
            {
                rhR.file = value;
            }
        }

        /// <summary>
        /// 获取或设置Lps资源封装文件头
        /// </summary>
        public rHeader Header
        {
            get
            {
                return rhR;
            }
            set
            {
                rhR = value;
            }
        }
        #endregion

        //拓展方法
        #region Extended Methods
        public Resource getResourceI(int i)
        {
            if (lR.Count == 0) return null;
            return lR[i];
        }

        public IEnumerator<Resource> getResourceEAll(string pattern)
        {
            if (lR.Count == 0) yield break;
            foreach (Resource r in lR.FindAll(r => Regex.IsMatch(r.fType, pattern)))
            {
                yield return r;
            }
        }

        public Stream getResourceStream(int i)
        {
            if (lR.Count == 0) return null;
            return new MemoryStream(lR[i].rData);
        }

        public byte[] getResourceK(string key)
        {
            if (lR.Count == 0 || !key.AvailableEx()) return null;
            Resource res = lR.Find(r => r.fname.Equals(key));
            if (res == null) return null;
            return res.rData;
        }
        public byte[] getResourceE(string key)
        {
            if (lR.Count == 0 || !key.AvailableEx()) return null;
            Resource res = lR.Find(r => r.fType.Equals(key));
            if (res == null) return null;
            return res.rData;
        }

        public Stream getResourceSN(string key)
        {
            if (lR.Count == 0 || !key.AvailableEx()) return null;
            Resource res = lR.Find(r => r.fname.Equals(key));
            if (res == null) return null;
            return new MemoryStream(res.rData);
        }
        /// <summary>
        /// 根据给定的类型进行全局匹配
        /// </summary>
        /// <param name="pattern">类型（支持正则表达式）</param>
        /// <returns></returns>
        public IEnumerator<StreamRes> getResourceSE(string pattern)
        {
            foreach (Resource r in lR.FindAll(r => Regex.IsMatch(r.fType, pattern)))
            {
                yield return new StreamRes()
                {
                    fname = r.fname,
                    ftype = r.fType,
                    rData = new MemoryStream(r.rData)
                };
            }
        }
#endregion

        //杂项
        #region Miscellaneous
        bool Regist2Header(long size)
        {
            if (ix >= 32) return false;
            rhR.size[ix] = size;
            ix++;
            return true;
        }

        public long EstLrssLength
        {
            get
            {
                int estSize = Marshal.SizeOf(typeof(rHeader));
                foreach (Resource r in lR)
                {
                    int btaken = (int)Math.Ceiling(r.rData.LongLength / 1024d);
                    estSize += Marshal.SizeOf(typeof(sNotation));
                    estSize += btaken * 1024;
                }
                return estSize;
            }
        }
#endregion
    }
}
