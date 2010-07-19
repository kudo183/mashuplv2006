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

namespace SilverlightHyperlinkControl
{
    public partial class Hyperlink : BasicControl
    {
        public Hyperlink()
        {
            InitializeComponent();
            parameterNameList.Add("Text");
            parameterNameList.Add("TextColor");
            parameterNameList.Add("FontStyle");
            parameterNameList.Add("FontSize");
            parameterNameList.Add("FontWeight");
            parameterNameList.Add("TextBackground");
            parameterNameList.Add("NavigateURL");

            AddOperationNameToList("ChangeText");
        }

        public string Text
        {
            get { return (string)hyperlinkButton1.Content; }
            set { hyperlinkButton1.Content = value; }
        }

        public Brush TextColor
        {
            get { return hyperlinkButton1.Foreground; }
            set { hyperlinkButton1.Foreground = value; }
        }

        public Brush TextBackground
        {
            get { return LayoutRoot.Background; }
            set { LayoutRoot.Background = value; }
        }

        public FontStyle FontStyle
        {
            get { return hyperlinkButton1.FontStyle; }
            set { hyperlinkButton1.FontStyle = value; }
        }

        public double FontSize
        {
            get { return hyperlinkButton1.FontSize; }
            set { hyperlinkButton1.FontSize = Math.Max(0, Math.Min(100, value)); }
        }

        public FontWeight FontWeight
        {
            get { return hyperlinkButton1.FontWeight; }
            set { hyperlinkButton1.FontWeight = value; }
        }

        public Uri NavigateURL
        {
            get { return hyperlinkButton1.NavigateUri; }
            set { hyperlinkButton1.NavigateUri = value; }
        }

        //public TextWrapping TextWrapping
        //{
        //    get { return hyperlinkButton1.TextWrapping; }
        //    set { hyperlinkButton1.TextWrapping = value; }
        //}
    }
}
