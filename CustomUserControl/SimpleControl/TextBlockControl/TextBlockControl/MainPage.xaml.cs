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

namespace TextBlockControl
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();

            textBlock1.Text = "Helojdalkfj dslfja ldfl fdlja ljfjd asfja fljaflkja lfdkja d";
            textBlock1.TextTrimming = TextTrimming.WordEllipsis;
            textBlock1.TextColor = new SolidColorBrush(Colors.Red);
            textBlock1.FontWeight = FontWeights.ExtraBold;
            textBlock1.FontSize = 15;

            //strokeTextBlock1.Text = "Hello world";
            strokeTextBlock1.StrokeOpacity = 0.8;
            strokeTextBlock1.StrokeThickness = 3;
            strokeTextBlock1.FontSize = 40;
            strokeTextBlock1.Stroke = new SolidColorBrush(Colors.Red);
            strokeTextBlock1.TextColor = new SolidColorBrush(Colors.Yellow);
            strokeTextBlock1.TextBackground = new SolidColorBrush(Colors.Black);
            //strokeTextBlock1.ChangeText("<xml><Text>hello</Text></xml>");
        }
    }
}
