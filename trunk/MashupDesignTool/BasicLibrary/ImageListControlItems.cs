using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BasicLibrary
{
    public class ImageListControlItems : BasicControl
    {
        private string _ImageUrl = "";

        public string ImageUrl
        {
            get { return _ImageUrl; }
            set
            {
                _ImageUrl = value;
                img.Source = new BitmapImage(new Uri(_ImageUrl, UriKind.RelativeOrAbsolute));
            }
        }
        private string _Title = "";

        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }
        private string _Description = "";

        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
                ToolTipService.SetToolTip(img, _Description);
            }
        }
        private Image img = new Image();

        public ImageListControlItems()
        {
            img.Stretch = Stretch.Fill;
            Content = img;
            //ImageUrl = "http://img140.imageshack.us/img140/9317/untitled96hf.png";
            ImageUrl = "Images/default.png";
        }

        public ImageListControlItems(string url)
        {
            img.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            img.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            Content = img;
            ImageUrl = url;
        }
    }
}
