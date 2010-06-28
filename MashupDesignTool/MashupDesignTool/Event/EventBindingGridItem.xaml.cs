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
using BasicLibrary;

namespace MashupDesignTool
{
    public partial class EventBindingGridItem : UserControl
    {
        bool b = false;
        string oldHandleOperation = "";
        BasicControl raiseControl;
        MDTEventInfo mdtei;
        List<ControlComboBoxItemData> listControls;

        internal EventBindingGridItem()
        {
            InitializeComponent();
        }

        public EventBindingGridItem(BasicControl raiseControl, string eventName, List<ControlComboBoxItemData> listControl)
            : this()
        {
            lblEventName.Content = eventName;
            this.raiseControl = raiseControl;
            this.listControls = listControl;
            mdtei = MDTEventManager.GetEventInfo(raiseControl, eventName);
            if (mdtei == null)
                btnHandle.Content = "None";
            else
                btnHandle.Content = "Handled";
        }

        private void btnHandle_Click(object sender, RoutedEventArgs e)
        {
            EventEditor ee = new EventEditor(raiseControl, (string)lblEventName.Content, listControls, mdtei);
            ee.Closed += new EventHandler(ee_Closed);
            ee.Show();
        }

        void ee_Closed(object sender, EventArgs e)
        {
            EventEditor ee = (EventEditor)sender;
            if (ee.DialogResult == true)
            {
                if (ee.IsHandled)
                {
                    mdtei = ee.NewMdtei;
                    btnHandle.Content = "Handled";
                }
                else
                {
                    mdtei = null;
                    btnHandle.Content = "None";
                }
            }
        }

        //private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    lblEventNameRegion.Background = new SolidColorBrush(Color.FromArgb(255, 254, 201, 0));
        //}

        //private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    lblEventNameRegion.Background = new SolidColorBrush(Color.FromArgb(255, 233, 236, 255));
        //}

        //private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    this.Focus();
        //}
    }
}
