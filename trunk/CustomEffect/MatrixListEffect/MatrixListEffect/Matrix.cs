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

namespace CustomListEffect
{
    public class Matrix : BasicListEffect
    {
        double _ItemWidth;
        double _ItemHeight;
        Orientation _ListOrientation;
        double _SpaceBetweenItem;

        Thickness _space = new Thickness();
        public double SpaceBetweenItem
        {
            get { return _SpaceBetweenItem; }
            set
            {
                _SpaceBetweenItem = value;
                UpdateSpace();
            }
        }

        public Orientation ListOrientation
        {
            get { return _ListOrientation; }
            set
            {
                _ListOrientation = value;
                UpdateScrollview();
                UpdateSpace();                
                LayoutRoot.Orientation = _ListOrientation;
            }
        }

        public double ItemWidth
        {
            get { return _ItemWidth; }
            set
            {
                _ItemWidth = value;
                foreach (FrameworkElement element in LayoutRoot.Children)
                    element.Width = _ItemWidth;
            }
        }

        public double ItemHeight
        {
            get { return _ItemHeight; }
            set
            {
                _ItemHeight = value;
                foreach (FrameworkElement element in LayoutRoot.Children)
                    element.Height = _ItemHeight;
            }
        }

        private Brush _BackgroundColor;

        public Brush BackgroundColor
        {
            get { return LayoutRoot.Background; }
            set
            {
                _BackgroundColor = value;
                LayoutRoot.Background = _BackgroundColor;
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
                        ui.Effect = new EffectLibrary.CustomPixelShader.ReflectionShader(_ItemHeight);
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

        internal void UpdateSpace()
        {
            _space = new Thickness(_SpaceBetweenItem);
            foreach (FrameworkElement element in LayoutRoot.Children)
                element.Margin = _space;
        }

        internal void UpdateScrollview()
        {
            if (_ListOrientation == Orientation.Horizontal)
            {
                scrollView.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                scrollView.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
            else
            {
                scrollView.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                scrollView.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }
        }

        #region implement abstact method
        public override void Start()
        {
        }
        public override void Stop()
        {
        }
        public override void DetachEffect()
        {
            ReflectionShader = false;
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

        }
        #endregion

        #region method for list change event

        public void AddItem(FrameworkElement element)
        {
            InsertItem(LayoutRoot.Children.Count, element);
        }

        public void InsertItem(int index, FrameworkElement element)
        {
            element.Width = _ItemWidth;
            element.Height = _ItemHeight;
            element.Margin = _space;
            LayoutRoot.Children.Insert(index, element);
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
        }

        public void RemoveItemAt(int index)
        {
            LayoutRoot.Children.RemoveAt(index);
        }
        public void RemoveItem(FrameworkElement ui)
        {
            int index = LayoutRoot.Children.IndexOf(ui);
            RemoveItemAt(index);

        }
        public void RemoveAllItem()
        {
            LayoutRoot.Children.Clear();
        }
        #endregion

        WrapPanel LayoutRoot;
        ScrollViewer scrollView;

        public Matrix(BasicListControl control)
            : base(control)
        {
            parameterNameList.Add("ItemWidth");
            parameterNameList.Add("ItemHeight");
            parameterNameList.Add("BackgroundColor");
            parameterNameList.Add("ReflectionShader");
            parameterNameList.Add("ListOrientation");
            parameterNameList.Add("SpaceBetweenItem");
            LayoutRoot = new WrapPanel();

            scrollView = new ScrollViewer();
            scrollView.Content = LayoutRoot;

            control.Content = scrollView;

            //control.Content = LayoutRoot;
            _ListOrientation = Orientation.Horizontal;
            scrollView.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            scrollView.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            LayoutRoot.Background = new SolidColorBrush(Colors.Transparent);
            LayoutRoot.Orientation = _ListOrientation;

            _ItemWidth = _ItemHeight = 100;

            foreach (EffectableControl c in control.Items)
            {
                AddItem(c);
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
    }
}
