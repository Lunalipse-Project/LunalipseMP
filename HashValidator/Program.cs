using System;
using System.Text;

namespace HashValidator
{
    class Program
    {
        static void Main(string[] args)
        {
            int mode = -1;
            string currentP = null;
            string targetT = null;
            string filter = null;
            for (int i = 0; i < args.Length; i += 2)
            {
                switch(args[i])
                {
                    case "-i":
                        currentP = args[i + 1];
                        break;
                    case "-t":
                        targetT = args[i + 1];
                        break;
                    case "-c":
                        filter = args[i + 1];
                        break;
                    case "-m":
                        try
                        {
                            mode = int.Parse(args[i + 1]);
                        }
                        catch
                        {
                            Console.WriteLine("Argments error");
                        }
                        break;
                }
            }
            if (mode != -1 && 
                (
                    !string.IsNullOrEmpty(currentP) || 
                    !string.IsNullOrEmpty(targetT) ||
                    !string.IsNullOrEmpty(filter)
                )
               )
            {
                FEntrance fe = new FEntrance();
                if(mode == 0)
                {
                    fe.Calculate(filter, currentP);
                    fe.Export(targetT);
                }
                else if(mode == 1 )
                {
                    MD5Export md5e = fe.getExport(currentP);
                    if(md5e!=null)
                    {
                        foreach (var v in md5e.MD5Dic)
                        {
                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < v.Value.Length; i++)
                            {
                                sb.Append(v.Value[i].ToString("x2"));
                            }
                            Console.WriteLine("{0}:   {1}", v.Key, sb);
                        }
                    }
                }
                else if(mode == 2)
                {
                    if (fe.ValidateMD5(currentP, targetT).Success)
                    {
                        Console.WriteLine("All files are origin");
                    }
                    else
                    {
                        Console.WriteLine("Some files have been modified");
                    }
                }
            }
        }
    }
}
