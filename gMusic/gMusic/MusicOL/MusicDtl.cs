using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gMusic.MusicOL
{
    public class MusicDtl
    {
        public string musicN="";
        public string perlisten;
        public string picURL;
        /// <summary>
        /// 高质量文件地址URL
        /// </summary>
        public string H_URL="";
        /// <summary>
        /// 中等质量文件地址URL
        /// </summary>
        public string M_URL="";
        /// <summary>
        /// 劣等质量文件地址URL
        /// </summary>
        public string L_URL="";
        public long[] SIZE;
        public string AblumName,Artist,brate;
    }
}
