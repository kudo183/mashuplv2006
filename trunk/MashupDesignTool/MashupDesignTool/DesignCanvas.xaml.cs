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
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Xml;
using SharpGIS.MouseExtensions;

namespace MashupDesignTool
{
    public partial class DesignCanvas : UserControl
    {
        #region enum resize direction
        enum ResizeDirection
        {
            TOP,
            LEFT,
            RIGHT,
            BOTTOM,
            TOPLEFT,
            TOPRIGHT,
            BOTTOMLEFT,
            BOTTOMRIGHT
        }
        #endregion enum resize direction

        UserControl selectedControl;
        ProxyControl selectedProxyControl;
        ResizeDirection resizeDirection;
        Point clickPoint;
        Point beginResizedPoint;
        bool isControlCaptured;
        bool isResizing;
        bool canResize;
        bool isShowingContextMenu;
        List<UserControl> controls;
        List<ProxyControl> proxyControls;

        public List<UserControl> Controls
        {
            get { return controls; }
            set { controls = value; }
        }

        public DesignCanvas()
        {
            InitializeComponent();

            selectedControl = null;
            isResizing = false;
            canResize = false;
            isShowingContextMenu = false;
            
            CursorManager.InitCursor(LayoutRoot);

            proxyControls = new List<ProxyControl>();
            controls = new List<UserControl>();

            this.AttachRightClick(OnRightClick);
        }

        public void AddControl(UserControl uc, double x, double y, int width, int height)
        {
            uc.Margin = new Thickness(0, 0, 0, 0);
            ProxyControl pc = new ProxyControl(uc, x, y, width, height);
            proxyControls.Add(pc);
            controls.Add(uc);
            pc.UpdateVisibility(System.Windows.Visibility.Collapsed);

            LayoutRoot.Children.Add(uc);
            LayoutRoot.Children.Add(pc);

            pc.MouseLeftButtonDown += new MouseButtonEventHandler(control_MouseLeftButtonDown);
            pc.MouseMove += new MouseEventHandler(control_MouseMove);
            pc.MouseLeftButtonUp += new MouseButtonEventHandler(control_MouseLeftButtonUp);
        }

        #region dragdrop control
        void control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ProxyControl pc = (ProxyControl)sender;
            pc.ReleaseMouseCapture();
            isControlCaptured = false;
        }

        void control_MouseMove(object sender, MouseEventArgs e)
        {
            if (isControlCaptured)
            {
                Point pt = e.GetPosition(LayoutRoot);
                double x = pt.X - clickPoint.X;
                double y = pt.Y - clickPoint.Y;
                ProxyControl pc = (ProxyControl)sender;
                UserControl uc = pc.RealControl;

                if (x < 0 || y < 0 || x + uc.Width >= this.ActualWidth || y + uc.Height >= this.ActualHeight)
                {
                    if (x < 0)
                        x = 0;
                    else if (x + uc.Width >= this.ActualWidth)
                        x = this.ActualWidth - uc.Width - 1;

                    if (y < 0)
                        y = 0;
                    else if (y + uc.Height >= this.ActualHeight)
                        y = this.ActualHeight - uc.Height - 1;
                }
                pc.MoveControl(x, y);
            }
        }

