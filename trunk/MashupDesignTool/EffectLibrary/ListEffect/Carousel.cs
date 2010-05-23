using System;
using System.Collections.Generic;
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
    public class Carousel : BasicListEffect
    {
        public class CarouselItem : DependencyObject
        {
            #region center
            public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center", typeof(Point), typeof(CarouselItem), new PropertyMetadata(OnCenterPropertyChanged));

            private static void OnCenterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                CarouselItem item = d as CarouselItem;
                if (item != null)
                {
                    item.CenterChanged((Point)e.OldValue, (Point)e.NewValue);
                }
            }

            private void CenterChanged(Point oldValue, Point newValue)
            {
            }
            public Point Center
            {
                get { return (Point)GetValue(CenterProperty); }
                set { SetValue(CenterProperty, value); }
            }
            #endregion

            #region Axis
            public static readonly DependencyProperty AxisProperty = DependencyProperty.Register("Axis", typeof(Size), typeof(CarouselItem), new PropertyMetadata(OnAxisPropertyChanged));

            private static void OnAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                CarouselItem item = d as CarouselItem;
                if (item != null)
                {
                    item.AxisChanged((Size)e.OldValue, (Size)e.NewValue);
                }
            }

            private void AxisChanged(Size oldValue, Size newValue)
            {
            }
            public Size Axis
            {
                get { return (Size)GetValue(AxisProperty); }
                set { SetValue(AxisProperty, value); }
            }
            #endregion

            #region Angle
            public static readonly DependencyProperty AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(CarouselItem), new PropertyMetadata(OnAnglePropertyChanged));

            private static void OnAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                CarouselItem item = d as CarouselItem;
                if (item != null)
                {
                    item.AngleChanged((double)e.OldValue, (double)e.NewValue);
                }
            }

            private void AngleChanged(double oldValue, double newValue)
            {
                //Position
                double dx = Axis.Width * Math.Cos(newValue) + Center.X;
                double dy = Axis.Height * Math.Sin(newValue) + Center.Y;
                Canvas.SetLeft(_Element, dx);
                Canvas.SetTop(this._Element, dy);

                // Scale 
                double sc = 2 + Math.Cos(newValue - Math.PI / 2) * 1;
                ScaleTransform st = _Element.RenderTransform as ScaleTransform;
                if (st == null)
                {
                    st = new ScaleTransform();
                    _Element.RenderTransform = st;
                }
                st.ScaleX = sc;
                st.ScaleY = sc;
                // Set the ZIndex based the distance from us, the far item 
                // is under the near item 
                Canvas.SetZIndex(_Element, (int)dy);
            }
            public double Angle
            {
                get { return (double)GetValue(AngleProperty); }
                set { SetValue(AngleProperty, value); }
            }
            #endregion

            #region Duration
            public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(double), typeof(CarouselItem), new PropertyMetadata(OnDurationPropertyChanged));

            private static void OnDurationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                CarouselItem item = d as CarouselItem;
                if (item != null)
                {
                    item.DurationChanged((double)e.OldValue, (double)e.NewValue);
                }
            }

            private void DurationChanged(double oldValue, double newValue)
            {
            }
            public double Duration
            {
                get { return (double)GetValue(DurationProperty); }
                set { SetValue(DurationProperty, value); }
            }
            #endregion

            #region PerAngle
            public static readonly DependencyProperty PerAngleProperty = DependencyProperty.Register("PerAngle", typeof(double), typeof(CarouselItem), new PropertyMetadata(OnPerAnglePropertyChanged));

            private static void OnPerAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                CarouselItem item = d as CarouselItem;
                if (item != null)
                {
                    item.PerAngleChanged((double)e.OldValue, (double)e.NewValue);
                }
            }

            private void PerAngleChanged(double oldValue, double newValue)
            {
                if (sb != null)
                    ((DoubleAnimation)sb.Children[0]).By = newValue;
            }
            public double PerAngle
            {
                get { return (double)GetValue(PerAngleProperty); }
                set { SetValue(PerAngleProperty, value); }
            }
            #endregion

            FrameworkElement _Element;
            public FrameworkElement Element
            {
                get { return _Element; }
                set { _Element = value; }
            }

            private Storyboard sb;

            public CarouselItem(Point center, Size axis, double duration, FrameworkElement element)
            {
                Center = center;
                Axis = axis;
                Duration = duration;
                _Element = element;
                sb = CreateStoryBoard(Duration);
            }

            public void Next()
            {
                sb.Begin();
            }

            public void Prev()
            {
                sb.Begin();
            }

            public void BeginAutoTurn()
            {
                sb.Begin();
                sb.Completed += new EventHandler(sb_Completed);
            }

            void sb_Completed(object sender, EventArgs e)
            {
                sb.Begin();
            }

            public void StopTurn()
            {
                sb.Stop();
                sb.Completed -= new EventHandler(sb_Completed);
            }

            public void Pause()
            {
                sb.Pause();
            }

            private Storyboard CreateStoryBoard(double duration)
            {
                Storyboard storyboard = new Storyboard();

                // Angle animation
                DoubleAnimation doubleAnimation = new DoubleAnimation();
                doubleAnimation.Duration = TimeSpan.FromMilliseconds(duration);

                // Set storyboard target property
                storyboard.Children.Add(doubleAnimation);
                Storyboard.SetTarget(doubleAnimation, this);
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Angle"));
                return storyboard;
            }
        }

        #region property
        private double _Duration;
        private double _PaddingLeftRight;
        private double _PaddingTopBottom;
        private double _ItemWidth;
        private double _ItemHeight;

        public double Duration
        {
            get { return _Duration; }
            set { _Duration = value; }
        }

        public double PaddingLeftRight
        {
            get { return _PaddingLeftRight; }
            set { _PaddingLeftRight = value; }
        }

        public double PaddingTopBottom
        {
            get { return _PaddingTopBottom; }
            set { _PaddingTopBottom = value; }
        }

        public double ItemWidth
        {
            get { return _ItemWidth; }
            set { _ItemWidth = value; }
        }

        public double ItemHeight
        {
            get { return _ItemHeight; }
            set { _ItemHeight = value; }
        }

        public Canvas LayoutRoot;
        public Point _Center;
        public double _PerAngel = Math.PI;
        public Size _Axis;
        #endregion

        #region implement abstact method
        public override void Start()
        {
            foreach (CarouselItem i in carouselItems)
                i.BeginAutoTurn();
        }
        public override void Stop()
        {
            foreach (CarouselItem i in carouselItems)
                i.StopTurn();
        }
        public override void DetachEffect()
        {
            foreach (CarouselItem ca in carouselItems)
                ca.Element.RenderTransform = null;
            LayoutRoot.Children.Clear();
            control.Content = null;
        }
        public override void Next()
        {
            foreach (CarouselItem i in carouselItems)
                i.Next();
        }
        public override void Prev()
        {
        }

        protected override void SetSelfHandle()
        {
            if (_isSelfHandle == true)
            {
                control.OnListChange += new BasicListControl.ListChangeHandler(control_OnListChange);
            }
            else
            {
                control.OnListChange -= new BasicListControl.ListChangeHandler(control_OnListChange);
            }
        }
        #endregion

        #region method for list change event
        List<CarouselItem> carouselItems = new List<CarouselItem>();
        public void AddItem(FrameworkElement element)
        {
            InsertItem(LayoutRoot.Children.Count, element);
        }

        public void InsertItem(int index, FrameworkElement element)
        {
            CarouselItem item = new CarouselItem(_Center, _Axis, _Duration, element);
            LayoutRoot.Children.Insert(index, element);
            carouselItems.Insert(index, item);
            UpdateItem();
        }

        public void Swap(int index1, int index2)
        {
            int min = Math.Min(index1, index2);
            int max = Math.Max(index1, index2);

            UIElement temp1 = LayoutRoot.Children[min];
            UIElement temp2 = LayoutRoot.Children[max];
            
            LayoutRoot.Children.RemoveAt(max);
            LayoutRoot.Children[min] = temp2;
            LayoutRoot.Children.Insert(max, temp1);

            CarouselItem temp3 = carouselItems[min];
            CarouselItem temp4 = carouselItems[max];

            carouselItems.RemoveAt(max);
            carouselItems[min] = temp4;
            carouselItems.Insert(max, temp3);

            UpdateItem();
        }

        public void RemoveItemAt(int index)
        {
            LayoutRoot.Children.RemoveAt(index);
            carouselItems.RemoveAt(index);
            UpdateItem();
        }
        public void RemoveItem(FrameworkElement ui)
        {
            int index = LayoutRoot.Children.IndexOf(ui);
            RemoveItemAt(index);
        }
        public void RemoveAllItem()
        {
            LayoutRoot.Children.Clear();
            carouselItems.Clear();
        }
        #endregion

        public Carousel(BasicListControl control)
            : base(control)
        {
            LayoutRoot = new Canvas();
            LayoutRoot.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            LayoutRoot.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            control.Content = LayoutRoot;

            control.Width = 200;
            control.Height = 200;
            Duration = 5000;

            control.Background = new SolidColorBrush(Colors.Red);

            ItemWidth = ItemHeight = 20;

            // X and Y axis of the ellipse
            double radiusX = (control.Width - 2 * ItemWidth - PaddingLeftRight) / 2;
            double radiusY = (control.Height - 3 * ItemHeight - PaddingTopBottom) / 2;

            // center point of the ellipse
            double centerX = radiusX + PaddingLeftRight / 2;
            double centerY = radiusY + PaddingTopBottom / 2;

            _Center = new Point(centerX, centerY);
            _Axis = new Size(radiusX, radiusY);
        }

        void control_OnListChange(string action, int index1, EffectableControl control, int index2)
        {
            switch (action)
            {
                case "ADD":
                    Stop();
                    AddItem(control);
                    break;
                case "INSERT":
                    Stop();
                    InsertItem(index1, control);
                    break;
                case "SWAP":
                    Stop();
                    Swap(index1, index2);
                    break;
                case "REMOVEAT":
                    Stop();
                    RemoveItemAt(index1);
                    break;
                case "REMOVE":
                    Stop();
                    RemoveItem(control);
                    break;
                case "REMOVEALL":
                    Stop();           
                    RemoveAllItem();
                    break;
            }
        }

        private void UpdateItem()
        {
            _PerAngel = Math.PI * 2 / carouselItems.Count;
            for (int i = 0; i < carouselItems.Count; i++)
            {
                carouselItems[i].Angle = i * _PerAngel;
                carouselItems[i].PerAngle = _PerAngel;
            }
            Start();
        }
    }
}
