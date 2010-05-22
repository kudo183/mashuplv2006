///////////////////////////////////////////////////////////////////////////////
//
//  RibbonComboBox.cs   			SilverlightRibbon 1.0
// 
//  Copyright (c) 2008 Mapul Inc. All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
using System.Windows;
using System.Windows.Controls;

namespace MapulRibbon
{
    public class RibbonComboBox : ComboBox
    {
        public RibbonComboBox()
        {
            this.IsTabStop = false;
        }

        public void SetStyle(Style style)
        {
            if (this.Style == null)
                this.Style = style;
        }

        private RibbonItem _ribbonItem;
        public RibbonItem RibbonItem
        {
            get { return _ribbonItem; }
            internal set { _ribbonItem = value; }
        }
    }

    public class RibbonComboBoxItem : ComboBoxItem
    {

    }
}
