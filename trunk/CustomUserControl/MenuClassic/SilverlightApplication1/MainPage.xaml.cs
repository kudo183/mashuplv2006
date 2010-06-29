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

namespace SilverlightApplication1
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            myMenu1.XmlString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Menu><MenuItem Text=\"item1\" Icon=\"http://farm5.static.flickr.com/4143/4745849827_548fe02fab_m.jpg\" URL=\"\"><SubMenu><SubMenuItem Text=\"subItem1\" Icon=\"\" URL=\"\"><SubMenu><SubMenuItem Text=\"subItem11\" Icon=\"\" URL=\"\"></SubMenuItem><SubMenuItem Text=\"subItem12\" Icon=\"\" URL=\"\"></SubMenuItem></SubMenu></SubMenuItem><SubMenuItem Text=\"subItem2\" Icon=\"\" URL=\"\"></SubMenuItem></SubMenu></MenuItem><MenuItem Text=\"item2\" Icon=\"http://farm5.static.flickr.com/4143/4745849827_548fe02fab_m.jpg\" URL=\"\"><SubMenu><SubMenuItem Text=\"subItem1\" Icon=\"\" URL=\"\"><SubMenu><SubMenuItem Text=\"subItem11\" Icon=\"\" URL=\"\"></SubMenuItem><SubMenuItem Text=\"subItem12\" Icon=\"\" URL=\"\"></SubMenuItem></SubMenu></SubMenuItem><SubMenuItem Text=\"subItem2\" Icon=\"\" URL=\"\"></SubMenuItem></SubMenu></MenuItem></Menu>";
        }
    }
}
