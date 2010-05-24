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

namespace Testcontrol1
{
    public partial class SilverlightControl1 : BasicLibrary.BasicControl
    {
        public SilverlightControl1()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            LayoutRoot.Background = new SolidColorBrush(Colors.Yellow);
        }

    }
}
