///////////////////////////////////////////////////////////////////////////////
//
//  RibbonItem.xaml.cs   			SilverlightRibbon 1.0
// 
//  Copyright (c) 2008 Mapul Inc. All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MapulRibbon
{        
    public partial class RibbonItem : StackPanel
    {
        #region Fields

        private RibbonButtonsGroup _content = null;
        private Ribbon _ribbon;
        //private RibbonButton _rxButton = null;
        private TabsItem _tabsItem = null;

        //public event RoutedEventHandler RibbonItemLoaded;

        #endregion

        #region Properties

        public TabsItem TabsItem
        {
            get { return _tabsItem; }
            set { _tabsItem = value; }
        }

        public RibbonItem this[string name]
        {
            get
            {
                RibbonItem item = null;
                foreach (RibbonItem ri in TabsItem.RibbonItems)
                    if (ri.Name == name)
                    {
                        item = ri;
                        break;
                    }
                return item;
            }
        }

        public string Title
        {
            get { return RITitleText.Text; }
            set { RITitleText.Text = value; }
        }

        public RibbonButtonsGroup Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                if (_content != null)
                    this.RIMain.Children.Add(_content);
            }
        }

        public Ribbon Ribbon
        {
            get { return _ribbon; }
            internal set { _ribbon = value; }
        }

        //public RibbonButton RxButton
        //{
        //    get { return _rxButton; }
        //    set { _rxButton = value; }
        //}

        //public bool HasRxButton
        //{
        //    get
        //    {
        //        if (_rxButton != null)
        //            return true;
        //        else return false;
        //    }
        //}

        #endregion        

        #region ctor

        public RibbonItem()
        {
            InitializeComponent();
            // Attach the events
            this.Loaded += new RoutedEventHandler(RibbonItem_Loaded); 
            this.MouseEnter += new MouseEventHandler(RibbonItem_MouseEnter);
            this.MouseLeave += new MouseEventHandler(RibbonItem_MouseLeave); 
            this.VerticalAlignment = VerticalAlignment.Top;            
        }       

        #endregion

        #region Events        

        void RibbonItem_Loaded(object sender, RoutedEventArgs e)
        {            
            try
            {
                if (!HasLoaded)
                {
                    this.Margin = new Thickness(0, 0, 2, 0);
                    if (_tabsItem != null)
                    {
                        Tabs tabs = _tabsItem.Tabs;
                        Ribbon ribbon = tabs.Ribbon as Ribbon;
                        this.Ribbon = ribbon;
                    }
                    //
                    this.RITitle.Height = 16;
                    this.RIMain.Height = this.Height - RITitle.Height;                    
                    // search RxButton
                    //foreach (UIElement el in this.Children)
                    //{
                    //    if (el is RxButton)
                    //    {
                    //        _rxButton = el as RxButton;
                    //        if (this.Children.Contains(_rxButton))
                    //            this.Children.Remove(_rxButton);
                    //        if (!RITitle.Children.Contains(_rxButton))
                    //            RITitle.Children.Add(_rxButton);
                    //        break;
                    //    }
                    //}
                    //if (HasRxButton)
                    //{
                    //    _rxButton.SetValue(Grid.ColumnProperty, 1);
                    //    _rxButton.Width = 15;
                    //    _rxButton.Height = 15;
                    //    _rxButton.VerticalAlignment = VerticalAlignment.Center;
                    //    _rxButton.ImageUrl = RibbonUtils.GetImageSource("img/rxImage.png");
                    //    RITitle_rightColumn.Width = new GridLength(15);
                    //}
                    //
                    ArrangeElements(this.Content);
                    HasLoaded = true;

                    //if (RibbonItemLoaded != null)
                    //    RibbonItemLoaded(this, null);
                }
            }
            catch
            {
                //throw new Exception("Invalid Ribbon format");                
            }            
        }

        private bool HasLoaded = false;

        void RibbonItem_MouseLeave(object sender, MouseEventArgs e)
        {
            sbBorderMouseEnter.Stop();
            sbMainMouseEnter.Stop();
            sbBorderMouseLeave.Begin();
            sbMainMouseLeave.Begin();
        }

        void RibbonItem_MouseEnter(object sender, MouseEventArgs e)
        {
            sbBorderMouseLeave.Stop();
            sbMainMouseLeave.Stop();
            sbBorderMouseEnter.Begin();
            sbMainMouseEnter.Begin();
        }

        #endregion

        #region Methods

        void ArrangeElements(RibbonButtonsGroup group)
        {                        
            // search buttons
            foreach (UIElement el in group.Children)
            {
                if (el is RibbonButtonBase)
                {
                    RibbonButtonBase btn = el as RibbonButtonBase;
                    btn.RibbonItem = this;
                    btn.ArrangeInGroup();
                }
                else if (el is RibbonColorButton)
                {
                    RibbonColorButton btn = el as RibbonColorButton;
                    btn.RibbonItem = this;
                    btn.ArrangeInGroup();
                }
                else if (el is RibbonComboBox)
                {
                    RibbonComboBox combo = el as RibbonComboBox;
                    combo.RibbonItem = this;
                    combo.SetStyle(this.Ribbon.comboStyle);                    
                    //combo.ArrangeInGroup();
                }                
            }
            // search 
            foreach (UIElement el in group.Children)
            {
                if (el is RibbonButtonsGroup)                                    
                    ArrangeElements(el as RibbonButtonsGroup);                
                if (el is Border)                
                    ArrangeElements((el as Border).Child as RibbonButtonsGroup);                 
            }
        }       

        public FrameworkElement FindControl(string id)
        {
            FrameworkElement result = null;
            foreach (RibbonButtonsGroup g in (this.Content as RibbonButtonsGroup).DescendantsChildsAndSelf)
            {
                FrameworkElement el = g.FindControl(id);
                if (el != null)
                {
                    result = el;
                    break;
                }
            }                            
            return result;            
        }

        #endregion       
    }

    public class RxButton : RibbonButton { }
}
