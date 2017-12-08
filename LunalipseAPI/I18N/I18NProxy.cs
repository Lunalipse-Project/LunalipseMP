using I18N;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunalipseAPI.I18N
{
    public delegate void REF_0(string k, PageLang v);
    public delegate void REF_1(string r, string i, string l);
    public delegate void RREF_0(string k);
    public delegate void RREF_1(string r, string i);

    public delegate PageLang GRef(string i);
    public delegate string GLang(string r, string i);
    public class I18NProxy
    {
        public static event REF_0 ReferrenceAdded;
        public static event REF_1 LangAdded;
        public static event RREF_0 ReferrenceRemoved;
        public static event RREF_1 LangRemoved;
        public static event GRef GReferrence;
        public static event GLang GLangV;

        public static Languages CURRENT;

        public static void AddReferrence(string RefKey,PageLang PL)
        {
            ReferrenceAdded(RefKey, PL);
        }

        public static void AddLang(string referrenceKey,string indexer,string lang)
        {
            LangAdded(referrenceKey, indexer, lang);
        }
        public static void RemoveReferrence(string RefKey)
        {
            ReferrenceRemoved(RefKey);
        }

        public static void RemoveLang(string referrenceKey, string indexer)
        {
            LangRemoved(referrenceKey, indexer);
        }

        public static PageLang GetReferrence(string indexer)
        {
            return GReferrence(indexer);
        }

        public static string GetLang(string referrence,string indexer)
        {
            return GLangV(referrence, indexer);
        }
    }
}
