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

namespace MyMenu
{
    public class SevenSubMenuItem : BasicLibrary.Menu.ISubMenuItem
    {
        Liquid.MenuItem _item = new Liquid.MenuItem();

        public delegate void ItemSelectedHandler(object sender, string url);
        public event ItemSelectedHandler ItemSelected;

        private string _URL;

        public string URL
        {
            get { return _URL; }
            set { _URL = value; }
        }

        public Liquid.MenuItem Item
        {
            get { return _item; }
            set { _item = value; }
        }

        public SevenSubMenuItem()
        {
            _item.MouseLeftButtonDown += new MouseButtonEventHandler(_item_MouseLeftButtonDown);
        }

        void _item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_item.Content == null || (_item.Content is Liquid.Menu) == false)
            {
                if (ItemSelected != null)
                    ItemSelected(this, _URL);
            }
        }

        #region SubMenuItem Members
        public void AddItemSubMenu(BasicLibrary.Menu.ISubMenu subMenu)
        {
            _item.Content = (subMenu as SevenSubMenu).Menu;
        }

        #endregion
    }
}
