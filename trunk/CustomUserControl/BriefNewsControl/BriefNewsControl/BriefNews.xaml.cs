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
using System.Xml;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;

namespace BriefNewsControl
{
    public partial class BriefNews : BasicDataListItem
    {
        private string dataURL = "";

        public BriefNews()
        {
            InitializeComponent();

            parameterNameList.Add("TitleColor");
            parameterNameList.Add("ContentColor");
            parameterNameList.Add("ControlBackground");
            parameterNameList.Add("ControlBorderBrush");
            parameterNameList.Add("ControlBorderThickness");
            parameterNameList.Add("DataURL");
            parameterNameList.Add("CornerRadius");
            parameterNameList.Add("DropShadow");
            parameterNameList.Add("TitleFontSize");
            parameterNameList.Add("ContentFontSize");

            parameterCanBindingNameList.Add("ImageURL");
            parameterCanBindingNameList.Add("Title");
            parameterCanBindingNameList.Add("Link");
            parameterCanBindingNameList.Add("ContentValue");

            tbTitle.Tag = "";
        }

        public string DataURL
        {
            get { return dataURL; }
            set
            {
                dataURL = value;
                if (dataURL.Length == 0)
                    return;
                Ultility ulti = new Ultility();
                ulti.OnGetStringAsyncCompleted += new Ultility.GetStringAsyncCompletedHandler(ulti_OnGetStringAsyncCompleted);
                ulti.GetStringAsync(dataURL);
            }
        }

        void ulti_OnGetStringAsyncCompleted(string result)
        {
            try
            {
                string imageURL = "", title = "", content = "", link = "";
                XmlReader xmlReader = XmlReader.Create(new StringReader(result));

                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        switch (xmlReader.LocalName.ToLower())
                        {
                            case "image":
                                imageURL = xmlReader.ReadInnerXml();
                                break;
                            case "title":
                                title = xmlReader.ReadInnerXml();
                                break;
                            case "content":
                                content = xmlReader.ReadInnerXml();
                                break;
                            case "href":
                                link = xmlReader.ReadInnerXml();
                                break;
                            default:
                                break;
                        }
                    }
                }

                ImageURL = imageURL;
                Title = title;
                ContentValue = content;
                Link = link;
            }
            catch { }
        }

        #region Color Property
        public Color TitleColor
        {
            get { return ((SolidColorBrush)tbTitleControl.Foreground).Color; }
            set { tbTitleControl.Foreground = new SolidColorBrush(value); }
        }

        public Color ContentColor
        {
            get { return ((SolidColorBrush)tbContent.Foreground).Color; }
            set { tbContent.Foreground = new SolidColorBrush(value); }
        }

        public Brush ControlBackground
        {
            get { return border.Background; }
            set { border.Background = value; }
        }

        public Brush ControlBorderBrush
        {
            get { return border.BorderBrush; }
            set { border.BorderBrush = value; }
        }

        public Thickness ControlBorderThickness
        {
            get { return border.BorderThickness; }
            set { border.BorderThickness = value; }
        }

        public CornerRadius CornerRadius
        {
            get { return border.CornerRadius; }
            set { border.CornerRadius = new CornerRadius() 
                    {   
                        TopLeft = Math.Max(0, Math.Min(20, value.TopLeft)),
                        TopRight = Math.Max(0, Math.Min(20, value.TopRight)),
                        BottomLeft = Math.Max(0, Math.Min(20, value.BottomRight)),
                        BottomRight = Math.Max(0, Math.Min(20, value.BottomRight)),
                    }; }
        }

        public bool DropShadow
        {
            get { return border.Effect != null; }
            set 
            {
                if (value == false)
                    border.Effect = null;
                else
                    border.Effect = new DropShadowEffect(); 
            }
        }

        public double TitleFontSize
        {
            get { return tbTitleControl.FontSize; }
            set { tbTitleControl.FontSize = value; }
        }

        public double ContentFontSize
        {
            get { return tbContent.FontSize; }
            set { tbContent.FontSize = value; }
        }
        #endregion Color Property

        #region Binding Property
        public string ImageURL
        {
            get
            {
                if (image.Source == null)
                    return "";
                return ((BitmapImage)image.Source).UriSource.AbsoluteUri;
            }
            set 
            {
                if (value.Length == 0)
                {
                    image.Width = 0;
                    image.Margin = new Thickness(0);
                }
                else
                {
                    image.Source = new BitmapImage(new Uri(value, UriKind.Absolute));
                    image.Margin = new Thickness(2, 0, 0, 0);
                    image.Width = 100;
                }
            }
        }

        public string Title
        {
            get { return (string)tbTitleControl.Text; }
            set { tbTitleControl.Text = value; }
        }

        public string Link
        {
            get { return (string)tbTitle.Tag; }
            set { tbTitle.Tag = value; }
        }

        public string ContentValue
        {
            get { return tbContent.Text; }
            set { tbContent.Text = value; }
        }
        #endregion Binding Property

        private void tbTitle_Click(object sender, RoutedEventArgs e)
        {
            base.OnLinkClicked((string)tbTitle.Tag);
        }
    }
}
