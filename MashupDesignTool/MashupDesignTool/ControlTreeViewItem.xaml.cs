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
        private ControlInfo controlInfo;

        public ControlInfo ControlInfo
        {
            get { return controlInfo; }
            set { controlInfo = value; }
        }

        public ControlTreeViewItem()
        {
            InitializeComponent();
        }

        public ControlTreeViewItem(ControlInfo controlInfo, string clientRoot) : this()
        {
            this.controlInfo = controlInfo;
            SetControlIcon(clientRoot + "/" + controlInfo.IconName);
            SetControlDisplayName(controlInfo.DisplayName);
            SetControlDescription(controlInfo.Description);
        }

        private void SetControlIcon(string uri)
        {
            try
            {
                ControlIcon.Source = new BitmapImage(new Uri(uri));
            }
            catch { }
        }

        private void SetControlDisplayName(string displayName)
        {
            ControlDisplayName.Content = displayName;
        }

        private void SetControlDescription(string description)
        {
            ControlDescription.Content = description;
        }
    }
}
