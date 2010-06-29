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
using Liquid;
namespace SL_Menu
{
    public class MyMenuItem : BasicLibrary.Menu.IMenuItem
    {
        Liquid.MainMenuItem _item = new MainMenuItem();

        public Liquid.MainMenuItem Item
        {
            get { return _item; }
            set { _item = value; }
        }
        #region MenuItem Members

        public void AddItemSubMenu(BasicLibrary.Menu.ISubMenu subMenu)
        {
            _item.Content = (subMenu as MySubMenu).Menu;
        }
        #endregion
    }
    public class MySubMenu : BasicLibrary.Menu.ISubMenu
    {
        Liquid.Menu _menu = new Menu();

        public Liquid.Menu Menu
        {
            get { return _menu; }
            set { _menu = value; }
        }
        #region SubMenu Members
        public void AddSubMenuItem(BasicLibrary.Menu.ISubMenuItem subMenuItem)
        {
            _menu.Items.Add((subMenuItem as MySubMenuItem).Item);
        }

        #endregion
    }
    public class MySubMenuItem : BasicLibrary.Menu.ISubMenuItem
    {
        Liquid.MenuItem _item = new MenuItem();

        public Liquid.MenuItem Item
        {
            get { return _item; }
            set { _item = value; }
        }
        #region SubMenuItem Members
        public void AddItemSubMenu(BasicLibrary.Menu.ISubMenu subMenu)
        {
            _item.Content = (subMenu as MySubMenu).Menu;
        }

        #endregion
    }
    public class MyMenu : BasicLibrary.Menu.BasicMenu
    {
        Liquid.MainMenu _menu;
        public MyMenu()
            : base()
        {
            _menu = new Liquid.MainMenu();
            _menu.ItemSelected += new MenuEventHandler(_menu_ItemSelected);
            Content = _menu;
        }

        void _menu_ItemSelected(object sender, MenuEventArgs e)
        {
            if (e.Parameter != null)
                OnSelectionChanged((e.Parameter as Liquid.MenuItem).Text);
        }

        public MyMenu(Liquid.MainMenu menu)
            : base()
        {
            Content = menu;
        }

        public override void ClearMenu()
        {
            _menu.Items.Clear();
        }
        public override void AddMenuItem(BasicLibrary.Menu.IMenuItem item)
        {
            _menu.Items.Add((item as MyMenuItem).Item);
        }

        public override BasicLibrary.Menu.IMenuItem CreateMenuItem(System.Xml.Linq.XElement e)
        {
            MyMenuItem mmi = new MyMenuItem();
            System.Xml.Linq.XAttribute att;
            att = e.Attribute("Text");
            mmi.Item.Text = att.Value;
            return mmi;
        }

        public override BasicLibrary.Menu.ISubMenu CreateSubMenu()
        {
            MySubMenu msm = new MySubMenu();
            return msm;
        }

        public override BasicLibrary.Menu.ISubMenuItem CreateSubMenuItem(System.Xml.Linq.XElement e)
        {
            MySubMenuItem msmi = new MySubMenuItem();
            System.Xml.Linq.XAttribute att;
            att = e.Attribute("Text");
            msmi.Item.Text = att.Value;
            return msmi;
        }
    }
}
