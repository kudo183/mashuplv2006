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
using System.Text;
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
                if (string.IsNullOrEmpty(value))
                    return;
                _HTMLUrl = value;
                _HTMLString = "";
                
                if (divIFrameHost == null)
                    return;
                if (iFrame == null)
                {
                    iFrame = CreateIFrame();
                    divIFrameHost.AppendChild(iFrame);
                }
                SetIFrameSrc(_HTMLUrl);
            }
        }

        String _HTMLString;
        [Category("HtmlHost")]
        public String HTMLString
        {
            get { return _HTMLString; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                _HTMLString = value;
                _HTMLUrl = "";

                if (divIFrameHost == null)
                    return;
                if (iFrame != null)                   
                    divIFrameHost.RemoveChild(iFrame);
                iFrame = CreateIFrame();
                divIFrameHost.AppendChild(iFrame);

                SetIFrameContent(_HTMLString);
            }
        }

        private HtmlElement divIFrameHost;
        private int padding = 10;
        HtmlElement iFrame;
        string str_iFrame_id = "";
        bool _Visible;
        public HtmlHost()
        {
            Loaded += new RoutedEventHandler(HtmlHost_Loaded);
            Unloaded += new RoutedEventHandler(HtmlHost_Unloaded);
            LayoutUpdated += new EventHandler(HtmlHost_LayoutUpdated);
            SizeChanged += new SizeChangedEventHandler(HtmlHost_SizeChanged);
            Width = Height = 100;
            parameterNameList.Add("HTMLString");
            parameterNameList.Add("HTMLUrl");
            _Visible = true;
        }

        void HtmlHost_Unloaded(object sender, RoutedEventArgs e)
        {
            _Visible = (Visibility == System.Windows.Visibility.Visible) ? true : false;
            Visibility = System.Windows.Visibility.Collapsed;
        }

        void HtmlHost_LayoutUpdated(object sender, EventArgs e)
        {
            GeneralTransform gt = TransformToVisual(Application.Current.RootVisual);
            Point p = gt.Transform(new Point(0, 0));
            
            HtmlControlLeft = (int)p.X + padding - 1;
            HtmlControlTop = (int)p.Y + padding - 1;
        }

        void HtmlHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (divIFrameHost == null)
                return;

            divIFrameHost.SetStyleAttribute("height", (Height - padding * 2).ToString() + "px");
            divIFrameHost.SetStyleAttribute("width", (Width - padding * 2).ToString() + "px");
        }

        void UpdateControl()
        {
            if (divIFrameHost == null)
                return;

            divIFrameHost.SetStyleAttribute("height", (Height - padding * 2).ToString() + "px");
            divIFrameHost.SetStyleAttribute("width", (Width - padding * 2).ToString() + "px");

            GeneralTransform gt = TransformToVisual(Application.Current.RootVisual);
            Point p = gt.Transform(new Point(0, 0));

            HtmlControlLeft = (int)p.X + padding - 1;
            HtmlControlTop = (int)p.Y + padding - 1;
        }

        void HtmlHost_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
            Visibility = (_Visible) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            UpdateControl();
        }

        public void InitializeComponent()
        {
            if (divIFrameHost != null)
                return;
            divIFrameHost = HtmlPage.Document.CreateElement("div");
            divIFrameHost.SetAttribute("id", _htmlControlId);

            divIFrameHost.SetStyleAttribute("position", "absolute");
            divIFrameHost.SetStyleAttribute("left", _htmlControlLeft.ToString() + "px");
            divIFrameHost.SetStyleAttribute("top", _htmlControlTop.ToString() + "px");
            
            HtmlPage.Document.Body.AppendChild(divIFrameHost);
            Ultility.RegisterForNotification("Visibility", this, VisibilityChanged);

            if (string.IsNullOrEmpty(_HTMLString))
                HTMLUrl = _HTMLUrl;
            else
                HTMLString = _HTMLString;
        }

        void VisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (Visibility == System.Windows.Visibility.Collapsed)
                divIFrameHost.SetStyleAttribute("display", "none");
            else
                divIFrameHost.SetStyleAttribute("display", "block");
        }

        private void SetIFrameSrc(string url)
        {
            StringBuilder script = new StringBuilder();
            
            script.Append("var iframe = document.getElementById(\"" + str_iFrame_id + "\");");            
            script.Append("iframe.src = \"" + url + "\";");

            HtmlPage.Window.Eval(script.ToString());
        }

        private void SetIFrameContent(string content)
        {
            StringBuilder script = new StringBuilder();
            
            script.Append("var doc = document.getElementById(\"" + str_iFrame_id + "\").contentDocument;");
            
            script.Append("doc.open();");
            script.Append("doc.write(\"<div>" + content.Replace("\"", "\\\"") + "</div>\");");
            script.Append("doc.close();");

            HtmlPage.Window.Eval(script.ToString());
            
            //HtmlPage.Window.Eval("ChangeIFrameContent(\"" + str_iFrame_id + "\"" + "," + "\"" + content.Replace("\"", "\\\"") + "\"" + ")");
        }
        
        private HtmlElement CreateIFrame()
        {
            HtmlElement element;
            element = HtmlPage.Document.CreateElement("IFRAME");
            element.SetAttribute("src", "");
            element.Id = Guid.NewGuid().ToString();
            str_iFrame_id = element.Id;

            element.SetStyleAttribute("height", "100%");
            element.SetStyleAttribute("width", "100%");
            element.SetStyleAttribute("left", "0px");
            element.SetStyleAttribute("position", "relative");
            element.SetStyleAttribute("top", "0px");

            //element.Id = str_iFrame_id; tuong duong voi element.SetAttribute("ID", str_iFrame_id);
            return element;
        }
        
        public override void Dispose()
        {
            HtmlPage.Document.Body.RemoveChild(divIFrameHost);
        }
    }
}
