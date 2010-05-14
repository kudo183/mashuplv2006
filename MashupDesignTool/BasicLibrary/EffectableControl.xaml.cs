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

namespace BasicLibrary
{
    public partial class EffectableControl : UserControl
    {
        private FrameworkElement control;
        private BasicEffect effect;

        public EffectableControl()
        {
            InitializeComponent();
        }

        public EffectableControl(FrameworkElement control) : this()
        {
            //control.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            //control.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            LayoutRoot.Children.Add(control);
            control.SizeChanged += new SizeChangedEventHandler(control_SizeChanged);
            this.control = control;

            this.SizeChanged += new SizeChangedEventHandler(LayoutRoot_SizeChanged);
        }

        void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            control.Width = e.NewSize.Width;
            control.Height = e.NewSize.Height;
            LayoutRoot.Clip = new RectangleGeometry() { Rect = new Rect(new Point(0, 0), e.NewSize) };
        }

        void control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            LayoutRoot.Width = e.NewSize.Width;
            LayoutRoot.Height = e.NewSize.Height;
            this.Width = LayoutRoot.Width;
            this.Height = LayoutRoot.Height;
        }

        public FrameworkElement Control
        {
            get { return control; }
        }

        public Canvas CanvasRoot
        {
            get { return LayoutRoot; }
        }

        public void ChangeEffect(BasicEffect be)
        {
            if (effect != null)
                effect.DetachEffect();
            effect = be;
        }
    }
}
