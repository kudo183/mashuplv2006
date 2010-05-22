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
using System.Collections.Generic;
using BasicLibrary;

namespace EffectLibrary
{
    public class GlowWinSeven : BasicEffect
    {
        private byte alpha = 64;
        GradientStop transitionColor;
        GradientStop transitionSubColor;

        #region property
        public Color TransitionColor
        {
            get
            {
                return transitionColor.Color;
            }
            set
            {
                transitionColor.Color = value;
                transitionSubColor.Color = Color.FromArgb(alpha, value.R, value.G, value.B);
            }
        }

        public byte TransitionAlpha
        {
            get { return alpha; }
            set { alpha = value; }
        }
        #endregion
        public override void Start()
        {
            sbEnter.Begin();
        }

        public override void Stop()
        {
            sbLeave.Begin();
        }

        public override void DetachEffect()
        {
            IsSelfHandle = false;
            control.CanvasRoot.Children.Clear();
            control.CanvasRoot.Children.Add(control.Control);
        }

        protected override void SetSelfHandle()
        {
            if (_isSelfHandle == true)
            {
                control.MouseEnter += new MouseEventHandler(control_MouseEnter);
                control.MouseLeave += new MouseEventHandler(control_MouseLeave);
                control.MouseMove += new MouseEventHandler(control_MouseMove);
            }
            else
            {
                control.MouseEnter -= new MouseEventHandler(control_MouseEnter);
                control.MouseLeave -= new MouseEventHandler(control_MouseLeave);
                control.MouseMove -= new MouseEventHandler(control_MouseMove);
            }
        }
        Storyboard sbEnter, sbLeave;
       
        RadialGradientBrush brushLight;
        Point tempPoint = new Point();

        public GlowWinSeven(EffectableControl control)
            : base(control)
        {
            parameterNameList.Add("TransitionColor");
            parameterNameList.Add("TransitionAlpha");

            Rectangle rect1 = new Rectangle();
            rect1.Fill = new SolidColorBrush(Color.FromArgb(0x20, 0x00, 0x00, 0x00));
            rect1.Stroke = new SolidColorBrush(Colors.White);
            rect1.StrokeThickness = 0.3;
            rect1.IsHitTestVisible = false;
            rect1.Width = control.Width;
            rect1.Height = control.Height;

            Rectangle rect2 = new Rectangle();
            rect2.IsHitTestVisible = false;
            brushLight = new RadialGradientBrush();
            brushLight.RadiusX = 0.7253;
            brushLight.RadiusY = 1;
            brushLight.GradientOrigin = new Point(0.5, 0.5);
            brushLight.Center = new Point(0.5, 0.5);
            brushLight.Opacity = 0;
            rect2.Fill = brushLight;
            rect2.Width = control.Width;
            rect2.Height = control.Height;

            GradientStop gs;
            gs = new GradientStop();
            gs.Offset = 0;
            gs.Color = Colors.White;
            brushLight.GradientStops.Add(gs);
            transitionColor = new GradientStop();
            transitionColor.Offset = 0.6;
            transitionColor.Color = Color.FromArgb(0xFF, 0xD8, 0x93, 0x5F);
            brushLight.GradientStops.Add(transitionColor);
            transitionSubColor = new GradientStop();
            transitionSubColor.Offset = 0.8;
            transitionSubColor.Color = Color.FromArgb(0x40, 0xD8, 0x93, 0x5F);
            brushLight.GradientStops.Add(transitionSubColor);

            control.CanvasRoot.Children.Clear();
            control.CanvasRoot.Children.Add(rect1);
            control.CanvasRoot.Children.Add(rect2);
            control.CanvasRoot.Children.Add(control.Control);

            sbEnter = new Storyboard();
            DoubleAnimation da = new DoubleAnimation();
            da.FillBehavior = FillBehavior.HoldEnd;
            da.To = 1;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(100));
            Storyboard.SetTarget(da, brushLight);
            Storyboard.SetTargetProperty(da, new PropertyPath("FrameworkElement.Opacity"));
            sbEnter.Children.Add(da);

            sbLeave = new Storyboard();
            da = new DoubleAnimation();
            da.FillBehavior = FillBehavior.HoldEnd;
            da.To = 0;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(400));
            Storyboard.SetTarget(da, brushLight);
            Storyboard.SetTargetProperty(da, new PropertyPath("FrameworkElement.Opacity"));

            sbLeave.Children.Add(da);
        }

        private void control_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(control);
            tempPoint.X = p.X / control.ActualWidth;
            tempPoint.Y = 1;
            brushLight.Center = tempPoint;
            brushLight.GradientOrigin = tempPoint;
        }

        void control_MouseLeave(object sender, MouseEventArgs e)
        {
            sbLeave.Begin();
        }

        void control_MouseEnter(object sender, MouseEventArgs e)
        {
            sbEnter.Begin();
        }

    }
}
