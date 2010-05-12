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
            //newsReader1.LoadHTML(htmls[i]);
            //i = (i + 1) % htmls.Length;

            //textBox1.Text = MashupDesignTool.MyXmlSerializer.Serialize(comboBox1);
            
            //Button element = (Button)MashupDesignTool.MyXmlSerializer.Load(MashupDesignTool.MyXmlSerializer.Serialize(button1));
            //element.Margin = new Thickness(100, 100, 0, 0);
            //element.Name = i.ToString();
            //element.Content = "Hello";
            //LayoutRoot.Children.Add(element);

            
            //ComboBox cb = (ComboBox)MashupDesignTool.MyXmlSerializer.Load(MashupDesignTool.MyXmlSerializer.Serialize(comboBox1));
            //cb.Margin = new Thickness(100, 100, 0, 0);
            //cb.Name = (i + 10).ToString();
            //cb.UpdateLayout();
            //cb.SelectedIndex = 0;
            //LayoutRoot.Children.Add(cb);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //newsPage1.LoadHTML("<strong>hello world</strong>");

            //htmlRichTextArea1.SetDefaultStyles();
            //htmlRichTextArea1.Load("<p><img src='http://www.google.com.vn/images/firefox/sprite2.png'/></p>");

            UserControl btn = new SilverlightControl1();
            btn.Width = 200;
            btn.Height = 50;

            BasicLibrary.EffectableControl ec = new BasicLibrary.EffectableControl(btn);

            EffectLibrary.SplitScreenEffect effect
                 = new EffectLibrary.SplitScreenEffect(ec);
            Canvas.SetLeft(ec, 100);
            Canvas.SetTop(ec, 100);

            LayoutRoot.Children.Add(ec);

            effect.SetParameterValue("Speed", EffectLibrary.SplitScreenEffect.SplitSpeed.SLOW);
            effect.SetParameterValue("Direction", EffectLibrary.SplitScreenEffect.SplitDirection.HORIZONTAL);
        }

        private void newsReader1_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void rssItemListControl1_LinkClicked(object sender, string link)
        {
            
        }

        private void rssItemListControl1_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void rssItemListControl1_ContentChoise(object sender, string data)
        {
        }
    }
}
