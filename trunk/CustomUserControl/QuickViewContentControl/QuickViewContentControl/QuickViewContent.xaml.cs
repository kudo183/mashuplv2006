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
using System.Xml;
using System.IO;

namespace QuickViewContentControl
{
    public partial class QuickViewContent : BasicDataListItem
    {
        private string dataURL = "";

        public QuickViewContent()
        {
            InitializeComponent();

            parameterNameList.Add("TitleColor");
            parameterNameList.Add("ImageBorderColor");
            parameterNameList.Add("ContentColor");
            parameterNameList.Add("Background");
            parameterNameList.Add("BorderBrush");
            parameterNameList.Add("BorderThickness");

            parameterCanBindingNameList.Add("ImageURL");
            parameterCanBindingNameList.Add("Title");
            parameterCanBindingNameList.Add("Link");
            parameterCanBindingNameList.Add("ContentValue");
        }

        public string DataURL
        {
            get { return dataURL; }
            set
            {
                dataURL = value;
                Ultility ulti = new Ultility();
                //ulti.ServerURL = new Uri("http://localhost:1646/", UriKind.Absolute);     //mo khoa dong nay de test
                ulti.OnGetStringAsyncCompleted += new Ultility.GetStringAsyncCompletedHandler(ulti_OnGetStringAsyncCompleted);
                ulti.GetStringAsync(dataURL);
            }
        }

        void ulti_OnGetStringAsyncCompleted(string result)
        {
            try
            {
                string imageURL = "", title = "", content = "";
                XmlReader xmlReader = XmlReader.Create(new StringReader(result));

                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        switch (xmlReader.LocalName)
                        {
                            case "ImageURL":
                                imageURL = xmlReader.ReadInnerXml();
                                break;
                            case "Title":
                                title = xmlReader.ReadInnerXml();
                                break;
                            case "Content":
                                content = xmlReader.ReadInnerXml();
                                break;
                            default:
                                break;
                        }
                    }
                }

                ImageURL = imageURL;
                Title = title;
                ContentValue = content;
            }
            catch { }
        }

        #region Color Property
        public Color TitleColor
        {
            get { return ((SolidColorBrush)tbTitle.Foreground).Color; }
            set { tbTitle.Foreground = new SolidColorBrush(value); }
        }

        public Color ImageBorderColor
        {
            get { return ((SolidColorBrush)borderImage.Background).Color; }
            set { borderImage.Background = new SolidColorBrush(value); }
        }

        public Color ContentColor
        {
            get { return ((SolidColorBrush)tbContent.Foreground).Color; }
            set { tbContent.Foreground = new SolidColorBrush(value); }
        }

        public new Color Background
        {
            get { return ((SolidColorBrush)LayoutRoot.Background).Color; }
            set { LayoutRoot.Background = new SolidColorBrush(value); }
        }

        public new Color BorderBrush
        {
            get { return ((SolidColorBrush)LayoutBorder.BorderBrush).Color; }
            set { LayoutBorder.BorderBrush = new SolidColorBrush(value); }
        }

        public new Thickness BorderThickness
        {
            get { return LayoutBorder.BorderThickness; }
            set { LayoutBorder.BorderThickness = value; }
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
            set { imgImage.Source = new BitmapImage(new Uri(value, UriKind.Absolute)); }
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

        public string ContentValue
        {
            get { return tbContent.Text; }
            set { tbContent.Text = value; }
        }
        #endregion Binding Property
    }
}
