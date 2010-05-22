///////////////////////////////////////////////////////////////////////////////
//
//  RibbonListItem.cs   			SilverlightRibbon 1.0
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
    public class RibbonListItem : ListBoxItem
    {
        #region fields
        private Image _image;
        private TextBlock _textBlock;
        private StackPanel _stackPanel;
        private double _imageWidth = 10.0;
        private double _imageHeight = 10.0;
        private ImageSource _imageSource;
        private string _text = "";
        private VerticalAlignment _vAlignment = VerticalAlignment.Center;
        private HorizontalAlignment _hAlignment = HorizontalAlignment.Center;
        #endregion

        #region properties
        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set
            {
                _imageSource = value;
                if (this._image != null)
                    _image.Source = value;
            }
        }

        public double ImageWidth
        {
            get { return _imageWidth; }
            set
            {
                _imageWidth = value;
                if (this._image != null)
                    _image.Width = value;
            }
        }

        public double ImageHeight
        {
            get { return _imageHeight; }
            set
            {
                _imageHeight = value;
                if (this._image != null)
                    _image.Height = value;
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                if (_textBlock != null)
                    _textBlock.Text = value;
            }
        }

        public VerticalAlignment vAlignment
        {
            get { return _vAlignment; }
            set { _vAlignment = value; }
        }

        public HorizontalAlignment hAlignment
        {
            get { return _hAlignment; }
            set { _hAlignment = value; }
        }
        #endregion       

        #region ctor
        public RibbonListItem()
        {
            this.Loaded += new RoutedEventHandler(RibbonListItem_Loaded);
        }
        #endregion

        #region Events
        // OnClick event        
        public delegate void OnClickEventHandler(object sender, EventArgs e);
        public event OnClickEventHandler OnClick;

        void RibbonListItem_Loaded(object sender, RoutedEventArgs e)
        {
            // create stackPanel
            _stackPanel = new StackPanel();
            _stackPanel.Orientation = Orientation.Horizontal;
            this.Content = _stackPanel;            

            // create _image
            if (this.ImageSource != null && this.ImageSource.ToString() != "")
            {
                _image = new Image();
                _image.Width = this.ImageWidth;
                _image.Height = this.ImageHeight;
                _image.Source = this.ImageSource;
                _image.Stretch = Stretch.Uniform;
                _image.Margin = new Thickness(4, 2, 0, 0);
                _image.VerticalAlignment = vAlignment;
                _stackPanel.Children.Add(_image);
            }
            // create _text
            if (this.Text != "")
            {
                _textBlock = new TextBlock();
                _textBlock.TextWrapping = TextWrapping.NoWrap;
                _textBlock.Text = this.Text;                
                _textBlock.Margin = new Thickness(20, 2, 4, 0);
                _textBlock.VerticalAlignment = vAlignment;
                _textBlock.HorizontalAlignment = hAlignment;
                _stackPanel.Children.Add(_textBlock);
            }                        
        } 
        #endregion

        #region methods
        internal void RaiseOnClick()
        {
            if (OnClick != null)
            {
                OnClick(this, new EventArgs());
            }
        }
        #endregion        
    }    
}
