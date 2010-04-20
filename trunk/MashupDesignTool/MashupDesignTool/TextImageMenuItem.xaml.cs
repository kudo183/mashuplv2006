using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace MashupDesignTool
{
    public partial class TextImageMenuItem : MenuItem
    {
        private const double MENU_ITEM_HEIGHT = 24;
        public delegate void OnSelectMenuItem(object sender, MenuItemEventArgs e);
        public event OnSelectMenuItem SelectMenuItem;

        private string text;
        private string imageSource;
        private bool enabled;

        public string Text
        {
            get { return text; }
            set 
            { 
                text = value;
                textBlock1.Text = text;
            }
        }
        
        public string ImageSource
        {
            get { return imageSource; }
            set 
            { 
                imageSource = value; 
                image1.Source = new BitmapImage(new Uri(imageSource, UriKind.Relative));
            }
        }

        public bool Enabled
        {
            get { return enabled; }
            set 
            { 
                enabled = value;
                if (enabled == true)
                {
                    textBlock1.Foreground = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    textBlock1.Foreground = new SolidColorBrush(Color.FromArgb(255, 172, 168, 153));
                }
            }
        }

        public TextImageMenuItem() : this("", "")
        {
        }

        public TextImageMenuItem(string text, string imageSource)
        {
            InitializeComponent();
            Text = text;
            ImageSource = imageSource;
            Enabled = true;
        }

        public override double MenuItemWidth()
        {
            return base.MenuItemWidth();
        }

        public override double MenuItemHeight()
        {
            return MENU_ITEM_HEIGHT;
        }

        private void MenuItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Enabled)
                rectangle1.Visibility = System.Windows.Visibility.Visible;
        }

        private void MenuItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Enabled)
                rectangle1.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void MenuItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Enabled == false)
                e.Handled = true;
            else if (SelectMenuItem != null)
                SelectMenuItem(this, new MenuItemEventArgs(this));
        }

        private void MenuItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Enabled == false)
                e.Handled = true;
        }

        private void MenuItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Enabled == false)
                e.Handled = true;
        }

        private void MenuItem_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Enabled == false)
                e.Handled = true;
            else if (SelectMenuItem != null)
                SelectMenuItem(this, new MenuItemEventArgs(this));
        }
    }
}
