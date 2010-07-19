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
            set
            {
                _Title = value;
                tbTitle.Text = _Title;
            }
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

        private string _Link;

        public string Link
        {
            get { return _Link; }
            set { _Link = value; }
        }

        private Image img = new Image();

        public Image Img
        {
            get { return img; }
            set { img = value; }
        }

        private bool _IsShowTitle;

        public bool IsShowTitle
        {
            get { return _IsShowTitle; }
            set
            {
                _IsShowTitle = value;
                tbTitle.Visibility = (_IsShowTitle) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                grid.RowDefinitions[1].Height = (_IsShowTitle) ? new GridLength(20, GridUnitType.Pixel) : new GridLength(0, GridUnitType.Pixel);
            }
        }

        public double TitleSize
        {
            get { return tbTitle.FontSize; }
            set { tbTitle.FontSize = value;}
        }

        public Brush TitleColor
        {
            get { return tbTitle.Foreground; }
            set { tbTitle.Foreground = value; }
        }

        public FontFamily TitleFontFamily
        {
            get { return tbTitle.FontFamily; }
            set { tbTitle.FontFamily = value; }
        }

        private TextBlock tbTitle = new TextBlock() { TextAlignment = TextAlignment.Center };
        private Grid grid = new Grid();

        public ImageListControlItems()
        {
            img.Stretch = Stretch.Uniform;
            img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
            //Content = img;
            Content = grid;
            // grid.Background = new SolidColorBrush(Colors.Red);
            grid.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(img, 0);
            grid.Children.Add(img);

            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) });
            Grid.SetRow(tbTitle, 1);
            grid.Children.Add(tbTitle);
            //img.Source = new BitmapImage(new Uri("Images/default.png", UriKind.Relative));
            ImageUrl = "/BasicLibrary;component/Images/default.png";
            _Link = "";
            IsShowTitle = false;
            Cursor = Cursors.Hand;
        }

        void img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnLinkClicked(_Link);
        }

        public ImageListControlItems(string url)
            : this()
        {
            ImageUrl = url;
        }

        public ImageListControlItems(string url, string title)
            : this(url)
        {
            Title = title;
        }

        public ImageListControlItems(string url, string title, string description)
            : this(url, title)
        {
            Description = description;
        }
    }
}
