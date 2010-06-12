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
    public partial class SilverlightControl1 : BasicLibrary.BasicControl
    {
        public SilverlightControl1()
        {
            InitializeComponent();

            this.AddOperationNameToList("Hello1");
            this.AddOperationNameToList("Hello2");
            this.AddOperationNameToList("abc");
        }

        public void abc() { }

        public void Hello1(string xmlString)
        {
            MessageBox.Show(xmlString);
        }

        public void Hello2(string xmlString)
        {
            LayoutRoot.Background = new SolidColorBrush(Colors.Yellow);
        }
    }
}
