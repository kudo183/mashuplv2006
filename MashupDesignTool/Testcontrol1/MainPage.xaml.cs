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

namespace Testcontrol1
{
    public partial class MainPage : UserControl
    {
        string[] htmls = new string[] 
                        { 
                            "<p><strong>hello world</strong></p>",
                            "<p>This is a html content. <strong>It only allow to render xhtml format.</strong><br/><br/> <br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><a href='www.google.com.vn'>Google</a></p>",
                            "<p><img src='http://www.google.com.vn/images/firefox/sprite2.png'/></p>",
                            "<p><strong>Good bye</strong></p>"
                        };
        int i;

        public MainPage()
        {
            InitializeComponent();
            i = 0;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            newsReader1.LoadHTML(htmls[i]);
            i = (i + 1) % htmls.Length;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            newsPage1.LoadHTML("<strong>hello world</strong>");

            htmlRichTextArea1.SetDefaultStyles();
            htmlRichTextArea1.Load("<p><img src='http://www.google.com.vn/images/firefox/sprite2.png'/></p>");
        }
    }
}
