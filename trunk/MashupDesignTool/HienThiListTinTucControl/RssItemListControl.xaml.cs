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
using System.Xml;
using System.ServiceModel.Syndication;
using System.Windows.Media.Imaging;
using System.IO;

namespace HienThiListTinTucControl
{
    public partial class RssItemListControl : UserControl
    {
        public delegate void LinkClicked(object sender, string link);
        public event LinkClicked LinkClickedHandler;

        string rssURL;

        public RssItemListControl()
        {
            InitializeComponent();
        }

        public string RssURL
        {
            get { return rssURL; }
            set
            {
                rssURL = value;
                ServiceReference1.MashupToolWCFServiceClient client = new ServiceReference1.MashupToolWCFServiceClient();
                client.GetStringFromURLCompleted += new EventHandler<ServiceReference1.GetStringFromURLCompletedEventArgs>(client_GetStringFromURLCompleted);
                client.GetStringFromURLAsync(rssURL);
            }
        }

        void client_GetStringFromURLCompleted(object sender, ServiceReference1.GetStringFromURLCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                XmlReader xmlReader = XmlReader.Create(new StringReader(e.Result));
                SyndicationFeed feed = SyndicationFeed.Load(xmlReader);

                channelTitle.Text = feed.Title.Text;
                channelImage.Source = new BitmapImage(feed.ImageUrl);

                double x, y;
                x = y = 0;
                foreach (SyndicationItem item in feed.Items)
                {
                    RssItemControl ric = new RssItemControl(item);
                    listRssItem.Items.Add(ric);
                    ric.LinkClickedHandler += new RssItemControl.LinkClicked(ric_LinkClickedHandler);
                }
            }
        }

        void ric_LinkClickedHandler(object sender, string link)
        {
            if (LinkClickedHandler != null)
                LinkClickedHandler(sender, link);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            double width, height;
            width = height = 0;
            if (!double.IsNaN(this.ActualHeight) && this.ActualHeight != 0 && this.ActualWidth != 0)
            {
                width = this.ActualWidth;
                height = this.ActualHeight;
            }
            else if (!double.IsNaN(this.Height) && this.Width != 0 && this.Height != 0)
            {
                width = this.Width;
                height = this.Height; 
            }
            if (width != 0 && height != 0)
            {
                height -= 50;
                double itemheight = height - 10;
                double itemwidth = itemheight * 1.5;
                if (itemwidth / width < 2)
                    itemwidth = itemwidth < width / 2 ? itemwidth : width / 2;
                listRssItem.SetWidthHeight(width, height, itemwidth, itemheight);
            }
        }

        public Brush Background
        {
            get { return LayoutRoot.Background; }
            set { LayoutRoot.Background = value; }
        }

        public Brush ChannelTitleColor
        {
            get { return channelTitle.Foreground; }
            set { channelTitle.Foreground = value; }
        }
    }
}
