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
    public class FishEyeMenuItem:WebFishEyeButton, BasicLibrary.Menu.IMenuItem
    {
        #region IMenuItem Members

        public void AddItemSubMenu(BasicLibrary.Menu.ISubMenu subMenu)
        {
            StackButtons  = (subMenu as FishEyeSubMenu).SubMenu;
        }

        #endregion
    }
}
