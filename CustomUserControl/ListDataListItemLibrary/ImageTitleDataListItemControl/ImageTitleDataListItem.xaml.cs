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
using BasicLibrary;
using System.Windows.Media.Imaging;

namespace ImageTitleDataListItemControl
{
    public partial class ImageTitleDataListItem : BasicDataListItem
    {
        private bool bZoomImageWhenOver = false;

        public ImageTitleDataListItem()
        {
            InitializeComponent();

            parameterNameList.Add("TitleColor");
            parameterNameList.Add("ImageBorderColor");
            parameterNameList.Add("ControlBackground");
            parameterNameList.Add("ControlBorderBrush");
            parameterNameList.Add("ControlBorderThickness");
            parameterNameList.Add("ZoomImageWhenEnter");

            parameterCanBindingNameList.Add("ImageURL");
            parameterCanBindingNameList.Add("Title");
            parameterCanBindingNameList.Add("Link");
            parameterCanBindingNameList.Add("Tooltip");
        }

        #region Color Property
        public Color TitleColor
        {
            get { return ((SolidColorBrush)tbTitle.Foreground).Color; }
            set { tbTitle.Foreground = new SolidColorBrush(value); }
        }

        public Color ImageBorderColor
        {
            get { return ((SolidColorBrush)borderImage.BorderBrush).Color; }
            set { borderImage.BorderBrush = new SolidColorBrush(value); }
        }

        public Color ControlBackground
        {
            get { return ((SolidColorBrush)LayoutRoot.Background).Color; }
            set { LayoutRoot.Background = new SolidColorBrush(value); }
        }

        public Color ControlBorderBrush
        {
            get { return ((SolidColorBrush)LayoutBorder.BorderBrush).Color; }
            set { LayoutBorder.BorderBrush = new SolidColorBrush(value); }
        }

        public Thickness ControlBorderThickness
        {
            get { return LayoutBorder.BorderThickness; }
            set { LayoutBorder.BorderThickness = value; }
        }

        public bool ZoomImageWhenOver
        {
            get { return bZoomImageWhenOver; }
            set
            {
                bZoomImageWhenOver = value;
                if (value)
                {
                    Image img = new Image() { Source = imgImage.Source };
                    ToolTipService.SetToolTip(imgImage, img);
                    //imgTooltip.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    //imgTooltip.Visibility = System.Windows.Visibility.Collapsed;
                    ToolTipService.SetToolTip(imgImage, null);
                }
            }
        }
        #endregion Color Property

        #region Binding Property
        public string ImageURL
        {
            get
            {
                if (imgImage.Source == null)
                    return "";
                return ((BitmapImage)imgImage.Source).UriSource.AbsoluteUri;
            }
            set 
            { 
                imgImage.Source = new BitmapImage(new Uri(value, UriKind.Absolute));
            }
        }

        public string Title
        {
            get { return tbTitle.Text; }
            set { tbTitle.Text = value; }
        }

        public string Link
        {
            get
            {
                if (tbTitle.Tag == null)
                    return "";
                return (string)tbTitle.Tag; 
            }
            set { tbTitle.Tag = value; }
        }

        public string Tooltip
        {
            get
            {
                string str = (string)ToolTipService.GetToolTip(LayoutRoot);
                if (str == null)
                    return "";
                return str;
            }
            set { ToolTipService.SetToolTip(LayoutRoot, value); }
        }
        #endregion Binding Property

        private void imgImage_MouseEnter(object sender, MouseEventArgs e)
        {
            //if (bZoomImageWhenOver)
            //    fade.Begin();
        }

        private void imgImage_MouseLeave(object sender, MouseEventArgs e)
        {
            //if (bZoomImageWhenOver)
            //    imgTooltip.Opacity = 0;
        }
    }
}
