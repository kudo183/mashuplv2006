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
using Editor;

namespace SL30PropertyGrid
{
    public class BrushValueEditor : ValueEditorBase
    {
        #region Fields
		object currentValue;
        bool showingCP;
        bool mouseInColorPicker;
		Grid pnl;
		protected TextBox txt;
        Rectangle rect;
        
        protected BrushEditor cp;
        
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="label"></param>
		/// <param name="property"></param>
        public BrushValueEditor(PropertyGridLabel label, PropertyItem property)
			: base(label, property)
		{
			currentValue = property.Value;
			property.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(property_PropertyChanged);
			property.ValueError += new EventHandler<ExceptionEventArgs>(property_ValueError);

			pnl = new Grid();
            
            rect = new Rectangle();
            rect.MouseLeftButtonUp += new MouseButtonEventHandler(rect_MouseLeftButtonUp);

            rect.Fill = currentValue as Brush;
            pnl.Children.Add(rect);            

            SizeChanged += new SizeChangedEventHandler(BrushValueEditor_SizeChanged);
			this.Content = pnl;

            cp = new BrushEditor(pnl);
            cp.BrushChanged += new BrushEditor.BrushChangeHandle(cp_BrushChanged);            
		}

        void BrushValueEditor_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            rect.Width = ActualWidth;
            rect.Height = ActualHeight;
        }

        void cp_BrushChanged(object sender, Brush newBrush)
        {
            currentValue = newBrush;
            this.Property.Value = newBrush;
            rect.Fill = newBrush;
        }
        #endregion
        #region Overrides
        /// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);

            if (this.Property.CanWrite)
                this.ShowColorPicker();
		}
		#endregion

		#region Methods
		void ShowColorPicker()
		{
            cp.ShowDialog(currentValue as Brush);
		}

        void rect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.Property.CanWrite)
                this.ShowColorPicker();
        }
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
		}
				
		#endregion
    }
}
