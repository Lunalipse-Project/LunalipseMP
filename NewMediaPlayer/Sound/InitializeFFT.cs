using CSCore;
using CSCore.DSP;
using CSCore.Streams;
using NewMediaPlayer.Shell;
using NewMediaPlayer.ui;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace NewMediaPlayer.Sound
{
    class InitializeFFT
    {
        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);
        LineSpectrum _lineSpectrum;
        CircularSpectrum cs;
        LunalipseInterface LI;
        Size _s;
        IWaveSource iws_;

        public delegate void SpectrumDrawnComplete(System.Windows.Media.ImageSource isur);
        public static event SpectrumDrawnComplete OnSpectrumDrawnComplete;

        public InitializeFFT()
        {
            LI = LunalipseInterface.INSTANCE;
        }

        public IWaveSource Initialize(ISampleSource iws)
        {
            const FftSize fftSize = FftSize.Fft4096;
            //create a spectrum provider which provides fft data based on some input
            var spectrumProvider = new BasicSpectrumProvider(iws.WaveFormat.Channels,
                iws.WaveFormat.SampleRate, fftSize);

            //linespectrum and voiceprint3dspectrum used for rendering some fft data
            //in oder to get some fft data, set the previously created spectrumprovider 
            _lineSpectrum = new LineSpectrum(fftSize)
            {
                SpectrumProvider = spectrumProvider,
                UseAverage = true,
                BarCount = 50,
                BarSpacing = 2,
                IsXLogScale = true,
                ScalingStrategy = global.SSTR
            };

            cs = new CircularSpectrum(fftSize)
            {
                SpectrumProvider = spectrumProvider,
                UseAverage = true,
                barCount = 50,
                BarWidth = 6,
                IsXLogScale = true,
                Radius = 70,
                WorkingArea = _s,
                ScalingStrategy = global.SSTR
            };
            cs.SetCeneter();
            cs.Initialize();

            var notificationSource = new SingleBlockNotificationStream(iws);
            //pass the intercepted samples as input data to the spectrumprovider (which will calculate a fft based on them)
            notificationSource.SingleBlockRead += (s, a) => spectrumProvider.Add(a.Left, a.Right);
            LI.OnScalingStrategyChange += (x) =>
            {
                if(!global.USE_CIRCULAR_SPECT)
                {
                    _lineSpectrum.ScalingStrategy = x;
                }
                else
                {
                    cs.ScalingStrategy = x;
                }
            };
            
            iws_ = notificationSource.ToWaveSource(16);
            return iws_;
        }

        public void SetCircularArgs(Size s)
        {
            _s = s;
        }

        public void GenerateLineSpectrum(System.Windows.Controls.Image fftimg)
        {
            fftimg.Dispatcher.Invoke(new Action(() =>
            {
                if (global.SHOW_FFT)
                {
                    Bitmap newImage;
                    if(!global.USE_CIRCULAR_SPECT)
                    {
                        newImage = _lineSpectrum.CreateSpectrumLine(new Size((int)fftimg.Width, (int)fftimg.Height), Color.FromArgb(193, 51, 51, 51), Color.FromArgb(193, 221, 221, 221), Color.Transparent, true);
                    }
                    else
                    {
                        newImage = cs.CreateCircularSpectrum(Color.FromArgb(193, 51, 51, 51), Color.FromArgb(193, 221, 221, 221), Color.Transparent, true);
                    }
                    if (newImage != null)
                    {
                        IntPtr ip = newImage.GetHbitmap();
                        OnSpectrumDrawnComplete(System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                            ip,
                            IntPtr.Zero,
                            System.Windows.Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions()));
                        DeleteObject(ip);

                    }
                }
                else OnSpectrumDrawnComplete(null);
            }));
        }
    }
}
