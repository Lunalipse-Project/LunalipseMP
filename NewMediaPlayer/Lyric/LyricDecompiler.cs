using NewMediaPlayer.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace NewMediaPlayer.Lyric
{
    class LyricDecompiler
    {
        /// <summary>
        /// 歌曲
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 艺术家
        /// </summary>
        public string Artist { get; set; }
        /// <summary>
        /// 专辑
        /// </summary>
        public string Album { get; set; }
        /// <summary>
        /// 歌词作者
        /// </summary>
        public string LrcBy { get; set; }
        /// <summary>
        /// 偏移量
        /// </summary>
        public string Offset { get; set; }

        public Dictionary<double, string> Lyrics
        {
            get { return dir; }
        }

        StreamReader fs;
        Dictionary<double, string> dir;

        public static volatile LyricDecompiler _LD_INS;
        public static readonly object LOCKER = new object();

        public static LyricDecompiler INSTANCE
        {
            get
            {
                if(_LD_INS==null)
                {
                    lock(LOCKER)
                    {
                        if (_LD_INS == null) return _LD_INS = new LyricDecompiler();
                    }
                }
                return _LD_INS;
            }
        }

        //Event
        public delegate void LyricNotFound();
        public event LyricNotFound LyricNotFoundTrigger;
        public delegate void LyricPrp();
        public event LyricPrp OnLyricPerpeared;

        public LyricDecompiler()
        {
            dir = new Dictionary<double, string>();
        }

        public void LoadLyric(string LrcPath)
        {
            try
            {
                if(fs!=null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                fs = new StreamReader(LrcPath, Encoding.UTF8);
            }
            catch (FileNotFoundException fne)
            {
                LogFile.WriteLog("WARNING", "Cannot load the lyric which suppose to be located in " + fne.FileName);
                LyricNotFoundTrigger();
            }
            catch (DirectoryNotFoundException)
            {
                LogFile.WriteLog("WARNING", "Lyric folder not found. Create automatically....");
                Directory.CreateDirectory(Path.GetDirectoryName(LrcPath));
            }
        }

        public Dictionary<double,string> DecompilerLyrics()
        {
            if (fs == null) return null;
            dir.Clear();
            int o = 0;
            using (fs)
            {
                String line;
                Regex rx = new Regex(@"\[.*?\]");
                while ((line = fs.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (line.StartsWith("[ti:"))
                        {
                            Title = SplitInfo(line);
                        }
                        else if (line.StartsWith("[ar:"))
                        {
                            Artist = SplitInfo(line);
                        }
                        else if (line.StartsWith("[al:"))
                        {
                            Album = SplitInfo(line);
                        }
                        else if (line.StartsWith("[by:"))
                        {
                            LrcBy = SplitInfo(line);
                        }
                        else if (line.StartsWith("[offset:"))
                        {
                            Offset = SplitInfo(line);
                            try { o = int.Parse(Offset); } catch { LogFile.WriteLog("ERROR", "Invalid offset value"); };
                        }
                        else
                        {
                            Regex regex = new Regex(@"\[([0-9.:]*)\]+(.*)", RegexOptions.Compiled);
                            MatchCollection mc = regex.Matches(line);
                            double time = TimeSpan.Parse("00:" + mc[0].Groups[1].Value).TotalSeconds;
                            time += o / 1000;
                            string word = mc[0].Groups[2].Value;
                            dir.Add(time, word);
                        }
                    }
                }
            }
            Offset = "";
            o = 0;
            LogFile.WriteLog("INFO", "Lyrics - Loaded " + dir.Count + " records of lyric");
            fs.Close();
            fs.Dispose();
            fs = null;
            OnLyricPerpeared?.Invoke();
            return dir;
        }

        static string SplitInfo(string line)
        {
            return line.Substring(line.IndexOf(":") + 1).TrimEnd(']');
        }
    }
}
