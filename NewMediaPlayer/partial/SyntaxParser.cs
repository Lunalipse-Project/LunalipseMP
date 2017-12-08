using LunapxCompiler;
using NewMediaPlayer.Generic;
using NewMediaPlayer.Shell;
using System.Collections;
using System.IO;
using System.Text;
using static LunapxCompiler.Generic.Structures;

namespace NewMediaPlayer.Lunalipx
{
    public partial class SyntaxParser
    {
        int mode = 0, prg_id = 0;
        string inp_f = "", out_f = "";
        bool rep_songsID = true;
        public void SP_CMD_Executor(LunalipseShell.Command cmd)
        {
            if (cmd.Args.Length <= 0)
            {
                PrintOutHelp();
                return;
            }
            for (int i = 0; i < cmd.Args.Length; i+=2)
            {
                if(cmd.Args[i]=="-help")
                {
                    PrintOutHelp();
                    break;
                }
                switch(cmd.Args[i])
                {
                    case "-mode":
                        switch (cmd.Args[i + 1])
                        {
                            case "RECOMPL":mode = 0;break;
                            case "DECOMPL":mode = 1;break;
                            case "CUR_STRUC": mode = 2; break;
                            case "CSPT": mode = 3; break;
                            default:LPXShell.WriteLine("Unknow mode! Use '-help' for details.");return;
                        }
                        break;
                    case "-i":inp_f = cmd.Args[i + 1];break;
                    case "-o":out_f = cmd.Args[i + 1];break;
                    case "-sp":
                        if (!int.TryParse(cmd.Args[i + 1], out prg_id))
                        {
                            LPXShell.WriteLine("[Fatal] Invalide argument for parameter '-sp'. Use '-help' for details.");
                            return;
                        }
                        break;
                    case "-rn":
                        if (cmd.Args[i + 1].Equals("y")) rep_songsID = true;
                        else if (cmd.Args[i + 1].Equals("n")) rep_songsID = false;
                        else
                        {
                            LPXShell.WriteLine("[Fatal] Invalide argument for parameter '-rn'. Use '-help' for details.");
                            return;
                        }
                        break;
                    default:
                        PrintOutHelp();
                        return;
                }
                
            }
            ExecuteByMode();
        }

        public void ExecuteByMode()
        {
            switch(mode)
            {
                case 0:
                    break;
                case 1:
                    lpxDecompiler ld;
                    bool isS = false;
                    if(inp_f.AvailableEx())
                    {
                        ld = new lpxDecompiler(global.MAGIC_NUMBER_4_LPX);
                        ld.Decompile(inp_f);
                    }
                    else if (prg_id > 0)
                    {
                        ld = new lpxDecompiler(global.MAGIC_NUMBER_4_LPX);
                        ld.Decompile("Scripts/bin/prg" + prg_id + "._lpx");
                    }
                    else
                    {
                        LPXShell.WriteLine("[Fatal] No argument available!");
                        return;
                    }
                    if (!isS)
                    {
                        LPXShell.WriteLine("[Fatal] Unable to open script which you required. Check the path or see whether the script are using by other software.");
                        return;
                    }
                    LunalipxDecomp LD = new LunalipxDecomp();
                    LPXShell.WriteLine("Decompiling the LXP file ....");
                    LD.commands = ld.CMDList;
                    LD.MagicNumber = ld.Header.type;
                    using (FileStream fs = new FileStream(out_f.AvailableEx() ? out_f : "Scripts/dcp_prg{0}.lunapx".FormateEx(prg_id), FileMode.Create))
                    {
                        byte[] __ = Encoding.UTF8.GetBytes(LD.commands.translate2lunapx());
                        fs.Write(__, 0, __.Length);
                    }
                    LPXShell.WriteLine("All done!\n The output source file is save under {0}", out_f.AvailableEx() ? out_f : "<Lunalipse Install Path>/Script/dcp_prg{0}.lunapx".FormateEx(prg_id));
                    break;
                case 3:
                    if ((!inp_f.AvailableEx()&&prg_id==0) || !out_f.AvailableEx())
                    {
                        LPXShell.WriteLine(" [Fatal] Please make sure that input/output file has been specified.");
                        return;
                    }
                    LPXShell.WriteLine(" [INFO] Parsing and re-compile script....");
                    if(MainWindow.al==null||MainWindow.al.Count<=0)
                    {
                        LPXShell.WriteLine(" [Fatal] No music presents in the Lunalipse.");
                        return;
                    }
                    ArrayList cmd = (ArrayList)commands.Clone();
                    if(!ParseCSPT(MainWindow.al, inp_f.AvailableEx() ? inp_f : "Scripts/prg{0}.lunapx".FormateEx(prg_id)))
                    {
                        LPXShell.WriteLine(" [Fatal] An error occurs while parsing the script");
                        return;
                    }
                    LPXShell.WriteLine(" [INFO] Exporting Lpx byte code....");
                    lpxCompiler lc = new lpxCompiler(global.MAGIC_NUMBER_4_LPX);
                    lc.SetExpression(commands);
                    lc.CompileTo(out_f);
                    //if (!)
                    //{
                    //    LPXShell.WriteLine(" [Fatal] Unable to export the byte code.");
                    //    return;
                    //}
                    LPXShell.WriteLine(" [INFO] Conversion done! Lpx file has been exported to {0}\nCleaning up the resources.", out_f);
                    commands = (ArrayList)cmd.Clone();
                    cmd.Clear();
                    break;
            }
        }

        void PrintOutHelp()
        {
            LPXShell.WriteLine("  Lunalipse Behavior Script (De)Compiler and Syntax Parser.  [Version:1.0.0.3]\n" +
                                "  Provide basic function of parsing script source file , (de)compiling and also platform cross tool.\n");
            LPXShell.WriteLine("  Arguments:");
            LPXShell.WriteLine("    {0,-10} - {1}","-mode <RECOMPL|DECOMPL|CUR_STRUC|CSPT>", "set the mode of compiler");
            LPXShell.WriteLine("       {0,-10} - {1}","RECOMPL", "Recompile specific scripts.(This will override exist one.)");
            LPXShell.WriteLine("       {0,-10} - {1}","DECOMPL", "Decompile specific scripts.(Replace current command list)");
            LPXShell.WriteLine("       {0,-10} - {1}","CUR_STRUC", "Get current list(Just print out or exporting to specific file)");
            LPXShell.WriteLine("       {0,-10} - {1}","CSPT", "Compile to Android platform compatible file.");
            LPXShell.WriteLine("    {0,-10} - {1}","-i <FILE>", "[ALL]set the input file.");
            LPXShell.WriteLine("    {0,-10} - {1}","-sp <ID>", "[ALL]set script ID (1 ~ 5 for default).");
            LPXShell.WriteLine("    {0,-10} - {1}","-o <FILE>", "[ALL]set the output file.");
            //LPXShell.WriteLine("    {0,-10} - {1}","-rn <y|n>", "[CSPT]Replace all songID by songName which it's represent.");
        }
    }
}
