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

        internal EventBindingGridItem()
        {
            InitializeComponent();
        }

        public EventBindingGridItem(BasicControl raiseControl, string eventName, List<ControlComboBoxItemData> listControl)
            : this()
        {
            lblEventName.Content = eventName;
            lblEventName.Tag = raiseControl;
            cbbControl.ItemsSource = listControl;
            cbbControl.SelectedIndex = 0;
        }

        public EventBindingGridItem(BasicControl raiseControl, string eventName, List<ControlComboBoxItemData> listControl, BasicControl handleControl, string handleOperation)
            : this(raiseControl, eventName, listControl)
        {
            for (int i = 1; i < listControl.Count; i++)
            {
                if (listControl[i].Control.Name == handleControl.Name)
                {
                    b = true;
                    oldHandleOperation = handleOperation;
                    cbbControl.SelectedIndex = i;
                }
            }
        }

        private void cbbControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbControl.SelectedIndex == 0)
            {
                MDTEventManager.RegisterEvent((BasicControl)lblEventName.Tag, (string)lblEventName.Content, null, "");
                cbbOperation.ItemsSource = new List<string>();
                cbbOperation.IsEnabled = false;
                return;
            }
            ControlComboBoxItemData item = (ControlComboBoxItemData)cbbControl.SelectedItem;
            if (item == null)
            {
                cbbOperation.IsEnabled = false;
                return;
            }
            List<string> list = item.Control.GetListOperationName();
            cbbOperation.Tag = item.Control;
            cbbOperation.IsEnabled = true;
            cbbOperation.ItemsSource = list;
            if (b == true)
            {
                for (int i = 0; i < list.Count; i++)
                    if (list[i] == oldHandleOperation)
                    {
                        cbbOperation.SelectedIndex = i;
                        break;
                    }
                b = false;
            }
        }

        private void cbbOperation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbOperation.SelectedIndex == -1)
                return;
            string operation = (string)cbbOperation.SelectedItem;
            if (operation == null)
                return;
            MDTEventManager.RegisterEvent((BasicControl)lblEventName.Tag, (string)lblEventName.Content, (BasicControl)cbbOperation.Tag, operation);
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            lblEventNameRegion.Background = new SolidColorBrush(Color.FromArgb(255, 254, 201, 0));
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            lblEventNameRegion.Background = new SolidColorBrush(Color.FromArgb(255, 233, 236, 255));
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
        }
    }
}
