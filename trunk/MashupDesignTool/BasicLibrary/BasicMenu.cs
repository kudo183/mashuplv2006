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
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace BasicLibrary.Menu
{
    /// <summary>
    /// MenuItem can thuc thi giao dien nay
    /// </summary>
    public interface IMenuItem
    {
        void AddItemSubMenu(ISubMenu subMenu);
    }
    /// <summary>
    /// SubMenu can thuc thi giao dien nay
    /// </summary>
    public interface ISubMenu
    {
        void AddSubMenuItem(ISubMenuItem subMenuItem);
    }
    /// <summary>
    /// SubMenuItem can thuc thi giao dien nay
    /// </summary>
    public interface ISubMenuItem
    {
        void AddItemSubMenu(ISubMenu subMenu);
    }

    /// <summary>
    /// Menu can thuc thi giao dien nay
    /// </summary>
    public interface IMenu
    {
        event BasicControl.MDTEventHandler SelectionChanged;
        IMenuItem CreateMenuItem(XElement e);
        ISubMenu CreateSubMenu();
        ISubMenuItem CreateSubMenuItem(XElement e);
        void AddMenuItem(IMenuItem item);
        void ClearMenu();
    }
    /// <summary>
    /// Lop Menu truu tuong
    /// </summary>
    public abstract class BasicMenu : BasicControl, IMenu
    {
        #region event
        public event MDTEventHandler SelectionChanged;
        protected virtual void OnSelectionChanged(string xml)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, xml);
                //try
                //{
                //    XDocument doc = XDocument.Parse(xml);
                //    XElement root = doc.Element("root");
                //    XAttribute att = root.Attribute("url");
                //    if (att != null)
                //    {
                //        OnLinkClicked(att.Value);
                //    }
                //}
                //catch { }
            }
        }
        #endregion

        private string _xmlString;

        public string XmlString
        {
            get { return _xmlString; }
            set
            {
                _xmlString = value;
                LoadMenu();
            }
        }

        public BasicMenu()
            : base()
        {
            parameterNameList.Add("XmlString");
            AddEventNameToList("SelectionChanged");
        }

        #region abstract method
        abstract public IMenuItem CreateMenuItem(XElement e);
        abstract public ISubMenu CreateSubMenu();
        abstract public ISubMenuItem CreateSubMenuItem(XElement e);
        abstract public void AddMenuItem(IMenuItem item);
        abstract public void ClearMenu();
        #endregion

        #region load menu from xml string
        private void LoadMenu()
        {
            ClearMenu();
            try
            {
                XDocument doc = XDocument.Parse(_xmlString);
                XElement root = doc.Element("menu");
                foreach (XElement element in root.Elements())
                {
                    XElement e = new XElement("root");
                    foreach (XAttribute attri in element.Attributes())
                    {
                        e.SetAttributeValue(attri.Name, attri.Value);
                    }
                    IMenuItem mi = CreateMenuItem(e);

                    AddMenuItem(mi);
                    LoadSubMenu(mi, element);
                }
            }
            catch { }
        }

        private void LoadSubMenu(IMenuItem mi, XElement MenuItemElement)
        {
            XElement subMenuElement = MenuItemElement.Element("submenu");
            if (subMenuElement == null)
                return;
            ISubMenu sm = CreateSubMenu();
            mi.AddItemSubMenu(sm);
            LoadSubMenuItem(sm, subMenuElement);
        }

        private void LoadSubMenu(ISubMenuItem smi, XElement SubMenuItemElement)
        {
            XElement subMenuElement = SubMenuItemElement.Element("submenu");
            if (subMenuElement == null)
                return;
            ISubMenu sm = CreateSubMenu();
            smi.AddItemSubMenu(sm);
            LoadSubMenuItem(sm, subMenuElement);
        }

        private void LoadSubMenuItem(ISubMenu sm, XElement subMenuElement)
        {
            foreach (XElement element in subMenuElement.Elements())
            {
                XElement e = new XElement("root");
                foreach (XAttribute attri in element.Attributes())
                {
                    e.SetAttributeValue(attri.Name, attri.Value);
                }
                ISubMenuItem mi = CreateSubMenuItem(e);
                sm.AddSubMenuItem(mi);
                LoadSubMenu(mi, element);
            }
        }
        #endregion
    }
}
