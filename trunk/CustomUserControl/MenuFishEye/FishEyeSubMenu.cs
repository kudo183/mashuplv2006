using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BasicLibrary;
using ISNet.Silverlight.WebAqua;

namespace MyMenu
{
    public class FishEyeSubMenu:BasicLibrary.Menu.ISubMenu
    {
        ObservableCollection<WebFishEyeStackButton> _subMenu = new ObservableCollection<WebFishEyeStackButton>();

        public ObservableCollection<WebFishEyeStackButton> SubMenu
        {
            get { return _subMenu; }
            set { _subMenu = value; }
        }
        #region ISubMenu Members

        public void AddSubMenuItem(BasicLibrary.Menu.ISubMenuItem subMenuItem)
        {
            _subMenu.Add(subMenuItem as WebFishEyeStackButton);
        }

        #endregion
    }
}
