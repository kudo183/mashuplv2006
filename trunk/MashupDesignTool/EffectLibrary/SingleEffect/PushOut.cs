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
    public class PushOut : BasicDisappearEffect
    {
        public enum PushOrientation
        {
            LEFT_TO_RIGHT,
            RIGHT_TO_LEFT,
            TOP_TO_BOTTOM,
            BOTTOM_TO_TOP
        }

        #region attributes
        private PushOrientation orientation;
        private Storyboard sb;
        private TimeSpan duration = TimeSpan.FromMilliseconds(350);
        private TimeSpan beginTime = new TimeSpan();
        double width, height;
        Geometry oldClip;
        Brush oldBackground;
        #endregion attributes

        #region properties
        public PushOrientation Orientation
        {
            get { return orientation; }
            set 
            { 
                orientation = value;
                InitStoryboard();
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

        public PushOut(EffectableControl control)
            : base(control)
        {
            parameterNameList.Add("Orientation");
            parameterNameList.Add("Duration");
            parameterNameList.Add("BeginTime");
            
            width = control.Width;
            height = control.Height;
            if (((double.IsNaN(width) && double.IsNaN(height)) || (width == 0 && height == 0)) && !double.IsNaN(control.ActualWidth) && !double.IsNaN(control.ActualHeight))
            {
                width = control.ActualWidth;
                height = control.ActualHeight;
            }

            orientation = PushOrientation.BOTTOM_TO_TOP;
            oldClip = control.CanvasRoot.Clip;
            control.CanvasRoot.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, width, height) };
            oldBackground = control.CanvasRoot.Background;
            InitStoryboard();

            control.SizeChanged += new SizeChangedEventHandler(control_SizeChanged);
        }

        void control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            width = e.NewSize.Width;
            height = e.NewSize.Height;
            InitStoryboard();
        }

        private void InitStoryboard()
        {
            double from, to;
            string propertyPath;
            switch (orientation)
            {
                case PushOrientation.BOTTOM_TO_TOP:
                    from = 0;
                    to = -height;
                    propertyPath = "(Canvas.Top)";
                    break;
                case PushOrientation.TOP_TO_BOTTOM:
                    from = 0;
                    to = height;
                    propertyPath = "(Canvas.Top)";
                    break;
                case PushOrientation.LEFT_TO_RIGHT:
                    from = 0;
                    to = width;
                    propertyPath = "(Canvas.Left)";
                    break;
                case PushOrientation.RIGHT_TO_LEFT:
                    from = 0;
                    to = -width;
                    propertyPath = "(Canvas.Left)";
                    break;
                default:
                    from = to = 0;
                    propertyPath = "(Canvas.Left)";
                    break;
            }

            sb = new Storyboard();
            sb.Completed += new EventHandler(sb_Completed);

            DoubleAnimation doubleAnimation = new DoubleAnimation() { BeginTime = beginTime, Duration = duration, From = from, To = to };
            Storyboard.SetTarget(doubleAnimation, control.Control);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(propertyPath));

            sb.Children.Add(doubleAnimation);
        }

        void sb_Completed(object sender, EventArgs e)
        {
            Canvas.SetLeft(control.Control, 0);
            Canvas.SetTop(control.Control, 0);
            control.CanvasRoot.Background = oldBackground;
            base.RaiseEffectCompleteEvent(this);
        }

        #region override methods
        public override void Start()
        {
            control.CanvasRoot.Background = new SolidColorBrush(Colors.Black);
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
            control.CanvasRoot.Clip = oldClip;
            control.CanvasRoot.Background = oldBackground;
        }

        protected override void SetSelfHandle()
        {
        }
        #endregion override methods
    }
}
