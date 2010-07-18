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

namespace test
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            briefNews1.Title = "Title dai thiet la dai ne xuong hang di";
            briefNews1.ContentValue = "Summary of the news";
            briefNews1.ImageURL = "http://localhost:53533/untitled.png";
            briefNews1.CornerRadius = new CornerRadius(0, 0, 0, 0);
            briefNews1.DropShadow = true;
            //briefNews1.ControlBackground = new SolidColorBrush(Colors.Red);
            briefNews1.ControlBorderThickness = new Thickness(2);
            //briefNews1.ControlBorderBrush = new SolidColorBrush(Colors.Red);
        }
    }
}
