using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MashupDesignTool;
using System.Windows.Browser;

namespace Present
{
    public partial class MainPage : UserControl
    {
        LoadingWindow lw;
        private bool loaded = false;

        public MainPage()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Appear.Completed += new EventHandler(Appear_Completed);
            Appear.Begin();
        }

        void lw_Closed(object sender, EventArgs e)
        {
            if (loaded == true)
                return;
            if (lw.DialogResult == false)
                NavigateToIndex();
        }

        void Appear_Completed(object sender, EventArgs e)
        {
            lw = new LoadingWindow();
            lw.Closed += new EventHandler(lw_Closed);
            lw.Show();

            Uri uri = HtmlPage.Document.DocumentUri;
            IDictionary<string,string> dic = ParseParams(uri.Query);
            if (dic.ContainsKey("app"))
            {
                bool ret = true;
                Guid id = new Guid();
                try { id = Guid.Parse(dic["app"]); }
                catch { ret = false; }

                DataService.DataServiceClient client = new DataService.DataServiceClient();
                client.GetDataCompleted += new EventHandler<DataService.GetDataCompletedEventArgs>(client_GetDataCompleted);
                client.GetDataAsync(id);

                if (ret == true)
                    return;
            }
            ErrorWindow ew = new ErrorWindow("Unknown application");
            ew.Closed += new EventHandler(ew_Closed);
            ew.Show();
        }

        void client_GetDataCompleted(object sender, DataService.GetDataCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ErrorWindow ew = new ErrorWindow("A error occusr while loading application\r\n" + e.Error.Message);
                ew.Closed += new EventHandler(ew_Closed);
                ew.Show();
                return;
            }
            if (e.Result == null)
            {
                ErrorWindow ew = new ErrorWindow("Unknown application");
                ew.Closed += new EventHandler(ew_Closed);
                ew.Show();
                return;
            }
            Preview(e.Result.XmlString);
        }

        private void Preview(string xml)
        {
            PageSerializer ps = new PageSerializer();
            ps.DeserializeCompleted += new PageSerializer.DeserializeCompletedHandler(ps_DeserializeCompleted);
            ps.Deserialize(xml, dockCanvas1);
        }

        void ps_DeserializeCompleted()
        {
            loaded = true;
            if (lw != null)
                lw.Close();
        }

        void ew_Closed(object sender, EventArgs e)
        {
            if (lw != null)
                lw.Close();
            NavigateToIndex();
        }

        private void NavigateToIndex()
        {
            Uri uri = HtmlPage.Document.DocumentUri;
            Uri uri1 = new Uri(uri, "../index.aspx");
            HtmlPage.Window.Navigate(uri1);
        }

        private IDictionary<string, string> ParseParams(string paramsString)
        {
            if (string.IsNullOrEmpty(paramsString))
                return new Dictionary<string, string>();

            var dict = new Dictionary<string, string>();
            if (paramsString.StartsWith("?"))
                paramsString = paramsString.Substring(1);
            var length = paramsString.Length;

            for (var i = 0; i < length; i++)
            {
                var startIndex = i;
                var pivotIndex = -1;

                while (i < length)
                {
                    char ch = paramsString[i];
                    if (ch == '=')
                    {
                        if (pivotIndex < 0)
                        {
                            pivotIndex = i;
                        }
                    }
                    else if (ch == '&')
                    {
                        break;
                    }
                    i++;
                }

                string name;
                string value;
                if (pivotIndex >= 0)
                {
                    name = paramsString.Substring(startIndex, pivotIndex - startIndex);
                    value = paramsString.Substring(pivotIndex + 1, (i - pivotIndex) - 1);
                }
                else
                {
                    name = paramsString.Substring(startIndex, i - startIndex);
                    value = null;
                }

                dict.Add(HttpUtility.UrlDecode(name), HttpUtility.UrlDecode(value));

                // if string ends with ampersand, add another empty token
                if ((i == (length - 1)) && (paramsString[i] == '&'))
                    dict.Add(null, string.Empty);
            }

            return dict;
        }

    }
}
