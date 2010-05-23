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
    public class CoverFlow : BasicListEffect
    {                
        #region implement abstact method
        public override void Start()
        {
           
        }
        public override void Stop()
        {
        }
        public override void DetachEffect()
        {
            LayoutRoot.Children.Clear();
            control.Content = null;
            IsSelfHandle = false;
        }
        public override void Next()
        {
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
       
        public void AddItem(FrameworkElement element)
        {
            InsertItem(LayoutRoot.Children.Count, element);
        }

        public void InsertItem(int index, FrameworkElement element)
        {
            CoverFlowItem item = new CoverFlowItem(element);
            element.Width = ItemWidth;
            element.Height = ItemHeight;
            element.RenderTransformOrigin = new Point(0.5, 0.5);
            item.ItemSelected += new EventHandler(item_ItemSelected);
            coverFlowItems.Add(item);
            LayoutRoot.Children.Add(element);
            LayoutChildren();
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

            CoverFlowItem temp3 = coverFlowItems[min];
            CoverFlowItem temp4 = coverFlowItems[max];

            coverFlowItems.RemoveAt(max);
            coverFlowItems[min] = temp4;
            coverFlowItems.Insert(max, temp3);

            LayoutChildren();
        }

        public void RemoveItemAt(int index)
        {
            LayoutRoot.Children.RemoveAt(index);
            coverFlowItems.RemoveAt(index);
            LayoutChildren();
        }
        public void RemoveItem(FrameworkElement ui)
        {
            int index = LayoutRoot.Children.IndexOf(ui);
            RemoveItemAt(index);            
        }
        public void RemoveAllItem()
        {
            LayoutRoot.Children.Clear();
            coverFlowItems.Clear();
        }
        #endregion

        public class CoverFlowItem : DependencyObject
        {
            #region property
            public event EventHandler ItemSelected;

            private FrameworkElement element;

            public FrameworkElement Element
            {
                get { return element; }
                set { element = value; }
            }

            private double yRotation;
            public double YRotation
            {
                get
                {
                    return yRotation;
                }
                set
                {
                    yRotation = value;
                    if (planeProjection != null)
                    {
                        planeProjection.RotationY = value;
                    }
                }
            }

            private double zOffset;
            public double ZOffset
            {
                get
                {
                    return zOffset;
                }
                set
                {
                    zOffset = value;
                    if (planeProjection != null)
                    {
                        planeProjection.LocalOffsetZ = value;
                    }
                }
            }

            private double scale;
            public double Scale
            {
                get
                {
                    return scale;
                }
                set
                {
                    scale = value;
                    if (scaleTransform != null)
                    {
                        scaleTransform.ScaleX = scale;
                        scaleTransform.ScaleY = scale;
                    }
                }
            }

            private double x;
            public double X
            {
                get
                {
                    return x;
                }
                set
                {
                    x = value;
                    Canvas.SetLeft(element, value);
                }
            }
            #endregion

            private bool isAnimating;

            void Animation_Completed(object sender, EventArgs e)
            {
                isAnimating = false;
            }

            private DoubleAnimationUsingKeyFrames CreateDAKF(DependencyObject o, string p, out EasingDoubleKeyFrame e)
            {
                DoubleAnimationUsingKeyFrames dakf = new DoubleAnimationUsingKeyFrames();
                e = new EasingDoubleKeyFrame();
                e.EasingFunction = new CubicEase();
                dakf.KeyFrames.Add(e);
                Storyboard.SetTarget(dakf, o);
                Storyboard.SetTargetProperty(dakf, new PropertyPath(p));
                return dakf;
            }

            private double duration;
            private IEasingFunction easingFunction;
            private PlaneProjection planeProjection;
            private Storyboard Animation;
            private ScaleTransform scaleTransform;
            private EasingDoubleKeyFrame rotationKeyFrame, offestZKeyFrame, scaleXKeyFrame, scaleYKeyFrame;
            private DoubleAnimation xAnimation;

            public CoverFlowItem(FrameworkElement e)
            {
                element = e;
                element.MouseLeftButtonUp += new MouseButtonEventHandler(element_MouseLeftButtonUp);

                easingFunction = new CubicEase();

                Animation = new Storyboard();
                Animation.Completed += new EventHandler(Animation_Completed);

                planeProjection = new PlaneProjection();
                element.Projection = planeProjection;

                scaleTransform = new ScaleTransform();
                scaleTransform.CenterX = scaleTransform.CenterY = 0.5;
                element.RenderTransform = scaleTransform;

                Animation.Children.Add(CreateDAKF(planeProjection, "RotationY", out rotationKeyFrame));
                Animation.Children.Add(CreateDAKF(planeProjection, "LocalOffsetZ", out offestZKeyFrame));
                Animation.Children.Add(CreateDAKF(scaleTransform, "ScaleX", out scaleXKeyFrame));
                Animation.Children.Add(CreateDAKF(scaleTransform, "ScaleY", out scaleYKeyFrame));

                xAnimation = new DoubleAnimation();
                Storyboard.SetTarget(xAnimation, element);
                Storyboard.SetTargetProperty(xAnimation, new PropertyPath("(Canvas.Left)"));
                Animation.Children.Add(xAnimation);
            }

            void element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            {
                if (ItemSelected != null)
                    ItemSelected(this, null);
            }

            public void SetValues(double x, int zIndex, double r, double z, double s, double d, IEasingFunction ease, bool useAnimation)
            {
                if (useAnimation)
                {
                    if (!isAnimating && Canvas.GetLeft(element) != x)
                        Canvas.SetLeft(element, this.x);

                    rotationKeyFrame.Value = r;
                    offestZKeyFrame.Value = z;
                    scaleYKeyFrame.Value = s;
                    scaleXKeyFrame.Value = s;
                    xAnimation.To = x;

                    if (duration != d)
                    {
                        duration = d;
                        rotationKeyFrame.KeyTime = TimeSpan.FromMilliseconds(d);
                        offestZKeyFrame.KeyTime = TimeSpan.FromMilliseconds(d);
                        scaleYKeyFrame.KeyTime = TimeSpan.FromMilliseconds(d);
                        scaleXKeyFrame.KeyTime = TimeSpan.FromMilliseconds(d);
                        xAnimation.Duration = TimeSpan.FromMilliseconds(d);
                    }
                    if (easingFunction != ease)
                    {
                        easingFunction = ease;
                        rotationKeyFrame.EasingFunction = ease;
                        offestZKeyFrame.EasingFunction = ease;
                        scaleYKeyFrame.EasingFunction = ease;
                        scaleXKeyFrame.EasingFunction = ease;
                        xAnimation.EasingFunction = ease;
                    }

                    isAnimating = true;
                    Animation.Begin();
                    Canvas.SetZIndex(element, zIndex);
                }

                this.x = x;
            }
        }

        #region property
        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                IndexSelected(value, false);
            }
        }
        private void IndexSelected(int index, bool mouseclick)
        {
            IndexSelected(index, mouseclick, true);
        }

        private void IndexSelected(int index, bool mouseclick, bool layoutChildren)
        {
            if (coverFlowItems.Count > 0)
            {
                selectedIndex = index;
                if (layoutChildren)
                    LayoutChildren();
            }
        }

        private double _SpaceBetweenItems;

        public double SpaceBetweenItems
        {
            get { return _SpaceBetweenItems; }
            set 
            {
                if (_SpaceBetweenItems == value)
                    return;

                _SpaceBetweenItems = value;
                LayoutChildren();
            }
        }

        private double _SpaceBetweenSelectedItemAndItems;

        public double SpaceBetweenSelectedItemAndItems
        {
            get { return _SpaceBetweenSelectedItemAndItems; }
            set
            {
                if (_SpaceBetweenSelectedItemAndItems == value)
                    return;

                _SpaceBetweenSelectedItemAndItems = value;
                LayoutChildren();
            }
        }

        private double _RotationAngle;

        public double RotationAngle
        {
            get { return _RotationAngle; }
            set
            {
                if (_RotationAngle == value)
                    return;

                _RotationAngle = value;
                LayoutChildren();
            }
        }

        private double _ZDistance;

        public double ZDistance
        {
            get { return _ZDistance; }
            set
            {
                if (_ZDistance == value)
                    return;

                _ZDistance = value;
                LayoutChildren();
            }
        }

        private double _Scale;

        public double Scale
        {
            get { return _Scale; }
            set
            {
                if (_Scale == value)
                    return;

                _Scale = value;
                LayoutChildren();
            }
        }

        private double _SingleItemDuration;

        public double SingleItemDuration
        {
            get { return _SingleItemDuration; }
            set
            {
                if (_SingleItemDuration == value)
                    return;

                _SingleItemDuration = value;
                LayoutChildren();
            }
        }

        private double _PageDuration;

        public double PageDuration
        {
            get { return _PageDuration; }
            set
            {
                if (_PageDuration == value)
                    return;

                _PageDuration = value;
                LayoutChildren();
            }
        }

        private IEasingFunction _EasingFunction;

        public IEasingFunction EasingFunction
        {
            get { return _EasingFunction; }
            set { _EasingFunction = value; }
        }

        private double _ItemWidth;

        public double ItemWidth
        {
            get { return _ItemWidth; }
            set { _ItemWidth = value; }
        }

        private double _ItemHeight;

        public double ItemHeight
        {
            get { return _ItemHeight; }
            set { _ItemHeight = value; }
        }

        #endregion

        private Canvas LayoutRoot;
        private List<CoverFlowItem> coverFlowItems;
        private double duration;

        public CoverFlow(BasicListControl control)
            : base(control)
        {
            LayoutRoot = new Canvas();
            LayoutRoot.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            LayoutRoot.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            control.Content = LayoutRoot;
            LayoutRoot.Background = new SolidColorBrush(Colors.Red);
            LayoutRoot.Width = 400;                        
            ItemWidth = ItemHeight = 150;

            coverFlowItems = new List<CoverFlowItem>();

            RotationAngle = 45;
            SpaceBetweenItems = 40;
            SpaceBetweenSelectedItemAndItems = 60;
            SingleItemDuration = 600;
            PageDuration = 900;
            duration = SingleItemDuration;
            EasingFunction = new CubicEase();
            _Scale = 0.7;    

            foreach (EffectableControl element in control.Items)
            {
                AddItem(element);
            }
        }

        void control_OnListChange(string action, int index1, EffectableControl control, int index2)
        {
            switch (action)
            {
                case "ADD":
                    AddItem(control);
                    break;
                case "INSERT":
                    InsertItem(index1, control);
                    break;
                case "SWAP":
                    Swap(index1, index2);
                    break;
                case "REMOVEAT":
                    RemoveItemAt(index1);
                    break;
                case "REMOVE":
                    RemoveItem(control);
                    break;
                case "REMOVEALL":
                    RemoveAllItem();
                    break;
            }
        }

        void item_ItemSelected(object sender, EventArgs e)
        {
            CoverFlowItem item = sender as CoverFlowItem;
            if (item == null)
                return;
            int index = coverFlowItems.IndexOf(item);
            if (index >= 0)
                IndexSelected(index, true);
        }

        protected void LayoutChildren()
        {
            for (int i = 0; i < coverFlowItems.Count; i++)
            {
                LayoutChild(coverFlowItems[i], i);
            }
        }

        protected void LayoutChild(CoverFlowItem item, int index)
        {
            double m = LayoutRoot.Width / 2;

            int b = index - SelectedIndex;
            double mu = 0;
            if (b < 0)
                mu = -1;
            else if (b > 0)
                mu = 1;
            double x = (m + ((double)b * SpaceBetweenItems + (SpaceBetweenSelectedItemAndItems * mu))) - item.Element.Width / 2;

            double s = mu == 0 ? 1 : Scale;

            int zindex = coverFlowItems.Count - Math.Abs(b);

            if (((x + item.Element.Width) < 0 || x > LayoutRoot.Width)
                && ((item.X + item.Element.Width) < 0 || item.X > LayoutRoot.Width)
                && !((x + item.Element.Width) < 0 && item.X > LayoutRoot.Width)
                && !((item.X + item.Element.Width) < 0 && x > LayoutRoot.Width))
            {
                item.SetValues(x, zindex, RotationAngle * mu, ZDistance * Math.Abs(mu), s, duration, EasingFunction, false);
            }
            else
            {
                item.SetValues(x, zindex, RotationAngle * mu, ZDistance * Math.Abs(mu), s, duration, EasingFunction, true);
            }
        }
    }
}
