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

namespace MashupDesignTool
{
    public partial class ContextMenu : UserControl
    {
        List<MenuItem> menuItems;

        public List<MenuItem> MenuItems
        {
            get { return menuItems; }
            set { menuItems = value; }
        }

        public ContextMenu()
        {
            InitializeComponent();
            Width = 250;
            Height = 2;
            menuItems = new List<MenuItem>();
        }

        public void AddMenuItem(MenuItem item)
        {
            menuItems.Add(item);
            //Width += item.MenuItemWidth();
            Height += item.MenuItemHeight();
            LayoutRoot.Children.Add(item);
        }

        public void RemoveMenuItem(MenuItem item)
        {
            if (menuItems.Contains(item))
            {
                menuItems.Remove(item);
                //Width += item.MenuItemWidth();
                Height -= item.MenuItemHeight();
                LayoutRoot.Children.Remove(item);
            }
        }

        public void ShowContextMenu()
        {
            ShowPopup.Begin();
            this.Visibility = System.Windows.Visibility.Visible;
            this.Focus();
        }

        public void HideContextMenu()
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            HideContextMenu();
            e.Handled = true;
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            HideContextMenu();
        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
