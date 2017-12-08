using LpxResource.LRTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LpxResource.Generic
{
    public delegate void UpdateStatus(EvtType et, long args);
    public delegate void ParsingError(EErrors s, params string[] args);
    public delegate void ReadDBlock(long num, long ptr);
    public class EventHoster
    {
        public static event UpdateStatus onStatusUpdate;
        public static event ParsingError onErrOcurr;
        public static event ReadDBlock onAllDBlockRead;

        internal static void IStatusUpdate(EvtType et, long args)
        {
            onStatusUpdate?.Invoke(et, args);
        }
        internal static void IErrOcurr(EErrors s, params string[] args)
        {
            onErrOcurr?.Invoke(s, args);
        }
        internal static void IAllDBlockRead(long num, long ptr)
        {
            onAllDBlockRead?.Invoke(num, ptr);
        }
    }
}
