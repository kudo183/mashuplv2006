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
    public partial class EventBindingGrid : UserControl
    {
        private BasicControl selectedObject;
        public EventBindingGrid()
        {
            InitializeComponent();
        }

        public BasicControl SelectedObject
        {
            get { return selectedObject; }
        }

        public void ChangeSelectedObject(BasicControl value, List<EffectableControl> controls)
        {
            if (selectedObject == value)
                return;
            selectedObject = value;
            UpdateGrid(controls);
        }

        private void UpdateGrid(List<EffectableControl> controls)
        {
            for (int i = stackPanel.Children.Count - 1; i > 0 ; i--)
                stackPanel.Children.RemoveAt(i);
            List<string> listEvent;
            if (selectedObject == null)
                listEvent = new List<string>();
            else
                listEvent = selectedObject.GetListEventName();
            if (listEvent.Count == 0)
            {
                stackPanel.Children.Add(new Label() { Content = "No event available", Margin = new Thickness(0, 20, 0, 0), VerticalAlignment = System.Windows.VerticalAlignment.Center, HorizontalAlignment = System.Windows.HorizontalAlignment.Center });
                return;
            }
            
            List<ControlComboBoxItemData> listControls = new List<ControlComboBoxItemData>();
            listControls.Add(ControlComboBoxItemData.None);
            foreach (EffectableControl fe in controls)
                if (typeof(BasicControl).IsAssignableFrom(fe.Control.GetType()) && fe.Control.Name != selectedObject.Name)
                    listControls.Add(new ControlComboBoxItemData((BasicControl)fe.Control));
            
            List<MDTEventInfo> listEventInfo = MDTEventManager.GetListEventInfoRaiseBy(selectedObject);
            foreach (string eventName in listEvent)
            {
                //MDTEventInfo mei = null;
                //EventBindingGridItem item;
                //foreach(MDTEventInfo ei in listEventInfo)
                //    if (ei.EventName == eventName)
                //    {
                //        mei = ei;
                //        break;
                //    }
                //if (mei == null)
                EventBindingGridItem item = new EventBindingGridItem(selectedObject, eventName, listControls);
                //else

                //    ///////////////////////////////////////////////////////////////////////////
                //    item = new EventBindingGridItem(selectedObject, eventName, listControls, mei.HandleControls[0], mei.HandleOperations[0]);
                stackPanel.Children.Add(item);
            }
        }
    }
}
