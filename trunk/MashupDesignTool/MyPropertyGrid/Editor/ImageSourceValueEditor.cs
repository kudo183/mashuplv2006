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

namespace SL30PropertyGrid
{
    public class ImageSourceValueEditor : ValueEditorBase
    {
        TextBox txt;

        public ImageSourceValueEditor(PropertyGridLabel label, PropertyItem property)
            : base(label, property)
        {
            property.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(property_PropertyChanged);
            property.ValueError += new EventHandler<ExceptionEventArgs>(property_ValueError);

            txt = new TextBox();
            txt.Height = 20;
            if (null != property.Value)
            {
                if (property.Value.GetType() == typeof(BitmapImage))
                {
                    if (((BitmapImage)property.Value).UriSource.IsAbsoluteUri)
                        txt.Text = ((BitmapImage)property.Value).UriSource.AbsoluteUri;
                    else
                        txt.Text = "";
                }
                else
                    txt.Text = "";
            }
            //txt.IsReadOnly = !this.Property.CanWrite;
            txt.Foreground = this.Property.CanWrite ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Gray);
            txt.BorderThickness = new Thickness(0);
            txt.Margin = new Thickness(0);

            if (this.Property.CanWrite)
                //	txt.TextChanged += new TextChangedEventHandler(Control_TextChanged);
                txt.KeyUp += new System.Windows.Input.KeyEventHandler(txt_KeyUp);

            this.Content = txt;
            this.GotFocus += new RoutedEventHandler(StringValueEditor_GotFocus);
        }

        void txt_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (this.Property.CanWrite)
                    this.Property.Value = new BitmapImage(new Uri(txt.Text, UriKind.Absolute));
                this.Label.Focus();
            }
        }

        void property_ValueError(object sender, ExceptionEventArgs e)
        {
            MessageBox.Show(e.EventException.Message);
        }
        void property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                if (null != this.Property.Value)
                {
                    if (this.Property.Value.GetType() == typeof(BitmapImage))
                    {
                        if (((BitmapImage)this.Property.Value).UriSource.IsAbsoluteUri)
                            txt.Text = ((BitmapImage)this.Property.Value).UriSource.AbsoluteUri;
                        else
                            txt.Text = "";
                    }
                    else
                        txt.Text = "";
                }
                else
                    txt.Text = string.Empty;
            }

            if (e.PropertyName == "CanWrite")
            {
                if (!this.Property.CanWrite)
                    txt.TextChanged -= new TextChangedEventHandler(Control_TextChanged);
                else
                    txt.TextChanged += new TextChangedEventHandler(Control_TextChanged);
                txt.IsReadOnly = !this.Property.CanWrite;
                txt.Foreground = this.Property.CanWrite ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Gray);
            }
        }

        void StringValueEditor_GotFocus(object sender, RoutedEventArgs e)
        {
            this.txt.Focus();
        }

        void Control_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.Property.CanWrite)
                this.Property.Value = new BitmapImage(new Uri(txt.Text, UriKind.Absolute));
        }

        override public void UpdatePropertyValue()
        {
            base.UpdatePropertyValue();
            txt.Text = "";
            if (null != Property.Value)
            {
                if (this.Property.Value.GetType() == typeof(BitmapImage))
                {
                    if (((BitmapImage)this.Property.Value).UriSource.IsAbsoluteUri)
                        txt.Text = ((BitmapImage)this.Property.Value).UriSource.AbsoluteUri;
                    else
                        txt.Text = "";
                }
                else
                    txt.Text = "";
            }
        }
    }
}
