///////////////////////////////////////////////////////////////////////////////
//
//  RibbonButtonBase.xaml.cs   			SilverlightRibbon 1.0
// 
//  Copyright (c) 2008 Mapul Inc. All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Controls.Primitives;

namespace MapulRibbon
{
    [ContentProperty("RibbonList")]
	public abstract partial class RibbonButtonBase : StackPanel 
	{
        public RibbonButtonBase()
        {
            // Required to initialize variables
            InitializeComponent();            
        }

        private ButtonBase _button;
        public ButtonBase Button
        {
            get { return _button; }
            set { _button = value; }
        }            

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { 
                _isEnabled = value;
                if (_button != null)
                    _button.IsEnabled = value;
            }
        }
        
        private StackPanel _contentPanel;
        public StackPanel ContentPanel
        {
            get {                 
                return _contentPanel; 
            }
            set { _contentPanel = value; }
        }        

        # region Events        
        
        public void ButtonLoaded()        
        {
            if (!this.HasLoaded)
            {
                ContentPanel = new StackPanel();
                Button.Content = ContentPanel;
                //ContentPanel.Orientation = this.Orientation;

                this.Button.Click += new RoutedEventHandler(Button_Click);
                this.Button.LostFocus += new RoutedEventHandler(_button_LostFocus);
                this.Button.IsTabStop = false;

                this.Button.Height = this.Height;
                this.Button.Width = this.Width;

                mainPanel.Children.Insert(0, Button);
                // create image
                if (image == null && ImageUrl != null && ImageUrl.ToString() != "")
                {
                    image = new Image();
                    image.VerticalAlignment = VerticalAlignment.Top;
                    image.HorizontalAlignment = HorizontalAlignment.Center;
                    image.Stretch = Stretch.Uniform;
                    image.SetValue(Image.SourceProperty, ImageUrl);

                    //image.Width = this.Width;
                    //image.Height = this.Height;
                    
                    ContentPanel.Children.Add(image);
                }

                // create text
                if (text == null && Text != null && Text != "")
                {
                    text = new TextBlock();
                    text.FontSize = 10;
                    text.Foreground = new SolidColorBrush(Color.FromArgb(255, 74, 133, 205));
                    text.TextAlignment = TextAlignment.Center;
                    text.VerticalAlignment = VerticalAlignment.Top;
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    text.TextWrapping = TextWrapping.Wrap;
                    text.Text = Text;

                    ContentPanel.Children.Add(text);
                }

                // create list
                if (RibbonList != null && !popupPanel.Children.Contains(RibbonList))
                {
                    popupPanel.Children.Add(RibbonList);
                    RibbonList.Popup = popup;
                    //
                    arrowImage = new Image();
                    arrowImage.Height = 4;
                    arrowImage.Width = 8;
                    arrowImage.VerticalAlignment = VerticalAlignment.Top;
                    arrowImage.HorizontalAlignment = HorizontalAlignment.Center;
                    arrowImage.Stretch = Stretch.Uniform;
                    RibbonUtils.SetImage(arrowImage, arrowImageSource);
                    ContentPanel.Children.Add(arrowImage);
                }

                // create tooltip
                if (TooltipTitle != "" || TooltipText != "" && ToolTipService.GetToolTip(this) == null)
                {
                    TextBlock title = new TextBlock();
                    TextBlock tooltiptext = new TextBlock();

                    title.Text = TooltipTitle;
                    title.Style = tooltipTitleStyle;

                    tooltiptext.Text = TooltipText;
                    tooltiptext.Style = tooltipTextStyle;

                    StackPanel panel = new StackPanel();
                    panel.HorizontalAlignment = HorizontalAlignment.Stretch;
                    panel.Children.Add(title);
                    panel.Children.Add(tooltiptext);

                    ToolTip t = new ToolTip();
                    t.Style = tooltipStyle;
                    t.Content = panel;
                    ToolTipService.SetToolTip(this, t);
                }                

                _button.IsEnabled = this.IsEnabled;

                HasLoaded = true;
            }
        }

        private bool HasLoaded = false;

        void Button_Click(object sender, RoutedEventArgs e)
        {
            if (RibbonList != null)
            {
                RibbonList.SelectedIndex = -1;
                popup.IsOpen = !popup.IsOpen;
            }
            else
            {
                if (this.OnClick != null)
                    this.OnClick(this, e);
            }
        }

        void _button_LostFocus(object sender, RoutedEventArgs e)
        {
            if (RibbonList != null && popup.IsOpen)
                popup.IsOpen = false;            
        }

        public void ArrangeInGroup()
        {
            Button.Height = Button.MaxHeight = this.RibbonItem.RIMain.Height / this.ParentGroup.VertButtonsCount;            
            // если задана высота группы
            if (this.ParentGroup.Height.ToString() != "NaN")
                Button.Height = this.ParentGroup.Height / this.ParentGroup.VertButtonsCount;
            // если задана высота кнопки
            if (this.Height.ToString() != "NaN")
                Button.Height = this.Height;
            
            if (this.ParentGroup.ButtonsCount > 1 && arrowImage != null)            
                ContentPanel.Orientation = Orientation.Horizontal;

            // if small button
            if (this.ParentGroup.VertButtonsCount > 1 || (this.ParentGroup.Orientation == Orientation.Horizontal))
            {
                image.Width = Button.Height;
                if (image.Width > 5)
                    image.Width -= 5;
            }

            if (ContentPanel.Orientation == Orientation.Vertical)
            {
                if (text != null)
                    image.Height = Button.Height - text.ActualHeight - 4;

                if (arrowImage != null)                
                    image.Height -= arrowImage.Height;                                
            }
            else
            {
                if (text != null)
                {
                    text.TextWrapping = TextWrapping.NoWrap;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.Margin = new Thickness(10, 0, 10, 0);
                }

                if (arrowImage != null)
                {
                    arrowImage.HorizontalAlignment = HorizontalAlignment.Left;
                    arrowImage.VerticalAlignment = VerticalAlignment.Center;                    
                }                                
            }            
        }
        
        # endregion
        
        # region Fields  
     
        public event RoutedEventHandler OnClick;

        private const string arrowImageSource = "/MapulRibbon;component/img/arrowImage.png";
       
        private string _tooltipTitle = "";

        public string TooltipTitle
        {
            get { return _tooltipTitle; }
            set { _tooltipTitle = value; }
        }

        private string _tooltipText = "";

        public string TooltipText
        {
            get { return _tooltipText; }
            set { _tooltipText = value; }
        }        

        private ImageSource _imageUrl;
        public ImageSource ImageUrl
        {
            get {                   
                return _imageUrl;                 
            }
            set { 
                _imageUrl = value; 
            }
        }

        private string _text;
        public string Text
        {
            get { 
                return _text; 
            }
            set {
                _text = value; 
            }
        }        

        private TextBlock text = null;

        private Image image = null;

        private Image arrowImage = null;

        public double FontSize
        {
            get { return text.FontSize; }
            set { text.FontSize = value; }
        }                

        private RibbonItem _ribbonItem;

        public RibbonItem RibbonItem
        {
            get { return _ribbonItem; }
            internal set { _ribbonItem = value; }
        }

        private RibbonButtonsGroup _parentGroup;

        public RibbonButtonsGroup ParentGroup
        {
            get { return _parentGroup; }
            internal set { _parentGroup = value; }
        }

        private RibbonList _ribbonList;

        public RibbonList RibbonList
        {
            get { return _ribbonList; }
            set { _ribbonList = value; }
        }
        
        #endregion                         
    }    
}