///////////////////////////////////////////////////////////////////////////////
//
//  RibbonButtonsGroup.cs   			SilverlightRibbon 1.0
// 
//  Copyright (c) 2008 Mapul Inc. All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace MapulRibbon
{
    public class RibbonButtonsGroup : StackPanel 
    {        
        public RibbonButtonsGroup()
        {
            this.Loaded += new RoutedEventHandler(RibbonButtonsGroup_Loaded);
        }        

        void RibbonButtonsGroup_Loaded(object sender, RoutedEventArgs e)
        {            
            foreach (FrameworkElement el in this.Children)
            {
                if (el is RibbonButtonBase)
                {
                    (el as RibbonButtonBase).ParentGroup = this;                    
                }
                else if (el is RibbonColorButton)
                {
                    (el as RibbonColorButton).ParentGroup = this;
                }
                else if (el is RibbonComboBox){ }
                else if (el is Border) { }
                else if (el is HyperlinkButton) { }
                else if (el is RibbonButtonsGroup)
                {
                    (el as RibbonButtonsGroup).ParentGroup = this;                    
                }
                else if (el is RibbonList) { }
                else if (el is RibbonSimpleListView) { }
                else throw new Exception("Ribbon is in not valid format. Only RibbonComboBox, RibbonButtonsGroup, RibbonButton, ToggleRibbonButton are allowed.");
            }                                                   
        }
        
        public List<RibbonButtonsGroup> Childs
        {
            get 
            {
                List<RibbonButtonsGroup> list = new List<RibbonButtonsGroup>();
                foreach (FrameworkElement el in this.Children)
                {
                    if (el is RibbonButtonsGroup)
                        list.Add(el as RibbonButtonsGroup);
                    else if (el is Border)
                    {
                        if ((el as Border).Child is RibbonButtonsGroup)
                            list.Add((el as Border).Child as RibbonButtonsGroup);                        
                    }                        
                }
                return list;
            }            
        }

        public List<RibbonButtonsGroup> DescendantsChilds
        {
            get
            {
                return _forGroupChilds(new List<RibbonButtonsGroup>());
            }
        }

        public List<RibbonButtonsGroup> DescendantsChildsAndSelf
        {
            get
            {
                List<RibbonButtonsGroup> list = new List<RibbonButtonsGroup>();
                list.Add(this);
                return _forGroupChilds(list);
            }
        }

        private List<RibbonButtonsGroup> _forGroupChilds(List<RibbonButtonsGroup> list)
        {            
            foreach (RibbonButtonsGroup b in this.Childs)
            {
                list.Add(b);
                list = b._forGroupChilds(list);
            }
            return list;
        }

        public FrameworkElement FindControl(string id)
        {
            FrameworkElement el = null;
            foreach (FrameworkElement e in this.Children)
                if (e.Name == id)
                {
                    el = e;
                    break;
                }
            return el;
        }

        public int ButtonsCount
        {
            get
            {
                int count = 0;
                foreach (FrameworkElement el in this.Children)                
                    if (el is RibbonButtonBase || el is RibbonComboBox || el is RibbonColorButton)
                        count++;
                return count;
            }
        }
        
        public int VertButtonsCount
        {
            get {
                if (this.Orientation == Orientation.Horizontal)
                {
                    if (this.ButtonsCount > 0)
                        return 1;
                    else return 0;
                }
                else return this.ButtonsCount;
            }
        }        

        public double MaxVHeight
        {
            get
            {
                double height = 0;
                if (this.Orientation == Orientation.Horizontal)
                {                    
                    foreach (FrameworkElement el in this.Children)
                        if (el.Height.ToString() != "NaN" && el.Height > height)
                            height = el.Height;                        
                }
                else
                {
                    foreach (FrameworkElement el in this.Children)
                        if (el.Height.ToString() != "NaN" && el.Height > height)
                            height += el.Height;                        
                }
                return height;
            }
        }

        private RibbonButtonsGroup _parentGroup = null;
        public RibbonButtonsGroup ParentGroup
        {
            get { return _parentGroup; }
            set { _parentGroup = value; }
        }     

        private bool _hasBorder;

        public bool HasBorder
        {
            get { return _hasBorder; }
            set { _hasBorder = value; }
        }
    }
}
