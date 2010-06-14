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
using System.Windows.Browser;
using System.ComponentModel;
using System.Windows.Data;
using BasicLibrary;

namespace ControlLibrary
{
    public class HtmlHost : BasicControl
    {
        int _htmlControlLeft = 0;
        [Category("HtmlHost")]
        public int HtmlControlLeft
        {
            get { return _htmlControlLeft; }
            set
            {
                _htmlControlLeft = value;
                if (divIFrameHost == null)
                    return;
                divIFrameHost.SetStyleAttribute("left", _htmlControlLeft.ToString() + "px");
            }
        }
        int _htmlControlTop = 0;
        [Category("HtmlHost")]
        public int HtmlControlTop
        {
            get { return _htmlControlTop; }
            set
            {
                _htmlControlTop = value;
                if (divIFrameHost == null)
                    return;
                divIFrameHost.SetStyleAttribute("top", _htmlControlTop.ToString() + "px");
            }
        }

        readonly String _htmlControlId = System.Guid.NewGuid().ToString();
        [Category("HtmlHost")]
        public String HtmlControlId
        {
            get { return _htmlControlId; }
        }

        String _HTMLUrl;
        [Category("HtmlHost")]

        public String HTMLUrl
        {
            get { return _HTMLUrl; }
            set
            {
                _HTMLUrl = value;
                _HTMLString = "";
                if (divIFrameHost == null)
                    return;
                if (iFrame != null)
                    divIFrameHost.RemoveChild(iFrame);
                iFrame = CreateIFrameFromURL();
                divIFrameHost.AppendChild(iFrame);
            }
        }

        String _HTMLString;
        [Category("HtmlHost")]
        public String HTMLString
        {
            get { return _HTMLString; }
            set
            {
                _HTMLString = value;
                _HTMLUrl = "";
                if (divIFrameHost == null)
                    return;
                if (iFrame != null)
                    divIFrameHost.RemoveChild(iFrame);
                iFrame = CreateIFrameFromString();
                divIFrameHost.AppendChild(iFrame);
            }
        }

        int _htmlZIndex = 1;
        [Category("HtmlHost")]
        public int HtmlZIndex
        {
            get { return _htmlZIndex; }
            set
            {
                _htmlZIndex = value;
                if (divIFrameHost != null)
                    divIFrameHost.SetStyleAttribute("z-index", _htmlZIndex.ToString());
            }
        }

        private HtmlElement divIFrameHost;
        private int padding = 5;
        public HtmlHost()
        {
            Loaded += new RoutedEventHandler(HtmlHost_Loaded);
            SizeChanged += new SizeChangedEventHandler(HtmlHost_SizeChanged);
            Width = Height = 100;
        }

        void HtmlHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (divIFrameHost == null)
                return;

            divIFrameHost.SetStyleAttribute("height", (Height - padding * 2).ToString() + "px");
            divIFrameHost.SetStyleAttribute("width", (Width - padding * 2).ToString() + "px");
        }

        void HtmlHost_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
            RegisterForNotification("Parent", this, new PropertyChangedCallback(OnParentValueChanged));

            RegisterForNotification("(Canvas.Left)", this, new PropertyChangedCallback(OnLeftValueChanged));
            RegisterForNotification("(Canvas.Top)", this, new PropertyChangedCallback(OnTopValueChanged));
        }

        public void InitializeComponent()
        {
            divIFrameHost = HtmlPage.Document.CreateElement("div");
            divIFrameHost.SetStyleAttribute("position", "absolute");
            divIFrameHost.SetAttribute("id", System.Guid.NewGuid().ToString());
            divIFrameHost.SetStyleAttribute("left", _htmlControlLeft.ToString() + "px");
            divIFrameHost.SetStyleAttribute("top", _htmlControlTop.ToString() + "px");
            divIFrameHost.SetStyleAttribute("z-index", _htmlZIndex.ToString());

            HtmlPage.Document.Body.AppendChild(divIFrameHost);
        }
        HtmlElement iFrame;
        private HtmlElement CreateIFrameFromURL()
        {
            HtmlElement element;
            element = HtmlPage.Document.CreateElement("IFRAME");
            element.SetAttribute("src", _HTMLUrl);
            element.SetStyleAttribute("height", "100%");
            element.SetStyleAttribute("width", "100%");
            element.SetStyleAttribute("left", "0px");
            element.SetStyleAttribute("position", "relative");
            element.SetStyleAttribute("top", "0px");
            element.Id = System.Guid.NewGuid().ToString();
            return element;
        }

        private HtmlElement CreateIFrameFromString()
        {
            HtmlElement element;
            element = HtmlPage.Document.CreateElement("DIV");

            if (HtmlPage.BrowserInformation.Name == "Netscape")
                element.SetProperty("innerHTML", _HTMLString);
            else
                element.SetAttribute("innerHTML", _HTMLString);

            element.SetStyleAttribute("overflow", "scroll");
            element.SetStyleAttribute("height", "100%");
            element.SetStyleAttribute("width", "100%");
            element.SetStyleAttribute("left", "0px");
            element.SetStyleAttribute("position", "relative");
            element.SetStyleAttribute("top", "0px");
            element.Id = System.Guid.NewGuid().ToString();
            return element;
        }

        private void RegisterForNotification(string propertyName, FrameworkElement element, PropertyChangedCallback callback)
        {
            //Bind to a depedency property
            Binding b = new Binding(propertyName) { Source = element };
            var prop = System.Windows.DependencyProperty.RegisterAttached(
                "ListenAttached" + propertyName,
                typeof(object),
                typeof(UserControl),
                new System.Windows.PropertyMetadata(callback));
            element.SetBinding(prop, b);
        }

        private void OnLeftValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement parent = o as FrameworkElement;
            parent = parent.Parent as FrameworkElement;
            GeneralTransform gt = parent.TransformToVisual(Application.Current.RootVisual);
            Point p = gt.Transform(new Point(0, 0));
            int d = int.Parse(args.NewValue.ToString());
            HtmlControlLeft = (int)p.X + d + padding;
            //MessageBox.Show("Left " + parent.Name + " " + d + " " + p.X.ToString() + " " + HtmlControlLeft.ToString());
        }

        private void OnTopValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement parent = o as FrameworkElement;
            parent = parent.Parent as FrameworkElement;
            GeneralTransform gt = parent.TransformToVisual(Application.Current.RootVisual);
            Point p = gt.Transform(new Point(0, 0));
            int d = int.Parse(args.NewValue.ToString());
            HtmlControlTop = (int)p.Y + d + padding;
            // MessageBox.Show("Top " + parent.Name + " " + d + " " + p.Y.ToString() + " " + HtmlControlTop.ToString());
        }

        private void OnParentValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement parent = args.NewValue as FrameworkElement;
            parent = parent.Parent as FrameworkElement;
            //MessageBox.Show(parent.Name + " : " + parent.GetType().ToString());

            RegisterForNotification("(Canvas.Left)", parent, new PropertyChangedCallback(OnLeftValueChanged));
            RegisterForNotification("(Canvas.Top)", parent, new PropertyChangedCallback(OnTopValueChanged));
        }

        public override void Dispose()
        {
            HtmlPage.Document.Body.RemoveChild(divIFrameHost);
        }
    }
}
