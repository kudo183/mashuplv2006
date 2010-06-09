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

namespace ListDataListItemControl
{
    public partial class ListDataListItem : BasicDataListItem
    {
        Label[] subItems;
        TextBlock[] subItemsContent, subItemsTooltip;
        Rectangle[] subItemsBullet;

        public ListDataListItem()
        {
            InitializeComponent();

            parameterNameList.Add("HeaderBackgroundColor");
            parameterNameList.Add("HeaderTextColor");
            parameterNameList.Add("ContentColor");
            parameterNameList.Add("BulletColor");
            parameterNameList.Add("Background");
            parameterNameList.Add("BorderBrush");
            parameterNameList.Add("BorderThickness");

            parameterCanBindingNameList.Add("Header");
            parameterCanBindingNameList.Add("SubItem1Content");
            parameterCanBindingNameList.Add("SubItem2Content");
            parameterCanBindingNameList.Add("SubItem3Content");
            parameterCanBindingNameList.Add("SubItem4Content");
            parameterCanBindingNameList.Add("SubItem5Content");
            parameterCanBindingNameList.Add("SubItem6Content");
            parameterCanBindingNameList.Add("SubItem7Content");
            parameterCanBindingNameList.Add("SubItem8Content");
            parameterCanBindingNameList.Add("SubItem1Tooltip");
            parameterCanBindingNameList.Add("SubItem2Tooltip");
            parameterCanBindingNameList.Add("SubItem3Tooltip");
            parameterCanBindingNameList.Add("SubItem4Tooltip");
            parameterCanBindingNameList.Add("SubItem5Tooltip");
            parameterCanBindingNameList.Add("SubItem6Tooltip");
            parameterCanBindingNameList.Add("SubItem7Tooltip");
            parameterCanBindingNameList.Add("SubItem8Tooltip");
            parameterCanBindingNameList.Add("SubItem1Link");
            parameterCanBindingNameList.Add("SubItem2Link");
            parameterCanBindingNameList.Add("SubItem3Link");
            parameterCanBindingNameList.Add("SubItem4Link");
            parameterCanBindingNameList.Add("SubItem5Link");
            parameterCanBindingNameList.Add("SubItem6Link");
            parameterCanBindingNameList.Add("SubItem7Link");
            parameterCanBindingNameList.Add("SubItem8Link");

            subItems = new Label[] { lblSubItem1, lblSubItem2, lblSubItem3, lblSubItem4, 
                                    lblSubItem5, lblSubItem6, lblSubItem7, lblSubItem8 };
            subItemsContent = new TextBlock[] { tbSubItemContent1, tbSubItemContent2, tbSubItemContent3, tbSubItemContent4, 
                                                tbSubItemContent5, tbSubItemContent6, tbSubItemContent7, tbSubItemContent8 };
            subItemsTooltip = new TextBlock[] { tbSubItemTooltip1, tbSubItemTooltip2, tbSubItemTooltip3, tbSubItemTooltip4, 
                                                tbSubItemTooltip5, tbSubItemTooltip6, tbSubItemTooltip7, tbSubItemTooltip8 };
            subItemsBullet = new Rectangle[] { recBullet1, recBullet2, recBullet3, recBullet4, 
                                                recBullet5, recBullet6, recBullet7, recBullet8 };
        }
        

        #region Color Property
        public Color HeaderBackgroundColor
        {
            get { return ((LinearGradientBrush)lblHeader.Background).GradientStops[1].Color; }
            set { ((LinearGradientBrush)lblHeader.Background).GradientStops[1].Color = value; }
        }

        public Color HeaderTextColor
        {
            get { return ((SolidColorBrush)lblHeader.Foreground).Color; }
            set { lblHeader.Foreground = new SolidColorBrush(value); }
        }

        public Color ContentColor
        {
            get { return ((SolidColorBrush)tbSubItemContent1.Foreground).Color; }
            set 
            { 
                for (int i = 0; i < subItemsContent.Length; i++)
                    subItemsContent[i].Foreground = new SolidColorBrush(value); 
            }
        }

