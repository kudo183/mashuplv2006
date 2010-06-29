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
using BasicLibrary;

namespace EffectLibrary
{
    public class WipeIn : BasicAppearEffect
    {
        public enum BeginWipe
        {
            TOP,
            BOTTOM,
            LEFT,
            RIGHT,
            TOPLEFT,
            TOPRIGHT,
            BOTTOMLEFT,
            BOTTOMRIGHT
        }

        #region attributes
        private BeginWipe beginPos = BeginWipe.TOP;
        private Storyboard sb;
        private TimeSpan duration = TimeSpan.FromMilliseconds(500);
        private TimeSpan beginTime = TimeSpan.FromMilliseconds(0);
        double width, height;
        private Rectangle rectangle;
        private GradientStop gs1, gs2;
        private LinearGradientBrush brush;
        #endregion attributes

        #region properties
        public BeginWipe BeginPos
        {
            get { return beginPos; }
            set
            {
                beginPos = value;
                UpdateRectangle();
            }
        }

        public double Duration
        {
            get { return duration.TotalMilliseconds; }
            set
            {
                duration = TimeSpan.FromMilliseconds(value);
                InitStoryboard();
            }
        }

        public double BeginTime
        {
            get { return beginTime.TotalMilliseconds; }
            set
            {
                beginTime = TimeSpan.FromMilliseconds(value);
                InitStoryboard();
            }
        }
        #endregion properties

        public WipeIn(EffectableControl control)
            : base(control)
        {
            parameterNameList.Add("BeginPos");
            parameterNameList.Add("Duration");
            parameterNameList.Add("BeginTime");
            width = control.Width;
            height = control.Height;
            if (((double.IsNaN(width) && double.IsNaN(height)) || (width == 0 && height == 0)) && !double.IsNaN(control.ActualWidth) && !double.IsNaN(control.ActualHeight))
            {
                width = control.ActualWidth;
                height = control.ActualHeight;
            }

            rectangle = new Rectangle();
            rectangle.Width = width;
            rectangle.Height = height;
            Canvas.SetZIndex(rectangle, 63000);
            Canvas.SetTop(rectangle, 0);
            Canvas.SetTop(rectangle, 0);

            brush = new LinearGradientBrush();
            gs1 = new GradientStop();
            gs2 = new GradientStop();
            gs1.Color = Colors.Black;
            gs1.Offset = 0;
            gs2.Color = Color.FromArgb(0, 0, 0, 0);
            gs2.Offset = 0;
            brush.GradientStops.Add(gs1);
            brush.GradientStops.Add(gs2);
            rectangle.Fill = brush;

            UpdateRectangle();
            InitStoryboard();

            control.CanvasRoot.Children.Add(rectangle);
            control.SizeChanged += new SizeChangedEventHandler(control_SizeChanged);
        }

        void control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            width = e.NewSize.Width;
            height = e.NewSize.Height;
            UpdateRectangle();
        }

        private void InitStoryboard()
        {
            TimeSpan ts = TimeSpan.FromMilliseconds(duration.TotalMilliseconds * 0.4 / 1.4);
            sb = new Storyboard();
            sb.Completed += new EventHandler(sb_Completed);
            DoubleAnimation doubleAnimation1 = new DoubleAnimation() { BeginTime = beginTime, Duration = duration.Subtract(ts), From = 1, To = 0 };
            Storyboard.SetTarget(doubleAnimation1, gs1);
            Storyboard.SetTargetProperty(doubleAnimation1, new PropertyPath("Offset"));
            sb.Children.Add(doubleAnimation1);

            DoubleAnimation doubleAnimation2 = new DoubleAnimation() { BeginTime = ts + beginTime, Duration = duration.Subtract(ts), From = 1, To = 0 };
            Storyboard.SetTarget(doubleAnimation2, gs2);
            Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath("Offset"));
            sb.Children.Add(doubleAnimation2);
        }

        void sb_Completed(object sender, EventArgs e)
        {
            base.RaiseEffectCompleteEvent(this);
        }

        private void UpdateRectangle()
        {
            rectangle.Width = width;
            rectangle.Height = height;
            Point start, end;
            switch (beginPos)
            {
                case BeginWipe.TOP:
                    start = new Point(0.5, 1);
                    end = new Point(0.5, 0);
                    break;
                case BeginWipe.BOTTOM:
                    start = new Point(0.5, 0);
                    end = new Point(0.5, 1);
                    break;
                case BeginWipe.LEFT:
                    start = new Point(1, 0.5);
                    end = new Point(0, 0.5);
                    break;
                case BeginWipe.RIGHT:
                    start = new Point(0, 0.5);
                    end = new Point(1, 0.5);
                    break;
                case BeginWipe.TOPLEFT:
                    start = new Point(1, 1);
                    end = new Point(0, 0);
                    break;
                case BeginWipe.TOPRIGHT:
                    start = new Point(0, 1);
                    end = new Point(1, 0);
                    break;
                case BeginWipe.BOTTOMLEFT:
                    start = new Point(1, 0);
                    end = new Point(0, 1);
                    break;
                case BeginWipe.BOTTOMRIGHT:
                    start = new Point(0, 0);
                    end = new Point(1, 1);
                    break;
                default:
                    start = end = new Point(0, 0);
                    break;
            }
            brush.StartPoint = start;
            brush.EndPoint = end;
        }

        #region override methods
        public override void Start()
        {
            gs1.Offset = 1;
            gs2.Offset = 1;
            sb.Begin();
        }

        public override void Stop()
        {
            sb.Stop();
        }

        public override void DetachEffect()
        {
            Canvas.SetLeft(control.Control, 0);
            Canvas.SetTop(control.Control, 0);
            control.CanvasRoot.Children.Remove(rectangle);
        }

        protected override void SetSelfHandle()
        {
        }
        #endregion override methods
    }
}
