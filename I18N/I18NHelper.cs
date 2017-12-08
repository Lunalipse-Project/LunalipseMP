using LpxResource.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace I18N
{
    public class I18NHelper
    {
        public volatile static I18NHelper _I18N_INSTANCE = null;
        public static readonly object I18N_OBJ = new object();

        Languages langs;

        IDictionary<string, PageLang> _applang;
        public static I18NHelper INSTANCE
        {
            get
            {
                if (_I18N_INSTANCE == null)
                {
                    lock (I18N_OBJ)
                    {
                        if (_I18N_INSTANCE == null)
                        {
                            return _I18N_INSTANCE = new I18NHelper();
                        }
                    }
                }
                return _I18N_INSTANCE;
            }
        }

        private I18NHelper()
        {
            _applang = new Dictionary<string, PageLang>();
        }

        public Languages LANGUAGES
        {
            get
            {
                return langs;
            }
            set
            {
                langs = value;
            }
        }

        public bool parseLangFile()
        {
            XmlReaderSettings xrs = new XmlReaderSettings();
            xrs.IgnoreComments = true;
            string final = "";
            switch (langs)
            {
                case Languages.CHINESE:
                    if (File.Exists("i18n/zh_cn.lnpl")) final = "i18n/zh_cn.lnpl";
                    else return false;
                    break;
                case Languages.ENGLISH:
                    if (File.Exists("i18n/en_us.lnpl")) final = "i18n/en_us.lnpl";
                    else return false;
                    break;
                case Languages.RUSSIAN:
                    if (File.Exists("i18n/ru_ru.lnpl")) final = "i18n/ru_ru.lnpl";
                    else return false;
                    break;
                case Languages.TRADITIONAL:
                    if (File.Exists("i18n/zh_hk.lnpl")) final = "i18n/zh_hk.lnpl";
                    else return false;
                    break;
                default:
                    final = "i18n/zh_cn.lnpl";
                    break;
            }
            return __load(XmlReader.Create(final,xrs));
        }

        public bool parseLangFile(ResourceCollection rc)
        {
            XmlReaderSettings xrs = new XmlReaderSettings();
            xrs.IgnoreComments = true;
            if (rc == null) return false;
            Stream ts;
            switch (langs)
            {
                case Languages.CHINESE:
                    ts = rc.getResourceSN("zh_cn");
                    break;
                case Languages.ENGLISH:
                    ts = rc.getResourceSN("en_us");
                    break;
                case Languages.RUSSIAN:
                    ts = rc.getResourceSN("ru_ru");
                    break;
                case Languages.TRADITIONAL:
                    ts = rc.getResourceSN("zh_hk");
                    break;
                default:
                    ts = rc.getResourceSN("en_us");
                    break;
            }
            if (ts == null) return false;
            return __load(XmlReader.Create(ts,xrs));
        }

        private bool __load(XmlReader xr)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(xr);
                XmlNode xmln = doc.SelectSingleNode("Lunalipse");
                foreach (XmlNode xn in xmln.ChildNodes)
                {
                    XmlElement xe = (XmlElement)xn;
                    string name = xe.GetAttribute("CompntName");
                    PageLang pl = new PageLang();
                    foreach (XmlNode xn_ in xe.ChildNodes)
                    {
                        XmlElement xe_ = (XmlElement)xn_;
                        pl.AddToLang(xe_.Name, xe_.InnerText);
                    }
                    _applang.Add(name, pl);
                }
                return true;
            }
            catch (Exception e)
            {
                //LogFile.WriteLog("ERROR", e.Message);
                return false;
            }
        }

        public PageLang GetReferrence(string ComponentName)
        {
            return _applang[ComponentName];
        }

        public void AddReferrence(string inx,PageLang pl)
        {
            _applang.Add(inx, pl);
        }

        public void RemoveReferrence(string inx)
        {
            _applang.Remove(inx);
        }
    }

    public class PageLang
    {
        IDictionary<string, string> _lang;
        public PageLang()
        {
            _lang = new Dictionary<string, string>();
        }

        public void AddToLang(string k, string v)
        {
            _lang.Add(k, v);
        }

        public void RemoveFromLang(string k)
        {
            _lang.Remove(k);
        }

        public string GetContent(string indexer)
        {
            return _lang[indexer];
        }

        public IDictionary<string, string> AllLang
        {
            get { return _lang; }
        }
    }
}
