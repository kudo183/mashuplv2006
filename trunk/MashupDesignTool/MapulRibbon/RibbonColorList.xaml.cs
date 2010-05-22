///////////////////////////////////////////////////////////////////////////////
//
//  RibbonColorList.cs   			SilverlightRibbon 1.0
// 
//  Copyright (c) 2008 Mapul Inc. All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows;
using SilverlightColorPicker;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace MapulRibbon
{
    public partial class RibbonColorList : StackPanel
    {
        #region ctor

        public RibbonColorList()
        {
            InitializeComponent();            
            this.MouseLeave += new MouseEventHandler(RibbonList_MouseLeave);
            this.Loaded += new RoutedEventHandler(RibbonList_Loaded);
            this.AutoHide = false;

            // create colors                       
            colorsList.Clear();
            //colorsList.Add(RibbonUtils.ColorFromArgb("#FF000000"));
            //colorsList.Add(RibbonUtils.ColorFromArgb("#FFFF0000"));
            //colorsList.Add(RibbonUtils.ColorFromArgb("#FF0000FF"));
            //colorsList.Add(RibbonUtils.ColorFromArgb("#FF008000"));
            //colorsList.Add(RibbonUtils.ColorFromArgb("#FF00FF00"));

            //colorsList.Add(RibbonUtils.ColorFromArgb("#FF800080"));
            //colorsList.Add(RibbonUtils.ColorFromArgb("#FF000000"));
            //colorsList.Add(RibbonUtils.ColorFromArgb("#FFFF9900"));
            //colorsList.Add(RibbonUtils.ColorFromArgb("#FF000080"));
            //colorsList.Add(RibbonUtils.ColorFromArgb("#FFFFCC00"));            
            colorsList.Add(RibbonUtils.ColorFromArgb("#FFAD5833"));
            colorsList.Add(RibbonUtils.ColorFromArgb("#FFFF6E00"));
            colorsList.Add(RibbonUtils.ColorFromArgb("#FFFFD500"));
            colorsList.Add(RibbonUtils.ColorFromArgb("#FF92D400"));
            colorsList.Add(RibbonUtils.ColorFromArgb("#FF00AF3F"));

            colorsList.Add(RibbonUtils.ColorFromArgb("#FF0088CE"));
            colorsList.Add(RibbonUtils.ColorFromArgb("#FF2526A9"));
            colorsList.Add(RibbonUtils.ColorFromArgb("#FF6E2585"));
            colorsList.Add(RibbonUtils.ColorFromArgb("#FFC50084"));
            colorsList.Add(RibbonUtils.ColorFromArgb("#FFD0103A"));            
            //colorsList.Add(RibbonUtils.ColorFromArgb("#FFAD5833"));            
        }

        #endregion

        #region Methods

        private void Hide()
        {                     
            if (_popup != null)
                _popup.IsOpen = false;
        }              
        
        public Color GetNextColor()
        {
            if (_indexColor < colorsList.Count - 1) _indexColor++;
            else _indexColor = 0;
            //            
            this._color = colorsList[_indexColor];
            return this.Color;
        }

        //public Color GetNextColor(int index)
        //{
        //    _indexColor = index;
        //    if (_indexColor > colorsList.Count - 1)
        //        _indexColor = index % 7;
        //    //
        //    this._color = colorsList[_indexColor];
        //    return this.Color;
        //}

        #endregion

        #region Events

        void RibbonList_Loaded(object sender, RoutedEventArgs e)
        {
            ColorPicker.ColorSelected += new ColorPicker.ColorSelectedHandler(ColorPicker_ColorSelected);
        }

        void ColorPicker_ColorSelected(Color c)
        {
            this.Color = c;
            this.Hide();
        }

        void RibbonList_MouseLeave(object sender, MouseEventArgs e)
        {
            if (AutoHide)
                this.Hide();
        }       

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel panel = sender as StackPanel;
            Color c = (panel.Background as SolidColorBrush).Color;
            this.Color = c;            
            this.Hide();
            e.Handled = true;
        }

        #endregion

        #region Fields

        public List<Color> colorsList = new List<Color>();
        //private int _indexColor = 0;    
        private int _indexColor = 0;    

        public ColorPicker ColorPicker
        {
            get { return _colorPicker; }            
        }

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

        private Color _color;

        public Color Color
        {
            get { return _color; }
            set {

                //if (value != _color) {

                    if (OnColorChanged != null)
                        OnColorChanged(value);

                    _color = value;
            }
        }

        public delegate void ColorChangedHandler(Color c);
        public event ColorChangedHandler OnColorChanged;
        
        #endregion        
    }    
}
