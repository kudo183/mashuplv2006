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
using System.Xml.Linq;
using System.Windows.Threading;

namespace MashupDesignTool
{
    public partial class MainPage : UserControl
    {
        Dictionary<String, Assembly> LoadedAssembly = new Dictionary<String, Assembly>();
        String controlName = "";
        String clientRoot = "";
        DispatcherTimer doubleClickTimer;

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
            doubleClickTimer = new DispatcherTimer();
            doubleClickTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            doubleClickTimer.Tick += new EventHandler(DoubleClick_Timer);
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
            XDocument document = XDocument.Load(reader);

            string name, displayName, description, group, iconName;
            foreach (XElement element in document.Descendants("Control"))
            {
                name = displayName = description = group = iconName = "";
                try
                {
                    name = element.Element("name").Value;
                    displayName = element.Element("displayname").Value;
                    description = element.Element("description").Value;
                    group = element.Element("group").Value;
                    iconName = element.Element("iconname").Value;
                }
                catch { }
                AddControlToTree(name, displayName, description, iconName, group);
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

        private void AddControlToTree(string name, string displayName, string description, string iconName, string group)
        {
            ControlTreeViewItem item = new ControlTreeViewItem();
            item.SetControlIcon(clientRoot + "controls/" + iconName);
            item.SetControlName(name);
            item.SetControlDescription(description);
            item.SetControlDisplayName(displayName);

            item.MouseLeftButtonDown += new MouseButtonEventHandler(item_MouseLeftButtonDown);
            AddTreeViewItem(TreeControl, group, item);
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
            LoadedAssembly.Add(controlName, assembly);
        }

        //when user click a control, download it if needed.
        private void item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (doubleClickTimer.IsEnabled)
            {
                doubleClickTimer.Stop();
                UserControl o = LoadedAssembly[controlName].CreateInstance("control." + controlName) as UserControl;
                if (o == null)
                    return; 
                designCanvas1.AddControl(o, 100.0, 100.0, 50, 40);
            }
            else
            {
                doubleClickTimer.Start();
                ControlTreeViewItem item = (ControlTreeViewItem)sender;
                controlName = item.GetControlName();
                if (LoadedAssembly.Keys.Contains(controlName) == true)
                    return;
                DownloadControl();
            }
        }

        void DoubleClick_Timer(object sender, EventArgs e)
        {
            doubleClickTimer.Stop();
        }

        private void TreeControl_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            doubleClickTimer.Stop();
        }
    }
}
