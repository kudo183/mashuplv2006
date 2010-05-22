using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace MapulRibbon
{
    public class RibbonSimpleListViewSourceItem
    {
        private string text;
        private BitmapImage imageSource;
        private object data;
        private string imgUrl;

        public RibbonSimpleListViewSourceItem(string text, string imgUrl, object data)
            : this(text, new BitmapImage(new Uri(imgUrl, UriKind.RelativeOrAbsolute)), data) { this.imgUrl = imgUrl; }

        public RibbonSimpleListViewSourceItem(string text, BitmapImage imageSource, object data)
        {
            this.text = text;
            this.imageSource = imageSource;
            this.data = data;
            this.imgUrl = imageSource.UriSource.AbsoluteUri;
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public BitmapImage ImageSource
        {
            get { return imageSource; }
            set { imageSource = value; }
        }

        public object Data
        {
            get { return data; }
            set { data = value; }
        }

        public string ImgUrl
        {
            get { return imgUrl; }
            set
            {
                imgUrl = value;
                imageSource = new BitmapImage(new Uri(imgUrl, UriKind.RelativeOrAbsolute));
            }
        }
    }
}
