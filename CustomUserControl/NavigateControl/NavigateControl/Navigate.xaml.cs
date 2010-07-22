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
                xml = xml.Replace("<root><Click>true</Click><Link>", "");
                xml = xml.Replace("</Link></root>", "");
                HtmlPage.Window.Navigate(new Uri(xml));
            }
            catch { }
        }

        public void OpenNewWindow(string xml)
        {
            try
            {
                xml = xml.Replace("<root><Click>true</Click><Link>", "");
                xml = xml.Replace("</Link></root>", "");
                HtmlPage.Window.Navigate(new Uri(xml), "_blank");
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }
    }
}
