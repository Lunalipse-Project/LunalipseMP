using System;
namespace I18N
{
    public enum Languages
    {
        CHINESE,
        TRADITIONAL,
        ENGLISH,
        RUSSIAN
    }

    public class LanguagesUtil
    {
        public static Languages? GetLanguage(int i)
        {
            foreach (Languages l in Enum.GetValues(typeof(Languages)))
            {
                if ((int)l == i) return l;
            }
            return null;
        }
    }
}
