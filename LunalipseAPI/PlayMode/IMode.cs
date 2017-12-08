using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunalipseAPI.PlayMode
{
    public interface IMode
    {
        void BeingInitialize();
        void ModeBehavior(ref int MUSIC_SELECTED,int modeID);
        void Deinitialize();
    }
}
