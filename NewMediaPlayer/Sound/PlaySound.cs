using CSCore;
using CSCore.Codecs.MP3;
using CSCore.Codecs.WAV;
using CSCore.Codecs.FLAC;
using CSCore.Codecs.AAC;
using CSCore.Codecs.AIFF;
using CSCore.SoundOut;
using System;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using System.Windows;
using CSCore.Tags;
using CSCore.Tags.ID3;

namespace NewMediaPlayer.Sound
{

    public class PlaySound
    {
        private ISoundOut iso;
        private IWaveSource soundSource;
        private bool isPlaying = false, isPaused = false;
        private Label dur;
        private Label cur;
        private Image img;
        public static float R = 20, L = 20, WD = 70;
        Thread t;
        Thread sec_count;
        InitializeFFT ifft;
        public static CSCore.Streams.Effects.Equalizer equzer;


        /*------- Declartion of Delegate --------*/
        public delegate void PlaySoundComplete();
        public delegate void SoundLoadedComplete(double newDuration);
        public delegate void ProgressUpdated(long newPosition);
        public delegate void DurationChanged(double durt,string formated);
        public delegate void UpdateInfo(mMetadata mm);

        /*------- Declartion of Event --------*/
        public event PlaySoundComplete OnPlaySoundComplete;
        public event SoundLoadedComplete OnSoundLoadedComplete;
        public event ProgressUpdated OnProgressUpdated;
        public event UpdateInfo OnInfoUpdated;
        public static event DurationChanged OnDurationChanged;


        public PlaySound(Label d,Label c,Image i,System.Drawing.Size ci)
        {
            iso = GetSoundOut();
            dur = d;
            cur = c;
            img = i;
            iso.Stopped += Iso_Stopped;
            ifft = new InitializeFFT();
            ifft.SetCircularArgs(ci);
            floating f = new floating();
            cirspt csr = new cirspt();
            f.ShowInTaskbar = csr.ShowInTaskbar = false;
            f.Left = SystemParameters.WorkArea.Width - f.Width;
            f.Top = SystemParameters.WorkArea.Height - f.Height;
            f.Topmost = true;
            f.Show();
            csr.Show();
        }

        public void PlayIt(FileStream s_fs,string ext)
        {
            if (isPlaying) iso.Stop();
            ShutdownTheThread();
            #region 播放进度计时器
            sec_count = new Thread(new ThreadStart(() =>
            {
                try
                {
                    double expect = 0;
                    while (isPlaying)
                    {
                        if (!isPaused)
                        {
                            string ft = calcTS(expect = soundSource.GetPosition().TotalSeconds);
                            cur.Dispatcher.Invoke(new Action(() =>
                            {
                                cur.Content = ft;
                            }));
                            OnDurationChanged?.Invoke(Math.Round(expect), ft);
                            OnProgressUpdated(soundSource.Position);
                        }
                        Thread.Sleep(500);
                    }
                }
                catch (ThreadAbortException) { LogFile.WriteLog("WARNING", "Shutting down counter thread ..."); }

            }));
            #endregion
            OnInfoUpdated(ext == ".mp3" ? GetMP3Info(s_fs) : null);
            #region 音频线程
            t = new Thread(new ThreadStart(() =>
            {
                using (soundSource = GetSoundSource(s_fs, ext))
                {
                    try
                    {
//#if !DEBUG
//                        DmoWavesReverbEffect dwre = new DmoWavesReverbEffect(soundSource);
//                        //dwre.HighFrequencyRTRatio = 0.5f;
//                        soundSource = dwre;
//#endif
                        iso.Initialize(
                            ifft.Initialize(
                                soundSource.ToSampleSource()
                                .ChangeSampleRate(32000)
                                .AppendSource(
                                    CSCore.Streams.Effects.Equalizer.Create10BandEqualizer,
                                    out equzer
                                )
                            )
                        );
                        iso.Volume = global.MUSIC_VOLUME;
                        isPlaying = true;
                        isPaused = false;
                        TimeSpan ts = soundSource.GetLength();
                        dur.Dispatcher.Invoke(new Action(() =>
                        {
                            dur.Content = ts.Hours.ToString().PadLeft(2, '0') + ":" +
                                          ts.Minutes.ToString().PadLeft(2, '0') + ":" +
                                          ts.Seconds.ToString().PadLeft(2, '0');
                        }));
                        OnSoundLoadedComplete(soundSource.Length);
                        iso.Play();
                        sec_count.Start();
                        while (isPlaying)
                        {
                            while (isPaused) { Thread.Sleep(1000); }
                            ifft.GenerateLineSpectrum(img);
                            Thread.Sleep(global.FFT_REF_FRQ);
                        }
                        isPlaying = false;
                        OnPlaySoundComplete();
                    }
                    catch(ThreadAbortException) { LogFile.WriteLog("WARNING", "Shutting down audio thread ..."); }
                }
            }));
            t.Start();
#endregion
        }

