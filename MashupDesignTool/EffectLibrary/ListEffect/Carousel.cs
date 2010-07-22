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
            #region ScaleX
            public static readonly DependencyProperty ScaleXProperty = DependencyProperty.Register("ScaleX", typeof(double), typeof(CarouselItem), new PropertyMetadata(1.0d, OnScaleXPropertyChanged));

            private static void OnScaleXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                CarouselItem item = d as CarouselItem;
                if (item != null)
                {
                    item.ScaleXChanged((double)e.OldValue, (double)e.NewValue);
                }
            }

            private void ScaleXChanged(double oldValue, double newValue)
            {
            }
            public double ScaleX
            {
                get { return (double)GetValue(ScaleXProperty); }
                set { SetValue(ScaleXProperty, value); }
            }
            #endregion

            #region ScaleY
            public static readonly DependencyProperty ScaleYProperty = DependencyProperty.Register("ScaleY", typeof(double), typeof(CarouselItem), new PropertyMetadata(1.0d, OnScaleYPropertyChanged));

            private static void OnScaleYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                CarouselItem item = d as CarouselItem;
                if (item != null)
                {
                    item.ScaleYChanged((double)e.OldValue, (double)e.NewValue);
                }
            }

            private void ScaleYChanged(double oldValue, double newValue)
            {
            }
            public double ScaleY
            {
                get { return (double)GetValue(ScaleYProperty); }
                set { SetValue(ScaleYProperty, value); }
            }
            #endregion

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
                Canvas.SetTop(_Element, dy);

                // Scale 
                double sc = 2 + Math.Cos(newValue - Math.PI / 2) * 1;

                ScaleTransform st = _Element.RenderTransform as ScaleTransform;
                if (st == null)
                {
                    st = new ScaleTransform();
                    _Element.RenderTransform = st;
                }
                st.ScaleX = Math.Pow(sc, ScaleX);
                st.ScaleY = Math.Pow(sc, ScaleY);

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
                if (sb != null)
                {
                    double count = Math.PI * 2 / PerAngle;
                    ((DoubleAnimation)sb.Children[0]).Duration = TimeSpan.FromMilliseconds(newValue / count);
                }
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
                {
                    ((DoubleAnimation)sb.Children[0]).By = newValue;
                    double count = Math.PI * 2 / newValue;
                    ((DoubleAnimation)sb.Children[0]).Duration = TimeSpan.FromMilliseconds(Duration / count);
                }
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

            public CarouselItem(Point center, Size axis, double scaleX, double scaleY, double duration, FrameworkElement element)
            {
                Center = center;
                Axis = axis;
                ScaleX = scaleX;
                ScaleY = scaleY;
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
        private double _PaddingLeft;
        private double _PaddingRight;
        private double _PaddingTop;
        private double _PaddingBottom;
        private double _ItemWidth;
        private double _ItemHeight;
        private double _ScaleX;
        private double _ScaleY;

        public double ScaleX
        {
            get { return _ScaleX; }
            set
            {
                _ScaleX = value;
                CalculateEllipse();
                Stop();
                foreach (CarouselItem item in carouselItems)
                {
                    item.Center = _Center;
                    item.Axis = _Axis;
                    item.ScaleX = _ScaleX;
                }
                Start();
            }
        }

        public double ScaleY
        {
            get { return _ScaleY; }
            set
            {
                _ScaleY = value;
                CalculateEllipse();
                Stop();
                foreach (CarouselItem item in carouselItems)
                {
                    item.Center = _Center;
                    item.Axis = _Axis;
                    item.ScaleY = _ScaleY;
                }
                Start();
            }
        }

        public double Duration
        {
            get { return _Duration; }
            set
            {
                _Duration = value;
                Stop();
                foreach (CarouselItem item in carouselItems)
                {
                    item.Duration = _Duration;
                }
                Start();
            }
        }

        public double PaddingLeft
        {
            get { return _PaddingLeft; }
            set
            {
                _PaddingLeft = value;
                CalculateEllipse();
                Stop();
                foreach (CarouselItem item in carouselItems)
                {
                    item.Axis = _Axis;
                    item.Center = _Center;
                }
                Start();
            }
        }

        public double PaddingRight
        {
            get { return _PaddingRight; }
            set
            {
                _PaddingRight = value;
                CalculateEllipse();
                Stop();
                foreach (CarouselItem item in carouselItems)
                {
                    item.Axis = _Axis;
                    item.Center = _Center;
                }
                Start();
            }
        }

        public double PaddingTop
        {
            get { return _PaddingTop; }
            set
            {
                _PaddingTop = value;
                CalculateEllipse();
                Stop();
                foreach (CarouselItem item in carouselItems)
                {
                    item.Axis = _Axis;
                    item.Center = _Center;
                }
                Start();
            }
        }

        public double PaddingBottom
        {
            get { return _PaddingBottom; }
            set
            {
                _PaddingBottom = value;
                CalculateEllipse();
                Stop();
                foreach (CarouselItem item in carouselItems)
                {
                    item.Axis = _Axis;
                    item.Center = _Center;
                }
                Start();
            }
        }

        public double ItemWidth
        {
            get { return _ItemWidth; }
            set
            {
                _ItemWidth = value;
                CalculateEllipse();
                Stop();
                foreach (CarouselItem item in carouselItems)
                {
                    item.Axis = _Axis;
                    item.Center = _Center;
                    item.Element.Width = ItemWidth;
                    item.Element.Height = ItemHeight;

                }
                Start();
            }
        }

        public double ItemHeight
        {
            get { return _ItemHeight; }
            set
            {
                _ItemHeight = value;
                CalculateEllipse();
                Stop();
                foreach (CarouselItem item in carouselItems)
                {
                    item.Axis = _Axis;
                    item.Center = _Center;
                    item.Element.Width = ItemWidth;
                    item.Element.Height = ItemHeight;
                    if (_ReflectionShader == true)
                        item.Element.Effect = new EffectLibrary.CustomPixelShader.ReflectionShader() { ElementHeight = _ItemHeight - 2};
                }
                Start();
            }
        }

        private Brush _Background;

        public Brush Background
        {
            get { return LayoutRoot.Background; }
            set
            {
                _Background = value;
                LayoutRoot.Background = _Background;
            }
        }

        private bool _ReflectionShader;

        public bool ReflectionShader
        {
            get { return _ReflectionShader; }
            set
            {
                _ReflectionShader = value;
                if (_ReflectionShader == true)
                {
                    foreach (UIElement ui in LayoutRoot.Children)
                    {
                        ui.Effect = new EffectLibrary.CustomPixelShader.ReflectionShader() { ElementHeight = _ItemHeight - 2 };
                    }
                }
                else
                {
                    foreach (UIElement ui in LayoutRoot.Children)
                    {
                        ui.Effect = null;
                    }
                }
            }
        }

        private Canvas LayoutRoot;
        private Point _Center;
        private double _PerAngel = Math.PI;
        private Size _Axis;
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
            Stop();
            foreach (CarouselItem ca in carouselItems)
            {
                ca.Element.RenderTransformOrigin = new Point(0, 0);
                ca.Element.RenderTransform = null;
                Canvas.SetTop(ca.Element, 0);
            }

            ReflectionShader = false;
            LayoutRoot.Children.Clear();
            control.Container.Content = null;
            IsSelfHandle = false;
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
            CarouselItem item = new CarouselItem(_Center, _Axis, _ScaleX, _ScaleY, _Duration, element);
            LayoutRoot.Children.Insert(index, element);
            element.Width = ItemWidth;
            element.Height = ItemHeight;
            element.RenderTransformOrigin = new Point(0.5, 0.5);
            if (_ReflectionShader == true)
                element.Effect = new EffectLibrary.CustomPixelShader.ReflectionShader() { ElementHeight = _ItemHeight - 2};
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
            parameterNameList.Add("Duration");
            parameterNameList.Add("PaddingLeft");
            parameterNameList.Add("PaddingRight");
            parameterNameList.Add("PaddingTop");
            parameterNameList.Add("PaddingBottom");
            parameterNameList.Add("ItemWidth");
            parameterNameList.Add("ItemHeight");
            parameterNameList.Add("ScaleX");
            parameterNameList.Add("ScaleY");
            parameterNameList.Add("Background");
            parameterNameList.Add("ReflectionShader");

            LayoutRoot = new Canvas();
            LayoutRoot.Background = new SolidColorBrush(Colors.Transparent);
            LayoutRoot.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            LayoutRoot.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            control.Container.Content = LayoutRoot;

            Duration = 5000;
            _ScaleX = 1;
            _ScaleY = 1;

            ItemWidth = ItemHeight = 20;

            foreach (EffectableControl c in control.Items)
            {
                AddItem(c);
            }

            LayoutRoot.SizeChanged += new SizeChangedEventHandler(LayoutRoot_SizeChanged);
        }

        void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (LayoutRoot.ActualWidth != 0)
                control.Width = LayoutRoot.ActualWidth;
            if (LayoutRoot.ActualHeight != 0)
                control.Height = LayoutRoot.ActualHeight;
            CalculateEllipse();
            Stop();
            foreach (CarouselItem item in carouselItems)
            {
                item.Axis = _Axis;
                item.Center = _Center;
            }
            Start();

        }

        private void CalculateEllipse()
        {
            // X and Y axis of the ellipse
            double maxItemWidth = (Math.Pow(2, ScaleX) * ItemWidth);
            double maxItemHeight = (Math.Pow(3, ScaleY) * ItemHeight);
            double radiusX = (control.Width - maxItemWidth - PaddingLeft - PaddingRight) / 2;
            double radiusY = (control.Height - maxItemHeight - PaddingTop - PaddingBottom) / 2;

            // center point of the ellipse
            double centerX = radiusX + PaddingLeft + maxItemWidth / 2 - ItemWidth / 2;
            double centerY = radiusY + PaddingTop + maxItemHeight / 2 - ItemHeight / 2;

            _Center = new Point(centerX, centerY);
            _Axis = new Size(radiusX, radiusY);
        }

        void control_OnListChange(BasicListControl.ListItemsAction action, int index1, EffectableControl control, int index2)
        {
            switch (action)
            {
                case BasicListControl.ListItemsAction.ADD:
                    Stop();
                    AddItem(control);
                    break;
                case BasicListControl.ListItemsAction.INSERT:
                    Stop();
                    InsertItem(index1, control);
                    break;
                case BasicListControl.ListItemsAction.SWAP:
                    Stop();
                    Swap(index1, index2);
                    break;
                case BasicListControl.ListItemsAction.REMOVEAT:
                    Stop();
                    RemoveItemAt(index1);
                    break;
                case BasicListControl.ListItemsAction.REMOVE:
                    Stop();
                    RemoveItem(control);
                    break;
                case BasicListControl.ListItemsAction.REMOVEALL:
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
