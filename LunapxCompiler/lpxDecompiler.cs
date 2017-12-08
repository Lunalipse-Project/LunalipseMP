using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using LunapxCompiler.Generic;

namespace LunapxCompiler
{
    public class lpxDecompiler
    {
        int size_lpxh, size_lpxe,ID;
        public lpxDecompiler(int ID)
        {
            this.ID = ID;
            size_lpxh = Marshal.SizeOf(typeof(Structures.lpxHeader));
            size_lpxe = Marshal.SizeOf(typeof(Structures.lpxExpression));
        }
        public Structures.lpxHeader Header { get; private set; }
        public List<LUNALIPS_Expression> CMDList { get; private set; }

        public void Decompile(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                byte[] b = new byte[size_lpxh];
                fs.Read(b, 0, b.Length);
                Header = (Structures.lpxHeader)(Utils.b2s(b, typeof(Structures.lpxHeader)) ?? 
                    throw new LPXCompilerException("无法解析标头。（长度{0}字节）", b.Length+""));
                if (Header.id != ID) throw new LPXDecompilerException("版本号形参不一致，Lunalipse脚本反编译器无法冒险解析。");
                for (int i = 0; i < Header.cmds; i++)
                {
                    byte[] b2 = new byte[size_lpxe];
                    fs.Read(b2, 0, b2.Length);
                    CMDList.Add(((Structures.lpxExpression)(Utils.b2s(b2,typeof(Structures.lpxExpression))?? 
                        throw new LPXCompilerException("无法解析命令数据块。（长度{0}字节）", b2.Length + ""))).ToExpression());
                }
            }
        }
    }
}
