using System;
using System.Collections.Generic;

namespace NewMediaPlayer.Lyric
{
    class LyricsDisplay
    {
        Dictionary<double, string> _lyc;
        public delegate void LrcMatch(string lyric);
        public static event LrcMatch OnLryicMatched;
        public LyricsDisplay()
        {
            
        }

        public void LrcParse(Dictionary<double, string> d)
        {
            _lyc = d;
        }

        public void IsMeetLrcBlock(double currentInSecond)
        {
            if (_lyc == null) return;
            foreach (var k in _lyc)
            {
                if (Math.Floor(k.Key) == Math.Floor(currentInSecond))
                {
                    OnLryicMatched(k.Value);
                    _lyc.Remove(k.Key);
                    break;
                }
            }
        }

    }
}
