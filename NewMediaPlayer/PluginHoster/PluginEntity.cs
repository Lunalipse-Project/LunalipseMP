using LunalipseAPI;
using LunalipseAPI.Graphics;
using LunalipseAPI.LunalipxPlugin;
using LunalipseAPI.Networking;
using LunalipseAPI.PlayMode;
using LunaNetCore;

namespace NewMediaPlayer.PluginHoster
{
    public class PluginEntity
    {
        public MainUI ENTRY;
        public FFT LFFT;
        public ILunalipseDrawing LDrawing;
        public ILunalipx LPX;
        public IMode LMode;
        public bool modeI18NReq;
        LNetC lnc;
        
        public PluginEntity()
        {
            
        }

        public void initialLNC()
        {
            LNC = new LNetC();
            LNC.OnHttpResponded += (x, y) =>
            {
                y.CallBack.Invoke(x, y.ResultData);
            };
            
        }

        public LNetC LNC
        {
            get
            {
                return lnc;
            }
            private set
            {
                lnc = value;
            }
        }
    }
}
