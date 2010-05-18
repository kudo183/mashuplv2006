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
using System.Windows.Media.Imaging;

namespace Testcontrol1
{
    public partial class MainPage : UserControl
    {
        BasicLibrary.BasicEffect effect;
        UserControl btn;
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

            btn = new SilverlightControl1();
            //btn.Width = 600;
            //btn.Height = 400;

            BasicLibrary.EffectableControl ec = new BasicLibrary.EffectableControl(btn);

            //EffectLibrary.SplitScreen effect
            //     = new EffectLibrary.SplitScreen(ec);
            Canvas.SetLeft(ec, 300);
            Canvas.SetTop(ec, 300);

            effect = new EffectLibrary.Jump(ec);

            ((BasicLibrary.BasicControl)ec.Control).ChangeMainEffect(effect);
            //effect = new EffectLibrary.Fade(ec);
            //((BasicLibrary.BasicControl)ec.Control).ChangeMainEffect(effect);

            LayoutRoot.Children.Add(ec);

            //effect.SetParameterValue("Speed", EffectLibrary.Push.PushSpeed.SLOW);
            //effect.SetParameterValue("Orientation", EffectLibrary.Push.PushOrientation.LEFT_TO_RIGHT);

            //effect.SetParameterValue("BeginPos", EffectLibrary.Wipe.BeginWipe.LEFT);
            effect.SetParameterValue("Direction", EffectLibrary.Jump.DIRECTION.DOWN);
            //effect.SetParameterValue("CellDuration", TimeSpan.FromMilliseconds(1400));

            //rssSlideshow1.RssUrl = "http://vnexpress.net/rss/gl/trang-chu.rss";
            //rssSlideshow1.MainEffect = effect;
            //rssSlideshow1.DelaySeconds = 1;

            RssSlideshowControl.RssSlideshow rss = new RssSlideshowControl.RssSlideshow();
            rss.Width = 200;
            rss.Height = 300;
            BasicLibrary.EffectableControl ec1 = new BasicLibrary.EffectableControl(rss);
            ec1.Width = 200;
            ec1.Height = 300;
            rss.RssUrl = "http://vnexpress.net/rss/gl/trang-chu.rss";
            rss.MainEffect = new EffectLibrary.Fade(ec1);
            rss.DelaySeconds = 2;
            Canvas.SetLeft(ec1, 0);
            Canvas.SetTop(ec1, 100);
            LayoutRoot.Children.Add(ec1);

            rss.LinkClicked += new RssSlideshowControl.RssSlideshow.LinkClickedHander(rss_LinkClicked);


            MacStyleContactFormControl.MacStyleContactForm frm = new MacStyleContactFormControl.MacStyleContactForm();
            frm.Width = 400;
            frm.Height = 300;
            BasicLibrary.EffectableControl ec2 = new BasicLibrary.EffectableControl(frm);
            ec2.Width = 400;
            ec2.Height = 300;
            frm.ReceiveEmail = "tranphuonghai144@yahoo.com";
            frm.MainEffect = new EffectLibrary.Jump(ec2);
            //frm.ContentBackgroundColor = Colors.Blue;
            //frm.ContentColor = Colors.Red;
            //frm.LabelColor = Colors.Yellow;
            //frm.ButtonColor = Colors.Purple;
            Canvas.SetLeft(ec2, 300);
            Canvas.SetTop(ec2, 100);
            LayoutRoot.Children.Add(ec2);
        }

        void rss_LinkClicked(object sender, string url)
        {
            ((RssSlideshowControl.RssSlideshow)sender).LinkColor = Colors.Red;
            ((RssSlideshowControl.RssSlideshow)sender).IndexColor = Colors.Red;
            ((RssSlideshowControl.RssSlideshow)sender).SummaryColor = Colors.Red;
            ((RssSlideshowControl.RssSlideshow)sender).TimeLabelColor = Colors.Red;
            ((RssSlideshowControl.RssSlideshow)sender).ButtonColor = Colors.Red;
            MessageBox.Show("SDF");
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

        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            ((BasicLibrary.BasicControl)btn).MainEffect.Start();
            WriteableBitmap bitmap = new WriteableBitmap(btn, rectangle1.RenderTransform);
            image1.Source = bitmap;
        }

        private void rssItemListControl1_Loaded_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
