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
using System.Windows.Threading;

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

        public delegate void OnSelectMenu(object sender, UIElement element);
        public event OnSelectMenu SelectPropertiesMenu;

        public delegate void OnSelectionChanged(object sender, UIElement element);
        public event OnSelectionChanged SelectionChanged;


        ResizeDirection resizeDirection;
        Point beginResizedPoint;
        bool isCaptured;
        bool isResizing;
        bool canResize;
        bool isShowingContextMenu;
        bool canvasClick;
        Point beginCanvasClicked;
        List<Point> clickPoints;
        List<UserControl> controls;
        List<ProxyControl> proxyControls;
        List<UserControl> selectedControls;
        List<ProxyControl> selectedProxyControls;
        DispatcherTimer timer;
        List<Key> arrowKeyPressed;
        ContextMenu contextMenu = new ContextMenu();
        TextImageMenuItem miBringToFront, miBringForward, miSendToBack, miSendBackward, miProperties, miDelete;

        public List<UserControl> Controls
        {
            get { return controls; }
            set { controls = value; }
        }

        public DesignCanvas()
        {
            InitializeComponent();

            isResizing = false;
            canResize = false;
            isShowingContextMenu = false;
            canvasClick = false;
            isCaptured = false;
            
            CursorManager.InitCursor(LayoutRoot);
            DockCanvas.DockCanvas.SetZIndex(multipleSelectRect, 9999);

            clickPoints = new List<Point>();
            proxyControls = new List<ProxyControl>();
            controls = new List<UserControl>();
            selectedControls = new List<UserControl>();
            selectedProxyControls = new List<ProxyControl>();
            arrowKeyPressed = new List<Key>();

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 20);
            timer.Tick += new EventHandler(timer_Tick);

            CreateContextMenu();
        }

        public DockCanvas.DockCanvas RootCanvas
        {
            get { return ControlContainer; }
        }

        #region context menu
        private void CreateContextMenu()
        {
            contextMenu = new ContextMenu();

            miBringToFront = new TextImageMenuItem("Bring to Front", @"Images/BringToFront.png");
            miBringForward = new TextImageMenuItem("Bring Forward", @"Images/BringForward.png");
            miSendToBack = new TextImageMenuItem("Send to Back", @"Images/SendToBack.png");
            miSendBackward = new TextImageMenuItem("Send Backward", @"Images/SendBackward.png");
            miDelete = new TextImageMenuItem("Delete", @"Images/Delete.png");
            miProperties = new TextImageMenuItem("Properties", @"Images/Properties.png");

            miBringToFront.SelectMenuItem += new TextImageMenuItem.OnSelectMenuItem(miBringToFront_SelectMenuItem);
            miBringForward.SelectMenuItem += new TextImageMenuItem.OnSelectMenuItem(miBringForward_SelectMenuItem);
            miSendToBack.SelectMenuItem += new TextImageMenuItem.OnSelectMenuItem(miSendToBack_SelectMenuItem);
            miSendBackward.SelectMenuItem += new TextImageMenuItem.OnSelectMenuItem(miSendBackward_SelectMenuItem);
            miDelete.SelectMenuItem += new TextImageMenuItem.OnSelectMenuItem(miDelete_SelectMenuItem);
            miProperties.SelectMenuItem += new TextImageMenuItem.OnSelectMenuItem(miProperties_SelectMenuItem);

            contextMenu.AddMenuItem(miBringToFront);
            contextMenu.AddMenuItem(miBringForward);
            contextMenu.AddMenuItem(miSendToBack);
            contextMenu.AddMenuItem(miSendBackward);
            contextMenu.AddMenuItem(new SeparatorMenuItem());
            contextMenu.AddMenuItem(miDelete);
            contextMenu.AddMenuItem(new SeparatorMenuItem());
            contextMenu.AddMenuItem(miProperties);

            DockCanvas.DockCanvas.SetZIndex(contextMenu, 9999);
            LayoutRoot.Children.Add(contextMenu);
        }

        void miBringToFront_SelectMenuItem(object sender, MenuItemEventArgs e)
        {
            int zindex = DockCanvas.DockCanvas.GetZIndex(selectedProxyControls[0]);
            int newZindex = zindex;
            foreach (ProxyControl pc in proxyControls)
            {
                int pcZIndex = DockCanvas.DockCanvas.GetZIndex(pc);
                if (pcZIndex > zindex)
                {
                    DockCanvas.DockCanvas.SetZIndex(pc, pcZIndex - 2);
                    DockCanvas.DockCanvas.SetZIndex(pc.RealControl, pcZIndex - 3);
                    newZindex = newZindex < pcZIndex ? pcZIndex : newZindex;
                }
            }
            DockCanvas.DockCanvas.SetZIndex(selectedProxyControls[0], newZindex);
            DockCanvas.DockCanvas.SetZIndex(selectedProxyControls[0].RealControl, newZindex - 1);
        }

        void miBringForward_SelectMenuItem(object sender, MenuItemEventArgs e)
        {
            int zindex = DockCanvas.DockCanvas.GetZIndex(selectedProxyControls[0]);
            int newZindex = zindex;
            ProxyControl swapPC = selectedProxyControls[0];
            foreach (ProxyControl pc in proxyControls)
            {
                int pcZIndex = DockCanvas.DockCanvas.GetZIndex(pc);
                if (pcZIndex > zindex && (pcZIndex < newZindex || newZindex == zindex))
                {
                    newZindex = pcZIndex;
                    swapPC = pc;
                }
            }
            DockCanvas.DockCanvas.SetZIndex(selectedProxyControls[0], newZindex);
            DockCanvas.DockCanvas.SetZIndex(selectedProxyControls[0].RealControl, newZindex - 1);
            if (swapPC != null)
            {
                DockCanvas.DockCanvas.SetZIndex(swapPC, zindex);
                DockCanvas.DockCanvas.SetZIndex(swapPC.RealControl, zindex - 1);
            }
        }

        void miSendToBack_SelectMenuItem(object sender, MenuItemEventArgs e)
        {
            int zindex = DockCanvas.DockCanvas.GetZIndex(selectedProxyControls[0]);
            int newZindex = zindex;
            foreach (ProxyControl pc in proxyControls)
            {
                int pcZIndex = DockCanvas.DockCanvas.GetZIndex(pc);
                if (pcZIndex < zindex)
                {
                    DockCanvas.DockCanvas.SetZIndex(pc, pcZIndex + 2);
                    DockCanvas.DockCanvas.SetZIndex(pc.RealControl, pcZIndex + 1);
                    newZindex = newZindex > pcZIndex ? pcZIndex : newZindex;
                }
            }
            DockCanvas.DockCanvas.SetZIndex(selectedProxyControls[0], newZindex);
            DockCanvas.DockCanvas.SetZIndex(selectedProxyControls[0].RealControl, newZindex - 1);
        }

        void miSendBackward_SelectMenuItem(object sender, MenuItemEventArgs e)
        {
            int zindex = DockCanvas.DockCanvas.GetZIndex(selectedProxyControls[0]);
            int newZindex = zindex;
            ProxyControl swapPC = selectedProxyControls[0];
            foreach (ProxyControl pc in proxyControls)
            {
                int pcZIndex = DockCanvas.DockCanvas.GetZIndex(pc);
                if (pcZIndex < zindex && (pcZIndex > newZindex || newZindex == zindex))
                {
                    newZindex = pcZIndex;
                    swapPC = pc;
                }
            }
            DockCanvas.DockCanvas.SetZIndex(selectedProxyControls[0], newZindex);
            DockCanvas.DockCanvas.SetZIndex(selectedProxyControls[0].RealControl, newZindex - 1);
            if (swapPC != null)
            {
                DockCanvas.DockCanvas.SetZIndex(swapPC, zindex);
                DockCanvas.DockCanvas.SetZIndex(swapPC.RealControl, zindex - 1);
            }
        }

        void miDelete_SelectMenuItem(object sender, MenuItemEventArgs e)
        {
            DeleteSelectedControl();
        }

        void miProperties_SelectMenuItem(object sender, MenuItemEventArgs e)
        {
            if (SelectPropertiesMenu != null)
                SelectPropertiesMenu(sender, selectedControls[0]);
        }
        #endregion context menu

        #region add new control to canvas
        public void AddControl(UserControl uc, double x, double y, int width, int height)
        {
            uc.Margin = new Thickness(0, 0, 0, 0);
            //ProxyControl pc = new ProxyControl(uc, x, y, width, height);
            ProxyControl pc = new ProxyControl(uc, 10, 10);
            proxyControls.Add(pc);
            controls.Add(uc);
            int index = FindTopProxyControlIndex();
            DockCanvas.DockCanvas.SetZIndex(pc, index + 2);
            DockCanvas.DockCanvas.SetZIndex(uc, index + 1);

            ControlContainer.Children.Add(uc);
            LayoutRoot.Children.Add(pc);

            pc.MouseLeftButtonDown += new MouseButtonEventHandler(control_MouseLeftButtonDown);
            pc.MouseMove += new MouseEventHandler(control_MouseMove);
            pc.MouseLeftButtonUp += new MouseButtonEventHandler(control_MouseLeftButtonUp);

            ClearSelectedList();
            AddSelectedControl(pc);

            textBox1.Focus();

            if (SelectionChanged != null)
                SelectionChanged(this, uc);

            this.Focus();
        }

        private int FindTopProxyControlIndex()
        {
            if (proxyControls.Count == 0)
                return 0;
            int index = DockCanvas.DockCanvas.GetZIndex(proxyControls[0]);
            for (int i = 1; i < proxyControls.Count; i++)
                if (index < DockCanvas.DockCanvas.GetZIndex(proxyControls[i]))
                    index = DockCanvas.DockCanvas.GetZIndex(proxyControls[i]);
            return index;
        }
        #endregion add new control to canvas

        #region dragdrop control
        void control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isCaptured)
            {
                isCaptured = false;
                foreach (ProxyControl pc in selectedProxyControls)
                    pc.ReleaseMouseCapture();
            }
        }

        void control_MouseMove(object sender, MouseEventArgs e)
        {
            if (isCaptured)
            {
                Point pt = e.GetPosition(LayoutRoot);
                pt = MoveControls(pt);
            }
        }

        void control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                isCaptured = true;
                ProxyControl pc = (ProxyControl)sender;
                bool b = false;
                foreach (ProxyControl selectedProxyControl in selectedProxyControls)
                    if (selectedProxyControl.Equals(pc))
                    {
                        RemoveSelectedControl(selectedProxyControl);
                        b = true;
                        break;
                    }
                if (b == false)
                {
                    AddSelectedControl(pc);
                }
                else if (b == true && selectedProxyControls.Count == 0)
                {
                    isCaptured = false;
                }
            }
            else if (selectedControls.Count >= 2 || !canResize)
            {
                ProxyControl pc = (ProxyControl)sender;
                isCaptured = false;
                foreach (ProxyControl selectedProxyControl in selectedProxyControls)
                    if (selectedProxyControl.Equals(pc))
                        isCaptured = true;
                if (isCaptured == false)
                {
                    ClearSelectedList();
                    clickPoints.Add(e.GetPosition(pc));
                    AddSelectedControl(pc);
                    isCaptured = true;
                    pc.CaptureMouse();
                }
                else
                {
                    clickPoints.Clear();
                    foreach (ProxyControl selectedProxyControl in selectedProxyControls)
                    {
                        clickPoints.Add(e.GetPosition(selectedProxyControl.RealControl));
                        selectedProxyControl.CaptureMouse();
                    }
                }
                if (selectedControls.Count == 1)
                    if (SelectionChanged != null)
                        SelectionChanged(this, selectedControls[0]);
            } 

            if (isShowingContextMenu)
            {
                HideContextMenu();
            }
        }

        private Point MoveControls(Point pt)
        {
            Point delta = new Point(0, 0);
            Point[] pts = new Point[selectedProxyControls.Count];

            for (int i = 0; i < selectedProxyControls.Count; i++)
            {
                double x = pt.X - clickPoints[i].X;
                double y = pt.Y - clickPoints[i].Y;
                ProxyControl pc = selectedProxyControls[i];
                UserControl uc = pc.RealControl;

                pts[i].X = x;
                pts[i].Y = y;

                if (x < 0 && x < delta.X)
                    delta.X = x;
                else if (x + uc.Width >= this.ActualWidth && x + uc.Width - this.ActualWidth + 1 > delta.X)
                    delta.X = x + uc.Width - this.ActualWidth + 1;

                if (y < 0 && y < delta.Y)
                    delta.Y = y;
                else if (y + uc.Height >= this.ActualHeight && y + uc.Height - this.ActualHeight + 1 > delta.Y)
                    delta.Y = y + uc.Height - this.ActualHeight + 1;
            }

            for (int i = 0; i < pts.Length; i++)
            {
                ProxyControl pc = selectedProxyControls[i];
                pc.MoveControl(pts[i].X - delta.X, pts[i].Y - delta.Y);
            }
            return pt;
        }
        #endregion dragdrop control

        #region resize control and select multiple controls
        private void LayoutRoot_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedControls.Count == 1)
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
            else if (canvasClick)
            {
                Point pt = e.GetPosition(LayoutRoot);
                double x = pt.X > beginCanvasClicked.X ? beginCanvasClicked.X : pt.X;
                double y = pt.Y > beginCanvasClicked.Y ? beginCanvasClicked.Y : pt.Y;
                double width = Math.Abs(pt.X - beginCanvasClicked.X);
                double height = Math.Abs(pt.Y - beginCanvasClicked.Y);

                multipleSelectRect.Visibility = System.Windows.Visibility.Visible;
                multipleSelectRect.SetValue(Canvas.LeftProperty, x);
                multipleSelectRect.SetValue(Canvas.TopProperty, y);
                multipleSelectRect.Width = width;
                multipleSelectRect.Height = height;
            }
        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
            if (canResize)
            {
                isResizing = true; 
                selectedProxyControls[0].CaptureMouse();
            }
            else if (!isCaptured)
            {
                ClearSelectedList();
                canvasClick = true;
                beginCanvasClicked = e.GetPosition(LayoutRoot);

                if (SelectionChanged != null)
                    SelectionChanged(this, ControlContainer);
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
                selectedProxyControls[0].ReleaseMouseCapture();
            }
            else if (canvasClick)
            {
                Point pt = e.GetPosition(LayoutRoot);
                Rect rect = new Rect(beginCanvasClicked, pt);

                ClearSelectedList();
                foreach (ProxyControl pc in proxyControls)
                {
                    double x = (double)pc.RealControl.GetValue(Canvas.LeftProperty);
                    double y = (double)pc.RealControl.GetValue(Canvas.TopProperty);
                    if (rect.Contains(new Point(x, y)) && rect.Contains(new Point(x + pc.RealControl.Width, y + pc.RealControl.Height)))
                    {
                        AddSelectedControl(pc);
                    }
                }
                multipleSelectRect.Visibility = System.Windows.Visibility.Collapsed;
                canvasClick = false;
            }
        }

        private void ClearSelectedList()
        {
            foreach (ProxyControl selectedProxyControl in selectedProxyControls)
                selectedProxyControl.UpdateVisibility(System.Windows.Visibility.Collapsed);
            selectedControls.Clear();
            selectedProxyControls.Clear();
            clickPoints.Clear();
        }

        private void CheckCanResize(MouseEventArgs e)
        {
            UserControl selectedControl = selectedControls[0];
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
            UserControl selectedControl = selectedControls[0];
            ProxyControl selectedProxyControl = selectedProxyControls[0];

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
        #endregion resize control and select multiple controls

        private void LayoutRoot_MouseLeave(object sender, MouseEventArgs e)
        {
            isResizing = false;
            isCaptured = false;
            foreach (ProxyControl pc in selectedProxyControls)
                pc.ReleaseMouseCapture();
            CursorManager.ChangeCursor(this, CursorManager.CursorType.Arrow);

            multipleSelectRect.Visibility = System.Windows.Visibility.Collapsed;
            canvasClick = false;
        }

        #region rightclick
        private void PositionContextMenu(Point p)
        {
            Point pt = new Point(0, 0);
            pt.X = p.X + contextMenu.ActualWidth > LayoutRoot.ActualWidth ? p.X + contextMenu.ActualWidth - LayoutRoot.ActualWidth : 0;
            pt.Y = p.Y + contextMenu.ActualHeight > LayoutRoot.ActualHeight ? p.Y + contextMenu.ActualHeight - LayoutRoot.ActualHeight : 0;
            contextMenu.SetValue(Canvas.LeftProperty, p.X - pt.X);
            contextMenu.SetValue(Canvas.TopProperty, p.Y - pt.Y);
        }

        private void HideContextMenu()
        {
            contextMenu.HideContextMenu();
            isShowingContextMenu = false;
        }

        private void lbContextMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HideContextMenu();
        }

        private void LayoutRoot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
            if (canvasClick)
            {
                canvasClick = false;
                multipleSelectRect.Visibility = System.Windows.Visibility.Collapsed;
            }

            Point pt = e.GetPosition(LayoutRoot);
            bool b = false;
            foreach (ProxyControl pc in selectedProxyControls)
            {
                Rect rect = new Rect((double)pc.GetValue(Canvas.LeftProperty), (double)pc.GetValue(Canvas.TopProperty), pc.ActualWidth, pc.ActualHeight);
                if (rect.Contains(pt))
                {
                    b = true;
                    break;
                }
            }
            if (b == false)
            {
                ClearSelectedList();
                ProxyControl temp = null;
                int zindex = -1;
                foreach (ProxyControl pc in proxyControls)
                {
                    UserControl uc = pc.RealControl;
                    Rect rect = new Rect((double)uc.GetValue(Canvas.LeftProperty), (double)uc.GetValue(Canvas.TopProperty), uc.ActualWidth, uc.ActualHeight);
                    if (rect.Contains(pt))
                    {
                        int pczindex = DockCanvas.DockCanvas.GetZIndex(pc);
                        if (pczindex > zindex)
                        {
                            zindex = pczindex;
                            temp = pc;
                        }
                    }
                }
                if (temp != null)
                    AddSelectedControl(temp);
            }

            bool order, delete, properties;
            if (selectedControls.Count == 0)
            {
                order = false;
                delete = false;
            }
            else if (selectedControls.Count == 1)
            {
                order = true;
                delete = true;
            }
            else
            {
                order = false;
                delete = true;
            }
            miBringForward.Enabled = order;
            miBringToFront.Enabled = order;
            miSendBackward.Enabled = order;
            miSendToBack.Enabled = order;
            miDelete.Enabled = delete;

            PositionContextMenu(pt);
            e.Handled = true;
            contextMenu.ShowContextMenu();
            isShowingContextMenu = true;
        }
        #endregion rightclick

        #region keyboard
        private void LayoutRoot_KeyDown(object sender, KeyEventArgs e)
        {
            if (selectedControls.Count != 0 && !isResizing)
            {
                switch(e.Key)
                {
                    case Key.Left:
                        if (arrowKeyPressed.Contains(Key.Right))
                            arrowKeyPressed.Remove(Key.Right);
                        if (!arrowKeyPressed.Contains(e.Key))
                            arrowKeyPressed.Add(e.Key);
                        timer.Start();
                        break;
                    case Key.Right:
                        if (arrowKeyPressed.Contains(Key.Left))
                            arrowKeyPressed.Remove(Key.Left);
                        if (!arrowKeyPressed.Contains(e.Key)) 
                            arrowKeyPressed.Add(e.Key);
                        timer.Start();
                        break;
                    case Key.Up:
                        if (arrowKeyPressed.Contains(Key.Down))
                            arrowKeyPressed.Remove(Key.Down);
                        if (!arrowKeyPressed.Contains(e.Key)) 
                            arrowKeyPressed.Add(e.Key);
                        timer.Start();
                        break;
                    case Key.Down:
                        if (arrowKeyPressed.Contains(Key.Up))
                            arrowKeyPressed.Remove(Key.Up);
                        if (!arrowKeyPressed.Contains(e.Key)) 
                            arrowKeyPressed.Add(e.Key);
                        timer.Start();
                        break;
                    case Key.Delete:
                        if (!isCaptured)
                            DeleteSelectedControl();
                        break;
                }
            }
        }

        private void LayoutRoot_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.Down)
            {
                arrowKeyPressed.Remove(e.Key);
                if (arrowKeyPressed.Count == 0)
                    timer.Stop();
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Point pt = new Point(0, 0);
            if (arrowKeyPressed.Contains(Key.Left))
                pt.X = -1;
            else if (arrowKeyPressed.Contains(Key.Right))
                pt.X = 1;
            if (arrowKeyPressed.Contains(Key.Up))
                pt.Y = -1;
            else if (arrowKeyPressed.Contains(Key.Down))
                pt.Y = 1;

            Point delta = new Point(0, 0);
            Point[] pts = new Point[selectedProxyControls.Count];

            for (int i = 0; i < selectedProxyControls.Count; i++)
            {
                ProxyControl pc = selectedProxyControls[i];
                UserControl uc = pc.RealControl;
                double x = (double)uc.GetValue(Canvas.LeftProperty) + pt.X;
                double y = (double)uc.GetValue(Canvas.TopProperty) + pt.Y;
                
                pts[i].X = x;
                pts[i].Y = y;

                if (x < 0 && x < delta.X)
                    delta.X = x;
                else if (x + uc.Width >= this.ActualWidth && x + uc.Width - this.ActualWidth + 1 > delta.X)
                    delta.X = x + uc.Width - this.ActualWidth + 1;

                if (y < 0 && y < delta.Y)
                    delta.Y = y;
                else if (y + uc.Height >= this.ActualHeight && y + uc.Height - this.ActualHeight + 1 > delta.Y)
                    delta.Y = y + uc.Height - this.ActualHeight + 1;
            }

            for (int i = 0; i < pts.Length; i++)
            {
                ProxyControl pc = selectedProxyControls[i];
                pc.MoveControl(pts[i].X - delta.X, pts[i].Y - delta.Y);
            }
        }
        #endregion keyboard

        private void DeleteSelectedControl()
        {
            foreach (ProxyControl pc in selectedProxyControls)
            {
                LayoutRoot.Children.Remove(pc);
                ControlContainer.Children.Remove(pc.RealControl);
                proxyControls.Remove(pc);
                controls.Remove(pc.RealControl);
            }
            selectedProxyControls.Clear();
            selectedControls.Clear();

            canResize = false;
            CursorManager.ChangeCursor(this, CursorManager.CursorType.Arrow);
        }

        private void RemoveSelectedControl(ProxyControl selectedProxyControl)
        {
            selectedProxyControls.Remove(selectedProxyControl);
            selectedControls.Remove(selectedProxyControl.RealControl);
            selectedProxyControl.UpdateVisibility(System.Windows.Visibility.Collapsed);
        }

        private void AddSelectedControl(ProxyControl pc)
        {
            selectedProxyControls.Add(pc);
            selectedControls.Add(pc.RealControl);
            pc.UpdateVisibility(System.Windows.Visibility.Visible);
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            isResizing = false;
            isCaptured = false;
            foreach (ProxyControl pc in selectedProxyControls)
                pc.ReleaseMouseCapture();
            CursorManager.ChangeCursor(this, CursorManager.CursorType.Arrow);

            multipleSelectRect.Visibility = System.Windows.Visibility.Collapsed;
            canvasClick = false;
        }

        private void root_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox1.Text = "";
        }
    }
}
