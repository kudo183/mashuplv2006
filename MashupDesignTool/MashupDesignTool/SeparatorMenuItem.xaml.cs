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
    public partial class SeparatorMenuItem : MenuItem
    {
        private const double MENU_ITEM_HEIGHT = 3;

        public SeparatorMenuItem()
        {
            InitializeComponent();
        }

        public override double MenuItemWidth()
        {
            return base.MenuItemWidth();
        }

        public override double MenuItemHeight()
        {
            return MENU_ITEM_HEIGHT;
        }
    }
}
