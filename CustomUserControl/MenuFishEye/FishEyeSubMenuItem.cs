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
using BasicLibrary;
using ISNet.Silverlight.WebAqua;

namespace MyMenu
{
    public class FishEyeSubMenuItem:WebFishEyeStackButton, BasicLibrary.Menu.ISubMenuItem
    {
        #region ISubMenuItem Members

        public void AddItemSubMenu(BasicLibrary.Menu.ISubMenu subMenu)
        {
            
        }

        #endregion
    }
}
