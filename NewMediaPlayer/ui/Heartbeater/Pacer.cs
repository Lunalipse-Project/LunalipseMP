using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewMediaPlayer.ui.Heartbeater
{
   
    internal class Pacer
    {
        public static volatile Pacer PACER_;
        public static readonly object _LOCK = new object();
        public static Pacer INSTANCE
        {
            get
            {
                if(_LOCK!=null)
                {
                    lock(_LOCK)
                    {
                        if (PACER_ == null) return PACER_ = new Pacer();
                    }
                }
                return PACER_;
            }
        }
        Dictionary<int, Heart> registed = new Dictionary<int, Heart>();
        public const int TIME_OUT = 2000;
        Thread t;

        public Pacer()
        {
            Beater.BoardcastHB += (i) =>
            {
                if (!registed.ContainsKey(i)) return false;
                registed[i].dt = DateTime.Now;
                return true;
            };
        }

        public bool RegisteHeartBeat(int id,Action failure)
        {
            if (!registed.ContainsKey(id))
            {
                registed.Add(id, new Heart()
                {
                    fail = failure,
                    dt = DateTime.Now
                });
                return true;
            }
            return false;
        }

        public void Listen(MainWindow mw)
        {
            if (t != null) return;
            t = new Thread(new ThreadStart(() => {
                //循环检测心跳
                try
                {
                    while (true)
                    {
                        foreach (var i in registed)
                        {
                            //如果距离上次心跳时间小于规定时间（2秒）
                            if ((DateTime.Now - i.Value.dt).TotalMilliseconds > TIME_OUT)
                            {
                                //启用起搏器
                                mw.Dispatcher.Invoke(() =>
                                {
                                    i.Value.fail.Invoke();
                                });
                            }
                        }
                        Thread.Sleep(TIME_OUT);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }));
            t.Start();
        }

        public void Stop()
        {
            if (t != null) t.Abort();
        }
    }
}
