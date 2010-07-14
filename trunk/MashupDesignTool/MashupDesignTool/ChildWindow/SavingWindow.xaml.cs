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
    public partial class SavingWindow : ChildWindow
    {
        public SavingWindow()
        {
            InitializeComponent();
            this.DialogResult = true;
            busyIndicator.IsBusy = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            busyIndicator.IsBusy = false;
            this.DialogResult = false;
        }

        private void ChildWindow_Closed(object sender, EventArgs e)
        {
            busyIndicator.IsBusy = false;
        }
    }
}

