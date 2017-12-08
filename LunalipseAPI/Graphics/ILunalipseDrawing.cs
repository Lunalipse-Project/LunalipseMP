using LunalipseAPI.Graphics.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunalipseAPI.Graphics
{
    public interface ILunalipseDrawing
    {
        /// <summary>
        /// Only Draw when the Plugin first load.
        /// You can call your own drawing function in the Initialize() method is the entry point
        /// </summary>
        void InitialDraw();

        /// <summary>
        /// Call by host when each new window instance were created.
        /// </summary>
        void UIDraw(LunalipseTarget lt);

        void UIUndraw();
    }
}
