using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using BasicLibrary;

namespace MashupDesignTool
{
    public class ControlComboBoxItemData
    {
        public static ControlComboBoxItemData None = new ControlComboBoxItemData("<None>");

        private string controlName;
        private ImageSource controlImage;
        private BasicControl control;

        internal ControlComboBoxItemData(string controlName)
        {
            this.controlName = controlName;
            this.control = null;
            this.controlImage = null;
        }

        public ControlComboBoxItemData(BasicControl control)
        {
            this.controlName = control.Name;
            this.controlImage = new WriteableBitmap(control, null);
            this.control = control;
        }

        public string ControlName
        {
            get { return controlName; }
        }

        public ImageSource ControlImage
        {
            get { return controlImage; }
        }

        public BasicControl Control
        {
            get { return control; }
        }
    }
}
