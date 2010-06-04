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
using RssSlideshowControl.ServiceReference1;
using BasicLibrary;
using System.ServiceModel;
using System.Xml;
using System.IO;
using System.ServiceModel.Syndication;
using System.Windows.Threading;

namespace RssSlideshowControl
{
    public partial class RssSlideshow : BasicControl
    {
        public delegate void LinkClickedHander(object sender, string url);
        public event LinkClickedHander LinkClicked;

        private List<RssItem> list = new List<RssItem>();
        private int currentIndex = -1;
        private string rssUrl = "";
        private string feedProxy = "http://localhost:1728/Service1.svc";
        private int delaySeconds = 6;
        DispatcherTimer timer = new DispatcherTimer();
                
        public RssSlideshow()
        {
            InitializeComponent();

            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromSeconds(delaySeconds);

            parameterNameList.Add("RssUrl");
            parameterNameList.Add("LinkColor");
            parameterNameList.Add("SummaryColor");
            parameterNameList.Add("TimeLabelColor");
            parameterNameList.Add("IndexColor");
            parameterNameList.Add("ButtonColor");
            parameterNameList.Add("DelaySeconds");
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Update(true);
        }

        public string RssUrl
        {
            get { return rssUrl; }
            set 
            { 
                rssUrl = value;
                GetRssList();
            }
        }

        public Color LinkColor
        {
            get { return ((SolidColorBrush)hlbLink.Foreground).Color; }
            set
            {
                hlbLink.Foreground = new SolidColorBrush(value);
                hlbTitle.Foreground = new SolidColorBrush(value);
            }
        }

        public Color SummaryColor
        {
            get { return ((SolidColorBrush)rtbSummary.Foreground).Color; }
            set { rtbSummary.Foreground = new SolidColorBrush(value); }
        }

        public Color TimeLabelColor
        {
            get { return ((SolidColorBrush)lbPubDate.Foreground).Color; }
            set { lbPubDate.Foreground = new SolidColorBrush(value); }
        }

        public Color IndexColor
        {
            get { return ((SolidColorBrush)lbIndex.Foreground).Color; }
            set { lbIndex.Foreground = new SolidColorBrush(value); }
        }

        public Color ButtonColor
        {
            get { return ((SolidColorBrush)btnToNext.Foreground).Color; }
            set
            {
                btnToNext.Foreground = new SolidColorBrush(value);
                btnToPrevious.Foreground = new SolidColorBrush(value);
            }
        }

        //public string FeedProxy
        //{
        //    get { return feedProxy; }
        //    set { feedProxy = value; }
        //}

        public int DelaySeconds
        {
            get { return delaySeconds; }
            set
            {
                delaySeconds = value;
            }
        }

        private void GetRssList()
        {
            try
            {
                timer.Stop();
                BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
                EndpointAddress endpointAddress = new EndpointAddress(feedProxy);
                ServiceReference1.Service1Client client = new ServiceReference1.Service1Client(basicHttpBinding, endpointAddress);
                client.GetStringFromURLCompleted += new EventHandler<ServiceReference1.GetStringFromURLCompletedEventArgs>(client_GetStringFromURLCompleted);
                client.GetStringFromURLAsync(rssUrl);
            }
            catch { }
        }

        void client_GetStringFromURLCompleted(object sender, ServiceReference1.GetStringFromURLCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                XmlReader xmlReader = XmlReader.Create(new StringReader(e.Result));
                SyndicationFeed feed = SyndicationFeed.Load(xmlReader);
                list.Clear();

                foreach (SyndicationItem item in feed.Items)
                {
                    RssItem rssItem = new RssItem(item);
                    list.Add(rssItem);
                }

                currentIndex = list.Count > 0 ? 0 : -1;
                Update();
            }
        }

        private void Update()
        {
            if (currentIndex == -1)
            {
                lbIndex.Content = "0/0";
                return;
            }

            timer.Stop();
            lbIndex.Content = string.Format("{0}/{1}", currentIndex + 1, list.Count);
            lbPubDate.Content = list[currentIndex].PubDate;
            hlbLink.Content = list[currentIndex].Link;
            hlbTitle.Content = list[currentIndex].Title;
            rtbSummary.Load(Liquid.Format.HTML, list[currentIndex].Summary);

            StartMainEffect();
            timer.Start();
        }

        private void btnToNext_Click(object sender, RoutedEventArgs e)
        {
            Update(true);
        }

        private void btnToPrevious_Click(object sender, RoutedEventArgs e)
        {
            Update(false);
        }

        private void Update(bool next)
        {
            if (currentIndex == -1)
                return;
            int i = 1;
            if (next == false)
                i = -1;

            currentIndex += i;
            if (currentIndex == -1)
                currentIndex = list.Count - 1;
            else if (currentIndex == list.Count)
                currentIndex = 0;
            Update(); 
        }

        private void hlbTitle_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex == -1)
                return;
            if (LinkClicked != null)
                LinkClicked(this, list[currentIndex].Link);
        }

        private void hlbLink_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex == -1)
                return;
            if (LinkClicked != null)
                LinkClicked(this, list[currentIndex].Link);
        }

        private void rtbSummary_LinkClicked(object sender, Liquid.RichTextBoxEventArgs e)
        {
            if (LinkClicked != null)
                LinkClicked(this, e.Content);
        }

        private void root_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
