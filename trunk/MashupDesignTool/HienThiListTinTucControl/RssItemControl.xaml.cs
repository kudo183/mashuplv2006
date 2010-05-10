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
using Effect;
using System.Xml;
using System.IO;
using System.Text;

namespace HienThiListTinTucControl
{
    public partial class RssItemControl : UserControl
    {
        public delegate void LinkClicked(object sender, string link);
        public event LinkClicked LinkClickedHandler;

        public delegate void ContentChoise(object sender, string data);
        public event ContentChoise ContentChoiseHandler;

        SyndicationItem item;

        internal RssItemControl()
        {
            InitializeComponent();
        }

        public static RssItemControl Create(SyndicationItem item)
        {
            RssItemControl control = new RssItemControl();
            control.Item = item;
            control.RssItemTitle.Load(Format.HTML, "<a href='" + item.Links[0].Uri.AbsoluteUri + "'><u><b>" + item.Title.Text + "</b></u></a>");
            control.RssItemPubDate.Text = item.PublishDate.ToString();
            string summary = item.Summary.Text.Replace(" &", " -");
            summary = item.Summary.Text.Replace("& ", "- ");

            XmlReader reader = XmlReader.Create(new StringReader("<content>" + summary + "</content>"));
            bool b = true;
            try { while (reader.Read()); }
            catch { b = false; }

            if (b == false)
                return null;
            control.RssItemDetail.Load(Format.HTML, summary);
            return control;
        }

        public SyndicationItem Item
        {
            get { return item; }
            internal set { item = value; }
        }

        private void RssItemTitle_LinkClicked(object sender, RichTextBoxEventArgs e)
        {
            if (LinkClickedHandler != null)
            {
                LinkClickedHandler(this, (string)e.Parameter.ToString());
            }
            if (ContentChoiseHandler != null)
            {
                ContentChoiseHandler(this, item.Summary.Text);
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RectangleGeometry rg = new RectangleGeometry();
            rg.Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);
            LayoutItem.Clip = rg;
            LayoutItem.Width = e.NewSize.Width - 10;
            LayoutItem.Height = e.NewSize.Height - 7;
        }
    }
}
