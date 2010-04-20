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
using System.Windows.Media.Imaging;

namespace MashupDesignTool
{
    public partial class ControlTreeViewItem : UserControl
    {
        private string controlName;

        public ControlTreeViewItem()
        {
            InitializeComponent();
        }

        public void SetControlIcon(string uri)
        {
            try
            {
                ControlIcon.Source = new BitmapImage(new Uri(uri));
            }
            catch { }
        }

        public void SetControlDisplayName(string displayName)
        {
            ControlDisplayName.Content = displayName;
        }

        public void SetControlDescription(string description)
        {
            ControlDescription.Content = description;
        }

        public string GetControlName()
        {
            return controlName;
        }

        public void SetControlName(string controlName)
        {
            this.controlName = controlName;
        }
    }
}
