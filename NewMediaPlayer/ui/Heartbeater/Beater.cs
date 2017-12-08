using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewMediaPlayer.ui.Heartbeater
{
    public delegate bool Boardcast_HB(int id);
    class Beater
    {
        int ID;
        Thread t;
        public Beater(int id,Action sol)
        {
            Pacer.INSTANCE.RegisteHeartBeat(id, sol);
            ID = id;
        }
        public static event Boardcast_HB BoardcastHB;
        public void startBeating()
        {
            t = new Thread(new ThreadStart(() =>
            {
                while(true)
                {
                    BoardcastHB?.Invoke(ID);
                    Thread.Sleep(1000);
                }
            }));
            t.Start();
        }

        public void kill()
        {
            if (t != null) t.Abort();
        }
    }
}
