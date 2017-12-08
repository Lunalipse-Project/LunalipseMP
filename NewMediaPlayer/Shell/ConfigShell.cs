using LunalipseShell;
using NewMediaPlayer.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NewMediaPlayer.Shell
{
    public class ConfigShell
    {
        bool cvt2android = false;
        int mode = 0;
        string out_f = "", key = "", value = "";
        public void ConfigShellCmd(Command cmd)
        {
            if (cmd.Args.Length <= 0)
            {
                ShowCommand();
                return;
            }
            for (int i=0;i<cmd.Args.Length;i+=2)
            {
                switch(cmd.Args[i])
                {
                    case "-help":
                        ShowCommand();
                        break;
                    case "-m":
                        switch(cmd.Args[i+1])
                        {
                            case "OBSV":
                                mode = 0;
                                break;
                            case "MODIF":
                                mode = 1;
                                break;
                        }
                        break;
                    case "-o":
                        out_f = cmd.Args[i + 1];
                        break;
                    case "-android":
                        cvt2android = true;
                        break;
                    case "-k":
                        key = cmd.Args[i + 1];
                        break;
                    case "-v":
                        value = cmd.Args[i + 1];
                        break;
                    default:
                        ShowCommand();
                        return;
                }
            }
            ExecuteByMode();
        }

        void ExecuteByMode()
        {
            switch(mode)
            {
                case 0:
                    if(!cvt2android)
                    {
                        if(!out_f.AvailableEx())
                            PrintOutConfig();
                    }
                    break;
                case 1:
                    Type t = typeof(global);
                    if(!key.AvailableEx()||!value.AvailableEx())
                    {
                        LPXShell.WriteLine(" [Fatal] Arguments missing.");
                        return;
                    }
                    FieldInfo fi = t.GetField(key, BindingFlags.Public|BindingFlags.Static);
                    if(fi==null)
                    {
                        LPXShell.WriteLine(" [Fatal] Field '{0}' not found.",key);
                        return;
                    }
                    Type t2 = fi.FieldType;
                    if (t2.Equals(typeof(bool)))
                    {
                        bool c = false;
                        if(!bool.TryParse(value,out c))
                        {
                            LPXShell.WriteLine(" [Fatal] Invalide parameter! It must be boolean(True or False).");
                            return;
                        }
                        fi.SetValue(null, c);
                    }
                    else if (t2.Equals(typeof(string)))
                    {
                        fi.SetValue(null, value);
                    }
                    else if (t2.Equals(typeof(int)))
                    {
                        int i = 0;
                        if(!int.TryParse(value,out i))
                        {
                            LPXShell.WriteLine(" [Fatal] Invalide parameter! It must be integer.");
                            return;
                        }
                        fi.SetValue(null, i);
                    }
                    else if (t2.Equals(typeof(double)))
                    {
                        double d = 0d;
                        if(!double.TryParse(value,out d))
                        {
                            LPXShell.WriteLine(" [Fatal] Invalide parameter! It must be double.");
                            return;
                        }
                        fi.SetValue(null, d);
                    }
                    LPXShell.WriteLine(" Applied value '{0}' to the target '{1}' successfully.", value, key);
                    break;
            }
        }

        private void PrintOutConfig()
        {
            Type t = typeof(global);
            foreach(FieldInfo f in t.GetFields())
            {
                LPXShell.WriteLine("    {0,-20} \n      | Type= {1}\n      | Value= {2}\n", f.Name, f.FieldType, f.GetValue(null));
                if(f.Name.Equals("__data"))
                {
                    foreach(var kv in global.__data)
                    {
                        LPXShell.WriteLine("        {0}", "Key= "+kv.Key);
                    }
                }
            }
        }

        private void ShowCommand()
        {
            LPXShell.WriteLine(" Lunalipse Global Configuration Tool    [Version: 1.0.0.0]");
            LPXShell.WriteLine("\n *** WARNING *** It may cause any unexpected ERRORS or PROBLEMS when you MODIFIED the configuration VIA CONSOLES.");
            LPXShell.WriteLine("                   Lunalipse WILL NOT take responsibility for your careless operation.");
            LPXShell.WriteLine();
            LPXShell.WriteLine("    {0,-20} - {1}", "-m <OBSV|MODIF>","Set the behavior of LGCT");
            LPXShell.WriteLine("    {0,-20} > {1}", "   OBSV", "Observation Mode. Get the conifguration fields and their current values.");
            LPXShell.WriteLine("    {0,-20} > {1}", "   MODIF", "Modification Mode. Set the custom value of any specific field.");
            LPXShell.WriteLine("    {0,-20} - {1}", "-o <FILE>", "[OBSV] Export the fields partten to file.");
            LPXShell.WriteLine("    {0,-20} - {1}", "-android", "[OBSV] Converting config to Android compatible formate.");
            LPXShell.WriteLine("    {0,-20} - {1}", "-k <FIELD_NAME>", "[MODIF] Specify the name of field.");
            LPXShell.WriteLine("    {0,-20} - {1}", "-v <FILED_VAL>", "[MODIF] Specify the value of field (MUST BE the SAME type to the field!).");
        }
    }
}
