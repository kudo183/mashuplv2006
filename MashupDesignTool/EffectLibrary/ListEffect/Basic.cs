using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BasicLibrary;

namespace EffectLibrary
{
    public class Basic : BasicListEffect
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
            if (_ListOrientation == Orientation.Horizontal)
            {
                _space.Right = _SpaceBetweenItem;
                _space.Bottom = 0;
            }
            else
            {
                _space.Right = 0;
                _space.Bottom = _SpaceBetweenItem;
            }
            foreach (FrameworkElement element in LayoutRoot.Children)
                element.Margin = _space;
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
            control.OnListChange -= new BasicListControl.ListChangeHandler(control_OnListChange);
            ReflectionShader = false;
            LayoutRoot.Children.Clear();
            control.Container.Content = null;
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

        StackPanel LayoutRoot;
        public Basic(BasicListControl control)
            : base(control)
        {
            parameterNameList.Add("ItemWidth");
            parameterNameList.Add("ItemHeight");
            parameterNameList.Add("Background");
            parameterNameList.Add("ReflectionShader");
            parameterNameList.Add("ListOrientation");
            parameterNameList.Add("SpaceBetweenItem");
            LayoutRoot = new StackPanel();

            ScrollViewer scrollView = new ScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
            scrollView.Content = LayoutRoot;

            control.Container.Content = scrollView;
            control.OnListChange += new BasicListControl.ListChangeHandler(control_OnListChange);
            _ListOrientation = Orientation.Horizontal;
            LayoutRoot.Background = new SolidColorBrush(Colors.Transparent);
            LayoutRoot.Orientation = _ListOrientation;

            _ItemWidth = _ItemHeight = 100;

            foreach (EffectableControl c in control.Items)
            {
                AddItem(c);
            }
        }

        void control_OnListChange(BasicListControl.ListItemsAction action, int index1, EffectableControl control, int index2)
        {
            switch (action)
            {
                case BasicListControl.ListItemsAction.ADD:
                    AddItem(control);
                    break;
                case BasicListControl.ListItemsAction.INSERT:
                    InsertItem(index1, control);
                    break;
                case BasicListControl.ListItemsAction.SWAP:
                    Swap(index1, index2);
                    break;
                case BasicListControl.ListItemsAction.REMOVEAT:
                    RemoveItemAt(index1);
                    break;
                case BasicListControl.ListItemsAction.REMOVE:
                    RemoveItem(control);
                    break;
                case BasicListControl.ListItemsAction.REMOVEALL:
                    RemoveAllItem();
                    break;
            }
        }
    }
}
