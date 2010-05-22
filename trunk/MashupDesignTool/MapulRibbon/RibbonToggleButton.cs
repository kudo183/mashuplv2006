///////////////////////////////////////////////////////////////////////////////
//
//  RibbonToggleButton.cs   			SilverlightRibbon 1.0
// 
//  Copyright (c) 2008 Mapul Inc. All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MapulRibbon
{
    public class RibbonToggleButton : RibbonButtonBase
    {
        public RibbonToggleButton()
        {           
            // Attach an events
            this.Loaded += new RoutedEventHandler(RibbonButton_Loaded);            
        }

        private bool _isChecked = false;

        public bool IsChecked
        {
            get {
                if (Button != null)
                    return (Button as ToggleButton).IsChecked.Value;
                else
                    return _isChecked; 
            }
            set {                 
                if (Button != null)
                    (Button as ToggleButton).IsChecked = value;
                else _isChecked = value; 
            }
        }       

        void RibbonButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (Button == null)
            {
                Button = new ToggleButton();
                if (Button.Style == null)
                    Button.Style = toggleButtonStyle;
                //
                ButtonLoaded();
            }

            //
            (Button as ToggleButton).IsChecked = _isChecked;
        }                        
    }
}
