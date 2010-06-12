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
    public partial class test : BasicLibrary.BasicControl
    {
        public event MDTEventHandler Test1;
        public event MDTEventHandler Test2;

        public delegate void abchandler ();
        public event abchandler abcdef;
        public delegate void a(object sender, string ab);
        public event a bde;

        public test()
        {
            InitializeComponent();

            this.AddEventNameToList("Test1");
            this.AddEventNameToList("Test2");
            this.AddEventNameToList("abcdef");
            this.AddEventNameToList("bde");
        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Test1 != null)
                Test1(this, "<abc>Hello world</abc>");
        }

        private void LayoutRoot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Test2 != null)
                Test2(this, "<abc>Hello world Hello world</abc>");
        }
    }
}