        public bool StopPlay()
        {
            if(isPlaying)
            {
                iso.Stop();
            }
            return isPlaying;
        }

        public bool ISPLAYING
        {
            get
            {
                return isPlaying;
            }
        }

        private void ShutdownTheThread()
        {
            if (t != null) t.Abort();t = null;
            if (sec_count != null) sec_count.Abort();sec_count = null;
        }

        public void setNewPosition(long incs)
        {
            if (isPlaying) soundSource.Position =incs;
        }

        public void setVolume(float v)
        {
            if (isPlaying) iso.Volume = v;
        }

        public bool ChangePlayStatus()
        {
            if(isPlaying)
            {
                if(isPaused)
                {
                    iso.Resume();
                    isPaused = false;
                }
                else
                {
                    iso.Pause();
                    isPaused = true;
                }
                return isPaused;
            }
            return isPlaying;
        }

        private mMetadata GetMP3Info(FileStream fs)
        {
            ID3v2 id3v2 = ID3v2.FromStream(fs);
            if (id3v2 == null)
            {
                //ID3v1 id3v1 = ID3v1.FromStream(fs);
                //return new mMetadata()
                //{
                //    Al = id3v1.Album,
                //    artist = id3v1.Artist,
                //    AlPic = null
                //};
                return null;
            }
            ID3v2QuickInfo qi = new ID3v2QuickInfo(id3v2);
            return new mMetadata()
            {
                Al = qi.Album,
                artist = qi.LeadPerformers,
                AlPic = qi.Image != null ? GUtil.bt2ibts(new System.Drawing.Bitmap(qi.Image)) : null
            };
        }

        public bool ChangePlayStatus(bool b)
        {
            if (isPlaying)
            {
                if (isPaused && b)
                {
                    iso.Resume();
                    isPaused = false;
                }
                else if(!b)
                {
                    iso.Pause();
                    isPaused = true;
                }
                return isPaused;
            }
            return isPlaying;
        }

        private void Iso_Stopped(object sender, PlaybackStoppedEventArgs e)
        {
            isPlaying = false;
        }

        public void ISO_DISPOSE()
        {
            ShutdownTheThread();
            iso?.Dispose();
        }

        private ISoundOut GetSoundOut()
        {
            if (WasapiOut.IsSupportedOnCurrentPlatform)
                return new WasapiOut();
            else
                return new DirectSoundOut();
        }

        private IWaveSource GetSoundSource(Stream stream, string EXT)
        {
            switch(EXT)
            {
                case ".mp3":
                    return new DmoMp3Decoder(stream);
                case ".flac":
                    return new FlacFile(stream);
                case ".wav":
                    return new WaveFileReader(stream);
                case ".acc":
                    return new AacDecoder(stream);
                case ".aiff":
                    return new AiffReader(stream);
            }
            return null;
        }

        public string calcTS(double ms_)
        {
            int h = 0, m = 0, s = 0;
            double ms = Math.Floor(ms_);
            while (ms > 0)
            {
                if (ms >= 3599d)
                {
                    h++;
                    ms -= 3600;
                }
                else if (ms >= 59d)
                {
                    m++;
                    ms -= 60;
                }
                else if (ms >= 0d)
                {
                    s++;
                    ms--;
                }
            }
            return h.ToString().PadLeft(2, '0') + ":" + m.ToString().PadLeft(2, '0') + ":" + s.ToString().PadLeft(2, '0');
        }

        
    }
}
