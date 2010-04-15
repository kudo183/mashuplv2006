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
    public partial class MenuItem : UserControl
    {
        public MenuItem()
        {
            InitializeComponent();
        }

        public virtual double MenuItemWidth()
        {
            return 0;
        }

        public virtual double MenuItemHeight()
        {
            return 0;
        }
    }
}
