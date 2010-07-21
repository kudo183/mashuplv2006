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
using System.Windows.Browser;
using System.Windows.Pivot;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ControlLibrary
{
    public class WrappedPivotView : BasicLibrary.BasicControl
    {
        PivotViewer p;
        string _XmlDataElement;

        public string XmlDataElement
        {
            get { return _XmlDataElement; }
            set { _XmlDataElement = value; }
        }

        string _XmlDataURL;

        public string XmlDataURL
        {
            get { return _XmlDataURL; }
            set { _XmlDataURL = value; }
        }
        string _XmlFacets;

        public string XmlFacets
        {
            get { return _XmlFacets; }
            set { _XmlFacets = value; }
        }
        string _CollectionName;

        public string CollectionName
        {
            get { return _CollectionName; }
            set { _CollectionName = value; }
        }
        string _TitleElement;

        public string TitleElement
        {
            get { return _TitleElement; }
            set { _TitleElement = value; }
        }
        string _LinkElement;

        public string LinkElement
        {
            get { return _LinkElement; }
            set { _LinkElement = value; }
        }
        string _DescriptionElement;

        public string DescriptionElement
        {
            get { return _DescriptionElement; }
            set { _DescriptionElement = value; }
        }
        string _ImageUrlElement;

        public string ImageUrlElement
        {
            get { return _ImageUrlElement; }
            set { _ImageUrlElement = value; }
        }

        public WrappedPivotView()
            : base()
        {
            parameterNameList.Add("XmlDataElement");
            parameterNameList.Add("XmlDataURL");
            parameterNameList.Add("XmlFacets");
            parameterNameList.Add("CollectionName");
            parameterNameList.Add("TitleElement");
            parameterNameList.Add("LinkElement");
            parameterNameList.Add("DescriptionElement");
            parameterNameList.Add("ImageUrlElement");
            AddOperationNameToList("Load");
            p = new PivotViewer();
            Content = p;
            Width = Height = 100;
            Loaded += new RoutedEventHandler(WrappedPivotView_Loaded);
            p.LinkClicked += new EventHandler<LinkEventArgs>(p_LinkClicked);
            _XmlDataElement = "item";
            _CollectionName = "";
            _DescriptionElement = "";
            _ImageUrlElement = "";
            _LinkElement = "";
            _TitleElement = "";
            _XmlDataURL = "";
            _XmlFacets = "";
        }

        void p_LinkClicked(object sender, LinkEventArgs e)
        {
            OnLinkClicked(e.Link.ToString());
        }

        void WrappedPivotView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_XmlDataURL == "")
                return;
            LoadCollection();
        }

        public void Load(string xml)
        {
            try
            {
                XDocument doc = XDocument.Parse(xml);
                XElement root = doc.Element("root");
                root = root.Element("Link");
                _XmlDataURL = root.Value;
                MessageBox.Show(_XmlDataURL);
                LoadCollection();
            }
            catch { }
        }

        public void LoadCollection()
        {
            string pageUrl = HtmlPage.Document.DocumentUri.AbsoluteUri;
            string rootUrl = pageUrl.Substring(0, pageUrl.LastIndexOf('/') + 1);

            //Create a URL to the desired CXML (and query if specified) on this JIT collection server.
            // Note, this assumes this webpage hosting the Silverlight control is at the root of the JIT collection server.
            string collectionUrl = rootUrl + "abc.cxml?url=" + _XmlDataURL;
            collectionUrl += "&Title=" + _TitleElement;
            collectionUrl += "&Link=" + _LinkElement;
            collectionUrl += "&Description=" + _DescriptionElement;
            collectionUrl += "&ImageUrl=" + _ImageUrlElement;
            collectionUrl += "&Name=" + _CollectionName;
            collectionUrl += "&Facets=" + _XmlFacets;
            collectionUrl += "&DataElement=" + _XmlDataElement;
            p.LoadCollection(collectionUrl, string.Empty);
        }
    }

}
