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

namespace ControlLibrary
{
    public class WrappedPivotView : BasicLibrary.BasicControl
    {
        PivotViewer p;
        public WrappedPivotView()
            : base()
        {
           p = new PivotViewer();
            Content = p;
            Width = Height = 100;
            LoadCollection("abc.cxml?rss=http://vnexpress.net/RSS/GL/Xa-hoi.rss");
        }

        public void LoadCollection(string cxmlName)
        {
            string pageUrl = HtmlPage.Document.DocumentUri.AbsoluteUri;
            string rootUrl = pageUrl.Substring(0, pageUrl.LastIndexOf('/') + 1);

            //Create a URL to the desired CXML (and query if specified) on this JIT collection server.
            // Note, this assumes this webpage hosting the Silverlight control is at the root of the JIT collection server.
            string collectionUrl = rootUrl + cxmlName;

            p.LoadCollection(collectionUrl, string.Empty);
        }
    }

}
