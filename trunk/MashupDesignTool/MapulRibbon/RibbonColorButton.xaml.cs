///////////////////////////////////////////////////////////////////////////////
//
//  RibbonColorButton.xaml.cs   			SilverlightRibbon 1.0
// 
//  Copyright (c) 2008 Mapul Inc. All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;

namespace MapulRibbon
{
    [ContentProperty("RibbonColorList")]
    public partial class RibbonColorButton : StackPanel
    {    
        public RibbonColorButton()
		{
            // Required to initialize variables
            InitializeComponent();
            // Attach an events
            this.Loaded += new RoutedEventHandler(RibbonButton_Loaded);
            this.button.Click += new RoutedEventHandler(Button_Click);
            this.button.LostFocus += new RoutedEventHandler(_button_LostFocus);
        }                   

        void RibbonButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.HasLoaded)
            {                
                // create image
                if (image == null && ImageUrl != null && ImageUrl.ToString() != "")
                {
                    image = new Image();
                    image.VerticalAlignment = VerticalAlignment.Top;
                    image.HorizontalAlignment = HorizontalAlignment.Center;
                    image.Stretch = Stretch.Uniform;
                    image.SetValue(Image.SourceProperty, ImageUrl);
                    ContentPanel.Children.Add(image);                                        
                }                

                // create list
                if (RibbonColorList != null && !popupPanel.Children.Contains(RibbonColorList))
                {
                    popupPanel.Children.Add(RibbonColorList);
                    RibbonColorList.Popup = popup;
                    RibbonColorList.OnColorChanged += new RibbonColorList.ColorChangedHandler(RibbonColorList_OnColorChanged);
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
                    //panel.Style = tooltipStyle2;                

                    ToolTip t = new ToolTip();
                    t.Style = tooltipStyle;
                    t.Content = panel;
                    ToolTipService.SetToolTip(this, t);
                }

                //if (this.button.Style == null)
                //    this.button.Style = buttonStyle;

                HasLoaded = true;
            }           
        }

        void RibbonColorList_OnColorChanged(Color c)
        {
            this.Color = c;
            //_c.Background = new SolidColorBrush(c);
        }        
                
        # region Events
        
        private bool HasLoaded = false;

        void Button_Click(object sender, RoutedEventArgs e)
        {
            if (RibbonColorList != null)
            {
                //RibbonList.SelectedIndex = -1;
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
            if (RibbonColorList != null && popup.IsOpen)
                popup.IsOpen = false;
        }

        public void ArrangeInGroup()
        {
            button.Height = button.MaxHeight = this.RibbonItem.RIMain.Height / this.ParentGroup.VertButtonsCount;
            // если задана высота группы
            if (this.ParentGroup.Height.ToString() != "NaN")
                button.Height = this.ParentGroup.Height / this.ParentGroup.VertButtonsCount;
            // если задана высота кнопки
            if (this.Height.ToString() != "NaN")
                button.Height = this.Height;

            ContentPanel.Orientation = Orientation.Horizontal;
            
            //if (ParentGroup.Orientation == Orientation.Vertical && this.ParentGroup.VertButtonsCount > 1)
            //if (this.ParentGroup.ButtonsCount > 1 && arrowImage != null)
            //    ContentPanel.Orientation = Orientation.Horizontal;

            // if small button
            //if (this.ParentGroup.VertButtonsCount > 1 || (this.ParentGroup.Orientation == Orientation.Horizontal))
            //{
                image.Width = button.Height;
                if (image.Width > 11)
                    image.Width -= 11;

                //image.Height -= 5;
            //}

            //if (ContentPanel.Orientation == Orientation.Vertical)
            //{                
            //    if (arrowImage != null)
            //        image.Height -= arrowImage.Height;
            //}
            //else
            //{                
                if (arrowImage != null)
                {
                    arrowImage.HorizontalAlignment = HorizontalAlignment.Left;
                    arrowImage.VerticalAlignment = VerticalAlignment.Center;
                }

                _c.Width = image.Width + 4;                
            //}            
        }

        # endregion

        # region Fields

        public event RoutedEventHandler OnClick;

        private const string arrowImageSource = "img/arrowImage.png";       

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

        public bool IsEnabled
        {
            get { return button.IsEnabled; }
            set { button.IsEnabled = value; }
        }

        private ImageSource _imageUrl;
        public ImageSource ImageUrl
        {
            get
            {
                return _imageUrl;
            }
            set
            {
                _imageUrl = value;
            }
        }     

        private Image image = null;
        private Image arrowImage = null;        

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

        private RibbonColorList _ribbonColorList;
        public RibbonColorList RibbonColorList
        {
            get { return _ribbonColorList; }
            set { _ribbonColorList = value; }
        }

        public delegate void OnColorChangedEventHandler(object sender, ColorEventArgs e);
        public event OnColorChangedEventHandler OnColorChanged;

        private Color _color = Colors.Black;
        public Color Color
        {
            get { return _color; }
            private set {

                //if (Color != value)
                //{
                    _c.Background = new SolidColorBrush(value);
                    _color = value;

                    if (OnColorChanged != null)
                        OnColorChanged(this, new ColorEventArgs(value));
                //}
            }
        }

        public Color NextColor
        {
            get {
                return RibbonColorList.GetNextColor(); 
            }           
        }

        public Color GetNextColor(int index)
        {
            //return RibbonColorList.GetNextColor(index);            
            return RibbonColorList.GetNextColor();            
        }
        #endregion
    }

    public class ColorEventArgs
    {
        public ColorEventArgs(Color color)
        {
            this.Color = color;
        }

        public Color Color;
    }
}