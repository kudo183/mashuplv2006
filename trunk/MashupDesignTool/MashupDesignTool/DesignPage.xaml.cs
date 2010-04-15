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

namespace MashupDesignTool
{
    public partial class MainPage : UserControl
    {
        Dictionary<String, Assembly> LoadedAssembly = new Dictionary<String, Assembly>();
        String controlName = "";
        String clientRoot = "";

        public MainPage()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            string absoluteUri = Application.Current.Host.Source.AbsoluteUri;
            int lastSlash = absoluteUri.LastIndexOf("/");
            clientRoot = absoluteUri.Substring(0, lastSlash + 1);

            DownloadControlInfo();
        }

        #region download info.xml and contruct control tree
        private void DownloadControlInfo()
        {
            //Get the Uri to ClientBin directory:
            string absoluteUri = Application.Current.Host.Source.AbsoluteUri;
            int lastSlash = absoluteUri.LastIndexOf("/");
            string assemblyPath = absoluteUri.Substring(0, lastSlash + 1);
            //And tack on name of assembly:
            assemblyPath += "controls/info.xml";

            Uri uri = new Uri(assemblyPath, UriKind.Absolute);
            //Start an async download:
            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_DownloadControlInfoCompleted);
            webClient.OpenReadAsync(uri);
        }

        private void webClient_DownloadControlInfoCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            XmlReader reader = XmlReader.Create(e.Result);
            reader.Read();
            int number;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "number")
                    {
                        number = int.Parse(reader.ReadInnerXml());
                    }
                    else if (reader.Name == "name")
                    {
                        AddControlToTree(reader.ReadInnerXml(), "test control");
                    }
                }
            }
        }

        private void AddTreeViewItem(TreeView t, string itemName, object item)
        {
            foreach (TreeViewItem tv in t.Items)
                if (itemName == tv.Header.ToString())
                {
                    tv.Items.Add(item);
                    return;
                }
            TreeViewItem tvi = new TreeViewItem();
            tvi.Header = itemName;
            tvi.Items.Add(item);
            t.Items.Add(tvi);
        }

        private void AddControlToTree(string name, string group)
        {
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;

            String imagePath = clientRoot + "controls/" + name + ".png";

            Image im = new Image();
            im.Source = new BitmapImage(new Uri(imagePath));
            Label lb = new Label();
            lb.Content = name;
            lb.Margin = new Thickness(lb.Margin.Left + 5, lb.Margin.Top, lb.Margin.Right, lb.Margin.Bottom);
            sp.Children.Add(im);
            sp.Children.Add(lb);

            sp.MouseLeftButtonDown += new MouseButtonEventHandler(sp_MouseLeftButtonDown);
            AddTreeViewItem(TreeControl, group, sp);
        }
        #endregion

        private void DownloadControl()
        {
            String assemblyPath = clientRoot + "controls/" + controlName + ".dll";

            Uri uri = new Uri(assemblyPath, UriKind.Absolute);
            //Start an async download:
            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_DownloadControlCompleted);
            webClient.OpenReadAsync(uri);
        }

        private void webClient_DownloadControlCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            AssemblyPart assemblyPart = new AssemblyPart();
            Assembly assembly = assemblyPart.Load(e.Result);
            //UIElement o = assembly.CreateInstance("Plugin.Controls." + controlName) as UIElement;            
            LoadedAssembly.Add(controlName, assembly);


            ////////////////////////////////////////////////////////////////////////////////////
            //test thoi
            UserControl o = LoadedAssembly[controlName].CreateInstance("control." + controlName) as UserControl;
            //UIElement o = LoadedAssembly[controlName].CreateInstance(controlName) as UIElement;
            if (o == null)
                return;
            //spanel.Children[1] = o;
            //InitControl(o, 500, 400, 100.0, 100.0);

            designCanvas1.AddControl(o, 100.0, 100.0, 50, 40);

            //string content = "";
            //Type t = spanel.GetType();
            //foreach (PropertyInfo pinfo in t.GetProperties())
            //{
            //    content += pinfo.ToString() + "\n";
            //}
            //textBlock1.Text = content;
        }

        //when user click a control, download it if needed.
        private void sp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel sp = (StackPanel)sender;
            Label lb = (Label)sp.Children[1];
            controlName = lb.Content.ToString();
            if (LoadedAssembly.Keys.Contains(controlName) == true)
                return;
            DownloadControl();
        }
    }
}
