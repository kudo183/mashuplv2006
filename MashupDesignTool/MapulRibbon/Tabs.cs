///////////////////////////////////////////////////////////////////////////////
//
//  Tabs.cs   			SilverlightRibbon 1.0
// 
//  Copyright (c) 2008 Mapul Inc. All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
using System.Windows;
using System.Windows.Controls;

namespace MapulRibbon
{
    public class Tabs : TabControl
    {
        public Tabs()
        {
            this.Margin = new Thickness(0, 6, -66, 0);             
            this.Loaded += new RoutedEventHandler(Tabs_Loaded);
            this.IsTabStop = false;
        }

        void Tabs_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (TabsItem tabsitem in this.Items)            
                tabsitem.Tabs = this;                                    
        }

        internal void ApplyStyle(Style style)
        {
            if (this.Style == null)
                this.Style = style;
        }

        private Ribbon _ribbon;
        public Ribbon Ribbon
        {
            get { return _ribbon; }
            set { _ribbon = value; }
        }

        public TabsItem this[string name]
        {
            get
            {
                TabsItem item = null;
                foreach (TabsItem ti in this.Items)
                    if (ti.Name == name)
                    {
                        item = ti;
                        break;
                    }
                return item;
            }
        }
    }
}
