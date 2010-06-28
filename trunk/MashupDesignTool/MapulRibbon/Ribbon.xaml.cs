///////////////////////////////////////////////////////////////////////////////
//
//  Ribbon.xaml.cs   			SilverlightRibbon 1.0
// 
//  Copyright (c) 2008 Mapul Inc. All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MapulRibbon
{
    public partial class Ribbon : StackPanel
    {

        #region ctor

        public Ribbon()
        {
            InitializeComponent();

            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.VerticalAlignment = VerticalAlignment.Stretch;            
            //
            this.Loaded += new RoutedEventHandler(Ribbon_Loaded);
            this.Height = 145;
        }   

        #endregion        

        #region Fields

        internal Style _ribbonTabItemStyle = null;
        public event RoutedEventHandler //OnRibbonButtonClick, 
            //OnHelpButtonClick, 
            OnMailButtonClick, OnCloseButtonClick, OnHideButtonClick;
        private AboutTemplate _ribbonAboutTemplate;
        private RibbonPanel _ribbonPanel;
        public delegate void OnSelectionTabChangedEventHandler(object sender, SelectionChangedEventArgs e);
        public event OnSelectionTabChangedEventHandler OnSelectionTabChanged;        

        private Tabs _tabs;

        #endregion

        #region Properties

        public Visibility RibbonVisibility
        {
            get { return this.Visibility; }
            set
            {
                this.Visibility = value;
                RibbonButton.Visibility = value;
            }
        }        

        public Tabs Tabs
        {
            get { return _tabs; }
            set { _tabs = value; }
        }

        public RibbonPanel RibbonPanel
        {
            get { return _ribbonPanel; }
            set { _ribbonPanel = value; }
        }

        public string Title
        {
            get
            {
                return lblTitle.Text;
            }
            set
            {
                lblTitle.Text = value;
            }
        }

        public string Title2
        {
            get
            {
                return lblTitle2.Text;
            }
            set
            {
                lblTitle2.Text = value;
            }
        }

        //public RibbonButton HelpButton
        //{
        //    get { return _helpButton; }
        //}

        //public HyperlinkButton HelpButton
        //{
        //    get { return _helpButton; }
        //}

        public RibbonButton MailButton
        {
            get { return _mailButton; }
        }

        public RibbonButton HideButton
        {
            get { return _hideButton; }
        }

        public RibbonButton CloseButton
        {
            get { return _closeButton; }
        }

        public Grid TabContainer
        {
            get { return tabContainer; }
        }
        
        //public bool ShowHelpButton
        //{
        //    get
        //    {
        //        if (_helpButton.Visibility == Visibility.Visible)
        //            return true;
        //        else return false;
        //    }
        //    set
        //    {
        //        if (value)
        //            _helpButton.Visibility = Visibility.Visible;
        //        else _helpButton.Visibility = Visibility.Collapsed;
        //    }
        //}

        public bool ShowHideButton
        {
            get
            {
                if (_hideButton.Visibility == Visibility.Visible)
                    return true;
                else return false;
            }
            set
            {
                if (value)
                    _hideButton.Visibility = Visibility.Visible;
                else _hideButton.Visibility = Visibility.Collapsed;
            }
        }

        public bool ShowMailButton
        {
            get
            {
                if (_mailButton.Visibility == Visibility.Visible)
                    return true;
                else return false;
            }
            set
            {
                if (value)
                    _mailButton.Visibility = Visibility.Visible;
                else _mailButton.Visibility = Visibility.Collapsed;
            }
        }

        public bool ShowCloseButton
        {
            get
            {
                if (_closeButton.Visibility == Visibility.Visible)
                    return true;
                else return false;
            }
            set
            {
                if (value)
                    _closeButton.Visibility = Visibility.Visible;
                else _closeButton.Visibility = Visibility.Collapsed;
            }
        }

        //public ImageSource ImageRibbon
        //{
        //    get { return imageRibbon.Source; }
        //    set { imageRibbon.Source = value; }
        //}

        //public bool ImageRibbonVisibillity
        //{
        //    get { return puRibbonButton.IsOpen; }
        //    set { puRibbonButton.IsOpen = value;  }            
        //}

        public AboutTemplate RibbonAboutTemplate
        {
            get { return _ribbonAboutTemplate; }
            set { _ribbonAboutTemplate = value; }
        }

        #endregion

        #region Events

        void Ribbon_Loaded(object sender, RoutedEventArgs e)
        {               
            // search tabControl
            foreach (UIElement el in this.Children)
            {
                if (el is Tabs)
                {
                    _tabs = el as Tabs;
                    // init tabControl
                    _tabs.SetValue(Canvas.ZIndexProperty, 1);
                    _tabs.Ribbon = this;
                    _tabs.Height = this.Height - HeaderPanel.Height;     //!
                    _tabs.SelectionChanged += new SelectionChangedEventHandler(_tabs_SelectionChanged);
                    if (this.Children.Contains(_tabs))
                        this.Children.Remove(_tabs);
                    if (!tabContainer.Children.Contains(_tabs))
                        tabContainer.Children.Add(_tabs);
                    _tabs.SetValue(Grid.ColumnProperty, 0);
                    break;
                }
            }
            //search about control
            foreach (UIElement el in this.Children)
            {
                if (el is AboutTemplate)
                {
                    _ribbonAboutTemplate = el as AboutTemplate;
                    if (this.Children.Contains(_ribbonAboutTemplate))
                        this.Children.Remove(_ribbonAboutTemplate);
                    if (!aboutTemplate.Children.Contains(_ribbonAboutTemplate))
                        aboutTemplate.Children.Add(_ribbonAboutTemplate);
                    break;
                }
            }
            // search RibbonPanel 
            foreach (UIElement el in this.Children)
            {
                if (el is RibbonPanel)
                {
                    RibbonPanel = el as RibbonPanel;
                    RibbonPanel.Margin = new Thickness(50, 0, 0, 0);
                    RibbonPanel.SetValue(Canvas.ZIndexProperty, 1);
                    if (this.Children.Contains(RibbonPanel))
                        this.Children.Remove(RibbonPanel);
                    if (!_topPanel.Children.Contains(RibbonPanel))
                        _topPanel.Children.Insert(0, RibbonPanel);
                    break;
                }
            }

            if (_tabs == null)
                throw new Exception("Not valid ribbon format.");

            if (RibbonTabItemStyle != null)
                _ribbonTabItemStyle = RibbonTabItemStyle;

            if (RibbonTabControlStyle != null && _tabs != null)
                _tabs.ApplyStyle(RibbonTabControlStyle);

            //RibbonButton.Click += new RoutedEventHandler(RibbonButton_Click);
            HideButton.OnClick -= new RoutedEventHandler(HideButton_OnClick);
            HideButton.OnClick += new RoutedEventHandler(HideButton_OnClick);
            //HelpButton.OnClick += new RoutedEventHandler(HelpButton_OnClick);            
            MailButton.OnClick += new RoutedEventHandler(MailButton_OnClick);
            CloseButton.OnClick += new RoutedEventHandler(CloseButton_OnClick);            
        }

        double oldHeight;
        bool isShown = true;

        void HideButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.OnHideButtonClick != null)
                this.OnHideButtonClick(sender, e);
            //else
            //{

            //if (tabContainer.Visibility == Visibility.Collapsed)
            //{
            //    tabContainer.Visibility = Visibility.Visible;
            //    this.Height = oldHeight;
            //}
            //else
            //{
            //    tabContainer.Visibility = Visibility.Collapsed;
            //    oldHeight = this.ActualHeight;
            //    this.Height = 55;
            //}
            //}
            if (!isShown)
            {
                this.Height = oldHeight;
                isShown = true;
            }
            else
            {
                oldHeight = this.Height;
                this.Height = 55;
                isShown = false;
            }
        }

        void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.OnCloseButtonClick != null)
                this.OnCloseButtonClick(sender, e);            
        }

        void MailButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.OnMailButtonClick != null)
                this.OnMailButtonClick(sender, e);
        }

        //void HelpButton_OnClick(object sender, RoutedEventArgs e)
        //{
        //    if (this.OnHelpButtonClick != null)
        //        this.OnHelpButtonClick(sender, e);
        //}

        void _tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OnSelectionTabChanged != null)
                OnSelectionTabChanged(sender, e);
        }

        private void _hideButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        //void RibbonButton_Click(object sender, RoutedEventArgs e)
        //{            
        //    if (this.OnRibbonButtonClick != null)
        //        this.OnRibbonButtonClick(RibbonButton, e);
        //    else
        //    {
        //        UIElement _element = aboutControl;
        //        if (_ribbonAboutTemplate != null)
        //            _element = _ribbonAboutTemplate;
        //        //
        //        if (_element.Visibility == Visibility.Collapsed)
        //            _element.Visibility = Visibility.Visible;
        //        else
        //            _element.Visibility = Visibility.Collapsed;
        //    }             
        //}

        #endregion                                                    
    }

    public class RibbonPanel : StackPanel
    {
        public FrameworkElement FindControl(string id)
        {
            FrameworkElement el = null;
            if (this.Children.Count > 0 && this.Children[0] is RibbonButtonsGroup)
            {            
                foreach (FrameworkElement e in (this.Children[0] as RibbonButtonsGroup).Children)
                    if (e.Name == id)
                    {
                        el = e;
                        break;
                    }
            }
            return el;
        }
    }
}
