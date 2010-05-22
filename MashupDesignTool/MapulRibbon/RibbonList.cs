///////////////////////////////////////////////////////////////////////////////
//
//  RibbonList.cs   			SilverlightRibbon 1.0
// 
//  Copyright (c) 2008 Mapul Inc. All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows;

namespace MapulRibbon
{
    public class RibbonList : ListBox
    {
        public RibbonList()
        {
            this.SelectionChanged += new SelectionChangedEventHandler(RibbonList_SelectionChanged);
            this.MouseLeave += new MouseEventHandler(RibbonList_MouseLeave);
            this.Loaded += new RoutedEventHandler(RibbonList_Loaded);
        }

        void RibbonList_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (RibbonListItem item in this.Items)
            {
                item.hAlignment = hAlignment;
                item.vAlignment = vAlignment;
            }
        }

        void RibbonList_MouseLeave(object sender, MouseEventArgs e)
        {
            if (AutoHide)
                this.Hide();            
        }

        void RibbonList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RibbonListItem item = SelectedItem as RibbonListItem;
            if (this.SelectedIndex != -1 && item != null)            
                item.RaiseOnClick();            
        }

        private void Hide()
        {            
            this.SelectedIndex = -1;
            if (_popup != null)
                _popup.IsOpen = false;            
        }        

        #region Events

        #endregion

        #region Fields

        private Popup _popup;

        public Popup Popup
        {
            get { return _popup; }
            internal set { _popup = value; }
        }

        private bool _autoHide = true;

        public bool AutoHide
        {
            get { return _autoHide; }
            set { _autoHide = value; }
        }

        private VerticalAlignment _vAlignment = VerticalAlignment.Center;
        public VerticalAlignment vAlignment
        {
            get { return _vAlignment; }
            set { _vAlignment = value; }
        }

        private HorizontalAlignment _hAlignment = HorizontalAlignment.Center;
        public HorizontalAlignment hAlignment
        {
            get { return _hAlignment; }
            set { _hAlignment = value; }
        }    

        #endregion
    }    
}
