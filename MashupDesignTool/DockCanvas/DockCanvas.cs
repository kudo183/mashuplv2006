using System;
using System.Net;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DockCanvas
{
    public class DockCanvas : Canvas
    {
        public enum DockType
        {
            None,
            Left,
            Top,
            Right,
            Bottom,
            Fill
        }

        private bool preventUpdateChildrenPosition;

        public bool PreventUpdateChildrenPosition
        {
            get { return preventUpdateChildrenPosition; }
            set { preventUpdateChildrenPosition = value; }
        }
        
        private List<int> lstZIndex = new List<int>();
        private List<int> lstArrayIndex = new List<int>();
        private List<Rect> lstControlOrginRect = new List<Rect>();
        
        #region DockTypeProperty
        public static readonly DependencyProperty DockTypeProperty =
            DependencyProperty.RegisterAttached("DockType", typeof(DockType), typeof(DockCanvas), new PropertyMetadata(DockType.None, OnDockTypeChanged));
        private static void OnDockTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {            
            DockCanvas dc = VisualTreeHelper.GetParent(d) as DockCanvas;
            if (dc != null)
                dc.OnDockTypeChanged((FrameworkElement)d, (DockType)e.OldValue, (DockType)e.NewValue);
        }
        
        private void OnDockTypeChanged(FrameworkElement element, DockType oldValue, DockType newValue)
        {
            if (newValue == DockType.None)
            {
                element.Width = 50;
                element.Height = 50;
            }
            if (preventUpdateChildrenPosition == true)
                return;            
            UpdateChildrenPosition();
        }

        public static void SetDockType(FrameworkElement element, DockType dockType)
        {
            element.SetValue(DockTypeProperty, dockType);
        }
        
        public static DockType GetDockType(FrameworkElement element)
        {
            return (DockType)element.GetValue(DockTypeProperty);
        }
        #endregion ZIndexTypeProperty

        #region ZIndexProperty
        //public static new void SetZIndex(UIElement element, int index)
        //{
        //    Canvas.SetZIndex(element, index);
        //}

        //public static new int GetZIndex(UIElement element)
        //{
        //    return (int)Canvas.GetZIndex(element);
        //}

        public static readonly DependencyProperty ZIndexProperty =
            DependencyProperty.RegisterAttached("ZIndex", typeof(int), typeof(DockCanvas), new PropertyMetadata(0, OnZIndexChanged));

        public static new void SetZIndex(UIElement element, int index)
        {
            Canvas.SetZIndex(element, index);
            element.SetValue(ZIndexProperty, index);
        }

        public static new int GetZIndex(UIElement element)
        {
            return (int)element.GetValue(ZIndexProperty);
        }
        private static void OnZIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockCanvas dc = VisualTreeHelper.GetParent(d) as DockCanvas;
            if (dc != null)
                dc.OnZIndexChanged((FrameworkElement)d, (int)e.OldValue, (int)e.NewValue);
        }
        private void OnZIndexChanged(FrameworkElement element, int oldValue, int newValue)
        {
            if (preventUpdateChildrenPosition == true)
                return;
            UpdateChildrenPosition();
        }
        #endregion
        
        private void UpdateIndex()
        {
            lstArrayIndex.Clear();
            lstZIndex.Clear();
            int i = 0;
            foreach (UIElement element in this.Children)
            {
                lstArrayIndex.Add(i);
                lstZIndex.Add(GetZIndex(element));
                i++;
            }
        }

        private void SortASCByZIndex()
        {
            for (int i = 0; i < lstZIndex.Count - 1; i++)
            {
                for (int j = i + 1; j < lstZIndex.Count; j++)
                {
                    if (lstZIndex[i] > lstZIndex[j])
                    {
                        int temp = lstZIndex[i];
                        lstZIndex[i] = lstZIndex[j];
                        lstZIndex[j] = temp;
                        temp = lstArrayIndex[i];
                        lstArrayIndex[i] = lstArrayIndex[j];
                        lstArrayIndex[j] = temp;
                    }
                }
            }
        }

        public void UpdateChildrenPosition()
        {
            UpdateIndex();
            SortASCByZIndex();

            double width = this.ActualWidth;
            double height = this.ActualHeight;

            if (width == 0 && !double.IsNaN(this.Width))
            {
                width = this.Width;
                height = this.Height;
            }

            Rect remainRect = new Rect(0, 0, width, height);
            
            for (int i = 0; i < lstZIndex.Count; i++)
            {
                FrameworkElement element = this.Children[lstArrayIndex[i]] as FrameworkElement;
                DockType dockType = DockCanvas.GetDockType(element);
                switch (dockType)
                {
                    case DockType.None:                       
                        break;
                    case DockType.Left:
                        Canvas.SetLeft(element, remainRect.Left);
                        Canvas.SetTop(element, remainRect.Top);
                        element.Height = remainRect.Height;

                        remainRect.X += element.Width;
                        remainRect.Width -= element.Width;                                               
                        break;
                    case DockType.Top:
                        Canvas.SetTop(element, remainRect.Top);
                        Canvas.SetLeft(element, remainRect.Left);
                        element.Width = remainRect.Width;

                        remainRect.Y += element.Height;
                        remainRect.Height -= element.Height;
                        break;
                    case DockType.Right:
                        Canvas.SetLeft(element, remainRect.Right - element.Width);
                        Canvas.SetTop(element, remainRect.Top);
                        element.Height = remainRect.Height;
                        
                        remainRect.Width -= element.Width;
                        break;
                    case DockType.Bottom:
                        Canvas.SetTop(element, remainRect.Bottom - element.Height);
                        Canvas.SetLeft(element, remainRect.Left);
                        element.Width = remainRect.Width;

                        remainRect.Height -= element.Height;                        
                        break;
                    case DockType.Fill:
                        Canvas.SetLeft(element, 0);
                        Canvas.SetTop(element, 0);
                        element.Width = remainRect.Width;
                        element.Height = remainRect.Height;
                        break;
                }
            }

            UpdateLayout();
            preventUpdateChildrenPosition = false;
        }
        
        //public DockCanvas()
        //    : base()
        //{
        //    this.LayoutUpdated += new EventHandler(DockCanvas_LayoutUpdated);
        //}

        //void DockCanvas_LayoutUpdated(object sender, EventArgs e)
        //{
        //    UpdateChildrenPosition();            
        //}
    }
}
