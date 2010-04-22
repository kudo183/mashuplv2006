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
using System.ServiceModel.Syndication;
using Liquid;

namespace HienThiListTinTucControl
{
    public partial class RssItemControl : UserControl
    {
        SyndicationItem item;
        public delegate void LinkClicked(object sender, string link);
        public event LinkClicked LinkClickedHandler;

        public RssItemControl()
        {
            InitializeComponent();
        }

        public RssItemControl(SyndicationItem item)
        {
            InitializeComponent();
            Item = item;
        }

        public SyndicationItem Item
        {
            get { return item; }
            set
            {
                item = value;
                RssItemTitle.Load(Format.HTML, "<a href='" + item.Links[0].Uri.AbsoluteUri + "'><u><b>" + item.Title.Text + "</b></u></a>");
                RssItemPubDate.Text = item.PublishDate.ToString();
                string detail = item.Summary.Text.Replace("&lt;", "<");
                detail = detail.Replace("&gt;", ">");
                RssItemDetail.Load(Format.HTML, detail);
            }
        }

        private void RssItemTitle_LinkClicked(object sender, RichTextBoxEventArgs e)
        {
            if (LinkClickedHandler != null)
            {
                LinkClickedHandler(this, (string)e.Parameter.ToString());
            }
        }
    }
}
