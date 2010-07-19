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

namespace WebLinkControl
{
    public partial class WebLink : BasicControl
    {
        public WebLink()
        {
            InitializeComponent();
            parameterNameList.Add("Text");
            parameterNameList.Add("TextColor");
            parameterNameList.Add("FontStyle");
            parameterNameList.Add("FontSize");
            parameterNameList.Add("FontWeight");
            parameterNameList.Add("TextBackground");
            parameterNameList.Add("NavigateURL");
            parameterNameList.Add("TextWrapping");

            AddOperationNameToList("ChangeText");
        }

        public string Text
        {
            get { return webLink1.Text; }
            set { webLink1.Text = value; }
        }

        public Brush TextColor
        {
            get { return webLink1.Foreground; }
            set { webLink1.Foreground = value; }
        }

        public Brush TextBackground
        {
            get { return LayoutRoot.Background; }
            set { LayoutRoot.Background = value; }
        }

        public FontStyle FontStyle
        {
            get { return webLink1.FontStyle; }
            set { webLink1.FontStyle = value; }
        }

        public double FontSize
        {
            get { return webLink1.FontSize; }
            set { webLink1.FontSize = Math.Max(0, Math.Min(100, value)); }
        }

        public FontWeight FontWeight
        {
            get { return webLink1.FontWeight; }
            set { webLink1.FontWeight = value; }
        }

        public string NavigateURL
        {
            get { return webLink1.Uri; }
            set { webLink1.Uri = value; }
        }

        public TextWrapping TextWrapping
        {
            get { return webLink1.TextWrapping; }
            set { webLink1.TextWrapping = value; }
        }
    }
}
