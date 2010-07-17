using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;
namespace MyMenu
{
    public class SevenMenuItem : BasicLibrary.BasicControl, BasicLibrary.Menu.IMenuItem
    {

        public delegate void ItemSelectedHandler(object sender, string url);
        public event ItemSelectedHandler ItemSelected;

        #region MenuItem Members

        public void AddItemSubMenu(BasicLibrary.Menu.ISubMenu subMenu)
        {
            _subMenu = subMenu as SevenSubMenu;
            _subMenu.Menu.ItemSelected += new Liquid.MenuEventHandler(Menu_ItemSelected);
            _subMenu.Menu.MouseLeave += new MouseEventHandler(Menu_MouseLeave);
            _subMenu.Menu.MouseEnter += new MouseEventHandler(Menu_MouseEnter);
            p.Child = _subMenu.Menu;
            Hide();
        }

        void Menu_ItemSelected(object sender, Liquid.MenuEventArgs e)
        {
            Hide();
        }

        #endregion

        Image _Icon = new Image();
        string _Title;

        SevenSubMenu _subMenu;
        Popup p = new Popup() { IsOpen = false };

        private string _ImageURL;

        public string ImageURL
        {
            get { return _ImageURL; }
            set
            {
                _ImageURL = value;
                _Icon.Source = new BitmapImage(new Uri(_ImageURL, UriKind.RelativeOrAbsolute));
            }
        }

        private string _URL;

        public string URL
        {
            get { return _URL; }
            set { _URL = value; }
        }

        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                //ToolTipService.SetToolTip(_Icon, _Title);
            }
        }

        Grid LayoutRoot = new Grid();
        SevenMenu _menu;

        public SevenMenuItem(SevenMenu menu)
        {
            _menu = menu;
            Content = LayoutRoot;
            LayoutRoot.Children.Add(_Icon);
            LayoutRoot.Children.Add(p);
            LayoutRoot.MouseEnter += new MouseEventHandler(_Icon_MouseEnter);
            LayoutRoot.MouseLeave += new MouseEventHandler(_Icon_MouseLeave);
            LayoutRoot.MouseLeftButtonUp += new MouseButtonEventHandler(LayoutRoot_MouseLeftButtonUp);
            //ToolTipService.SetPlacement(_Icon, System.Windows.Controls.Primitives.PlacementMode.Top);
        }

        void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_subMenu != null && _subMenu.Menu != null)
                return;
            if (ItemSelected != null)
                ItemSelected(this, URL);
        }

        void Menu_MouseEnter(object sender, MouseEventArgs e)
        {
            Show();
        }

        void Menu_MouseLeave(object sender, MouseEventArgs e)
        {
            Hide();
        }

        void _Icon_MouseLeave(object sender, MouseEventArgs e)
        {
            Hide();
        }

        void _Icon_MouseEnter(object sender, MouseEventArgs e)
        {
            _menu.TbTooptip.Text = Title;
            Show();
        }

        private void Hide()
        {
            if (_subMenu == null || _subMenu.Menu == null)
                return;
            _subMenu.Menu.Hide();
            foreach (UIElement ui in _subMenu.Menu.Items)
            {
                Liquid.MenuItem mi = ui as Liquid.MenuItem;
                if (mi != null)
                {
                    Liquid.Menu m = mi.Content as Liquid.Menu;
                    if (m != null)
                    {
                        m.Hide();
                    }
                }
            }
            p.IsOpen = false;
        }

        private void Show()
        {
            if (_subMenu == null || _subMenu.Menu == null)
                return;
            _subMenu.Menu.Show();
            p.VerticalOffset = _Icon.ActualHeight;
            p.IsOpen = true;
        }
    }
}
