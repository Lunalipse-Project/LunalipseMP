using CSCore.DSP;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMediaPlayer.Sound
{
    class CircularSpectrum : SpectrumBase
    {
        public int barCount = 50;
        public Size WorkingArea = new Size(512, 512);
        public int padding = 20;
        public int Radius = 30;
        public int BarWidth = 4;
        double dd = 0;
        int total = 0;
        public CircularSpectrum(FftSize fftSize)
        {
            FftSize = fftSize;
        }
        public void SetCeneter()
        {
            if (WorkingArea.Width == WorkingArea.Height)
            {
                padding = (int)Math.Round(WorkingArea.Width / 2d) - Radius;
            }
        }

        public void Initialize()
        {
            dd = 360d / barCount;
            total = padding + Radius;
            SpectrumResolution = barCount;
            UpdateFrequencyMapping();
        }

        public void GenerateCircularSpect(Graphics graphics, Pen p, int height, float[] fftBuffer)
        {
            SpectrumPointData[] spectrumPoints = CalculateSpectrumPoints(padding, fftBuffer);

            for (int d = 0; d < barCount; d++)
            {
                SpectrumPointData spd = spectrumPoints[d];

                float vcos = (float)Math.Cos(deg2rad(dd * spd.SpectrumPointIndex));
                float vsin = (float)Math.Sin(deg2rad(dd * spd.SpectrumPointIndex));

                float rL = Radius + ((float)spd.Value - 1f);

                PointF pf0 = new PointF(Radius * vsin + total, Radius * vcos + total);
                PointF pf1 = new PointF(rL * vsin + total, rL * vcos + total);

                graphics.DrawLine(p, pf0, pf1);
            }
        }

        public Bitmap CreateCircularSpectrum(Color color1, Color color2, Color background, bool highQuality)
        {
            using (
                Brush brush = new LinearGradientBrush(new RectangleF(0, 0, (float)BarWidth, WorkingArea.Height), color2,
                    color1, LinearGradientMode.Vertical))
            {
                return CreateCircularSpectrum(brush, background, highQuality);
            }
        }

        public Bitmap CreateCircularSpectrum(Brush brush, Color background, bool highQuality)
        {
            var fftBuffer = new float[(int)FftSize];

            //get the fft result from the spectrum provider
            if (SpectrumProvider.GetFftData(fftBuffer, this))
            {
                using (var pen = new Pen(brush, (float)BarWidth))
                {
                    var bitmap = new Bitmap(WorkingArea.Width, WorkingArea.Height);

                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        GUtil.PrepareGraphics(graphics, highQuality);
                        graphics.Clear(background);

                        GenerateCircularSpect(graphics, pen, WorkingArea.Height, fftBuffer);
                    }

                    return bitmap;
                }
            }
            return null;
        }

        double rad2deg(double rad)
        {
            return (180d / Math.PI) * rad;
        }

        double deg2rad(double deg)
        {
            return (Math.PI / 180d) * deg;
        }
        
    }
}
