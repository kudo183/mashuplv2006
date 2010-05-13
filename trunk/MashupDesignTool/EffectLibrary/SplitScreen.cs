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
    public class SplitScreen : BasicEffect
    {
        public enum SplitDirection
        {
            VERTICAL,
            HORIZONTAL
        }

        public enum SplitSpeed
        {
            SLOW,
            MEDIUM,
            FAST
        }

        #region attributes
        private double maskOpacity = 0.3;
        Color maskColor = Colors.LightGray;
        SplitDirection direction;
        SplitSpeed speed;
        Storyboard sb1;
        Storyboard sb2;
        Rectangle rectangle;
        Rectangle topHalf;
        Rectangle bottomHalf;
        double width, height;
        TimeSpan openingDuration, closingDuration;
        #endregion attributes

        #region properties
        public double MaskOpacity
        {
            get { return maskOpacity; }
            set 
            { 
                maskOpacity = value;
                if (rectangle != null)
                    rectangle.Opacity = maskOpacity;
                if (topHalf != null)
                    topHalf.Opacity = maskOpacity;
                if (bottomHalf != null)
                    bottomHalf.Opacity = maskOpacity;
            }
        }

        public Color MaskColor
        {
            get { return maskColor; }
            set 
            { 
                maskColor = value;
                if (rectangle != null)
                    rectangle.Fill = new SolidColorBrush(maskColor);
                if (topHalf != null)
                    topHalf.Fill = new SolidColorBrush(maskColor);
                if (bottomHalf != null)
                    bottomHalf.Fill = new SolidColorBrush(maskColor);
            }
        }

        public SplitDirection Direction
        {
            get { return direction; }
            set 
            { 
                direction = value;
                InitStoryboard();
            }
        }

        public SplitSpeed Speed
        {
            get { return speed; }
            set 
            { 
                speed = value;
                if (speed == SplitSpeed.SLOW)
                {
                    openingDuration = new TimeSpan(0, 0, 0, 0, 600);
                    closingDuration = new TimeSpan(0, 0, 0, 0, 300);
                }
                else if (speed == SplitSpeed.MEDIUM)
                {
                    openingDuration = new TimeSpan(0, 0, 0, 0, 400);
                    closingDuration = new TimeSpan(0, 0, 0, 0, 200);
                }
                else
                {
                    openingDuration = new TimeSpan(0, 0, 0, 0, 200);
                    closingDuration = new TimeSpan(0, 0, 0, 0, 100);
                }
                InitStoryboard();
            }
        }
        #endregion properties

        public override void Start()
        {
        }

        public override void Stop()
        {
        }

        public override void DetachEffect()
        {
            control.CanvasRoot.Children.Remove(rectangle);
            control.CanvasRoot.Children.Remove(topHalf);
            control.CanvasRoot.Children.Remove(bottomHalf);
            IsSelfHandle = false;
        }

        protected override void SetSelfHandle()
        {
            if (_isSelfHandle == true)
            {
                control.MouseEnter += new MouseEventHandler(control_MouseEnter);
                control.MouseLeave += new MouseEventHandler(control_MouseLeave);
            }
            else
            {
                control.MouseEnter -= new MouseEventHandler(control_MouseEnter);
                control.MouseLeave -= new MouseEventHandler(control_MouseLeave);
            }
        }
        public SplitScreen(EffectableControl control) : base(control)
        {
            parameterNameList.Add("MaskOpacity");
            parameterNameList.Add("MaskColor");
            parameterNameList.Add("Direction");
            parameterNameList.Add("Speed");

            width = control.Width;
            height = control.Height;
            if (((double.IsNaN(width) && double.IsNaN(height)) || (width == 0 && height == 0)) && !double.IsNaN(control.ActualWidth) && !double.IsNaN(control.ActualHeight))
            {
                width = control.ActualWidth;
                height = control.ActualHeight;
            }

            rectangle = new Rectangle();
            rectangle.Visibility = System.Windows.Visibility.Visible;
            control.CanvasRoot.Children.Add(rectangle);
            Canvas.SetZIndex(rectangle, 63000);

            topHalf = new Rectangle();
            topHalf.Visibility = System.Windows.Visibility.Collapsed;
            control.CanvasRoot.Children.Add(topHalf);
            Canvas.SetZIndex(topHalf, 63000);

            bottomHalf = new Rectangle();
            bottomHalf.Visibility = System.Windows.Visibility.Collapsed;
            control.CanvasRoot.Children.Add(bottomHalf);
            Canvas.SetZIndex(bottomHalf, 63000);

            direction = SplitDirection.VERTICAL;
            MaskOpacity = maskOpacity;
            MaskColor = maskColor;
            Speed = SplitSpeed.MEDIUM;

            //InitStoryboard();           //da duoc goi luc gan speed
            control.SizeChanged += new SizeChangedEventHandler(control_SizeChanged);
        }

        void control_MouseLeave(object sender, MouseEventArgs e)
        {
            sb2.Begin();
        }

        void control_MouseEnter(object sender, MouseEventArgs e)
        {
            sb1.Begin();
        }

        void control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            width = e.NewSize.Width;
            height = e.NewSize.Height;
            InitStoryboard();
        }

        #region create storyboard
        private void InitStoryboard()
        {
            UpdateRectangesPostionAndSize();

            double value, halfRectWidth, halfRectHeight;
            string propertyPath1, propertyPath2; 
            CalculateParameters(out value, out halfRectWidth, out halfRectHeight, out propertyPath1, out propertyPath2);
            InitStoryboard1(value, propertyPath1, propertyPath2);
            InitStoryboard2(value, propertyPath1, propertyPath2);
        }

        private void InitStoryboard1(double value, string propertyPath1, string propertyPath2)
        {
            sb1 = new Storyboard();
            ObjectAnimationUsingKeyFrames oaufk1 = new ObjectAnimationUsingKeyFrames() { BeginTime = TimeSpan.FromMilliseconds(0) };
            DiscreteObjectKeyFrame dokf1 = new DiscreteObjectKeyFrame()
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)),
                Value = Visibility.Collapsed
            };
            oaufk1.KeyFrames.Add(dokf1);
            Storyboard.SetTarget(oaufk1, rectangle);
            Storyboard.SetTargetProperty(oaufk1, new PropertyPath("(Rectangle.Visibility)"));
            sb1.Children.Add(oaufk1);

            ObjectAnimationUsingKeyFrames oaufk2 = new ObjectAnimationUsingKeyFrames() { BeginTime = TimeSpan.FromMilliseconds(0) };
            DiscreteObjectKeyFrame dokf2 = new DiscreteObjectKeyFrame()
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)),
                Value = Visibility.Visible
            };
            oaufk2.KeyFrames.Add(dokf2);
            Storyboard.SetTarget(oaufk2, topHalf);
            Storyboard.SetTargetProperty(oaufk2, new PropertyPath("(Rectangle.Visibility)"));
            sb1.Children.Add(oaufk2);

            ObjectAnimationUsingKeyFrames oaufk3 = new ObjectAnimationUsingKeyFrames() { BeginTime = TimeSpan.FromMilliseconds(0) };
            DiscreteObjectKeyFrame dokf3 = new DiscreteObjectKeyFrame()
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)),
                Value = Visibility.Visible
            };
            oaufk3.KeyFrames.Add(dokf3);
            Storyboard.SetTarget(oaufk3, bottomHalf);
            Storyboard.SetTargetProperty(oaufk3, new PropertyPath("(Rectangle.Visibility)"));
            sb1.Children.Add(oaufk3);

            DoubleAnimation doubleAnimation1 = new DoubleAnimation()
            {
                To = 0,
                BeginTime = new TimeSpan(),
                Duration = openingDuration
            };
            Storyboard.SetTargetProperty(doubleAnimation1, new PropertyPath(propertyPath1));
            Storyboard.SetTarget(doubleAnimation1, topHalf);
            sb1.Children.Add(doubleAnimation1);

            DoubleAnimation doubleAnimation2 = new DoubleAnimation()
            {
                To = 0,
                BeginTime = new TimeSpan(),
                Duration = openingDuration
            };
            Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath(propertyPath1));
            Storyboard.SetTarget(doubleAnimation2, bottomHalf);
            sb1.Children.Add(doubleAnimation2);

            DoubleAnimation doubleAnimation3 = new DoubleAnimation()
            {
                To = value * 2,
                BeginTime = new TimeSpan(),
                Duration = openingDuration
            };
            Storyboard.SetTarget(doubleAnimation3, bottomHalf);
            Storyboard.SetTargetProperty(doubleAnimation3, new PropertyPath(propertyPath2));
            sb1.Children.Add(doubleAnimation3);
        }

        private void InitStoryboard2(double value, string propertyPath1, string propertyPath2)
        {
            sb2 = new Storyboard();
            ObjectAnimationUsingKeyFrames oaufk1 = new ObjectAnimationUsingKeyFrames() { BeginTime = closingDuration };
            DiscreteObjectKeyFrame dokf1 = new DiscreteObjectKeyFrame()
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)),
                Value = Visibility.Visible
            };
            oaufk1.KeyFrames.Add(dokf1);
            Storyboard.SetTarget(oaufk1, rectangle);
            Storyboard.SetTargetProperty(oaufk1, new PropertyPath("(Rectangle.Visibility)"));
            sb2.Children.Add(oaufk1);

            ObjectAnimationUsingKeyFrames oaufk2 = new ObjectAnimationUsingKeyFrames() { BeginTime = closingDuration };
            DiscreteObjectKeyFrame dokf2 = new DiscreteObjectKeyFrame()
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)),
                Value = Visibility.Collapsed
            };
            oaufk2.KeyFrames.Add(dokf2);
            Storyboard.SetTarget(oaufk2, topHalf);
            Storyboard.SetTargetProperty(oaufk2, new PropertyPath("(Rectangle.Visibility)"));
            sb2.Children.Add(oaufk2);

            ObjectAnimationUsingKeyFrames oaufk3 = new ObjectAnimationUsingKeyFrames() { BeginTime = closingDuration };
            DiscreteObjectKeyFrame dokf3 = new DiscreteObjectKeyFrame()
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)),
                Value = Visibility.Collapsed
            };
            oaufk3.KeyFrames.Add(dokf3);
            Storyboard.SetTarget(oaufk3, bottomHalf);
            Storyboard.SetTargetProperty(oaufk3, new PropertyPath("(Rectangle.Visibility)"));
            sb2.Children.Add(oaufk3);

            DoubleAnimation doubleAnimation4 = new DoubleAnimation()
            {
                To = value,
                BeginTime = new TimeSpan(),
                Duration = closingDuration
            };
            Storyboard.SetTarget(doubleAnimation4, topHalf);
            Storyboard.SetTargetProperty(doubleAnimation4, new PropertyPath(propertyPath1));
            sb2.Children.Add(doubleAnimation4);

            DoubleAnimation doubleAnimation5 = new DoubleAnimation()
            {
                To = value,
                BeginTime = new TimeSpan(),
                Duration = closingDuration
            };
            Storyboard.SetTarget(doubleAnimation5, bottomHalf);
            Storyboard.SetTargetProperty(doubleAnimation5, new PropertyPath(propertyPath1));
            sb2.Children.Add(doubleAnimation5);

            DoubleAnimation doubleAnimation6 = new DoubleAnimation()
            {
                To = value,
                BeginTime = new TimeSpan(),
                Duration = closingDuration
            };
            Storyboard.SetTarget(doubleAnimation6, bottomHalf);
            Storyboard.SetTargetProperty(doubleAnimation6, new PropertyPath(propertyPath2));
            sb2.Children.Add(doubleAnimation6);
        }
        #endregion create storyboard

        private void CalculateParameters(out double value,
                                            out double halfRectWidth, out double halfRectHeight,
                                            out string propertyPath1, out string propertyPath2)
        {
            if (direction == SplitDirection.HORIZONTAL)
            {
                halfRectHeight = height;
                halfRectWidth = width / 2;
                value = halfRectWidth;
                propertyPath1 = "(Rectangle.Width)";
                propertyPath2 = "(Canvas.Left)";
            }
            else
            {
                halfRectHeight = height / 2;
                halfRectWidth = width;
                value = halfRectHeight;
                propertyPath1 = "(Rectangle.Height)";
                propertyPath2 = "(Canvas.Top)";
            }
        }

        private void UpdateRectangesPostionAndSize()
        {
            double value, halfRectWidth, halfRectHeight;
            string propertyPath1, propertyPath2;
            CalculateParameters(out value, out halfRectWidth, out halfRectHeight, out propertyPath1, out propertyPath2);

            topHalf.Width = halfRectWidth;
            topHalf.Height = halfRectHeight;
            rectangle.Width = width;
            rectangle.Height = height;
            if (direction == SplitDirection.HORIZONTAL)
            {
                Canvas.SetLeft(bottomHalf, halfRectWidth);
                Canvas.SetTop(bottomHalf, 0);
            }
            else
            {
                Canvas.SetLeft(bottomHalf, 0);
                Canvas.SetTop(bottomHalf, halfRectHeight);
            }
            bottomHalf.Width = halfRectWidth;
            bottomHalf.Height = halfRectHeight;
            
        }
    }
}
