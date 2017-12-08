using I18N;
using LunalipseAPI.I18N;

namespace NewMediaPlayer.PluginHoster
{
    public class I18NBridge
    {
        PluginHelper PH_;
        I18NHelper NH;
        public I18NBridge(PluginHelper ph)
        {
            PH_ = ph;
            NH = I18NHelper.INSTANCE;
            I18NProxy.CURRENT = global.LANG ?? Languages.CHINESE;
            REvent();
        }

        private void REvent()
        {
            I18NProxy.GLangV += (a, b) =>
            {
                return NH.GetReferrence(a).GetContent(b);
            };
            I18NProxy.GReferrence += (a) =>
            {
                return NH.GetReferrence(a);
            };
            I18NProxy.LangAdded += (a, b, c) =>
            {
                NH.GetReferrence(a).AddToLang(b, c);
            };
            I18NProxy.ReferrenceAdded += (a, b) =>
            {
                NH.AddReferrence(a, b);
            };
            I18NProxy.LangRemoved += (a, b) =>
            {
                NH.GetReferrence(a).RemoveFromLang(b);
            };
            I18NProxy.ReferrenceRemoved += (a) =>
            {
                NH.RemoveReferrence(a);
            };
        }
    }
}
