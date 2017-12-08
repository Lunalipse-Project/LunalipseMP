using LunalipseAPI.Generic;
using System;
using System.Collections;

namespace LunalipseAPI
{
    public delegate void Volume(double VOL);
    public delegate void VolAdjB(double sign);
    public delegate void PlayMusic(int index);
    public delegate void PlayM0de(int mode);
    public delegate void Position(long p);
    public delegate void SetML(ArrayList musicl);
    public interface MainUI
    {
        /// <summary>
        /// 插件入口函数，Lunalipse会最先调用这个函数执行初始化
        /// </summary>
        void Initialize();
        /// <summary>
        /// 销毁函数，在插件被停用时，Lunalipse会调用这个函数执行资源回收
        /// </summary>
        void Destroy();
        /// <summary>
        /// 获取歌单列表
        /// <para>
        /// Lunalipse会在插件加载时传入歌单列表
        /// </para>
        /// </summary>
        /// <param name="ml">传入的歌单列表</param>
        void GrabMusicList(ArrayList ml);
        void MusicChange(PlayInfo pi);
        void VolumeChange(double vol);
        void ModeChange(int mode);
        void LunalipseExit();

    }

    public class MainUIEvent
    {
        public static event Volume SetVolume;
        public static event PlayMusic PlayMusic;
        public static event PlayM0de SetPlayMode;
        public static event Position SetPosition;
        public static event VolAdjB AdjustVolBit;
        public static event SetML SetMuL;

        public void Invoke(Setter s,params object[] args)
        {
            switch(s)
            {
                case Setter.VOL:
                    SetVolume(Convert.ToDouble(args[0]));
                    break;
                case Setter.MODE:
                    SetPlayMode(Convert.ToInt32(args[0]));
                    break;
                case Setter.MUSIC:
                    PlayMusic(Convert.ToInt32(args[0]));
                    break;
                case Setter.POSITION:
                    SetPosition(Convert.ToInt64(args[0]));
                    break;
                case Setter.INCS_VOL:
                    AdjustVolBit(0.05);
                    break;
                case Setter.DECS_VOL:
                    AdjustVolBit(-0.05);
                    break;
                case Setter.SET_MUSICL:
                    SetMuL(args[0] == null ? null : args[0] as ArrayList);
                    break;
            }
        }
    }
}