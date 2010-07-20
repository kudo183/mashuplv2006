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
using System.ComponentModel;
namespace CustomListEffect
{
    public class AccordionEffect : BasicListEffect
    {
        double _ItemWidth;
        double _ItemHeight;
        ExpandDirection _ExpandDirection;

        Brush _HeaderBackground;
        //Brush _ExpandedBackground;
        //Brush _MouseOverBackground;
        Brush _TextColor;
        double _FontSize;
        FontFamily _Font;

        [Category("Header")]
        public FontFamily Font
        {
            get { return _Font; }
            set
            {
                _Font = value;
                LayoutRoot.FontFamily = _Font;
            }
        }

        [Category("Header")]
        public double FontSize
        {
            get { return _FontSize; }
            set
            {
                _FontSize = value;
                LayoutRoot.FontSize = _FontSize;
                LayoutRoot.OnApplyTemplate();
            }
        }

        [Category("Header")]
        public Brush TextColor
        {
            get { return _TextColor; }
            set
            {
                _TextColor = value;
                LayoutRoot.Foreground = _TextColor;
            }
        }

        [Category("Header")]
        public Brush HeaderBackground
        {
            get { return _HeaderBackground; }
            set
            {
                _HeaderBackground = value;
                main.MainGrid.Background = value;
            }
        }

        //[Category("Header")]
        //public Brush ExpandedBackground
        //{
        //    get { return _ExpandedBackground; }
        //    set
        //    {
        //        _ExpandedBackground = value;
        //        main.Expanded.Background = value;
        //    }
        //}

        //[Category("Header")]
        //public Brush MouseOverBackground
        //{
        //    get { return _MouseOverBackground; }
        //    set
        //    {
        //        _MouseOverBackground = value;
        //        main.MouseOver.Background = value;
        //    }
        //}
        public ExpandDirection ExpandDirection
        {
            get { return _ExpandDirection; }
            set
            {
                _ExpandDirection = value;
                LayoutRoot.ExpandDirection = _ExpandDirection;
            }
        }

        public double ItemWidth
        {
            get { return _ItemWidth; }
            set
            {
                _ItemWidth = value;

                //foreach (FrameworkElement element in LayoutRoot.Items)
                //    element.Height = _ItemWidth;

                foreach (AccordionItem item in LayoutRoot.Items)
                {
                    FrameworkElement element = item.Content as FrameworkElement;
                    element.Width = _ItemWidth;
                }
            }
        }

        public double ItemHeight
        {
            get { return _ItemHeight; }
            set
            {
                _ItemHeight = value;
                //foreach (FrameworkElement element in LayoutRoot.Items)
                //    element.Height = _ItemHeight;
                foreach (AccordionItem item in LayoutRoot.Items)
                {
                    FrameworkElement element = item.Content as FrameworkElement;
                    element.Height = _ItemHeight;
                }
            }
        }

        //private Brush _BackgroundColor;

        //public Brush BackgroundColor
        //{
        //    get { return LayoutRoot.Background; }
        //    set
        //    {
        //        _BackgroundColor = value;
        //        LayoutRoot.Background = _BackgroundColor;
        //    }
        //}

        #region implement abstact method
        public override void Start()
        {
        }
        public override void Stop()
        {
        }
        public override void DetachEffect()
        {
            RemoveAllItem();
            control.Content = null;
            IsSelfHandle = false;
            control.OnListChange -= new BasicListControl.ListChangeHandler(control_OnListChange);
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
            InsertItem(LayoutRoot.Items.Count, element);
        }

        public void InsertItem(int index, FrameworkElement element)
        {
            element.Width = _ItemWidth;
            element.Height = _ItemHeight;

            element.HorizontalAlignment = HorizontalAlignment.Stretch;
            element.VerticalAlignment = VerticalAlignment.Stretch;
            AccordionItem item = new AccordionItem();
            //Border b = new Border() { Background = _HeaderBackground };
            //b.Child = new TextBlock() { Text = "hgfjhgfh" };
            //item.Header = b;
            item.Header = " ";
            EffectableControl ec = element as EffectableControl;
            if (ec != null && ec.Control != null)
            {
                System.Reflection.PropertyInfo p = ec.Control.GetType().GetProperty("Title");

                if (p != null)
                {
                    string s = p.GetValue(ec.Control, null).ToString();
                    if (string.IsNullOrEmpty(s))
                        s = " ";
                    item.Header = s;
                }
            }
            item.Content = element;
            LayoutRoot.Items.Insert(index, item);
        }

        public void Swap(int index1, int index2)
        {
            int min = Math.Min(index1, index2);
            int max = Math.Max(index1, index2);

            UIElement temp1 = LayoutRoot.Items[min] as UIElement;
            UIElement temp2 = LayoutRoot.Items[max] as UIElement;

            LayoutRoot.Items.RemoveAt(max);
            LayoutRoot.Items[min] = temp2;
            LayoutRoot.Items.Insert(max, temp1);
        }

        public void RemoveItemAt(int index)
        {
            AccordionItem item = LayoutRoot.Items[index] as AccordionItem;
            item.Content = null;
            LayoutRoot.Items.RemoveAt(index);
        }
        public void RemoveItem(FrameworkElement ui)
        {
            int index = LayoutRoot.Items.IndexOf(ui);
            RemoveItemAt(index);
        }
        public void RemoveAllItem()
        {
            foreach (AccordionItem item in LayoutRoot.Items)
                item.Content = null;
            LayoutRoot.Items.Clear();
        }
        #endregion

        Accordion LayoutRoot;
        SilverlightControl1 main;
        public AccordionEffect(BasicListControl control)
            : base(control)
        {
            parameterNameList.Add("ItemWidth");
            parameterNameList.Add("ItemHeight");
            parameterNameList.Add("HeaderBackground");
            //parameterNameList.Add("ExpandedBackground");
            //parameterNameList.Add("MouseOverBackground");
            parameterNameList.Add("ExpandDirection");
            parameterNameList.Add("Font");
            parameterNameList.Add("FontSize");
            parameterNameList.Add("TextColor");

            main = new SilverlightControl1();
            main.HorizontalAlignment = HorizontalAlignment.Stretch;
            main.VerticalAlignment = VerticalAlignment.Stretch;
            LayoutRoot = main.LayoutRoot;

            _Font = LayoutRoot.FontFamily;
            _FontSize = LayoutRoot.FontSize;
            _TextColor = LayoutRoot.Foreground;

            control.Content = main;
            control.OnListChange+=new BasicListControl.ListChangeHandler(control_OnListChange);
           
            ExpandDirection = ExpandDirection.Right;

            LayoutRoot.Background = new SolidColorBrush(Colors.Transparent);
            LayoutRoot.ExpandDirection = _ExpandDirection;
            LayoutRoot.HorizontalAlignment = HorizontalAlignment.Stretch;
            LayoutRoot.VerticalAlignment = VerticalAlignment.Stretch;

            _ItemWidth = _ItemHeight = 100;
            _HeaderBackground = new SolidColorBrush(Colors.Green);
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
