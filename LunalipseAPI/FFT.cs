using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LunalipseAPI
{
    public interface FFT
    {
        void onSpectrumUpdate(ImageSource iss);
        void onSoundDataReceived(float left, float right);
        void onFFTDataUpdate(float[] ft);
    }
}
