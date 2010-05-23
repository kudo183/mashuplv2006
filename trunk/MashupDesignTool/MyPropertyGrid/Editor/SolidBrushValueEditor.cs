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
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using Controls;


namespace SL30PropertyGrid
{
    public class SolidBrushValueEditor : ValueEditorBase
    {
        #region Fields
		object currentValue;
        bool showingCP;
        bool mouseInColorPicker;
		StackPanel pnl;
		protected TextBox txt;
        Rectangle rect;
        protected ColorPicker cp;
        protected Popup p;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="label"></param>
		/// <param name="property"></param>
        public SolidBrushValueEditor(PropertyGridLabel label, PropertyItem property)
			: base(label, property)
		{
			currentValue = property.Value;
			property.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(property_PropertyChanged);
			property.ValueError += new EventHandler<ExceptionEventArgs>(property_ValueError);

			pnl = new StackPanel();
            pnl.Orientation = Orientation.Horizontal;
                 
			this.Content = pnl;

            p = new Popup();
            cp = new ColorPicker();
			cp.Visibility = Visibility.Visible;
			cp.Margin = new Thickness(0);
			cp.VerticalAlignment = VerticalAlignment.Center;
			cp.HorizontalAlignment = HorizontalAlignment.Stretch;

            //cp.LostFocus += new RoutedEventHandler(cp_LostFocus);
            cp.MouseEnter += new MouseEventHandler(cp_MouseEnter);
            cp.MouseLeave += new MouseEventHandler(cp_MouseLeave);
            mouseInColorPicker = false;
            p.Child = cp;
            p.IsOpen = true;
			pnl.Children.Add(p);
			
			this.ShowTextBox();
		}

        void cp_MouseLeave(object sender, MouseEventArgs e)
        {
            mouseInColorPicker = false;
            if (mouseInColorPicker)
                return;
            currentValue = new SolidColorBrush(cp.SelectedColor);
            this.Property.Value = currentValue;

            ShowTextBox();
        }

        void cp_MouseEnter(object sender, MouseEventArgs e)
        {
            mouseInColorPicker = true;
        }
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnGotFocus(RoutedEventArgs e)
		{
			Debug.WriteLine("DateTimeValueEditor : OnGotFocus");

			if (showingCP)
				return;

			base.OnGotFocus(e);

			//if (this.Property.CanWrite)
				//this.ShowColorPicker();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLostFocus(RoutedEventArgs e)
		{            
            //if (showingCP)
            //    return;

            //base.OnLostFocus(e);
		}
		#endregion

		#region Methods
		void ShowColorPicker()
		{
			if (null == txt)
				return;

			cp.Visibility = Visibility.Visible;
			cp.Focus();

			txt.Visibility = Visibility.Collapsed;
            pnl.Children.Remove(rect);
			pnl.Children.Remove(txt);
			txt = null;

			cp.SelectedColor = ((SolidColorBrush)currentValue).Color;
            cp.ColorSelected +=new ColorPicker.ColorSelectedHandler(cp_ColorSelected);

		}

        void cp_ColorSelected(Color c)
        {
            currentValue = new SolidColorBrush(c);
            this.Property.Value = currentValue; ;
        }
		void ShowTextBox()
		{
			if (null != txt)
				return;

			txt = new TextBox();
			txt.Height = 20;
			txt.BorderThickness = new Thickness(0);
			txt.Margin = new Thickness(0);
			txt.VerticalAlignment = VerticalAlignment.Center;
			txt.HorizontalAlignment = HorizontalAlignment.Stretch;
			txt.Text = currentValue.ToString();
			txt.IsReadOnly = !this.Property.CanWrite;
            Color c = ((SolidColorBrush)this.Property.Value).Color;
            //txt.Background = (SolidColorBrush)this.Property.Value;
            //Color neg = Color.FromArgb((byte)0xFF,(byte)(255 - c.R),(byte)(255-c.G),(byte)(255-c.B));
			txt.Foreground = this.Property.CanWrite ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Gray);
            txt.IsReadOnly = true;
			txt.Text = c.ToString();
            txt.Width = 3000;
            rect = new Rectangle();
            rect.MouseLeftButtonUp += new MouseButtonEventHandler(rect_MouseLeftButtonUp);
            rect.Height = 20;
            rect.Width = 30;
            rect.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            rect.Fill = (SolidColorBrush)this.Property.Value;
            pnl.Children.Add(rect);
            pnl.Children.Add(txt);

			showingCP = false;
			cp.Visibility = Visibility.Collapsed;
		}

        void rect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.Property.CanWrite)
                this.ShowColorPicker();
        }
        /*
        void cp_LostFocus(object sender, RoutedEventArgs e)
        {
            if (mouseInColorPicker)
                return;
            currentValue = new SolidColorBrush(cp.SelectedColor);
            this.Property.Value = currentValue;

            ShowTextBox();
        }*/
		#endregion

		#region Event Handlers
		void property_ValueError(object sender, ExceptionEventArgs e)
		{
			MessageBox.Show(e.EventException.Message);
		}
		void property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Value")
				currentValue = this.Property.Value;

			if (e.PropertyName == "CanWrite")
			{
				if (!this.Property.CanWrite && showingCP)
					ShowTextBox();
			}
		}
				
		#endregion
    }
}
