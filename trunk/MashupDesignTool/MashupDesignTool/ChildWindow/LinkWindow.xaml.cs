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
using System.Windows.Browser;

namespace MashupDesignTool
{
    public partial class LinkWindow : ChildWindow
    {
        internal LinkWindow()
        {
            InitializeComponent();
        }

        public LinkWindow(DataService.DesignedApplicationData data) 
            : this()
        {
            lblAppName.Content = data.ApplicationName;
            lblLastUpdate.Content = data.LastUpdate.ToString("HH:mm dd/MM/yyyy");

            Uri uri = HtmlPage.Document.DocumentUri;
            txtEditLink.Text = new Uri(uri, "../Design/design.aspx?app=" + data.Id.ToString()).ToString();
            txtPresentLink.Text = new Uri(uri, "../Present/present.aspx?app=" + data.Id.ToString()).ToString();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtEditLink.Text);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtPresentLink.Text);
        }
    }
}

