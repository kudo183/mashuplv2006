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
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BasicLibrary
{
    public class Ultility
    {
        public delegate void GetImageAsyncCompletedHandler(byte[] result);
        public event GetImageAsyncCompletedHandler OnGetImageAsyncCompleted;
        public void GetImageAsync(string URL)
        {
            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_GetImageOpenReadCompleted);
            Uri xmlUri = new Uri(HtmlPage.Document.DocumentUri, "GetBinaryData.ashx?URL=" + URL);
            webClient.OpenReadAsync(xmlUri);
        }

        void webClient_GetImageOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (OnGetImageAsyncCompleted != null)
            {   
                OnGetImageAsyncCompleted(ReadStream(e.Result));
            }
        }

        public delegate void GetListDataFromDatabaseAsyncCompletedHandler(List<List<string>> result);
        public event GetListDataFromDatabaseAsyncCompletedHandler OnGetListDataFromDatabaseAsyncCompleted;
        public void GetListDataFromDatabaseAsync(string server, string username, string pass, string db, string table)
        {
            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_GetListDataFromDatabaseOpenReadCompleted);
            
            string url = "GetListDataFromDatabase.ashx?";
            url += "SERVER=" + server;
            url += "&USER=" + username;
            url += "&PASS=" + pass;
            url += "&DB=" + db;
            url += "&TABLE=" + table;

            Uri xmlUri = new Uri(HtmlPage.Document.DocumentUri, url);
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

        public delegate void GetListDataFromXmlAsyncCompletedHandler(List<List<string>> result);
        public event GetListDataFromXmlAsyncCompletedHandler OnGetListDataFromXmlAsyncCompleted;
        public void GetListDataFromXmlAsync(string xmlUrl, string elementName)
        {
            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_GetListDataFromXmlOpenReadCompleted);

            string url = "GetListDataFromXml.ashx?";
            url += "URL=" + xmlUrl;
            url += "&ELEMENT=" + elementName;

            Uri xmlUri = new Uri(HtmlPage.Document.DocumentUri, url);
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

        private byte[] ReadStream(Stream s)
        {
            byte[] result = new byte[s.Length];

            BinaryReader br = new BinaryReader(s);
            result = br.ReadBytes((int)s.Length);

            return result;
        }

        public delegate void GetStringAsyncCompletedHandler(string result);
        public event GetStringAsyncCompletedHandler OnGetStringAsyncCompleted;
        public void GetStringAsync(string URL)
        {
            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_OpenReadCompleted);
            Uri xmlUri = new Uri(HtmlPage.Document.DocumentUri, "GetStringDataFromURL.ashx?URL=" + URL);
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
    }
}
