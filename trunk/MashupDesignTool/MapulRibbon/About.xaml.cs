///////////////////////////////////////////////////////////////////////////////
//
//  About.xaml.cs   			SilverlightRibbon 1.0
// 
//  Copyright (c) 2008 Mapul Inc. All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MapulRibbon
{
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(About_Loaded);
        }

        void About_Loaded(object sender, RoutedEventArgs e)
        {
            aboutTitle.Text = "Silverlight Ribbon";
            aboutText.Text = "The Silverlight Ribbon is a free UI control for your Web Applications.";
            aboutHl.Content = "www.siverlightribbon.com";
            aboutHl.NavigateUri = new System.Uri("http://www.siverlightribbon.com");
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