        public Color BulletColor
        {
            get { return ((SolidColorBrush)recBullet1.Fill).Color; }
            set
            {
                for (int i = 0; i < subItemsBullet.Length; i++)
                    subItemsBullet[i].Fill = new SolidColorBrush(value);
            }
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
        public string Header
        {
            get { return (string)lblHeader.Content; }
            set { lblHeader.Content = value; }
        }

        public string SubItem1Content
        {
            get { return GetSubItemsContent(0); }
            set { SetSubItemsContent(0, value); }
        }

        public string SubItem2Content
        {
            get { return GetSubItemsContent(1); }
            set { SetSubItemsContent(1, value); }
        }

        public string SubItem3Content
        {
            get { return GetSubItemsContent(2); }
            set { SetSubItemsContent(2, value); }
        }

        public string SubItem4Content
        {
            get { return GetSubItemsContent(3); }
            set { SetSubItemsContent(3, value); }
        }

        public string SubItem5Content
        {
            get { return GetSubItemsContent(4); }
            set { SetSubItemsContent(4, value); }
        }

        public string SubItem6Content
        {
            get { return GetSubItemsContent(5); }
            set { SetSubItemsContent(5, value); }
        }

        public string SubItem7Content
        {
            get { return GetSubItemsContent(6); }
            set { SetSubItemsContent(6, value); }
        }

        public string SubItem8Content
        {
            get { return GetSubItemsContent(7); }
            set { SetSubItemsContent(7, value); }
        }

        private string GetSubItemsContent(int i)
        {
            if (i >= subItemsContent.Length || i < 0)
                return "";
            return subItemsContent[i].Text;
        }

        private void SetSubItemsContent(int i, string value)
        {
            if (i >= subItemsContent.Length || i < 0)
                return;
            subItemsContent[i].Text = value;
            if (value.Trim().Length != 0)
                subItems[i].Visibility = System.Windows.Visibility.Visible;
            else
                subItems[i].Visibility = System.Windows.Visibility.Collapsed;
        }

        public string SubItem1Tooltip
        {
            get { return GetSubItemsTooltip(0); }
            set { SetSubItemsTooltip(0, value); }
        }

        public string SubItem2Tooltip
        {
            get { return GetSubItemsTooltip(1); }
            set { SetSubItemsTooltip(1, value); }
        }

        public string SubItem3Tooltip
        {
            get { return GetSubItemsTooltip(2); }
            set { SetSubItemsTooltip(2, value); }
        }

        public string SubItem4Tooltip
        {
            get { return GetSubItemsTooltip(3); }
            set { SetSubItemsTooltip(3, value); }
        }

        public string SubItem5Tooltip
        {
            get { return GetSubItemsTooltip(4); }
            set { SetSubItemsTooltip(4, value); }
        }

        public string SubItem6Tooltip
        {
            get { return GetSubItemsTooltip(5); }
            set { SetSubItemsTooltip(5, value); }
        }

        public string SubItem7Tooltip
        {
            get { return GetSubItemsTooltip(6); }
            set { SetSubItemsTooltip(6, value); }
        }

        public string SubItem8Tooltip
        {
            get { return GetSubItemsTooltip(7); }
            set { SetSubItemsTooltip(7, value); }
        }

        private string GetSubItemsTooltip(int i)
        {
            if (i >= subItemsContent.Length || i < 0)
                return "";
            return subItemsTooltip[i].Text;
        }

        private void SetSubItemsTooltip(int i, string value)
        {
            if (i >= subItemsContent.Length || i < 0)
                return;
            subItemsTooltip[i].Text = value;
        }

        public string SubItem1Link
        {
            get { return GetSubItemsLink(0); }
            set { SetSubItemsLink(0, value); }
        }

        public string SubItem2Link
        {
            get { return GetSubItemsLink(1); }
            set { SetSubItemsLink(1, value); }
        }

        public string SubItem3Link
        {
            get { return GetSubItemsLink(2); }
            set { SetSubItemsLink(2, value); }
        }

        public string SubItem4Link
        {
            get { return GetSubItemsLink(3); }
            set { SetSubItemsLink(3, value); }
        }

        public string SubItem5Link
        {
            get { return GetSubItemsLink(4); }
            set { SetSubItemsLink(4, value); }
        }

        public string SubItem6Link
        {
            get { return GetSubItemsLink(5); }
            set { SetSubItemsLink(5, value); }
        }

        public string SubItem7Link
        {
            get { return GetSubItemsLink(6); }
            set { SetSubItemsLink(6, value); }
        }

        public string SubItem8Link
        {
            get { return GetSubItemsLink(7); }
            set { SetSubItemsLink(7, value); }
        }

        private string GetSubItemsLink(int i)
        {
            if (i >= subItemsContent.Length || i < 0)
                return "";
            if (subItemsContent[i].Tag == null)
                return null;
            return (string)subItemsContent[i].Tag;
        }

        private void SetSubItemsLink(int i, string value)
        {
            if (i >= subItemsContent.Length || i < 0)
                return;
            subItemsContent[i].Tag = value;
        }
        #endregion Binding Property
    }
}
