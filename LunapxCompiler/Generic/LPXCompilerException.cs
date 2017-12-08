using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunapxCompiler.Generic
{
    public class LPXCompilerException:Exception
    {
        public LPXCompilerException(string message,params string[] args) 
            : base(string.Format(message,args))
        {
            
        }
    }

    public class LPXDecompilerException:Exception
    {
        public LPXDecompilerException(string msg,params string[] args)
            : base(string.Format(msg, args))
        {

        }
    }
}
