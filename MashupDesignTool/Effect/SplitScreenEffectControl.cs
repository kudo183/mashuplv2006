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

namespace Effect
{
    public class SplitScreenEffectControl : UserControl
    {
        public enum SplitDirection
        {
            VERTICAL,
            HORIZONTAL
        }

        #region attributes
        private double opacity = 0.4;
        Canvas LayoutRoot = new Canvas();
        Color mashColor = Colors.Gray;
        Storyboard sb1;
        Storyboard sb2;
        Rectangle rectangle;
        Rectangle topHalf;
        Rectangle bottomHalf;
        SplitDirection splitDirection;
        #endregion attributes

        #region Properties
        public Color MashColor
        {
            get { return mashColor; }
            set { mashColor = value; }
        }

        public double MashOpacity
        {
            get { return opacity; }
            set { opacity = value; }
        }

        public FrameworkElement Item
        {
            get { return (FrameworkElement)LayoutRoot.Children[2]; }
            set
            {
                value.IsHitTestVisible = false;
                LayoutRoot.Children[2] = value;
            }
        }
        #endregion Properties

        public SplitScreenEffectControl(FrameworkElement element, SplitDirection direction)
        {
            splitDirection = direction;
            LayoutRoot.Children.Add(element);

            double x, y, width, height;
            x = Canvas.GetLeft(element);
            y = Canvas.GetTop(element);
            width = element.Width;
            height = element.Height;
            if (((double.IsNaN(width) && double.IsNaN(height)) || (width == 0 && height == 0)) && !double.IsNaN(element.ActualWidth) && !double.IsNaN(element.ActualHeight))
            {
                width = element.ActualWidth;
                height = element.ActualHeight;
            }

            double value, halfRectWidth, halfRectHeight;
            string propertyPath1, propertyPath2;
            CalculateParameters(direction, width, height, out value, out halfRectWidth, out halfRectHeight, out propertyPath1, out propertyPath2);
            
            rectangle = new Rectangle();
            rectangle.Fill = new SolidColorBrush(mashColor);
            rectangle.Opacity = opacity;
            rectangle.Width = width;
            rectangle.Height = height;
            rectangle.Visibility = System.Windows.Visibility.Visible;
            LayoutRoot.Children.Add(rectangle);

            topHalf = new Rectangle();
            topHalf.Fill = new SolidColorBrush(mashColor);
            topHalf.Opacity = opacity;
            topHalf.Width = halfRectWidth;
            topHalf.Height = halfRectHeight;
            topHalf.Visibility = System.Windows.Visibility.Collapsed;
            LayoutRoot.Children.Add(topHalf);

            bottomHalf = new Rectangle();
            bottomHalf.Fill = new SolidColorBrush(mashColor);
            bottomHalf.Opacity = opacity;
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
            bottomHalf.Visibility = System.Windows.Visibility.Collapsed;
            LayoutRoot.Children.Add(bottomHalf);

            InitStoryboard1(value, propertyPath1, propertyPath2);
            InitStoryboard2(value, propertyPath1, propertyPath2);

            this.Content = LayoutRoot;

            element.SizeChanged += new SizeChangedEventHandler(element_SizeChanged);
            this.MouseEnter += new MouseEventHandler(element_MouseEnter);
            this.MouseLeave += new MouseEventHandler(element_MouseLeave);
            //element.SizeChanged += new SizeChangedEventHandler(element_SizeChanged);
        }

        #region calculate parameters for effect
        private void CalculateParameters(SplitDirection direction, 
                                            double width, double height, 
                                            out double value, 
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
        #endregion calculate parameters for effect

        #region event handler
        void element_MouseEnter(object sender, MouseEventArgs e)
        {
            sb1.Begin();
        }

        void element_MouseLeave(object sender, MouseEventArgs e)
        {
            sb2.Begin();
        }

        void element_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize == e.PreviousSize)
                return;
            double value, halfRectWidth, halfRectHeight;
            string propertyPath1, propertyPath2;

            CalculateParameters(splitDirection, e.NewSize.Width, e.NewSize.Height, out value, out halfRectWidth, out halfRectHeight, out propertyPath1, out propertyPath2);

