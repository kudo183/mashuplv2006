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
using System.Reflection;
using System.Windows.Threading;

namespace BasicLibrary
{
    public partial class EffectableControl : UserControl
    {
        private FrameworkElement control;

        public EffectableControl()
        {
            InitializeComponent();
        }

        public EffectableControl(FrameworkElement control) : this()
        {
            control.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            control.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            LayoutRoot.Children.Add(control);
            control.SizeChanged += new SizeChangedEventHandler(control_SizeChanged);
            this.control = control;
            this.Width = control.Width;
            this.Height = control.Height;
            this.SizeChanged += new SizeChangedEventHandler(LayoutRoot_SizeChanged);

            BasicControl bc = control as BasicControl;
            if (bc !=null)
                bc.BCVisibilityChanged +=new BasicControl.BCVisibilityChangedHandler(bc_BCVisibilityChanged);
        }

        void bc_BCVisibilityChanged(object sender, Visibility newValue)
        {
            this.Visibility = newValue;
        }

        void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (control.Width != e.NewSize.Width || control.Height != e.NewSize.Height)
            {
                control.Width = e.NewSize.Width;
                control.Height = e.NewSize.Height;
                LayoutRoot.Clip = new RectangleGeometry() { Rect = new Rect(new Point(0, 0), e.NewSize) };
            }
        }

        void control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            LayoutRoot.Clip = new RectangleGeometry() { Rect = new Rect(new Point(0, 0), e.NewSize) };
            //if (LayoutRoot.Width != e.NewSize.Width || LayoutRoot.Height != e.NewSize.Height)
            //{
            //    LayoutRoot.Width = e.NewSize.Width;
            //    LayoutRoot.Height = e.NewSize.Height;
            //    this.Width = LayoutRoot.Width;
            //    this.Height = LayoutRoot.Height;
            //}
        }

        public FrameworkElement Control
        {
            get { return control; }
        }

        public Canvas CanvasRoot
        {
            get { return LayoutRoot; }
        }

        public void ChangeEffect(string propertyName, Type effectType)
        {
            if (typeof(BasicControl).IsAssignableFrom(control.GetType()))
            {
                ((BasicControl)control).ChangeEffect(propertyName, effectType, this);
            }
        }

        public List<string> GetListEffectPropertyName()
        {
            List<string> list = new List<string>();

            if (typeof(BasicControl).IsAssignableFrom(control.GetType()))
            {
                List<string> temp = ((BasicControl)control).GetListEffectPropertyName();
                foreach (string str in temp)
                    list.Add(str);
            }
            return list;
        }

        public Type GetEffectType(string effectName)
        {
            if (typeof(BasicControl).IsAssignableFrom(control.GetType()))
            {
                return ((BasicControl)control).GetEffectType(effectName);
            }
            return null;
        }

        public IBasic GetEffect(string effectName)
        {
            if (typeof(BasicControl).IsAssignableFrom(control.GetType()))
            {
                return ((BasicControl)control).GetEffect(effectName);
            }
            return null;
        }
    }
}
