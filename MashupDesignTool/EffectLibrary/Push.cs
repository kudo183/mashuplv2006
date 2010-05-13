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
    public class Push : BasicEffect
    {
        public enum PushOrientation
        {
            LEFT_TO_RIGHT,
            RIGHT_TO_LEFT,
            TOP_TO_BOTTOM,
            BOTTOM_TO_TOP
        }

        public enum PushSpeed
        {
            SLOW,
            MEDIUM,
            FAST
        }

        #region attributes
        private PushOrientation orientation;
        private PushSpeed speed;
        private Storyboard sb;
        private TimeSpan pushDuration = new TimeSpan();
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

        public PushSpeed Speed
        {
            get { return speed; }
            set 
            { 
                speed = value;
                switch (speed)
                {
                    case PushSpeed.SLOW:
                        pushDuration = TimeSpan.FromMilliseconds(1000);
                        break;
                    case PushSpeed.MEDIUM:
                        pushDuration = TimeSpan.FromMilliseconds(650);
                        break;
                    case PushSpeed.FAST:
                        pushDuration = TimeSpan.FromMilliseconds(300);
                        break;
                    default:
                        break;
                }
                InitStoryboard();
            }
        }
        #endregion properties

        public Push(EffectableControl control)
            : base(control)
        {
            parameterNameList.Add("Orientation");
            parameterNameList.Add("Speed");
            
            width = control.Width;
            height = control.Height;
            if (((double.IsNaN(width) && double.IsNaN(height)) || (width == 0 && height == 0)) && !double.IsNaN(control.ActualWidth) && !double.IsNaN(control.ActualHeight))
            {
                width = control.ActualWidth;
                height = control.ActualHeight;
            }

            orientation = PushOrientation.BOTTOM_TO_TOP;
            Speed = PushSpeed.MEDIUM;
            oldClip = control.CanvasRoot.Clip;
            control.CanvasRoot.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, width, height) };
            oldBackground = control.CanvasRoot.Background;

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
                    from = height;
                    to = 0;
                    propertyPath = "(Canvas.Top)";
                    break;
                case PushOrientation.TOP_TO_BOTTOM:
                    from = -height;
                    to = 0;
                    propertyPath = "(Canvas.Top)";
                    break;
                case PushOrientation.LEFT_TO_RIGHT:
                    from = -width;
                    to = 0;
                    propertyPath = "(Canvas.Left)";
                    break;
                case PushOrientation.RIGHT_TO_LEFT:
                    from = width;
                    to = 0;
                    propertyPath = "(Canvas.Left)";
                    break;
                default:
                    from = to = 0;
                    propertyPath = "(Canvas.Left)";
                    break;
            }

            sb = new Storyboard();
            sb.Completed += new EventHandler(sb_Completed);

            DoubleAnimation doubleAnimation = new DoubleAnimation() { BeginTime = TimeSpan.FromSeconds(0), Duration = pushDuration, From = from, To = to };
            Storyboard.SetTarget(doubleAnimation, control.Control);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(propertyPath));

            sb.Children.Add(doubleAnimation);
        }

        void sb_Completed(object sender, EventArgs e)
        {
            control.CanvasRoot.Background = oldBackground;
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
