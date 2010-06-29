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
    public class SevenSubMenu : BasicLibrary.Menu.ISubMenu
    {
        Liquid.Menu _menu = new Liquid.Menu();

        public Liquid.Menu Menu
        {
            get { return _menu; }
            set { _menu = value; }
        }
        #region SubMenu Members
        public void AddSubMenuItem(BasicLibrary.Menu.ISubMenuItem subMenuItem)
        {
            _menu.Items.Add((subMenuItem as SevenSubMenuItem).Item);
        }

        #endregion
    }
}
