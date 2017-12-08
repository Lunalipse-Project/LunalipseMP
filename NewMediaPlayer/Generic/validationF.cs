using FileSecurity;
using I18N;
using NewMediaPlayer.Dialog;
using System;

namespace NewMediaPlayer.Generic
{
    public class validationF
    {
        FEntrance fe;
        PageLang PL;
        public validationF()
        {
            fe = new FEntrance();
            PL = I18NHelper.INSTANCE.GetReferrence("valChk");
        }

        public bool Validation()
        {
#if TRACE
            return true;
#endif
            if(!".vldF".DExist(FType.FILE))
            {
                new LDailog(controler.LunalipsContentUI.TIPMESSAGE, PL.GetContent("title"), PL.GetContent("c1")).ShowDialog();
                return false;
            }
            STATUS s = fe.ValidateMD5(".vldF", AppDomain.CurrentDomain.BaseDirectory + @"\");
            if (!s.Success)
            {
                if (s.scode == 2)
                {
                    new LDailog(controler.LunalipsContentUI.TIPMESSAGE, PL.GetContent("title"), PL.GetContent("c3")).ShowDialog();
                }
                else if (s.scode == 1)
                {
                    new LDailog(controler.LunalipsContentUI.TIPMESSAGE, PL.GetContent("title"), PL.GetContent("c2")).ShowDialog();
                }
                return false;
            }
            return true;
        }
    }
}
