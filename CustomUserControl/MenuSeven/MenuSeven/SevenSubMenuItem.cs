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

        public Liquid.MenuItem Item
        {
            get { return _item; }
            set { _item = value; }
        }
        #region SubMenuItem Members
        public void AddItemSubMenu(BasicLibrary.Menu.ISubMenu subMenu)
        {
            _item.Content = (subMenu as SevenSubMenu).Menu;
        }

        #endregion
    }
}
