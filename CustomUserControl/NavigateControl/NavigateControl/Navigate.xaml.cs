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
using BasicLibrary;
using System.Xml;
using System.IO;
using System.Windows.Browser;

namespace NavigateControl
{
    public partial class Navigate : BasicControl
    {
        public Navigate()
        {
            InitializeComponent();

            AddOperationNameToList("NavigateTo");
            AddOperationNameToList("OpenNewWindow");
        }

        public void NavigateTo(string xml)
        {
            try
            {
                XmlReader reader = XmlReader.Create(new StringReader(xml));
                while (reader.Read())
                {
                    if (reader.Name == "Link")
                    {
                        HtmlPage.Window.Navigate(new Uri(reader.ReadInnerXml()));
                    }
                }
            }
            catch { }
        }

        public void OpenNewWindow(string xml)
        {
            try
            {
                XmlReader reader = XmlReader.Create(new StringReader(xml));
                while (reader.Read())
                {
                    if (reader.Name == "Link")
                        HtmlPage.Window.Navigate(new Uri(reader.ReadInnerXml()), "_blank");
                }
            }
            catch { }
        }
    }
}
