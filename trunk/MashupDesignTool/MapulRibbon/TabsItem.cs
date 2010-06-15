///////////////////////////////////////////////////////////////////////////////
//
//  TabsItem.cs   			SilverlightRibbon 1.0
// 
//  Copyright (c) 2008 Mapul Inc. All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System;

namespace MapulRibbon
{
    public class TabsItem : TabItem
    {
        public TabsItem()
        {
            this.Height = 20;
            this.HorizontalAlignment = HorizontalAlignment.Center;
            this.Margin = new Thickness(47, 0, -47, 0);
            this.Padding = new Thickness(22, 0, 22, 0);
            this.Loaded += new RoutedEventHandler(TabsItem_Loaded);
            this.IsTabStop = false;
        }

        public event RoutedEventHandler TabsItemLoaded;

        void TabsItem_Loaded(object sender, RoutedEventArgs e)
        {                       
                // set style
                try
                {
                    Style style = (this.Parent as Tabs).Ribbon._ribbonTabItemStyle;
                    if (style != null && this.Style == null)
                        this.Style = style;
                }
                catch { }
                //
                RibbonItems content = this.Content as RibbonItems;
                this.RibbonItems.Clear();
                foreach (UIElement element in content.Children)
                {
                    if (element is RibbonItem)
                    {
                        RibbonItem item = element as RibbonItem;
                        item.TabsItem = this;
                        item.Height = this.ItemHeight - 17;
                        RibbonItems.Add(item);
                    }
                    else
                        throw new FormatException("Ribbon is not valid format: Tabs.Content.RibbonItems can consist only RibbonItem's elements");
                }

                if (TabsItemLoaded != null)
                    TabsItemLoaded(this, null);           
        }

        private Tabs _tabs;
        public Tabs Tabs
        {
            get { return _tabs; }
            set { _tabs = value; }
        }
         
        public double ItemHeight
        {
            get { return _tabs.Height - this.Height; }
        }

        private List<RibbonItem> _ribbonItems = new List<RibbonItem>();
        public List<RibbonItem> RibbonItems
        {
            get { return _ribbonItems; }
            set { _ribbonItems = value; }
        }   

        public RibbonItem this[string name]
        {
            get
            {
                RibbonItem item = null;
                foreach (RibbonItem ri in this.RibbonItems)
                    if (ri.Name == name)
                    {
                        item = ri;
                        break;
                    }
                return item;
            }
        }
    }
}
