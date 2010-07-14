using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;
using System.Windows.Data;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BasicLibrary
{
    public class Ultility
    {
        private Uri _ServerURL;

        public Uri ServerURL
        {
            get { return _ServerURL; }
            set { _ServerURL = value; }
        }

        public Ultility()
        {
            _ServerURL = new Uri(HtmlPage.Document.DocumentUri, "../index.aspx");
        }

        #region GetImage
        public delegate void GetImageAsyncCompletedHandler(byte[] result);
        public event GetImageAsyncCompletedHandler OnGetImageAsyncCompleted;
        public void GetImageAsync(string URL)
        {
            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_GetImageOpenReadCompleted);
            Uri xmlUri = new Uri(_ServerURL, "GetBinaryData.ashx?URL=" + URL);
            webClient.OpenReadAsync(xmlUri);
        }

        void webClient_GetImageOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (OnGetImageAsyncCompleted != null)
            {   
                OnGetImageAsyncCompleted(ReadStream(e.Result));
            }
        }
        #endregion

        #region GetListDataFromDatabase
        public delegate void GetListDataFromDatabaseAsyncCompletedHandler(List<List<string>> result);
        public event GetListDataFromDatabaseAsyncCompletedHandler OnGetListDataFromDatabaseAsyncCompleted;
        public void GetListDataFromDatabaseAsync(string server, string username, string pass, string db, string table, int startIndex, int count)
        {
            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_GetListDataFromDatabaseOpenReadCompleted);
            
            string url = "GetListDataFromDatabase.ashx?";
            url += "SERVER=" + server;
            url += "&USER=" + username;
            url += "&PASS=" + pass;
            url += "&DB=" + db;
            url += "&TABLE=" + table;
            url += "&INDEX=" + startIndex;
            url += "&COUNT=" + count;

            Uri xmlUri = new Uri(_ServerURL, url);
            webClient.OpenReadAsync(xmlUri);
        }

        void webClient_GetListDataFromDatabaseOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (OnGetListDataFromDatabaseAsyncCompleted != null)
            {
                XmlSerializer xm = new XmlSerializer(typeof(List<List<string>>));                
                List<List<string>> result = xm.Deserialize(e.Result) as List<List<string>>;
                OnGetListDataFromDatabaseAsyncCompleted(result);
            }
        }
        #endregion

        #region GetListDataFromDatabaseStructure
        public delegate void GetListDataFromDatabaseStructureAsyncCompletedHandler(string result);
        public event GetListDataFromDatabaseStructureAsyncCompletedHandler OnGetListDataFromDatabaseStructureAsyncCompleted;
        public void GetListDataFromDatabaseStructureAsync(string server, string username, string pass, string db, string table)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_GetListDataFromDatabaseStructureDownloadStringCompleted);

            string url = "GetListDataFromDatabaseStructure.ashx?";
            url += "SERVER=" + server;
            url += "&USER=" + username;
            url += "&PASS=" + pass;
            url += "&DB=" + db;
            url += "&TABLE=" + table;

            Uri xmlUri = new Uri(_ServerURL, url);
            webClient.DownloadStringAsync(xmlUri);
        }

        void webClient_GetListDataFromDatabaseStructureDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (OnGetListDataFromDatabaseStructureAsyncCompleted != null)
            {
                OnGetListDataFromDatabaseStructureAsyncCompleted(e.Result);
            }
        }
        #endregion

        #region GetListDataFromXml
        public delegate void GetListDataFromXmlAsyncCompletedHandler(List<List<string>> result);
        public event GetListDataFromXmlAsyncCompletedHandler OnGetListDataFromXmlAsyncCompleted;
        public void GetListDataFromXmlAsync(string xmlUrl, string elementName, int startIndex, int count)
        {
            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_GetListDataFromXmlOpenReadCompleted);

            string url = "GetListDataFromXml.ashx?";
            url += "URL=" + xmlUrl;
            url += "&ELEMENT=" + elementName;
            url += "&INDEX=" + startIndex;
            url += "&COUNT=" + count;

            Uri xmlUri = new Uri(_ServerURL, url);
            webClient.OpenReadAsync(xmlUri);
        }

        void webClient_GetListDataFromXmlOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (OnGetListDataFromXmlAsyncCompleted != null)
            {
                XmlSerializer xm = new XmlSerializer(typeof(List<List<string>>));
                List<List<string>> result = xm.Deserialize(e.Result) as List<List<string>>;
                OnGetListDataFromXmlAsyncCompleted(result);
            }
        }
        #endregion

        #region GetListDataFromXmlStructure
        public delegate void GetListDataFromXmlStructureAsyncCompletedHandler(string result);
        public event GetListDataFromXmlStructureAsyncCompletedHandler OnGetListDataFromXmlStructureAsyncCompleted;
        public void GetListDataFromXmlStructureAsync(string xmlUrl, string elementName)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadXmlStructureStringCompleted);

            string url = "GetListDataFromXmlStructure.ashx?";
            url += "URL=" + xmlUrl;
            url += "&ELEMENT=" + elementName;

            Uri xmlUri = new Uri(_ServerURL, url);
            webClient.DownloadStringAsync(xmlUri);
        }

        void webClient_DownloadXmlStructureStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (OnGetListDataFromXmlStructureAsyncCompleted != null)
            {
                OnGetListDataFromXmlStructureAsyncCompleted(e.Result);
            }
        }
        
        #endregion
        
        #region GetString
        public delegate void GetStringAsyncCompletedHandler(string result);
        public event GetStringAsyncCompletedHandler OnGetStringAsyncCompleted;
        public void GetStringAsync(string URL)
        {
            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_OpenReadCompleted);
            Uri xmlUri = new Uri(_ServerURL, "GetStringDataFromURL.ashx?URL=" + URL);
            webClient.OpenReadAsync(xmlUri);
        }

        void webClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (OnGetStringAsyncCompleted != null)
            {
                //StreamReader sr = new StreamReader(e.Result);
                //OnGetStringAsyncCompleted(sr.ReadToEnd());
                XmlSerializer xm = new XmlSerializer(typeof(string));
                OnGetStringAsyncCompleted((string)xm.Deserialize(e.Result));
            }
        }
        #endregion

        private byte[] ReadStream(Stream s)
        {
            byte[] result = new byte[s.Length];

            BinaryReader br = new BinaryReader(s);
            result = br.ReadBytes((int)s.Length);

            return result;
        }

        public static void RegisterForNotification(string propertyName, FrameworkElement element, PropertyChangedCallback callback)
        {
            //Bind to a depedency property
            Binding b = new Binding(propertyName) { Source = element };
            var prop = System.Windows.DependencyProperty.RegisterAttached(
                "ListenAttached" + propertyName,
                typeof(object),
                typeof(UserControl),
                new System.Windows.PropertyMetadata(callback));
            element.SetBinding(prop, b);
        }
    }
}
