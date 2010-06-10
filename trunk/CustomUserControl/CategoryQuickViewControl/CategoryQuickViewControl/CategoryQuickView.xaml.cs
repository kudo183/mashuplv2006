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

namespace CategoryQuickViewControl
{
    public partial class CategoryQuickView : BasicDataListItem
    {
        private string dataURL;

        public CategoryQuickView()
        {
            InitializeComponent();

            parameterNameList.Add("CategoryColor");
            parameterNameList.Add("MainTitleColor");
            parameterNameList.Add("OtherTitleColor");
            parameterNameList.Add("MainContentColor");
            parameterNameList.Add("NavigateCategoryColor");
            parameterNameList.Add("ControlBackground");
            parameterNameList.Add("ControlBorderBrush");
            parameterNameList.Add("ControlBorderThickness");
            parameterNameList.Add("DataURL");

            parameterCanBindingNameList.Add("Category");
            parameterCanBindingNameList.Add("CategoryLink");
            parameterCanBindingNameList.Add("ImageURL");
            parameterCanBindingNameList.Add("MainTitle");
            parameterCanBindingNameList.Add("MainContent");
            parameterCanBindingNameList.Add("MainLink");
            parameterCanBindingNameList.Add("OtherTitle1");
            parameterCanBindingNameList.Add("OtherLink1");
            parameterCanBindingNameList.Add("OtherTitle2");
            parameterCanBindingNameList.Add("OtherLink2");
            parameterCanBindingNameList.Add("NavigateCategoryText");
        }

        public string DataURL
        {
            get { return dataURL; }
            set
            {
                dataURL = value;
                Ultility ulti = new Ultility();
                ulti.ServerURL = new Uri("http://localhost:1646/", UriKind.Absolute);     //mo khoa dong nay de test
                ulti.OnGetStringAsyncCompleted += new Ultility.GetStringAsyncCompletedHandler(ulti_OnGetStringAsyncCompleted);
                ulti.GetStringAsync(dataURL);
            }
        }

        void ulti_OnGetStringAsyncCompleted(string result)
        {
            try
            {
                string category = "", categoryLink = "", imageURL = "", mainTitle = "", 
                        mainContent = "", mainLink = "", navigateCategory = "";
                List<string> otherTitle = new List<string>(), otherLink = new List<string>();
                XmlReader xmlReader = XmlReader.Create(new StringReader(result));

                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        switch (xmlReader.LocalName)
                        {
                            case "Category":
                                category = xmlReader.ReadInnerXml();
                                break;
                            case "CategoryLink":
                                categoryLink = xmlReader.ReadInnerXml();
                                break;
                            case "ImageURL":
                                imageURL = xmlReader.ReadInnerXml();
                                break;
                            case "MainTitle":
                                mainTitle = xmlReader.ReadInnerXml();
                                break;
                            case "MainContent":
                                mainContent = xmlReader.ReadInnerXml();
                                break;
                            case "MainLink":
                                mainLink = xmlReader.ReadInnerXml();
                                break;
                            case "NavigateCategoryText":
                                navigateCategory = xmlReader.ReadInnerXml();
                                break;
                            case "OtherTitle":
                                otherTitle.Add(xmlReader.ReadInnerXml());
                                break;
                            case "OtherLink":
                                otherLink.Add(xmlReader.ReadInnerXml());
                                break;
                            default:
                                break;
                        }
                    }
                }

                Category = category;
                CategoryLink = categoryLink;
                ImageURL = imageURL;
                MainTitle = mainTitle;
                MainLink = MainLink;
                MainContent = mainContent;
                NavigateCategoryText = navigateCategory;
                if (otherTitle.Count > 0)
                    OtherTitle1 = otherTitle[0];
                if (otherTitle.Count > 1)
                    OtherTitle2 = otherTitle[1];
                if (otherLink.Count > 0)
                    OtherLink1 = otherLink[0];
                if (otherLink.Count > 1)
                    OtherLink2 = otherLink[1];
            }
            catch { }
        }

        #region Color Property
        public Color CategoryColor
        {
            get { return ((SolidColorBrush)tbCategory.Foreground).Color; }
            set { tbCategory.Foreground = new SolidColorBrush(value); }
        }

        public Color MainTitleColor
        {
            get { return ((SolidColorBrush)tbMainTitle.Foreground).Color; }
            set { tbMainTitle.Foreground = new SolidColorBrush(value); }
        }

        public Color OtherTitleColor
        {
            get { return ((SolidColorBrush)tbOtherTitle1.Foreground).Color; }
            set 
            {
                tbOtherTitle1.Foreground = new SolidColorBrush(value);
                tbOtherTitle2.Foreground = new SolidColorBrush(value); 
            }
        }

        public Color MainContentColor
        {
            get { return ((SolidColorBrush)tbMainDetail.Foreground).Color; }
            set { tbMainDetail.Foreground = new SolidColorBrush(value); }
        }

        public Color NavigateCategoryColor
        {
            get { return ((SolidColorBrush)tbNavigateCategory.Foreground).Color; }
            set { tbNavigateCategory.Foreground = new SolidColorBrush(value); }
        }

        public Color ControlBackground
        {
            get { return ((LinearGradientBrush)LayoutBorder.Background).GradientStops[0].Color; }
            set { ((LinearGradientBrush)LayoutBorder.Background).GradientStops[0].Color = value; }
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
        #endregion Color Property

        #region Binding Property
        public string Category
        {
            get { return tbCategory.Text; }
            set { tbCategory.Text = value; }
        }

        public string CategoryLink
        {
            get
            {
                if (tbCategory.Tag == null)
                    return "";
                return (string)tbCategory.Tag;
            }
            set { tbCategory.Tag = value; }
        }

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
                imgImage.Width = imgImage.Height;
            }
        }

        public string MainTitle
        {
            get { return tbMainTitle.Text; }
            set { tbMainTitle.Text = value; }
        }

        public string MainContent
        {
            get { return tbMainDetail.Text; }
            set { tbMainDetail.Text = value; }
        }

        public string MainLink
        {
            get
            {
                if (tbMainTitle.Tag == null)
                    return "";
                return (string)tbMainTitle.Tag;
            }
            set { tbMainTitle.Tag = value; }
        }

        public string OtherTitle1
        {
            get { return tbOtherTitle1.Text; }
            set { tbOtherTitle1.Text = value; }
        }

        public string OtherLink1
        {
            get
            {
                if (tbOtherTitle1.Tag == null)
                    return "";
                return (string)tbOtherTitle1.Tag;
            }
            set { tbOtherTitle1.Tag = value; }
        }

        public string OtherTitle2
        {
            get { return tbOtherTitle2.Text; }
            set { tbOtherTitle2.Text = value; }
        }

        public string OtherLink2
        {
            get
            {
                if (tbOtherTitle2.Tag == null)
                    return "";
                return (string)tbOtherTitle2.Tag;
            }
            set { tbOtherTitle2.Tag = value; }
        }

        public string NavigateCategoryText
        {
            get { return tbNavigateCategory.Text; }
            set { tbNavigateCategory.Text = value; }
        }

        public string NavigateCategoryLink
        {
            get { return CategoryLink; }
            set { CategoryLink = value; }
        }
        #endregion Binding Property
    }
}
