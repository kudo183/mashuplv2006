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
    public partial class InsertApplicationName : ChildWindow
    {
        public InsertApplicationName(bool saveAs)
        {
            InitializeComponent();
            if (saveAs)
                this.Title = "Save as";
        }

        public string ApplicationName
        {
            get { return txtAppName.Text; }
            set { txtAppName.Text = value; }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (txtAppName.Text.Length != 0)
                this.DialogResult = true;
        }
    }
}

