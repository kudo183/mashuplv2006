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
        Dictionary<string, Assembly> LoadedAssembly = new Dictionary<string, Assembly>();
        Dictionary<string, Assembly> LoadingAssembly = new Dictionary<string, Assembly>();
        //List<string> downloadingDllReferences = new List<string>();
        //List<string> downloadingDllFilename = new List<string>();
        Dictionary<WebClient, string> downloadingDllReferences = new Dictionary<WebClient, string>();
        Dictionary<WebClient, string> downloadingDllFilenames = new Dictionary<WebClient, string>();
        String clientRoot = "";
        DispatcherTimer doubleClickTimer;
        double toolbarWidthBeforeCollapse;
        double propertiesGridWidthBeforeCollapse;
        List<ControlInfo> listControls =  new List<ControlInfo>();

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

            propertiesGrid.SelectedObject = designCanvas1.RootCanvas;
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

            foreach (XElement element in document.Descendants("Control"))
            {
                ControlInfo ci = new ControlInfo(element);
                listControls.Add(ci);
                AddControlToTree(ci);
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

        private void AddControlToTree(ControlInfo ci)
        {
            ControlTreeViewItem item = new ControlTreeViewItem(ci, clientRoot);
            item.MouseLeftButtonDown += new MouseButtonEventHandler(item_MouseLeftButtonDown);
            AddTreeViewItem(TreeControl, ci.Group, item);
        }
        #endregion

        private void DownloadControl(ControlInfo ci)
        {
            String assemblyPath = clientRoot + "Controls/ControlDll/" + ci.DllFilename;

            if (!ci.IsDllFileDownloaded)
            {
                if (!downloadingDllFilenames.ContainsValue(assemblyPath))
                {
                    Uri uri = new Uri(assemblyPath, UriKind.Absolute);
                    //Start an async download:
                    WebClient webClient = new WebClient();
                    webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_DownloadControlCompleted);
                    webClient.OpenReadAsync(uri);
                    downloadingDllFilenames.Add(webClient, ci.ControlName);
                }
            }

            for (int i = 0; i < ci.DllReferences.Count; i++)
            {
                if (!ci.IsDllReferencesDownloaded[i])
                {
                    assemblyPath = clientRoot + "Controls/ReferenceDll/" + ci.DllReferences[i];
                    if (!downloadingDllReferences.ContainsValue(ci.DllReferences[i]))
                    {
                        Uri uri = new Uri(assemblyPath, UriKind.Absolute);
                        //Start an async download:
                        WebClient webClient = new WebClient();
                        webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_DownloadDllDependenceCompleted);
                        webClient.OpenReadAsync(uri);
                        downloadingDllReferences.Add(webClient, ci.DllReferences[i]);
                    }
                }
            }
        }

        private void webClient_DownloadControlCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                string controlName = downloadingDllFilenames[(WebClient)sender];
                downloadingDllFilenames.Remove((WebClient)sender);
                AssemblyPart assemblyPart = new AssemblyPart();
                Assembly assembly = assemblyPart.Load(e.Result);
                for (int i = 0; i < listControls.Count; i++)
                {
                    if (listControls[i].ControlName == controlName)
                    {
                        listControls[i].IsDllFileDownloaded = true;
                        if (listControls[i].IsReady)
                            LoadedAssembly.Add(controlName, assembly);
                        else
                            LoadingAssembly.Add(controlName, assembly);
                    }
                }
            }
        }

        private void webClient_DownloadDllDependenceCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                string dll = downloadingDllReferences[(WebClient)sender];
                downloadingDllReferences.Remove((WebClient)sender);
                AssemblyPart assemblyPart = new AssemblyPart();
                Assembly assembly = assemblyPart.Load(e.Result);

                for (int i = 0; i < listControls.Count; i++)
                {
                    string controlName = listControls[i].ControlName;
                    if (!LoadedAssembly.ContainsKey(controlName))
                    {
                        listControls[i].CheckDllReferences(dll);
                        if (listControls[i].IsReady)
                        {
                            LoadedAssembly.Add(controlName, LoadingAssembly[controlName]);
                            LoadingAssembly.Remove(controlName);
                        }
                    }
                }
            }
        }

        //when user click a control, download it if needed.
        private void item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ControlTreeViewItem item = (ControlTreeViewItem)sender; 
            if (doubleClickTimer.IsEnabled)
            {
                doubleClickTimer.Stop();
                if (!LoadedAssembly.ContainsKey(item.ControlInfo.ControlName))
                    return;
                UserControl uc = LoadedAssembly[item.ControlInfo.ControlName].CreateInstance(item.ControlInfo.ControlName) as UserControl;
                if (uc == null)
                    return;
                designCanvas1.AddControl(uc, 100.0, 100.0, 400, 300);
            }
            else
            {
                doubleClickTimer.Start();
                if (LoadedAssembly.Keys.Contains(item.ControlInfo.ControlName) == true)
                    return;
                DownloadControl(item.ControlInfo);
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

        private void designCanvas1_SelectPropertiesMenu(object sender, UIElement element)
        {
            propertiesGrid.SelectedObject = element;
            if (propertiesGrid.Visibility == System.Windows.Visibility.Collapsed)
            {
                ExpandPropertiesGridPanelColumn.To = propertiesGridWidthBeforeCollapse;
                ExpandPropertiesGridPanel.Begin();
            }
        }

        private void designCanvas1_SelectionChanged(object sender, UIElement element)
        {
            propertiesGrid.SelectedObject = element;
        }

        private void ExpanderToolbar_Click(object sender, RoutedEventArgs e)
        {
            if (TreeControl.Visibility == System.Windows.Visibility.Visible)
            {
                toolbarWidthBeforeCollapse = ToolbarPanel.ActualWidth;
                CollapseToolbarPanelColumn.From = toolbarWidthBeforeCollapse;
                CollapseToolbarPanel.Begin();
            }
            else
            {
                ExpandToolbarPanelColumn.To = toolbarWidthBeforeCollapse;
                ExpandToolbarPanel.Begin();
            }
        }

        private void PropertiesGridExpander_Click(object sender, RoutedEventArgs e)
        {
            if (propertiesGrid.Visibility == System.Windows.Visibility.Visible)
            {
                propertiesGridWidthBeforeCollapse = PropertiesGridPanel.ActualWidth;
                CollapsePropertiesGridPanelColumn.From = propertiesGridWidthBeforeCollapse;
                CollapsePropertiesGridPanel.Begin();
            }
            else
            {
                ExpandPropertiesGridPanelColumn.To = propertiesGridWidthBeforeCollapse;
                ExpandPropertiesGridPanel.Begin();
            }
        }

        private static readonly DependencyProperty ColumnToolbarWidthProperty = DependencyProperty.Register("ColumnToolbarWidth", typeof(double), typeof(MainPage), new PropertyMetadata(ColumnToolbarWidthChanged));

        private double ColumnToolbarWidth
        {
            get { return (double)GetValue(ColumnToolbarWidthProperty); }
            set { SetValue(ColumnToolbarWidthProperty, value); }
        }

        private static void ColumnToolbarWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MainPage)d).ToolbarColumn.Width = new GridLength((double)e.NewValue);
        }

        private static readonly DependencyProperty ColumnPropertiesGridWidthProperty = DependencyProperty.Register("ColumnPropertiesGridWidth", typeof(double), typeof(MainPage), new PropertyMetadata(ColumnPropertiesGridWidthChanged));

        private double ColumnPropertiesGridWidth
        {
            get { return (double)GetValue(ColumnPropertiesGridWidthProperty); }
            set { SetValue(ColumnPropertiesGridWidthProperty, value); }
        }

        private static void ColumnPropertiesGridWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MainPage)d).PropertiesGridColumn.Width = new GridLength((double)e.NewValue);
        }
    }
}
