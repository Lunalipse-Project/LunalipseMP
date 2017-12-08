using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gMusic.util
{
    public class sizeCalc
    {
        public static string Calc(long bytes)
        {
            double final = bytes;
            string uit = "";
            //Unit: B
            if (final <=1024)
            {
                uit = " B";
            }
            else if(final >1024 && final<=1024*1024)
            {
                final /= 1024;
                uit = "KB";
            }
            else if(final >Math.Pow(1024,2) && final <= Math.Pow(1024,3))
            {
                final /= Math.Pow(1024, 2);
                uit = "MB";
            }
            return Decimal.Round(new decimal(final), 2).ToString() + uit;
        }
    }
}
