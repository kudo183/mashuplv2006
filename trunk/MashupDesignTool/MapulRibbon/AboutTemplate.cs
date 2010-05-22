///////////////////////////////////////////////////////////////////////////////
//
//  AboutTemplate.cs   			SilverlightRibbon 1.0
// 
//  Copyright (c) 2008 Mapul Inc. All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MapulRibbon
{
    public class AboutTemplate : Canvas 
    {
        public AboutTemplate()
        {
            this.Visibility = Visibility.Collapsed;            
            this.MouseLeftButtonDown += new MouseButtonEventHandler(AboutTemplate_MouseLeftButtonDown);            
        }        

        void AboutTemplate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
        }

        public void Hide()
        {
            this.Visibility = Visibility.Collapsed;
        }

        public void Show()
        {
            this.Visibility = Visibility.Visible;
        }       
    }
}
