using LunalipseAPI.Graphics.Generic;

namespace LunalipseAPI.Graphics
{
    public class LButton:LControl
    {
        EventBus.ONCLICK OnClickEvt;
        public LButton()
        {
            
        }
        public EventBus.ONCLICK ButtonEvent
        {
            get
            {
                return OnClickEvt;
            }
            set
            {
                OnClickEvt = value;
            }
        }
    }
}
