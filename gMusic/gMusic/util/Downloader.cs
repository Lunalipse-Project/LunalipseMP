using System;
using System.Net;

namespace gMusic.util
{
    public class Downloader
    {
        public delegate void ODF(bool gotError,Exception e);
        public delegate void DataSetup(long d);
        public delegate void Update(long d);
        public event ODF OnDownloadFinish;
        public event DataSetup OnDataSetup;
        public event Update OnTaskUpdate;
        bool isFirstTime = true;
        WebClient wc = new WebClient();

        public Downloader()
        {
            
            /*wc.DownloadProgressChanged += (x, y) =>
            {
                if(isFirstTime)
                {
                    OnDataSetup(y.TotalBytesToReceive);
                    isFirstTime = false;
                }
                OnTaskUpdate(y.BytesReceived);
            };
            wc.DownloadFileCompleted += (x,y) =>
            {
                isFirstTime = true;
                OnDownloadFinish(y.Error != null, y.Error);
            };*/
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="URL">文件URL</param>
        /// <param name="filename">文件保存路径</param>
        /// <param name="prog"></param>
        /// <param name="label1"></param>
        public void DownloadFile(string URL, string filename,long tb)
        {
            //wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:53.0) Gecko/20100101 Firefox/53.0");
            //wc.DownloadFile(new Uri(URL), filename);
            try
            {
                System.Net.HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);
                Myrq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:53.0) Gecko/20100101 Firefox/53.0";
                Myrq.Method = "GET";
                Myrq.Credentials = CredentialCache.DefaultNetworkCredentials;

                System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse();
                long totalBytes = myrp.ContentLength;
                OnDataSetup(totalBytes);
                System.IO.Stream st = myrp.GetResponseStream();
                System.IO.Stream so = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                long totalDownloadedByte = 0;
                byte[] by = new byte[1024];
                int osize = 0;
                while(totalDownloadedByte<totalBytes)
                {
                    osize = st.Read(by, 0, (int)by.Length);
                    while (osize > 0)
                    {
                        totalDownloadedByte = osize + totalDownloadedByte;
                        so.Write(by, 0, osize);
                        OnTaskUpdate(totalDownloadedByte);
                        osize = st.Read(by, 0, (int)by.Length);
                        Console.WriteLine(osize + "|" + totalDownloadedByte + "|" + totalBytes);
                    }
                }
                so.Close();
                st.Close();
                OnDownloadFinish(false, null);
            }
            catch (System.Exception e)
            {
                OnDownloadFinish(true, e);
            }
        }
    }
}
