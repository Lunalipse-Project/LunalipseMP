using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMediaPlayer.Lunalipx
{
    public class LPXException:Exception
    {
        public string Type = "";
        public LPXException(string m,string t):base(m)
        {
            Type = t;
        } 
    }


}
