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

namespace HyperlinkControl
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();

            hyperlink1.NavigateURL = new Uri("http://google.com.vn");
            hyperlink1.Text = "Googles";
            hyperlink1.FontSize = 30;

            webLink1.NavigateURL = "http://google.com.vn";
            webLink1.Text = "Hello";
            webLink1.FontSize = 30;
            webLink1.TextBackground = new SolidColorBrush(Colors.Red);
        }
    }
}
