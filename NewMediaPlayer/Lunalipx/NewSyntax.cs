using LpxResource.Generic;
using LunapxCompiler;
using LunapxCompiler.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NewMediaPlayer.Lunalipx
{
    class NewSyntax
    {
        const string perfix = "luna";
        readonly Regex dquote = new Regex("(?<=\").*(?=\")");
        public Dictionary<string, int> Commands = new Dictionary<string, int>
        {
            {"play",Command.LUNA_PLAYS},
            {"playN",Command.LUNA_PLAYN},
            {"eqzr",Command.LUNA_SETEQU},
            {"next",Command.LUNA_PASS},
            {"lloop",Command.LUNA_LLOOP},
            {"load",1}
        };
        int translate(string command)
        {
            if (Commands.ContainsKey(command)) return Commands[command];
            return -1;
        }

        public LUNALIPS_Expression ParseStatement(string statement)
        {
            if(!preCheck(statement))
            {
                //throw new Exception("Lunapx Syntax Error | " + s + "|. Braket or quote unpaired.");
                return null;
            }
            LUNALIPS_Expression lex = new LUNALIPS_Expression();
            string cmd = "", args = "";
            for (int i = 0; i < statement.Length;)
            {
                cmd += statement[i];
                //Where the argument start
                if (statement[i + 1] == '(')
                {
                    i += 2;
                    lex.exe_CMD = translate(cmd.Contains(perfix) ? cmd.Replace(perfix + ".", "") : cmd);
                    if (lex.exe_CMD == -1)
                        throw new LPXException("未定义的函数：{0}".FormateEx(cmd), statement);
                    if (lex.exe_CMD == Command.LUNA_PLAYN ||
                       lex.exe_CMD == Command.LUNA_PLAYS)
                    {
                        if (statement.Contains(":"))
                        {
                            string[] s = statement.Split(':');
                            uint u;
                            if (!uint.TryParse(s[s.Length - 1], out u))
                                throw new LPXException("不正确的循环次数", statement);
                            lex.hasREP = (lex.REP_Times = (int)u) > 0;
                        }
                    }
                    bool isInQuote = false;
                    for (; i < statement.Length - 1; i++)
                    {
                        args += statement[i];
                        if (statement[i] == '"' && !isInQuote) isInQuote = true;
                        else if (statement[i] == '"' && isInQuote) isInQuote = false;
                        if (statement[i + 1] == ')' /*|| statement[i + 1] == '!')*/  && !isInQuote) break;
                    }
                    ParseArgs(ref lex, args,statement);
                    break;
                }
                i++;
            }
            return lex;
        }

        private void ParseArgs(ref LUNALIPS_Expression lex, string args, string stes)
        {
            List<string> Args = new List<string>();
            if(lex.exe_CMD==Command.LUNA_PLAYS)
            {
                if(!dquote.IsMatch(args))
                    throw new LPXException("未提供与方法play(string,int)对应的实参", stes);
                lex.songsName = dquote.Match(args).Value;
                args = dquote.Replace(args, "").Replace("\"\"", "");
            }
            Args = args.Split(',').ToList();
            switch (lex.exe_CMD)
            {
                case Command.LUNA_PLAYN:
                    uint u = 0;
                    if (Args[0] == "#RAND") lex.isRandom = true;
                    else if (!uint.TryParse(Args[0], out u))
                        throw new LPXException("指定了一个错误的歌曲ID", stes);
                    if (Args.Count == 2)
                        if (!double.TryParse(Args[1], out lex.Vol) || (lex.Vol < 0 && lex.Vol > 1))
                            throw new LPXException("指定了一个错误的音量", stes);
                    lex.SongID = (int)u;
                    break;
                case Command.LUNA_PLAYS:
                    if (Args.Count == 2)
                        if (!double.TryParse(Args[1], out lex.Vol) || (lex.Vol < 0 && lex.Vol > 1))
                            throw new LPXException("指定了一个错误的音量", stes);
                    break;
                case Command.LUNA_SETEQU:
                    lex.equerz = new int[10];
                    if(Args.Count<10) new LPXException("未提供与方法eqz(int,int,int,int,int,int,int,int,int,int)对应的实参", stes);
                    for (int i=0;i< Args.Count();i++)
                        if(!int.TryParse(Args[i],out lex.equerz[i]))
                            throw new LPXException("未提供与方法eqz(int,int,int,int,int,int,int,int,int,int)对应的实参", stes);
                    break;
            }

        }

        bool preCheck(string statement)
        {
            Stack<byte> quotes = new Stack<byte>();
            bool hasOquotes = false;
            Stack<byte> barket = new Stack<byte>();
            foreach(char c in statement)
            {
                if (c == '(') barket.Push(1);
                if (c == ')') barket.Pop();
                if (c == '"' && !hasOquotes)
                {
                    hasOquotes = true;
                    quotes.Push(1);
                }
                if (c == '"' && hasOquotes)
                {
                    hasOquotes = false;
                    quotes.Pop();
                }
            }
            return quotes.Count == 0 && barket.Count == 0;
        }        
    }
}