        void control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!canResize)
            {
                if (selectedProxyControl != null)
                    selectedProxyControl.UpdateVisibility(System.Windows.Visibility.Collapsed);

                ProxyControl pc = (ProxyControl)sender;
                clickPoint = e.GetPosition(pc.RealControl);
                pc.CaptureMouse();
                isControlCaptured = true;

                pc.UpdateVisibility(System.Windows.Visibility.Visible);
                selectedControl = pc.RealControl;
                selectedProxyControl = pc;
            }

            if (isShowingContextMenu)
            {
                HideContextMenu();
            }
        }
        #endregion dragdrop control

        #region resize control
        private void LayoutRoot_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedControl != null)
            {
                if (!isResizing)
                {
                    CheckCanResize(e);
                }
                else
                {
                    OnResizing(e);
                }
            }
        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (canResize)
            {
                isResizing = true;
            }
            else if (!isControlCaptured)
            {
                selectedControl = null;
                if (selectedProxyControl != null)
                    selectedProxyControl.UpdateVisibility(System.Windows.Visibility.Collapsed);
            }
            
            if (isShowingContextMenu)
            {
                HideContextMenu();
            }
        }

        private void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isResizing)
            {
                Point pt = e.GetPosition(LayoutRoot);
                ResizeControl(pt);
                isResizing = false;
            }
        }

        private void CheckCanResize(MouseEventArgs e)
        {
            Point pt = e.GetPosition(selectedControl);
            canResize = false;

            #region marked mouse position 
            int hDirection = 0;
            int vDirection = 0;

            if (pt.X >= -8 && pt.X <= 0)
                hDirection = 1;
            else if (pt.X > 0 && pt.X <= 4)
                hDirection = 2;
            else if (pt.X > 4 && pt.X < selectedControl.Width - 4)
                hDirection = 3;
            else if (pt.X >= selectedControl.Width - 4 && pt.X < selectedControl.Width)
                hDirection = 4;
            else if (pt.X >= selectedControl.Width && pt.X <= selectedControl.Width + 8)
                hDirection = 5;

            if (pt.Y >= -8 && pt.Y <= 0)
                vDirection = 1;
            else if (pt.Y > 0 && pt.Y <= 4)
                vDirection = 2;
            else if (pt.Y > 4 && pt.Y < selectedControl.Height - 4)
                vDirection = 3;
            else if (pt.Y >= selectedControl.Height - 4 && pt.Y <= selectedControl.Height)
                vDirection = 4;
            else if (pt.Y >= selectedControl.Height && pt.Y <= selectedControl.Height + 8)
                vDirection = 5;
            #endregion marked mouse position

            #region set cursor, begin resized point, resize direction
            beginResizedPoint.X = beginResizedPoint.Y = 0;
            if (hDirection == 1)
            {
                beginResizedPoint.X = pt.X;
                if (vDirection == 1 || vDirection == 2)
                {
                    resizeDirection = ResizeDirection.TOPLEFT;
                    canResize = true;
                    beginResizedPoint.Y = pt.Y;
                    CursorManager.ChangeCursor(this, CursorManager.CursorType.SizeWNES);
                }
                else if (vDirection == 3)
                {
                    resizeDirection = ResizeDirection.LEFT;
                    canResize = true;
                    CursorManager.ChangeCursor(this, CursorManager.CursorType.SizeWE);
                }
                else if (vDirection == 4 || vDirection == 5)
                {
                    resizeDirection = ResizeDirection.BOTTOMLEFT;
                    canResize = true;
                    beginResizedPoint.Y = pt.Y - selectedControl.Height;
                    CursorManager.ChangeCursor(this, CursorManager.CursorType.SizeENWS);
                }
            }
            else if (hDirection == 2)
            {
                beginResizedPoint.X = pt.X;
                if (vDirection == 1)
                {
                    resizeDirection = ResizeDirection.TOPLEFT;
                    canResize = true;
                    beginResizedPoint.Y = pt.Y;
                    CursorManager.ChangeCursor(this, CursorManager.CursorType.SizeWNES);
                }
                else if (vDirection == 5)
                {
                    resizeDirection = ResizeDirection.BOTTOMLEFT;
                    canResize = true;
                    beginResizedPoint.Y = pt.Y - selectedControl.Height;
                    CursorManager.ChangeCursor(this, CursorManager.CursorType.SizeENWS);
                }
            }
            else if (hDirection == 3)
            {
                if (vDirection == 1)
                {
                    resizeDirection = ResizeDirection.TOP;
                    canResize = true;
                    beginResizedPoint.Y = pt.Y;
                    CursorManager.ChangeCursor(this, CursorManager.CursorType.SizeNS);
                }
                else if (vDirection == 5)
                {
                    this.Cursor = Cursors.SizeNS;
                    resizeDirection = ResizeDirection.BOTTOM;
                    canResize = true;
                    beginResizedPoint.Y = pt.Y - selectedControl.Height;
                    CursorManager.ChangeCursor(this, CursorManager.CursorType.SizeNS);
                }
            }
            else if (hDirection == 4)
            {
                beginResizedPoint.X = pt.X - selectedControl.Width;
                if (vDirection == 1)
                {
                    resizeDirection = ResizeDirection.TOPRIGHT;
                    canResize = true;
                    beginResizedPoint.Y = pt.Y;
                    CursorManager.ChangeCursor(this, CursorManager.CursorType.SizeENWS);
                }
                else if (vDirection == 5)
                {
                    resizeDirection = ResizeDirection.BOTTOMRIGHT;
                    canResize = true;
                    beginResizedPoint.Y = pt.Y - selectedControl.Height;
                    CursorManager.ChangeCursor(this, CursorManager.CursorType.SizeWNES);
                }
            }
            else if (hDirection == 5)
            {
                beginResizedPoint.X = pt.X - selectedControl.Width;
                if (vDirection == 1 || vDirection == 2)
                {
                    resizeDirection = ResizeDirection.TOPRIGHT;
                    canResize = true;
                    beginResizedPoint.Y = pt.Y;
                    CursorManager.ChangeCursor(this, CursorManager.CursorType.SizeENWS);
                }
                else if (vDirection == 3)
                {
                    this.Cursor = Cursors.SizeWE;
                    resizeDirection = ResizeDirection.RIGHT;
                    canResize = true;
                    CursorManager.ChangeCursor(this, CursorManager.CursorType.SizeWE);
                }
                else if (vDirection == 4 || vDirection == 5)
                {
                    resizeDirection = ResizeDirection.BOTTOMRIGHT;
                    canResize = true;
                    beginResizedPoint.Y = pt.Y - selectedControl.Height;
                    CursorManager.ChangeCursor(this, CursorManager.CursorType.SizeWNES);
                }
            }
            #endregion set cursor, begin resized point, resize direction

            if (!canResize)
                CursorManager.ChangeCursor(this, CursorManager.CursorType.Arrow);

            CursorManager.UpdateCursorPosition(e.GetPosition(LayoutRoot));
        }

        private void OnResizing(MouseEventArgs e)
        {
            Point pt = e.GetPosition(LayoutRoot);
            bool b = false;
            if (pt.X < beginResizedPoint.X)
            {
                b = true;
                pt.X = 0;
            }
            else if (pt.X >= this.ActualWidth + beginResizedPoint.X)
            {
                b = true;
                pt.X = this.ActualWidth - 1;
            }
            if (pt.Y < beginResizedPoint.Y)
            {
                b = true;
                pt.Y = 0;
            }
            else if (pt.Y >= this.ActualHeight + beginResizedPoint.Y)
            {
                b = true;
                pt.Y = this.ActualHeight - 1;
            }

            ResizeControl(pt);

            if (b == true)
            {
                isResizing = false;
                canResize = false;
                CursorManager.ChangeCursor(this, CursorManager.CursorType.Arrow);
            }
        }

        private void ResizeControl(Point pt)
        {
            double x, y;
            double width, height;

            x = (double)selectedControl.GetValue(Canvas.LeftProperty);
            y = (double)selectedControl.GetValue(Canvas.TopProperty);
            width = selectedControl.Width;
            height = selectedControl.Height;

            #region calculate new position, dimension
            if (resizeDirection == ResizeDirection.TOP || resizeDirection == ResizeDirection.TOPLEFT || resizeDirection == ResizeDirection.TOPRIGHT)
            {
                double h = height;
                height = height - (pt.Y - y - beginResizedPoint.Y);
                if (height < 0)
                {
                    height = 0;
                    y = y + h;
                }
                else
                    y = pt.Y - beginResizedPoint.Y;
            }
            if (resizeDirection == ResizeDirection.LEFT || resizeDirection == ResizeDirection.TOPLEFT || resizeDirection == ResizeDirection.BOTTOMLEFT)
            {
                double w = width;
                width = width - (pt.X - x - beginResizedPoint.X);
                if (width < 0)
                {
                    width = 0;
                    x = x + w;
                }
                else
                    x = pt.X - beginResizedPoint.X;
            }
            if (resizeDirection == ResizeDirection.BOTTOM || resizeDirection == ResizeDirection.BOTTOMLEFT || resizeDirection == ResizeDirection.BOTTOMRIGHT)
            {
                height = pt.Y - beginResizedPoint.Y - y;
                height = height < 0 ? 0 : height;
            }
            if (resizeDirection == ResizeDirection.RIGHT || resizeDirection == ResizeDirection.TOPRIGHT || resizeDirection == ResizeDirection.BOTTOMRIGHT)
            {
                width = pt.X - beginResizedPoint.X - x;
                width = width < 0 ? 0 : width;
            }
            #endregion calculate new position, dimension

            selectedProxyControl.ResizeControl(width, height);
            selectedProxyControl.MoveControl(x, y);
            CursorManager.UpdateCursorPosition(pt);
        }
        #endregion resize control

        private void LayoutRoot_MouseLeave(object sender, MouseEventArgs e)
        {
            isResizing = false;
            CursorManager.ChangeCursor(this, CursorManager.CursorType.Arrow);
        }

        #region contextmenu
        private void OnRightClick(object sender, SharpGIS.MouseExtensions.RightMouseButtonEventArgs args)
        {
            PositionContextMenu(args.GetPosition(contextMenu.Parent as UIElement), true);
            args.Handled = true;
            gridContextMenu.Visibility = System.Windows.Visibility.Visible;
        }

        private void PositionContextMenu(Point p, bool useTransition)
        {
            if (useTransition)
                contextMenu.IsOpen = false;
            contextMenu.HorizontalOffset = p.X;
            contextMenu.VerticalOffset = p.Y;
            contextMenu.IsOpen = true;
        }

        private void contextMenu_Opened(object sender, EventArgs e)
        {
            isShowingContextMenu = true;
            ShowPopup.Begin();
        }

        private void contextMenu_Closed(object sender, EventArgs e)
        {
            isShowingContextMenu = false;
            HidePopup.Begin();
        }

        private void HideContextMenu()
        {
            HidePopup.Begin();
            isShowingContextMenu = false;
            gridContextMenu.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void lbContextMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HideContextMenu();

            lbContextMenu.SelectedIndex = -1;
        }
        #endregion contextmenu
    }
}
