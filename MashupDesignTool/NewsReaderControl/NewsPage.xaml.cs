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
using Divelements.SilverlightTools;

namespace NewsReaderControl
{
    public partial class NewsPage : UserControl
    {
        public NewsPage()
        {
            InitializeComponent();
        }

        public string PageURL
        {
            get { return page.SourceUri.AbsoluteUri; }
            set { page.SourceUri = new Uri(value); }
        }

        public void SetPageContent(string content)
        {
            page.SourceHtml = content;
        }
    }
}
