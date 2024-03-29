﻿using System;
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
    public class FishEye : BasicListEffect
    {
        List<Storyboard> lstStoryEnter = new List<Storyboard>();
        List<Storyboard> lstStoryLeave = new List<Storyboard>();
        List<SplineDoubleKeyFrame> lstSDKFEnterWidth = new List<SplineDoubleKeyFrame>();
        List<SplineDoubleKeyFrame> lstSDKFEnterHeight = new List<SplineDoubleKeyFrame>();
        List<SplineDoubleKeyFrame> lstSDKFLeaveWidth = new List<SplineDoubleKeyFrame>();
        List<SplineDoubleKeyFrame> lstSDKFLeaveHeight = new List<SplineDoubleKeyFrame>();

        Image _ImageLeft = new Image();
        Image _ImageCenter = new Image();
        Image _ImageRight = new Image();
        double _ItemWidth;
        double _ItemHeight;
        double _Scale;
        double _Range;
        double _SpaceBetweenItem;

        public double SpaceBetweenItem
        {
            get { return _SpaceBetweenItem; }
            set
            {
                _SpaceBetweenItem = value / 2;
                foreach (FrameworkElement element in LayoutRoot.Children)
                {
                    element.Margin = new Thickness(_SpaceBetweenItem, 0, _SpaceBetweenItem, 0);
                }
            }
        }
        public enum VerticalAlignment
        {
            Top,
            Center,
            Bottom
        }

        VerticalAlignment _ItemVerticalAlignment;

        public VerticalAlignment ItemVerticalAlignment
        {
            get { return _ItemVerticalAlignment; }
            set
            {
                _ItemVerticalAlignment = value;
                System.Windows.VerticalAlignment align;
                switch (value)
                {
                    case VerticalAlignment.Top:
                        align = System.Windows.VerticalAlignment.Top;
                        break;
                    case VerticalAlignment.Center:
                        align = System.Windows.VerticalAlignment.Center;
                        break;
                    case VerticalAlignment.Bottom:
                        align = System.Windows.VerticalAlignment.Bottom;
                        break;
                    default:
                        align = System.Windows.VerticalAlignment.Center;
                        break;
                }
                LayoutRoot.VerticalAlignment = align;
                foreach (FrameworkElement element in LayoutRoot.Children)
                {
                    element.VerticalAlignment = LayoutRoot.VerticalAlignment;
                }
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
                foreach (SplineDoubleKeyFrame sdkf in lstSDKFLeaveWidth)
                {
                    sdkf.Value = _ItemWidth;
                }
                foreach (SplineDoubleKeyFrame sdkf in lstSDKFEnterWidth)
                {
                    sdkf.Value = _ItemWidth;
                }
                control.UpdateLayout();
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
                foreach (SplineDoubleKeyFrame sdkf in lstSDKFLeaveHeight)
                {
                    sdkf.Value = _ItemHeight;
                }
                foreach (SplineDoubleKeyFrame sdkf in lstSDKFEnterHeight)
                {
                    sdkf.Value = _ItemHeight;
                }

                ReflectionShader = _ReflectionShader;
                if (_ReflectionShader == true)
                {
                    foreach (FrameworkElement element in LayoutRoot.Children)
                        element.Effect = new EffectLibrary.CustomPixelShader.ReflectionShader() { ElementHeight = _ItemHeight };
                }
                control.UpdateLayout();
            }
        }

        public double Scale
        {
            get { return _Scale; }
            set { _Scale = value; }
        }

        public double Range
        {
            get { return _Range; }
            set { _Range = value; }
        }


        private Brush _Background;

        public Brush Background
        {
            get { return b.Background; }
            set
            {
                _Background = value;
                b.Background = _Background;
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
            foreach (FrameworkElement element in LayoutRoot.Children)
            {
                element.VerticalAlignment = origin;
                element.Margin = originSpace;
            }
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
            if (_isSelfHandle == true)
            {
                control.MouseEnter += new MouseEventHandler(MenuMacOS_MouseEnter);
                control.MouseMove += new MouseEventHandler(MenuMacOS_MouseEnter);
                control.MouseLeave += new MouseEventHandler(MenuMacOS_MouseLeave);
                control.OnListChange += new BasicListControl.ListChangeHandler(control_OnListChange);
            }
            else
            {
                control.MouseEnter -= new MouseEventHandler(MenuMacOS_MouseEnter);
                control.MouseMove -= new MouseEventHandler(MenuMacOS_MouseEnter);
                control.MouseLeave -= new MouseEventHandler(MenuMacOS_MouseLeave);
                control.OnListChange -= new BasicListControl.ListChangeHandler(control_OnListChange);
            }
        }
        #endregion

        #region method for list change event

        public void AddItem(FrameworkElement element)
        {
            InsertItem(LayoutRoot.Children.Count, element);
        }

        System.Windows.VerticalAlignment origin;
        Thickness originSpace;

        public void InsertItem(int index, FrameworkElement element)
        {
            element.Width = _ItemWidth;
            element.Height = _ItemHeight;
            origin = element.VerticalAlignment;
            originSpace = element.Margin;
            element.Margin = new Thickness(_SpaceBetweenItem, 0, _SpaceBetweenItem, 0);
            element.VerticalAlignment = LayoutRoot.VerticalAlignment;
            if(_ReflectionShader == true)
                element.Effect = new EffectLibrary.CustomPixelShader.ReflectionShader() { ElementHeight = _ItemHeight };
            LayoutRoot.Children.Insert(index, element);

            Storyboard sb;
            DoubleAnimationUsingKeyFrames dakf;
            SplineDoubleKeyFrame sdkf;

            //create storyboard for mouse leave animation
            sb = new Storyboard();
            dakf = new DoubleAnimationUsingKeyFrames();
            dakf.BeginTime = TimeSpan.FromSeconds(0);
            sdkf = new SplineDoubleKeyFrame();
            sdkf.KeyTime = TimeSpan.FromSeconds(0.08);
            sdkf.Value = _ItemWidth;
            dakf.KeyFrames.Add(sdkf);
            lstSDKFLeaveWidth.Add(sdkf);
            Storyboard.SetTarget(dakf, element);
            Storyboard.SetTargetProperty(dakf, new PropertyPath("(FrameworkElement.Width)"));
            sb.Children.Add(dakf);

            dakf = new DoubleAnimationUsingKeyFrames();
            dakf.BeginTime = TimeSpan.FromSeconds(0);
            sdkf = new SplineDoubleKeyFrame();
            sdkf.KeyTime = TimeSpan.FromSeconds(0.08);
            sdkf.Value = _ItemHeight;
            dakf.KeyFrames.Add(sdkf);
            lstSDKFLeaveHeight.Add(sdkf);
            Storyboard.SetTarget(dakf, element);
            Storyboard.SetTargetProperty(dakf, new PropertyPath("(FrameworkElement.Height)"));
            sb.Children.Add(dakf);

            lstStoryLeave.Insert(index, sb);

            //create storyboard for mouse enter animation
            sb = new Storyboard();
            dakf = new DoubleAnimationUsingKeyFrames();
            dakf.BeginTime = TimeSpan.FromSeconds(0);
            sdkf = new SplineDoubleKeyFrame();
            sdkf.KeyTime = TimeSpan.FromSeconds(0.08);
            sdkf.Value = _ItemWidth;
            dakf.KeyFrames.Add(sdkf);
            lstSDKFEnterWidth.Add(sdkf);
            Storyboard.SetTarget(dakf, element);
            Storyboard.SetTargetProperty(dakf, new PropertyPath("(FrameworkElement.Width)"));
            sb.Children.Add(dakf);

            dakf = new DoubleAnimationUsingKeyFrames();
            dakf.BeginTime = TimeSpan.FromSeconds(0);
            sdkf = new SplineDoubleKeyFrame();
            sdkf.KeyTime = TimeSpan.FromSeconds(0.08);
            sdkf.Value = _ItemHeight;
            dakf.KeyFrames.Add(sdkf);
            lstSDKFEnterHeight.Add(sdkf);
            Storyboard.SetTarget(dakf, element);
            Storyboard.SetTargetProperty(dakf, new PropertyPath("(FrameworkElement.Height)"));
            sb.Children.Add(dakf);

            lstStoryEnter.Insert(index, sb);
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

            SplineDoubleKeyFrame temp3 = lstSDKFEnterWidth[min];
            lstSDKFEnterWidth[min] = lstSDKFEnterWidth[max];
            lstSDKFEnterWidth[max] = temp3;

            SplineDoubleKeyFrame temp4 = lstSDKFEnterHeight[min];
            lstSDKFEnterHeight[min] = lstSDKFEnterHeight[max];
            lstSDKFEnterHeight[max] = temp4;

            Storyboard temp5 = lstStoryEnter[min];
            lstStoryEnter[min] = lstStoryEnter[max];
            lstStoryEnter[max] = temp5;

            Storyboard temp6 = lstStoryLeave[min];
            lstStoryLeave[min] = lstStoryLeave[max];
            lstStoryLeave[max] = temp6;
        }

        public void RemoveItemAt(int index)
        {
            LayoutRoot.Children.RemoveAt(index);
            lstSDKFEnterHeight.RemoveAt(index);
            lstSDKFEnterWidth.RemoveAt(index);
            lstStoryEnter.RemoveAt(index);
            lstStoryLeave.RemoveAt(index);
        }
        public void RemoveItem(FrameworkElement ui)
        {
            int index = LayoutRoot.Children.IndexOf(ui);
            RemoveItemAt(index);

        }
        public void RemoveAllItem()
        {
            LayoutRoot.Children.Clear();
            lstSDKFEnterHeight.Clear();
            lstSDKFEnterWidth.Clear();
            lstStoryEnter.Clear();
            lstStoryLeave.Clear();
        }
        #endregion


        StackPanel LayoutRoot;
        Border b;
        public FishEye(BasicListControl control)
            : base(control)
        {
            parameterNameList.Add("ItemWidth");
            parameterNameList.Add("ItemHeight");
            parameterNameList.Add("Scale");
            parameterNameList.Add("Range");
            parameterNameList.Add("Background");
            parameterNameList.Add("ReflectionShader");
            parameterNameList.Add("ItemVerticalAlignment");
            parameterNameList.Add("SpaceBetweenItem");

            LayoutRoot = new StackPanel();
            //b.Children.Add(LayoutRoot);
            b = new Border() { Child = LayoutRoot };            
            control.Container.Content = b;

            Background = new SolidColorBrush(Colors.White);
            LayoutRoot.Background = new SolidColorBrush(Colors.Transparent);
            LayoutRoot.Orientation = Orientation.Horizontal;
            LayoutRoot.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            ItemVerticalAlignment = VerticalAlignment.Center;

            _ItemWidth = _ItemHeight = 100;
            _Range = 500;
            _Scale = 88;
            foreach (EffectableControl c in control.Items)
            {
                AddItem(c);
            }

            b.SizeChanged += new SizeChangedEventHandler(LayoutRoot_SizeChanged);
        }

        void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            b.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height) };
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

        private void MenuMacOS_MouseEnter(object sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition((FrameworkElement)sender);
            double rangeMin = mousePosition.X - _Range / 2;
            double rangeMax = mousePosition.X + _Range / 2;

            FrameworkElement thisElement = sender as FrameworkElement;

            for (int i = 0; i < LayoutRoot.Children.Count; i++)
            {
                GeneralTransform gt = LayoutRoot.Children[i].TransformToVisual(thisElement);
                Point offset = gt.Transform(new Point(0, 0));

                offset.X = offset.X + _ItemWidth / 2;
                offset.Y = offset.Y + _ItemHeight / 2;

                if (offset.X > rangeMin && offset.X < rangeMax)
                {
                    lstSDKFEnterWidth[i].Value = _ItemWidth + _Scale * (Math.Sin(Math.PI * ((offset.X - rangeMin) / _Range)));
                    lstSDKFEnterHeight[i].Value = _ItemHeight + _Scale * (Math.Sin(Math.PI * ((offset.X - rangeMin) / _Range)));
                    lstStoryEnter[i].Begin();
                }
                else
                {
                    lstSDKFEnterWidth[i].Value = _ItemWidth;
                    lstSDKFEnterHeight[i].Value = _ItemHeight;
                    lstStoryEnter[i].Begin();
                }
            }
        }

        private void MenuMacOS_MouseLeave(object sender, MouseEventArgs e)
        {
            foreach (Storyboard sb in lstStoryLeave)
                sb.Begin();
        }
    }
}
