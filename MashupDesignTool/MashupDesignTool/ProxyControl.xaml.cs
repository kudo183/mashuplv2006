using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BasicLibrary;

namespace MashupDesignTool
{
    public partial class ProxyControl : UserControl
    {
        private EffectableControl realControl;

        public ProxyControl()
        {
            InitializeComponent();
        }

        public ProxyControl(EffectableControl uc, double x, double y, double width, double height)
        {
            InitializeComponent();
            realControl = uc;
            MoveControl(x, y);
            ResizeControl(width, height);

            uc.SizeChanged += new SizeChangedEventHandler(uc_SizeChanged);
        }

        public ProxyControl(EffectableControl uc)
        {
            InitializeComponent();
            realControl = uc;
            double x = Canvas.GetLeft(realControl);
            double y = Canvas.GetTop(realControl);
            MoveControl(x, y);
            uc.SizeChanged += new SizeChangedEventHandler(uc_SizeChanged);
        }

        public ProxyControl(EffectableControl uc, double x, double y)
        {
            InitializeComponent();
            realControl = uc;
            MoveControl(x, y);
            uc.SizeChanged += new SizeChangedEventHandler(uc_SizeChanged);
        }

        void uc_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = e.NewSize.Width;
            double height = e.NewSize.Height;
            double x = Canvas.GetLeft(realControl);
            double y = Canvas.GetTop(realControl);

            realControl.Width = width;
            realControl.Height = height;

            this.SetValue(Canvas.LeftProperty, x - 7);
            this.SetValue(Canvas.TopProperty, y - 7);
            this.Width = width + 14;
            this.Height = height + 14;
        }

        public EffectableControl RealControl
        {
            get { return realControl; }
            set { realControl = value; }
        }

        public void ResizeControl(double width, double height)
        {
            SetWidth(width);
            SetHeight(height);
        }

        public void MoveControl(double x, double y)
        {
            SetX(x);
            SetY(y);
        }

        public void SetX(double x)
        {
            realControl.SetValue(Canvas.LeftProperty, x);
            this.SetValue(Canvas.LeftProperty, x - 7);
        }

        public void SetY(double y)
        {
            realControl.SetValue(Canvas.TopProperty, y);
            this.SetValue(Canvas.TopProperty, y - 7);
        }

        public void SetWidth(double width)
        {
            realControl.Width = width;
            this.Width = width + 14;
        }

        public void SetHeight(double height)
        {
            realControl.Height = height;
            this.Height = height + 14;
        }

        public void UpdateVisibility(Visibility vis)
        {
            controlBound.Visibility = vis;
            controlBoundBottomLeft.Visibility = vis;
            controlBoundBottomRight.Visibility = vis;
            controlBoundTopLeft.Visibility = vis;
            controlBoundTopRight.Visibility = vis;
            if (vis == System.Windows.Visibility.Collapsed)
                controlBound2.Visibility = System.Windows.Visibility.Visible;
            else
                controlBound2.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
