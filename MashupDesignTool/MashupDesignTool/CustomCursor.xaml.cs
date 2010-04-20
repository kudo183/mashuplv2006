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

namespace MashupDesignTool
{
    public partial class CustomCursor : UserControl
    {
        public CustomCursor()
        {
            InitializeComponent();
        }

        public CustomCursor(string resource)
        {
            InitializeComponent();
            Uri uri = new Uri(resource, UriKind.Relative);
            BitmapImage bm = new BitmapImage(uri);
            image1.Source = bm;
        }

        public void MoveTo(Point pt)
        {
            this.SetValue(Canvas.LeftProperty, pt.X - (image1.Width / 2));
            this.SetValue(Canvas.TopProperty, pt.Y - (image1.Height / 2));
        }

        public void MoveTo(double x, double y)
        {
            this.SetValue(Canvas.LeftProperty, x - (image1.Width / 2));
            this.SetValue(Canvas.TopProperty, y - (image1.Height / 2));
        }
    }
}
