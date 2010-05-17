using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ServiceModel.Syndication;

namespace RssSlideshowControl
{
    public class RssItem
    {
        private string link;
        private string summary;
        private string title;
        private DateTimeOffset pubDate;
        private string formatDate;

        public RssItem()
        {
            link = "http:\\link.com";
            summary = "summary";
            title = "title";
            pubDate = new DateTimeOffset(new DateTime(2010, 10, 30));
        }

        public RssItem(SyndicationItem si) : this(si, "") { }

        internal RssItem(SyndicationItem si, string formatDate)
        {
            link = si.Links[0].Uri.AbsoluteUri;
            summary = si.Summary.Text;
            title = si.Title.Text;
            pubDate = si.PublishDate;
            this.formatDate = formatDate;
        }

        public string Link
        {
            get { return link; }
        }

        public string Summary
        {
            get { return summary; }
        }

        public string Title
        {
            get { return title; }
        }

        public string PubDate
        {
            get { return pubDate.ToString(formatDate); }
        }
    }
}
