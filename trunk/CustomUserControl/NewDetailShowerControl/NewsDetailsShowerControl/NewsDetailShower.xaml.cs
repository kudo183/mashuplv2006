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
using System.Windows.Media.Effects;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Text;

namespace NewsDetailsShowerControl
{
    public partial class NewsDetailShower : BasicControl
    {
        List<List<string>> mappingLink = new List<List<string>>();

        public NewsDetailShower()
        {
            InitializeComponent();

            parameterNameList.Add("TitleColor");
            parameterNameList.Add("BoldContentColor");
            parameterNameList.Add("ContentColor");
            parameterNameList.Add("ControlBackground");
            parameterNameList.Add("ControlBorderBrush");
            parameterNameList.Add("ControlBorderThickness");
            parameterNameList.Add("DropShadow");
            parameterNameList.Add("MappingLink");

            //AddOperationNameToList("LoadData");
            AddOperationNameToList("LoadData");
        }

        public Brush TitleColor
        {
            get { return tbTitle.Foreground; }
            set { tbTitle.Foreground = value; }
        }

        public Brush BoldContentColor
        {
            get { return tbBoldContent.Foreground; }
            set { tbBoldContent.Foreground = value; }
        }

        public Brush ContentColor
        {
            get { return tbContent.Foreground; }
            set { tbContent.Foreground = value; }
        }

        internal string Title
        {
            get { return tbTitle.Text; }
            set { tbTitle.Text = value; }
        }

        internal string BoldContent
        {
            get { return tbBoldContent.Text; }
            set { tbBoldContent.Text = value; }
        }

        internal string ContentText
        {
            get { return tbContent.Text; }
            set { tbContent.Text = value; }
        }

        internal Uri ImageURL
        {
            get 
            { 
                if (image.Source == null)
                    return new Uri("");
                return ((BitmapImage)image.Source).UriSource; 
            }
            set { image.Source = new BitmapImage(value); }
        }

        public Brush ControlBackground
        {
            get { return LayoutRoot.Background; }
            set { LayoutRoot.Background = value; }
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

        public List<List<string>> MappingLink
        {
            get { return mappingLink; }
            set 
            {
                if (value == null)
                    return;
                mappingLink = value; 
            }
        }

        public bool DropShadow
        {
            get { return this.Effect != null; }
            set { this.Effect = new DropShadowEffect(); }
        }

        public void View(string xml)
        {
            try
            {
                XmlReader reader = XmlReader.Create(new StringReader(xml));
                XDocument doc = XDocument.Load(reader);

                XElement root = doc.Root;
                LoadData(root);
            }
            catch { }
        }

        private void LoadData(XElement element)
        {
            try
            {
                if (!element.HasElements)
                    return;
                foreach (XElement child in element.Elements())
                {
                    switch (child.Name.LocalName.ToLower())
                    {
                        case "title":
                            Title = child.Value;
                            break;
                        case "boldcontent":
                            BoldContent = child.Value;
                            break;
                        case "content":
                            StringBuilder sb = new StringBuilder();
                            foreach (XElement e in child.Elements("text"))
                            {
                                if (e.Value.Trim() != "")
                                {
                                    sb.Append(e.Value);
                                    sb.Append("\r\n");
                                }
                            }
                            ContentText = sb.ToString();
                            break;
                        case "image":
                            ImageURL = new Uri(child.Value, UriKind.Absolute);
                            break;
                        default:
                            LoadData(child);
                            break;
                    }
                }
            }
            catch { }
        }

        public void LoadData(string xml)
        {
            string link = "";
            try
            {
                XmlReader reader = XmlReader.Create(new StringReader(xml));
                while (reader.Read())
                {
                    if (reader.Name.ToLower() == "link")
                    {
                        link = reader.ReadInnerXml();
                        break;
                    }
                }
            }
            catch { }

            if (link != "")
            {
                Ultility ulti = new Ultility();
                ulti.OnGetStringAsyncCompleted += new Ultility.GetStringAsyncCompletedHandler(ulti_OnGetStringAsyncCompleted);
                //gan tham so vao
                ulti.GetStringAsync(link);
            }

            //for (int i = 0; i < mappingLink.Count; i++)
            //{
            //    if (link.StartsWith(mappingLink[i][0]))
            //    {
            //        Ultility ulti = new Ultility();
            //        ulti.OnGetStringAsyncCompleted += new Ultility.GetStringAsyncCompletedHandler(ulti_OnGetStringAsyncCompleted);
            //        //gan tham so vao
            //        string url = mappingLink[i][1] + link;
            //        ulti.GetStringAsync(url);
            //    }
            //}
        }

        void ulti_OnGetStringAsyncCompleted(string result)
        {
            View(result);
        }
    }
}
