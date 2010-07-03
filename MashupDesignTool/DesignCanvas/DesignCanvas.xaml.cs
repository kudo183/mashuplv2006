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
using BasicLibrary;
using Liquid;

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

        public delegate void ControlPositionChangedHander(object sender, bool multiControl);
        public event ControlPositionChangedHander ControlPositionChanged;

        public delegate void ControlSizeChangedHander(object sender, Size newSize);
        public event ControlSizeChangedHander ControlSizeChanged;
        
        public delegate void ControlZIndexChangedHandler(object sender, int zindex);
        public event ControlZIndexChangedHandler ControlZIndexChanged;

        public delegate void ControlDeleteHandler(object sender, List<EffectableControl> list);
        public event ControlDeleteHandler ControlDelete;

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
        bool canMove;
        Point beginCanvasClicked;
        List<Point> clickPoints;
        List<EffectableControl> controls;
        List<ProxyControl> proxyControls;
        List<EffectableControl> selectedControls;
        List<ProxyControl> selectedProxyControls;
        DispatcherTimer timer;
        List<Key> arrowKeyPressed;
        Menu menu = new Menu();
        Liquid.MenuItem lmiBringToFront, lmiBringForward, lmiSendToBack, lmiSendBackward, lmiProperties, lmiDelete;
        int iCount = 0;
        DispatcherTimer dispatcherTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(40) };

        public List<EffectableControl> Controls
        {
            get { return controls; }
        }

        public List<EffectableControl> SelectedControls
        {
            get { return selectedControls; }
            set { selectedControls = value; }
        }

        public List<ProxyControl> SelectedProxyControls
        {
            get { return selectedProxyControls; }
        }

        public List<string> GetPropertyNameList()
        {
            List<string> list = new List<string>();
            list.Add("Width");
            list.Add("Height");
            list.Add("Background");
            return list;
        }

        public DesignCanvas()
        {
            InitializeComponent();

            isResizing = false;
            canResize = false;
            isShowingContextMenu = false;
            canvasClick = false;
            isCaptured = false;
            canMove = false;
            
            CursorManager.InitCursor(LayoutRoot);
            DockCanvas.DockCanvas.SetZIndex(multipleSelectRect, 99999);

            clickPoints = new List<Point>();
            proxyControls = new List<ProxyControl>();
            controls = new List<EffectableControl>();
            selectedControls = new List<EffectableControl>();
            selectedProxyControls = new List<ProxyControl>();
            arrowKeyPressed = new List<Key>();

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 20);
            timer.Tick += new EventHandler(timer_Tick);

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);

            CreateContextMenu();
        }

        public DockCanvas.DockCanvas ControlContainerCanvas
        {
            get { return ControlContainer; }
        }

        public Canvas ProxyContainerCanvas
        {
            get { return LayoutRoot; }
        }

        public void Clear()
        {
            selectedControls.Clear();
            selectedProxyControls.Clear();
            for (int i = 0; i < controls.Count; i++)
            {
                ControlContainer.Children.Remove(controls[i]);
                LayoutRoot.Children.Remove(proxyControls[i]);
            }
            controls.Clear();
            proxyControls.Clear();
            LayoutRoot.Width = ControlContainer.Width = 800;
            LayoutRoot.Height = ControlContainer.Height = 500;
            ControlContainer.Background = new SolidColorBrush(Colors.White);
        }

        #region context menu
        private void CreateContextMenu()
        {
            menu = new Menu();
            menu.Hide();
            menu.RenderTransform = new ScaleTransform() { ScaleX = 0.9, ScaleY = 0.9 };
            lmiBringToFront = new Liquid.MenuItem("Bring to Front", @"Images/BringToFront.png");
            lmiBringForward = new Liquid.MenuItem("Bring Forward", @"Images/BringForward.png");
            lmiSendToBack = new Liquid.MenuItem("Send to Back", @"Images/SendToBack.png");
            lmiSendBackward = new Liquid.MenuItem("Send Backward", @"Images/SendBackward.png");
            lmiDelete = new Liquid.MenuItem("Delete", @"Images/Delete.png");
            lmiProperties = new Liquid.MenuItem("Properties", @"Images/Properties.png");

            lmiBringToFront.MouseLeftButtonDown += new MouseButtonEventHandler(lmiBringToFront_MouseLeftButtonDown);
            lmiBringForward.MouseLeftButtonDown += new MouseButtonEventHandler(lmiBringForward_MouseLeftButtonDown);
            lmiSendToBack.MouseLeftButtonDown += new MouseButtonEventHandler(lmiSendToBack_MouseLeftButtonDown);
            lmiSendBackward.MouseLeftButtonDown += new MouseButtonEventHandler(lmiSendBackward_MouseLeftButtonDown);
            lmiDelete.MouseLeftButtonDown += new MouseButtonEventHandler(lmiDelete_MouseLeftButtonDown);
            lmiProperties.MouseLeftButtonDown += new MouseButtonEventHandler(lmiProperties_MouseLeftButtonDown);

            menu.Items.Add(lmiBringToFront);
            menu.Items.Add(lmiBringForward);
            menu.Items.Add(lmiSendToBack);
            menu.Items.Add(lmiSendBackward);
            menu.Items.Add(new Liquid.MenuDivider());
            menu.Items.Add(lmiDelete);
            menu.Items.Add(new Liquid.MenuDivider());
            menu.Items.Add(lmiProperties);
            LayoutRoot.Children.Add(menu);
            DockCanvas.DockCanvas.SetZIndex(menu, 63000);
        }

        void lmiBringToFront_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int zindex = DockCanvas.DockCanvas.GetZIndex(selectedProxyControls[0]);
            int newZindex = zindex;
            foreach (ProxyControl pc in proxyControls)
            {
                int pcZIndex = DockCanvas.DockCanvas.GetZIndex(pc);
                if (pcZIndex > zindex)
                {
                    SetZindex(pc, pcZIndex - 1);
                    newZindex = newZindex < pcZIndex ? pcZIndex : newZindex;
                }
            }
            SetZindex(selectedProxyControls[0], newZindex);

            if (ControlZIndexChanged != null)
                ControlZIndexChanged(selectedControls[0].Control, newZindex);

            HideContextMenu();
        }

        void lmiBringForward_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
            SetZindex(selectedProxyControls[0], newZindex);
            if (swapPC != null)
            {
                SetZindex(swapPC, zindex);
            }

            if (ControlZIndexChanged != null)
                ControlZIndexChanged(selectedControls[0].Control, newZindex);

            HideContextMenu();
        }

        void lmiSendToBack_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int zindex = DockCanvas.DockCanvas.GetZIndex(selectedProxyControls[0]);
            int newZindex = zindex;
            foreach (ProxyControl pc in proxyControls)
            {
                int pcZIndex = DockCanvas.DockCanvas.GetZIndex(pc);
                if (pcZIndex < zindex)
                {
                    SetZindex(pc, pcZIndex + 1);
                    newZindex = newZindex > pcZIndex ? pcZIndex : newZindex;
                }
            }
            SetZindex(selectedProxyControls[0], newZindex);

            if (ControlZIndexChanged != null)
                ControlZIndexChanged(selectedControls[0].Control, newZindex);
            
            HideContextMenu();
        }

        void lmiSendBackward_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
            SetZindex(selectedProxyControls[0], newZindex);
            if (swapPC != null)
            {
                SetZindex(swapPC, zindex);
            }

            if (ControlZIndexChanged != null)
                ControlZIndexChanged(selectedControls[0].Control, newZindex);

            HideContextMenu();
        }

        void lmiDelete_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DeleteSelectedControl();
            HideContextMenu();
        }

        void lmiProperties_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectPropertiesMenu != null)
            {
                if (selectedControls.Count != 0)
                    SelectPropertiesMenu(sender, selectedControls[0].Control);
                else
                    SelectPropertiesMenu(sender, ControlContainer);
            }
            HideContextMenu();
        }

        public void SetZindex(ProxyControl pc, int zindex)
        {
            DockCanvas.DockCanvas.SetZIndex(pc, zindex);
            DockCanvas.DockCanvas.SetZIndex(pc.RealControl, zindex);
            //DockCanvas.DockCanvas.SetZIndex(pc.RealControl.Control, zindex);
        }

        #endregion context menu

        #region add new control to canvas
        public void AddEffectableControl(EffectableControl ec)
        {
            ProxyControl pc = new ProxyControl(ec);
            proxyControls.Add(pc);
            controls.Add(ec);
            int index = FindTopProxyControlIndex();
            SetZindex(pc, index + 1);

            ControlContainer.Children.Add(ec);
            LayoutRoot.Children.Add(pc);

            pc.MouseLeftButtonDown += new MouseButtonEventHandler(control_MouseLeftButtonDown);
            pc.MouseMove += new MouseEventHandler(control_MouseMove);
            pc.MouseLeftButtonUp += new MouseButtonEventHandler(control_MouseLeftButtonUp);

            ClearSelectedList();
            AddSelectedControl(pc);

            textBox1.Focus();

            if (SelectionChanged != null)
                SelectionChanged(this, ec.Control);

            this.Focus();
            HideContextMenu();

            ec.Control.Tag = ec;
            ec.Control.SizeChanged += new SizeChangedEventHandler(uc_SizeChanged);
        }

        public void AddControl(FrameworkElement uc)
        {
            EffectableControl ec = new EffectableControl(uc);
            //uc.Name = iCount.ToString();
            //ec.Name = (iCount + 1).ToString();
            uc.Name = uc.GetType().Name.ToLower() + "_" + Guid.NewGuid().ToString();
            ec.Name = uc.GetType().Name.ToLower() + "_" + Guid.NewGuid().ToString();
            BasicControl bc = uc as BasicControl;
            if (bc != null)
                bc.ControlName = uc.Name;

            iCount += 2;
            uc.Margin = new Thickness(0, 0, 0, 0);
            Canvas.SetLeft(ec, 200);
            Canvas.SetTop(ec, 200);
            AddEffectableControl(ec);

            //ProxyControl pc = new ProxyControl(ec, 10, 10);
            //proxyControls.Add(pc);
            //controls.Add(ec);
            //int index = FindTopProxyControlIndex();
            //SetZindex(pc, index + 1);

            //ControlContainer.Children.Add(ec);
            //LayoutRoot.Children.Add(pc);

            //pc.MouseLeftButtonDown += new MouseButtonEventHandler(control_MouseLeftButtonDown);
            //pc.MouseMove += new MouseEventHandler(control_MouseMove);
            //pc.MouseLeftButtonUp += new MouseButtonEventHandler(control_MouseLeftButtonUp);

            //ClearSelectedList();
            //AddSelectedControl(pc);

            //textBox1.Focus();

            //if (SelectionChanged != null)
            //    SelectionChanged(this, uc);

            //this.Focus();
            //HideContextMenu();

            //uc.Tag = ec;
            //uc.SizeChanged += new SizeChangedEventHandler(uc_SizeChanged);
        }

        void uc_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement uc = (FrameworkElement)sender;
            uc.SizeChanged -= new SizeChangedEventHandler(uc_SizeChanged);
            ((EffectableControl)uc.Tag).Width = e.NewSize.Width;
            ((EffectableControl)uc.Tag).Height = e.NewSize.Height;
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
            if (isCaptured && canMove)
            {
                if (!dispatcherTimer.IsEnabled)
                    dispatcherTimer.Start();
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
            }

            if (SelectionChanged != null)
            {
                if (selectedControls.Count != 0)
                    SelectionChanged(this, selectedControls[selectedControls.Count - 1].Control);
                else
                    SelectionChanged(this, ControlContainer);
            }

            if (isShowingContextMenu)
            {
                HideContextMenu();
            }
        }

        private Point MoveControls(Point pt)
        {
            if (selectedProxyControls.Count == 0)
                return pt;

            Point delta = new Point(0, 0);
            Point[] pts = new Point[selectedProxyControls.Count];

            for (int i = 0; i < selectedProxyControls.Count; i++)
            {
                //if (((DockCanvas.DockCanvas.DockType)selectedControls[i].GetValue(DockCanvas.DockCanvas.DockTypeProperty)) != DockCanvas.DockCanvas.DockType.None)
                //{
                //    delta.X = delta.Y = 0;
                //    for (int j = 0; j < selectedProxyControls.Count; j++)
                //    {
                //        pts[j].X = Canvas.GetLeft(selectedControls[j]);
                //        pts[j].Y = Canvas.GetTop(selectedControls[j]);
                //    }
                //    break;
                //}
                double x = pt.X - clickPoints[i].X;
                double y = pt.Y - clickPoints[i].Y;
                ProxyControl pc = selectedProxyControls[i];
                FrameworkElement uc = pc.RealControl;

                pts[i].X = x;
                pts[i].Y = y;

                if (x < 0 && x < delta.X)
                    delta.X = x;
                else if (x + uc.Width >= ControlContainer.ActualWidth && x + uc.Width - ControlContainer.ActualWidth + 1 > delta.X)
                    delta.X = x + uc.Width - ControlContainer.ActualWidth + 1;

                if (y < 0 && y < delta.Y)
                    delta.Y = y;
                else if (y + uc.Height >= ControlContainer.ActualHeight && y + uc.Height - ControlContainer.ActualHeight + 1 > delta.Y)
                    delta.Y = y + uc.Height - ControlContainer.ActualHeight + 1;
            }

            bool b = true;
            if (pts[0].X - delta.X == Canvas.GetLeft(selectedControls[0]) && pts[0].Y - delta.Y == Canvas.GetTop(selectedControls[0]))
                b = false;

            for (int i = 0; i < pts.Length; i++)
            {
                ProxyControl pc = selectedProxyControls[i];
                pc.MoveControl(pts[i].X - delta.X, pts[i].Y - delta.Y);
            }

            //if (b == true && PositionChanged != null)
            //{
            //    if (selectedControls.Count == 1)
            //        PositionChanged(this, false);
            //    else
            //        PositionChanged(this, true);
            //}
            return pt;
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (ControlPositionChanged != null)
            {
                if (selectedControls.Count == 1)
                    ControlPositionChanged(this, false);
                else
                    ControlPositionChanged(this, true);
            }
            if (!isCaptured)
            {
                dispatcherTimer.Stop();
            }
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
            if (isShowingContextMenu)
            {
                Rect rect = new Rect(Canvas.GetLeft(menu), Canvas.GetTop(menu), menu.ActualWidth * 0.9, menu.ActualHeight * 0.9);
                if (!rect.Contains(e.GetPosition(LayoutRoot)))
                {
                    HideContextMenu();
                    return;
                }
            }
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

                if (SelectionChanged != null)
                {
                    if (selectedControls.Count != 0)
                        SelectionChanged(this, selectedControls[selectedControls.Count - 1].Control);
                    else
                        SelectionChanged(this, ControlContainer);
                }
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
            FrameworkElement selectedControl = selectedControls[0];
            if (double.IsNaN(selectedControl.Width))
                selectedControl.Width = 0;
            if (double.IsNaN(selectedControl.Height))
                selectedControl.Height = 0;
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

            #region check docktype
            DockCanvas.DockCanvas.DockType dt = (DockCanvas.DockCanvas.DockType)selectedControl.GetValue(DockCanvas.DockCanvas.DockTypeProperty);
            switch (dt)
            {
                case DockCanvas.DockCanvas.DockType.None:
                    break;
                case DockCanvas.DockCanvas.DockType.Fill:
                    canResize = false;
                    break;
                case DockCanvas.DockCanvas.DockType.Left:
                    if (resizeDirection != ResizeDirection.RIGHT)
                        canResize = false;
                    break;
                case DockCanvas.DockCanvas.DockType.Right:
                    if (resizeDirection != ResizeDirection.LEFT)
                        canResize = false;
                    break;
                case DockCanvas.DockCanvas.DockType.Top:
                    if (resizeDirection != ResizeDirection.BOTTOM)
                        canResize = false;
                    break;
                case DockCanvas.DockCanvas.DockType.Bottom:
                    if (resizeDirection != ResizeDirection.TOP)
                        canResize = false;
                    break;
            }
            #endregion check docktype

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
            else if (pt.X >= ControlContainer.ActualWidth + beginResizedPoint.X)
            {
                b = true;
                pt.X = ControlContainer.ActualWidth - 1;
            }
            if (pt.Y < beginResizedPoint.Y)
            {
                b = true;
                pt.Y = 0;
            }
            else if (pt.Y >= ControlContainer.ActualHeight + beginResizedPoint.Y)
            {
                b = true;
                pt.Y = ControlContainer.ActualHeight - 1;
            }

            ResizeControl(pt);

            if (b == true)
            {
                isResizing = false;
                canResize = false;
                CursorManager.ChangeCursor(this, CursorManager.CursorType.Arrow);
            }

            ControlContainer.UpdateChildrenPosition();
            UpdateAllProxyControlPosition();
        }

        private void ResizeControl(Point pt)
        {
            FrameworkElement selectedControl = selectedControls[0];
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

            if (ControlSizeChanged != null)
            {
                ControlSizeChanged(selectedProxyControl.RealControl.Control, new Size(width, height));
            }
        }

        public void UpdateAllProxyControlPosition()
        {
            for (int i = 0; i < proxyControls.Count; i++)
            {
                proxyControls[i].MoveControl(Canvas.GetLeft(controls[i]), Canvas.GetTop(controls[i]));
            }
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
            pt.X = p.X + menu.ActualWidth * 0.9 > LayoutRoot.ActualWidth ? p.X + menu.ActualWidth * 0.9 - LayoutRoot.ActualWidth : 0;
            pt.Y = p.Y + menu.ActualHeight * 0.9 > LayoutRoot.ActualHeight ? p.Y + menu.ActualHeight * 0.9 - LayoutRoot.ActualHeight : 0;
            menu.SetValue(Canvas.LeftProperty, p.X - pt.X);
            menu.SetValue(Canvas.TopProperty, p.Y - pt.Y);
        }

        private void HideContextMenu()
        {
            menu.Hide();
            isShowingContextMenu = false;
        }

        private void ShowContextMenu()
        {
            menu.Show();
            isShowingContextMenu = true;
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
                    FrameworkElement uc = pc.RealControl;
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
                {
                    AddSelectedControl(temp);
                    if (SelectionChanged != null)
                        SelectionChanged(this, selectedControls[0].Control);
                }
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

            UpdateMenuItem(order, delete);
            PositionContextMenu(pt);
            e.Handled = true;
            ShowContextMenu();
        }

        private void UpdateMenuItem(bool order, bool delete)
        {
            lmiBringForward.IsEnabled = order;
            lmiBringToFront.IsEnabled = order;
            lmiSendBackward.IsEnabled = order;
            lmiSendToBack.IsEnabled = order;
            lmiDelete.IsEnabled = delete;
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
                if (((DockCanvas.DockCanvas.DockType)selectedControls[i].GetValue(DockCanvas.DockCanvas.DockTypeProperty)) != DockCanvas.DockCanvas.DockType.None)
                {
                    delta.X = delta.Y = 0;
                    for (int j = 0; j < selectedProxyControls.Count; j++)
                    {
                        pts[j].X = Canvas.GetLeft(selectedControls[j]);
                        pts[j].Y = Canvas.GetTop(selectedControls[j]);
                    }
                    break;
                }

                ProxyControl pc = selectedProxyControls[i];
                FrameworkElement uc = pc.RealControl;
                double x = (double)uc.GetValue(Canvas.LeftProperty) + pt.X;
                double y = (double)uc.GetValue(Canvas.TopProperty) + pt.Y;
                
                pts[i].X = x;
                pts[i].Y = y;

                if (x < 0 && x < delta.X)
                    delta.X = x;
                else if (x + uc.Width >= ControlContainer.ActualWidth && x + uc.Width - ControlContainer.ActualWidth + 1 > delta.X)
                    delta.X = x + uc.Width - ControlContainer.ActualWidth + 1;

                if (y < 0 && y < delta.Y)
                    delta.Y = y;
                else if (y + uc.Height >= ControlContainer.ActualHeight && y + uc.Height - ControlContainer.ActualHeight + 1 > delta.Y)
                    delta.Y = y + uc.Height - ControlContainer.ActualHeight + 1;
            }

            bool b = true;
            if (pts[0].X - delta.X == Canvas.GetLeft(selectedControls[0]) && pts[0].Y - delta.Y == Canvas.GetTop(selectedControls[0]))
                b = false;

            for (int i = 0; i < pts.Length; i++)
            {
                ProxyControl pc = selectedProxyControls[i];
                pc.MoveControl(pts[i].X - delta.X, pts[i].Y - delta.Y);
            }

            if (b == true && ControlPositionChanged != null)
            {
                if (selectedControls.Count == 1)
                    ControlPositionChanged(this, false);
                else
                    ControlPositionChanged(this, true);
            }
        }
        #endregion keyboard

        private List<ProxyControl> deletingControl = new List<ProxyControl>();
        private void DeleteSelectedControl()
        {
            deletingControl.Clear();
            foreach (ProxyControl pc in selectedProxyControls)
                deletingControl.Add(pc);
            DeleteConfirm deleteConfirm = new DeleteConfirm();
            deleteConfirm.Closed += new EventHandler(deleteConfirm_Closed);
            deleteConfirm.Show();
        }

        void deleteConfirm_Closed(object sender, EventArgs e)
        {
            DeleteConfirm deleteConfirm = (DeleteConfirm)sender;
            if (deleteConfirm.DialogResult == false)
            {
                selectedControls.Clear();
                selectedProxyControls.Clear();
                foreach (ProxyControl pc in deletingControl)
                    AddSelectedControl(pc);
                return;
            }

            foreach (ProxyControl pc in deletingControl)
            {
                LayoutRoot.Children.Remove(pc);
                ControlContainer.Children.Remove(pc.RealControl);
                proxyControls.Remove(pc);
                controls.Remove(pc.RealControl);
                if (typeof(BasicControl).IsAssignableFrom(pc.RealControl.Control.GetType()))
                {
                    ((BasicControl)pc.RealControl.Control).Dispose();
                }
            }

            if (ControlDelete != null)
                ControlDelete(this, selectedControls);

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
            canMove = true;
            for (int i = 0; i < selectedControls.Count; i++)
            {
                if (((DockCanvas.DockCanvas.DockType)selectedControls[i].GetValue(DockCanvas.DockCanvas.DockTypeProperty)) != DockCanvas.DockCanvas.DockType.None)
                {
                    canMove = false;
                    break;
                }
            }
        }

        private void AddSelectedControl(ProxyControl pc)
        {
            selectedProxyControls.Add(pc);
            selectedControls.Add(pc.RealControl);
            pc.UpdateVisibility(System.Windows.Visibility.Visible);
            canMove = true;
            for (int i = 0; i < selectedControls.Count; i++)
            {
                if (((DockCanvas.DockCanvas.DockType)selectedControls[i].GetValue(DockCanvas.DockCanvas.DockTypeProperty)) != DockCanvas.DockCanvas.DockType.None)
                {
                    canMove = false;
                    break;
                }
            }
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

        private void ControlContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            LayoutRoot.Width = ControlContainer.Width;
            LayoutRoot.Height = ControlContainer.Height;
        }
    }
}