            rectangle.Width = e.NewSize.Width;
            rectangle.Height = e.NewSize.Height;
            topHalf.Width = halfRectWidth;
            topHalf.Height = halfRectHeight;
            bottomHalf.Width = halfRectWidth;
            bottomHalf.Height = halfRectHeight;
            if (splitDirection == SplitDirection.HORIZONTAL)
                Canvas.SetLeft(bottomHalf, halfRectWidth);
            else
                Canvas.SetTop(bottomHalf, halfRectHeight);

            InitStoryboard1(value, propertyPath1, propertyPath2);
            InitStoryboard2(value, propertyPath1, propertyPath2);
        }
        #endregion event handler

        #region create storyboard
        private void InitStoryboard1(double value, string propertyPath1, string propertyPath2)
        {
            sb1 = new Storyboard();
            ObjectAnimationUsingKeyFrames oaufk1 = new ObjectAnimationUsingKeyFrames();
            oaufk1.BeginTime = TimeSpan.FromMilliseconds(0);
            DiscreteObjectKeyFrame dokf1 = new DiscreteObjectKeyFrame();
            dokf1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0));
            dokf1.Value = Visibility.Collapsed;
            oaufk1.KeyFrames.Add(dokf1);
            Storyboard.SetTarget(oaufk1, rectangle);
            Storyboard.SetTargetProperty(oaufk1, new PropertyPath("(Rectangle.Visibility)"));
            sb1.Children.Add(oaufk1);

            ObjectAnimationUsingKeyFrames oaufk2 = new ObjectAnimationUsingKeyFrames();
            oaufk2.BeginTime = TimeSpan.FromMilliseconds(0);
            DiscreteObjectKeyFrame dokf2 = new DiscreteObjectKeyFrame();
            dokf2.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0));
            dokf2.Value = Visibility.Visible;
            oaufk2.KeyFrames.Add(dokf2);
            Storyboard.SetTarget(oaufk2, topHalf);
            Storyboard.SetTargetProperty(oaufk2, new PropertyPath("(Rectangle.Visibility)"));
            sb1.Children.Add(oaufk2);

            ObjectAnimationUsingKeyFrames oaufk3 = new ObjectAnimationUsingKeyFrames();
            oaufk3.BeginTime = TimeSpan.FromMilliseconds(0);
            DiscreteObjectKeyFrame dokf3 = new DiscreteObjectKeyFrame();
            dokf3.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0));
            dokf3.Value = Visibility.Visible;
            oaufk3.KeyFrames.Add(dokf3);
            Storyboard.SetTarget(oaufk3, bottomHalf);
            Storyboard.SetTargetProperty(oaufk3, new PropertyPath("(Rectangle.Visibility)"));
            sb1.Children.Add(oaufk3);

            DoubleAnimation doubleAnimation1 = new DoubleAnimation();
            doubleAnimation1.To = 0;
            doubleAnimation1.BeginTime = new TimeSpan();
            doubleAnimation1.Duration = CalculateDurationOpening(value);
            Storyboard.SetTargetProperty(doubleAnimation1, new PropertyPath(propertyPath1));
            Storyboard.SetTarget(doubleAnimation1, topHalf);
            sb1.Children.Add(doubleAnimation1);

            DoubleAnimation doubleAnimation2 = new DoubleAnimation();
            doubleAnimation2.To = 0;
            doubleAnimation2.BeginTime = new TimeSpan();
            doubleAnimation2.Duration = CalculateDurationOpening(value);
            Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath(propertyPath1));
            Storyboard.SetTarget(doubleAnimation2, bottomHalf);
            sb1.Children.Add(doubleAnimation2);

            DoubleAnimation doubleAnimation3 = new DoubleAnimation();
            doubleAnimation3.To = value * 2;
            doubleAnimation3.BeginTime = new TimeSpan();
            doubleAnimation3.Duration = CalculateDurationOpening(value);
            Storyboard.SetTarget(doubleAnimation3, bottomHalf);
            Storyboard.SetTargetProperty(doubleAnimation3, new PropertyPath(propertyPath2));
            sb1.Children.Add(doubleAnimation3);
        }

        private void InitStoryboard2(double value, string propertyPath1, string propertyPath2)
        {
            sb2 = new Storyboard();
            ObjectAnimationUsingKeyFrames oaufk1 = new ObjectAnimationUsingKeyFrames();
            oaufk1.BeginTime = TimeSpan.FromMilliseconds(0);
            DiscreteObjectKeyFrame dokf1 = new DiscreteObjectKeyFrame();
            dokf1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0));
            dokf1.Value = Visibility.Visible;
            oaufk1.KeyFrames.Add(dokf1);
            Storyboard.SetTarget(oaufk1, rectangle);
            Storyboard.SetTargetProperty(oaufk1, new PropertyPath("(Rectangle.Visibility)"));
            sb2.Children.Add(oaufk1);

            ObjectAnimationUsingKeyFrames oaufk2 = new ObjectAnimationUsingKeyFrames();
            oaufk2.BeginTime = TimeSpan.FromMilliseconds(0);
            DiscreteObjectKeyFrame dokf2 = new DiscreteObjectKeyFrame();
            dokf2.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0));
            dokf2.Value = Visibility.Collapsed;
            oaufk2.KeyFrames.Add(dokf2);
            Storyboard.SetTarget(oaufk2, topHalf);
            Storyboard.SetTargetProperty(oaufk2, new PropertyPath("(Rectangle.Visibility)"));
            sb2.Children.Add(oaufk2);

            ObjectAnimationUsingKeyFrames oaufk3 = new ObjectAnimationUsingKeyFrames();
            oaufk3.BeginTime = TimeSpan.FromMilliseconds(0);
            DiscreteObjectKeyFrame dokf3 = new DiscreteObjectKeyFrame();
            dokf3.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0));
            dokf3.Value = Visibility.Collapsed;
            oaufk3.KeyFrames.Add(dokf3);
            Storyboard.SetTarget(oaufk3, bottomHalf);
            Storyboard.SetTargetProperty(oaufk3, new PropertyPath("(Rectangle.Visibility)"));
            sb2.Children.Add(oaufk3);

            DoubleAnimation doubleAnimation4 = new DoubleAnimation();
            doubleAnimation4.To = value;
            doubleAnimation4.BeginTime = new TimeSpan();
            doubleAnimation4.Duration = CalculateDurationClosing(value);
            Storyboard.SetTarget(doubleAnimation4, topHalf);
            Storyboard.SetTargetProperty(doubleAnimation4, new PropertyPath(propertyPath1));
            sb2.Children.Add(doubleAnimation4);

            DoubleAnimation doubleAnimation5 = new DoubleAnimation();
            doubleAnimation5.To = value;
            doubleAnimation5.BeginTime = new TimeSpan();
            doubleAnimation5.Duration = CalculateDurationClosing(value);
            Storyboard.SetTarget(doubleAnimation5, bottomHalf);
            Storyboard.SetTargetProperty(doubleAnimation5, new PropertyPath(propertyPath1));
            sb2.Children.Add(doubleAnimation5);

            DoubleAnimation doubleAnimation6 = new DoubleAnimation();
            doubleAnimation6.To = value;
            doubleAnimation6.BeginTime = new TimeSpan();
            doubleAnimation6.Duration = CalculateDurationClosing(value);
            Storyboard.SetTarget(doubleAnimation6, bottomHalf);
            Storyboard.SetTargetProperty(doubleAnimation6, new PropertyPath(propertyPath2));
            sb2.Children.Add(doubleAnimation6);
        }
        #endregion create storyboard

        #region calculate duration for effect
        private TimeSpan CalculateDurationOpening(double distance)
        {
            TimeSpan timeSpan = new TimeSpan();
            TimeSpan temp = new TimeSpan(0, 0, 0, 0, 400);
            int n = (int)(distance / 400) + 1;
            for (int i = 0; i < n; i++)
                timeSpan += temp;
            return timeSpan;
        }

        private TimeSpan CalculateDurationClosing(double distance)
        {
            TimeSpan timeSpan = new TimeSpan();
            TimeSpan temp = new TimeSpan(0, 0, 0, 0, 250);
            int n = (int)(distance / 400) + 1;
            for (int i = 0; i < n; i++)
                timeSpan += temp;
            return timeSpan;
        }
        #endregion calculate duration for effect
    }
}
