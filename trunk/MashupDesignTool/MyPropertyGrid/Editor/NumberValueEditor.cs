using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SL30PropertyGrid
{
    public class NumberValueEditor : ValueEditorBase
    {
        EasyPainter.Imaging.Silverlight.EditableSlider slider = new EasyPainter.Imaging.Silverlight.EditableSlider();
        public NumberValueEditor(PropertyGridLabel label, PropertyItem property)
            : base(label, property)
        {
            
            property.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(property_PropertyChanged);
            property.ValueError += new EventHandler<ExceptionEventArgs>(property_ValueError);


            slider.Height = 15;

            SetMaxMinValue();

            slider.ValueChanged += new EasyPainter.Imaging.Silverlight.EditableSlider.ValueChangedHandler(slider_ValueChanged);
            if (null != property.Value)
                slider.Value = double.Parse(property.Value.ToString());
            //txt.IsReadOnly = !this.Property.CanWrite;
            

            this.Content = slider;
            this.GotFocus += new RoutedEventHandler(StringValueEditor_GotFocus);
        }

        void SetMaxMinValue()
        {
            if (Property.PropertyType == typeof(sbyte))
            {
                slider.Minimum = sbyte.MinValue;
                slider.Maximum = sbyte.MaxValue;
                return;
            }
            if (Property.PropertyType == typeof(byte))
            {
                slider.Minimum = byte.MinValue;
                slider.Maximum = byte.MaxValue;
                return;
            }
            if (Property.PropertyType == typeof(Int16))
            {
                slider.Minimum = Int16.MinValue;
                slider.Maximum = Int16.MaxValue;
                return;
            }
            if (Property.PropertyType == typeof(UInt16))
            {
                slider.Minimum = UInt16.MinValue;
                slider.Maximum = UInt16.MaxValue;
                return;
            }
            if (Property.PropertyType == typeof(Int32))
            {
                slider.Minimum = Int32.MinValue;
                slider.Maximum = Int32.MaxValue;
                return;
            }
            if (Property.PropertyType == typeof(long))
            {
                slider.Minimum = long.MinValue;
                slider.Maximum = long.MaxValue;
                return;
            }
            if (Property.PropertyType == typeof(float))
            {
                slider.Minimum = float.MinValue;
                slider.Maximum = float.MaxValue;
                slider.IsFloatingPoint = true;                
                return;
            }
            if (Property.PropertyType == typeof(double))
            {
                slider.Minimum = double.MinValue;
                slider.Maximum = double.MaxValue;
                slider.IsFloatingPoint = true;
                return;
            }
            if (Property.PropertyType == typeof(Single))
            {
                slider.Minimum = Single.MinValue;
                slider.Maximum = Single.MaxValue;
                slider.IsFloatingPoint = true;
                return;
            }
        }

        void slider_ValueChanged(double newValue)
        {
            this.Property.Value = newValue.ToString();
        }

        void property_ValueError(object sender, ExceptionEventArgs e)
        {
            MessageBox.Show(e.EventException.Message);
        }
        void property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "Value")
            //{
            //    if (null != this.Property.Value)
            //        slider.Value = double.Parse(this.Property.Value.ToString());
            //}
        }

        void StringValueEditor_GotFocus(object sender, RoutedEventArgs e)
        {
        
        }

        
        override public void UpdatePropertyValue()
        {
            base.UpdatePropertyValue();
        
            if (null != Property.Value)
                slider.Value = double.Parse(this.Property.Value.ToString());
        }
    }
}
