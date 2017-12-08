using LunapxCompiler.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LunapxCompiler.Generic.Structures;

namespace LunapxCompiler._old
{
    
    public class LunalipxDecompiler
    {
        List<LUNALIPS_Expression> _commands;
        byte[] _lpx;
        public LunalipxDecompiler(string path, ref bool isSuccess)
        {
            _commands = new List<LUNALIPS_Expression>();
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    _lpx = new byte[fs.Length];
                    fs.Read(_lpx, 0, _lpx.Length);
                    isSuccess = true;
                }
            }
            catch
            {
                isSuccess = false;
            }
        }

        public bool DecompileLPX(ref LunalipxDecomp ldxp)
        {
            byte[] intTmp = new byte[4];
            List<byte> str = new List<byte>();
            int[] eqz = new int[10];
            int f_int = 0, f_int_arr = 0, f_str = 0, f_c = 0, f_eqz = 0;
            LUNALIPS_Expression lxe = null;
            for (int i = 0; i < _lpx.Length; i++)
            {
                if (i < 4)
                {
                    intTmp[i] = _lpx[i];
                    if (i == 3)
                    {
                        ldxp.MagicNumber = BitConverter.ToInt32(intTmp, 0);
                    }
                    continue;
                }
                if (f_int + 12 < i || f_int == 0)
                {
                    switch (_lpx[i])
                    {
                        case 0xFF:
                            if (i == 4)
                            {
                                lxe = new LUNALIPS_Expression();
                            }
                            else if (i > 5)
                            {
                                _commands.Add(lxe);
                                lxe = new LUNALIPS_Expression();
                            }
                            f_int = f_int_arr = f_str = 0;
                            continue;
                        case 0xFE:
                            f_int = i;
                            f_int_arr = 0;
                            f_str = 0;
                            f_c = 0;
                            continue;
                        case 0xFD:
                            f_int = 0;
                            f_int_arr = i;
                            f_str = 0;
                            f_c = 0;
                            continue;
                        case 0xFC:
                            f_int = 0;
                            f_int_arr = 0;
                            f_str = i;
                            f_c = 0;
                            continue;
                    }
                }
                if (f_int == 0 && f_int_arr == 0 && f_str == 0)
                {
                    f_c++;
                    switch (f_c)
                    {
                        case 1:
                            lxe.hasREP = _lpx[i] == 0x00 ? false : true;
                            break;
                        case 2:
                            lxe.isRandom = _lpx[i] == 0x00 ? false : true;
                            break;
                        case 3:
                            lxe.isShutdownREQ = _lpx[i] == 0x00 ? false : true;
                            f_c = 0;
                            break;
                    }
                }
                else if (f_int != 0)
                {
                    intTmp[f_c % 4] = _lpx[i];
                    f_c++;
                    if (f_c == 4)
                    {
                        lxe.REP_Times = BitConverter.ToInt32(intTmp, 0);
                    }
                    else if (f_c == 8)
                    {
                        lxe.exe_CMD = BitConverter.ToInt32(intTmp, 0);
                    }
                    else if (f_c == 12)
                    {
                        lxe.SongID = BitConverter.ToInt32(intTmp, 0);
                        f_c = 0;
                    }
                }
                else if (f_int_arr != 0)
                {
                    intTmp[f_c % 4] = _lpx[i];
                    f_c++;
                    if (f_c % 4 == 0)
                    {
                        eqz[f_eqz] = BitConverter.ToInt32(intTmp, 0);
                        if (f_eqz >= 9) lxe.equerz = eqz;
                        f_eqz++;
                    }
                }
                else if (f_str != 0)
                {
                    str.Add(_lpx[i]);
                    if (_lpx[i+1] == 0xFF)
                    {
                        lxe.songsName = Encoding.UTF8.GetString(str.ToArray());
                        f_int = f_int_arr = f_str = 0;
                        str.Clear();
                    }
                }
            }
            ldxp.commands = _commands;
            return true;
        }
    }
}
