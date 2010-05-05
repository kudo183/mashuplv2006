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

namespace NewsReaderControl
{
    public partial class NewsPage : UserControl
    {
        public delegate void OnLoadPageComplete(NewsPage sender);
        public event OnLoadPageComplete LoadPageComplete;

        string url = "";

        public NewsPage()
        {
            InitializeComponent();
            page.SetDefaultStyles();
        }

        public string PageURL
        {
            get { return url; }
            set 
            {
                url = value;
                ServiceReference1.MashupToolWCFServiceClient client = new ServiceReference1.MashupToolWCFServiceClient();
                client.GetStringFromURLCompleted += new EventHandler<ServiceReference1.GetStringFromURLCompletedEventArgs>(client_GetStringFromURLCompleted);
                client.GetStringFromURLAsync(url);
            }
        }

        public void LoadHTML(string html)
        {
            page.Load(html);
        }

        void client_GetStringFromURLCompleted(object sender, ServiceReference1.GetStringFromURLCompletedEventArgs e)
        {
            try
            {
                string data = e.Result.Substring(e.Result.IndexOf("<html>"));
                page.Load(data);
                if (LoadPageComplete != null)
                    LoadPageComplete(this);
            }
            catch { }
        }

        private void root_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
