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
using System.Windows.Threading;
using Effect;

namespace HienThiListTinTucControl
{
    public partial class RssItemListControl : UserControl
    {
        public delegate void LinkClickedHandler(object sender, string link);
        public event LinkClickedHandler LinkClicked;
        public delegate void ContentChoiseHandler(object sender, string data);
        public event ContentChoiseHandler ContentChoise;

        double itemwidth, itemheight;
        bool isMovingToLeft;
        int numItemPerView = 1;
        int currView = 0;
        int numView = 0;
        List<SplitScreenEffectControl> listItems = new List<SplitScreenEffectControl>();
        double dx = 0;

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
                ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
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

                listItems.Clear();
                foreach (SyndicationItem item in feed.Items)
                {
                    RssItemControl ric = RssItemControl.Create(item);
                    if (ric != null)
                    {
                        SplitScreenEffectControl effect = new SplitScreenEffectControl(ric, SplitScreenEffectControl.SplitDirection.VERTICAL);
                        Canvas.SetTop(effect, 2);
                        listItems.Add(effect);
                        ric.Width = itemwidth;
                        ric.Height = itemheight;
                        ric.LinkClickedHandler += new RssItemControl.LinkClicked(ric_LinkClickedHandler);
                        ric.ContentChoiseHandler += new RssItemControl.ContentChoise(ric_ContentChoiseHandler);
                    }
                }
                numView = listItems.Count / numItemPerView + 1;
                currView = 0;
                UpdateViewList();
                UpdateButtonEnable();
            }
        }

        void ric_ContentChoiseHandler(object sender, string data)
        {
            if (ContentChoise != null)
                ContentChoise(sender, data);
        }

        void ric_LinkClickedHandler(object sender, string link)
        {
            if (LinkClicked != null)
                LinkClicked(sender, link);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

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

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetClipRegion();

            for (int i = 0; i < listItems.Count; i++)
            {
                listItems[i].Width = itemwidth;
                listItems[i].Height = itemheight;
            }
            UpdateViewList();
        }

        private void SetClipRegion()
        {
            double width = listRssItem.ActualWidth;
            double height = listRssItem.ActualHeight;

            if (width > 0 && height > 0)
            {
                RectangleGeometry rg = new RectangleGeometry();
                rg.Rect = new Rect(0, 0, width, height);
                listRssItem.Clip = rg;
            }

            if (width != 0 && height >= 4)
            {
                itemheight = height - 4;
                itemwidth = itemheight * 1.2;
                numItemPerView = (int)(width / itemwidth);
                dx = (width - (numItemPerView * itemwidth)) / numItemPerView;
            }
            else
            {
                itemheight = 4;
                itemwidth = 6;
                numItemPerView = 1;
                dx = 0;
            }
        }

        private void UpdateViewList()
        {
            listRssItem.Children.Clear();

            double x = -listRssItem.ActualWidth + (dx / 2);
            for (int i = -1; i <= 1; i++)
            {
                int view = currView + i;
                if (view >= 0 && view < numView)
                {
                    int k = view * numItemPerView;
                    for (int j = 0; j < numItemPerView; j++)
                    {
                        Canvas.SetLeft(listItems[k], x);
                        listRssItem.Children.Add(listItems[k]);
                        x += itemwidth + dx;
                        k++;
                    }
                }
                else
                {
                    x += listRssItem.ActualWidth;
                }
            }
        }

        private void RunAnimation(bool moveLeft)
        {
            BasicMoveEffect[] effects = new BasicMoveEffect[listRssItem.Children.Count];
            EnableButton(LeftButton, LeftButtonDisable, false);
            EnableButton(RighttButton, RighttButtonDisable, false);
            double deltax = listRssItem.ActualWidth;
            if (!moveLeft)
                deltax = -listRssItem.ActualWidth;

            int n = listRssItem.Children.Count;
            for (int i = 0; i < n; i++)
            {
                UIElement element = listRssItem.Children[i];
                Point begin = new Point(Canvas.GetLeft(element), Canvas.GetTop(element));
                Point end = new Point(Canvas.GetLeft(element) + deltax, Canvas.GetTop(element));
                BasicMoveEffect.AttachEffect(element, begin, end, BasicMoveEffect.BasicMoveEffectSpeed.NORMAL);
            }
            for (int i = 0; i < n; i++)
                BasicMoveEffect.Start(listRssItem.Children[i]);
            if (n > 0)
            {
                Storyboard sb = BasicMoveEffect.GetStoryboard(listRssItem.Children[n - 1]);
                if (sb != null)
                    sb.Completed += new EventHandler(sb_Completed);
            }
        }

        void sb_Completed(object sender, EventArgs e)
        {
            UpdateViewList();
            UpdateButtonEnable();
            Storyboard sb = BasicMoveEffect.GetStoryboard(listRssItem.Children[listRssItem.Children.Count - 1]);
            if (sb != null)
                sb.Completed -= new EventHandler(sb_Completed);
        }

        void RssItemListControl_EffectComplete(object sender, UIElement element)
        {
            UpdateViewList();
            UpdateButtonEnable();
        }

        private void UpdateButtonEnable()
        {
            bool left, right;
            left = right = false;
            if (listRssItem.Children.Count != 0)
            {
                if (currView > 0)
                    left = true;
                if (currView < numView - 1)
                    right = true;
            }

            EnableButton(LeftButton, LeftButtonDisable, left);
            EnableButton(RighttButton, RighttButtonDisable, right);
        }

        private void EnableButton(Button normal, Button disable, bool b)
        {
            normal.IsEnabled = b;
            disable.IsEnabled = !b;
            if (b)
            {
                normal.Visibility = System.Windows.Visibility.Visible;
                disable.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                normal.Visibility = System.Windows.Visibility.Collapsed;
                disable.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void RighttButton_Click(object sender, RoutedEventArgs e)
        {
            currView++;
            isMovingToLeft = false;
            RunAnimation(isMovingToLeft);
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            currView--;
            isMovingToLeft = true;
            RunAnimation(isMovingToLeft);
        }
    }
}
