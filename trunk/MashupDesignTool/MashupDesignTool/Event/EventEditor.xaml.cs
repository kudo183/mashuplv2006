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
    public partial class EventEditor : ChildWindow
    {
        List<ControlComboBoxItemData> listControls;
        MDTEventInfo mdtei;
        BasicControl raiseControl;
        List<ComboBox> cbbHandleControls = new List<ComboBox>();
        List<ComboBox> cbbHandleOperations = new List<ComboBox>();
        double rowHeight = 30;
        MDTEventInfo newMdtei = null;

        public EventEditor(BasicControl raiseControl, string eventName, List<ControlComboBoxItemData> listControls, MDTEventInfo mdtei)
        {
            InitializeComponent();

            lblEventName.Content = eventName;
            this.raiseControl = raiseControl;
            this.listControls = listControls;
            this.mdtei = mdtei;
            if (mdtei == null)
            {
                AddRow();
                rdNone.IsChecked = true;
            }
            else
            {
                LoadGridEventBinding();
                rdHandle.IsChecked = true;
            }
        }

        public bool IsHandled
        {
            get { return (newMdtei != null); }
        }

        public MDTEventInfo NewMdtei
        {
            get { return newMdtei; }
        }

        private void LoadGridEventBinding()
        {
            for (int i = 0; i < mdtei.HandleControls.Count; i++)
                AddRow(mdtei.HandleControls[i], mdtei.HandleOperations[i]);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (rdHandle.IsChecked == true)
            {
                List<BasicControl> handleControls = new List<BasicControl>();
                List<string> handleOperations = new List<string>();

                for (int i = 0; i < cbbHandleOperations.Count; i++)
                {
                    if (cbbHandleOperations[i].Tag != null && cbbHandleOperations[i].SelectedIndex != -1)
                    {
                        handleControls.Add((BasicControl)cbbHandleOperations[i].Tag);
                        handleOperations.Add((string)cbbHandleOperations[i].SelectedItem);
                    }
                }
                newMdtei = MDTEventManager.RegisterEvent(raiseControl, (string)lblEventName.Content, handleControls, handleOperations);
            }
            else
            {
                if (mdtei != null)
                    MDTEventManager.RemoveEvent(mdtei.RaiseControl, mdtei.EventName);
                newMdtei = null;
            }

            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #region grid splitter
        bool _isMouseDownOnGridTitleGridSplitter;
        private void GridSplitter_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDownOnGridTitleGridSplitter == false)
                return;
            gridEventBinding.ColumnDefinitions[0].Width = gridTitle.ColumnDefinitions[0].Width;
            OKButton.Content = gridEventBinding.ColumnDefinitions[0].Width.ToString() + "  " + gridEventBinding.ColumnDefinitions[0].Width.ToString();
        }

        private void GridSplitter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseDownOnGridTitleGridSplitter = true;
        }

        private void GridSplitter_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDownOnGridTitleGridSplitter = false;
        }
        #endregion grid splitter

        private void btnAddRow_Click(object sender, RoutedEventArgs e)
        {
            AddRow();
        }

        #region AddRow Function
        private void AddRow()
        {
            AddRow(0, null, "");
        }

        private void AddRow(BasicControl handleControl, string handleOperation)
        {
            ControlComboBoxItemData item = null;
            int index = 0;
            for (int i = 0; i < listControls.Count; i++)
                if (listControls[i].ControlName == handleControl.Name)
                {
                    index = i;
                    item = listControls[i];
                    break;
                }
            AddRow(index, item, handleOperation);
        }

        private void AddRow(int controlSelectedIndex, ControlComboBoxItemData controlSelectedItem, string handleOperation)
        {
            ComboBox control = new ComboBox();
            control.Tag = cbbHandleControls.Count;
            control.ItemTemplate = (DataTemplate)Resources["cbbControlDataTemplate"];
            control.ItemsSource = listControls;
            control.SelectedIndex = controlSelectedIndex;
            control.Margin = new Thickness(0, 0, 0, 3);

            ComboBox operation = new ComboBox();
            operation.Margin = new Thickness(0, 0, 0, 3);
            if (controlSelectedItem == null)
            {
                operation.IsEnabled = false;
                operation.Tag = null;
            }
            else
            {
                List<string> list = controlSelectedItem.Control.GetListOperationName();
                operation.Tag = controlSelectedItem.Control;
                operation.IsEnabled = true;
                operation.ItemsSource = list;
                operation.SelectedItem = handleOperation;
            }

            control.SelectionChanged += new SelectionChangedEventHandler(cbbControl_SelectionChanged);

            Border bd1 = new Border();
            bd1.Child = control;
            bd1.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 148, 173, 235));
            bd1.BorderThickness = new Thickness(0, 0, 0, 1);
            bd1.Margin = new Thickness(0, 3, 0, 0);

            Border bd2 = new Border();
            bd2.Child = operation;
            bd2.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 148, 173, 235));
            bd2.BorderThickness = new Thickness(0, 0, 0, 1);
            bd2.Margin = new Thickness(0, 3, 0, 0);
            
            gridEventBinding.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(rowHeight, GridUnitType.Pixel) });
            //Grid.SetRow(control, cbbHandleControls.Count);
            //Grid.SetRow(operation, cbbHandleControls.Count);
            //Grid.SetColumn(operation, 1);
            //gridEventBinding.Children.Add(control);
            //gridEventBinding.Children.Add(operation);
            Grid.SetRow(bd1, cbbHandleControls.Count);
            Grid.SetRow(bd2, cbbHandleControls.Count);
            Grid.SetColumn(bd2, 1);
            gridEventBinding.Children.Add(bd1);
            gridEventBinding.Children.Add(bd2);

            cbbHandleControls.Add(control);
            cbbHandleOperations.Add(operation);
        }
        #endregion AddRow Function

        private void cbbControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbb = (ComboBox)sender;
            ComboBox operation = cbbHandleOperations[(int)cbb.Tag];
            if (cbb.SelectedIndex == 0)
            {
                operation.ItemsSource = new List<string>();
                operation.IsEnabled = false;
                operation.Tag = null;
                return;
            }
            ControlComboBoxItemData item = (ControlComboBoxItemData)cbb.SelectedItem;
            if (item == null)
            {
                operation.IsEnabled = false;
                operation.Tag = null;
                return;
            }
            List<string> list = item.Control.GetListOperationName();
            operation.Tag = item.Control;
            operation.IsEnabled = true;
            operation.ItemsSource = list;
        }
    }
}

