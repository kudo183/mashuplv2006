﻿using System;
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
using BasicLibrary;
using MapulRibbon;

namespace MashupDesignTool
{
    public partial class MainPage : UserControl
    {
        Dictionary<string, Assembly> LoadedAssembly = new Dictionary<string, Assembly>();
        Dictionary<string, Assembly> LoadingAssembly = new Dictionary<string, Assembly>();
        Dictionary<WebClient, string> downloadingDllReferences = new Dictionary<WebClient, string>();
        Dictionary<WebClient, string> downloadingDllFilenames = new Dictionary<WebClient, string>();
        String clientRoot = "";
        string downloadingControlName = "";
        string downloadingEffectName = "";
        PropertyInfo piEffect;
        bool bAdd = false, bChange = false;
        DispatcherTimer doubleClickTimer;
        double toolbarWidthBeforeCollapse;
        double propertiesGridWidthBeforeCollapse;
        List<ControlInfo> listControls = new List<ControlInfo>();
        List<EffectInfo> listSingleEffects = new List<EffectInfo>();
        List<EffectInfo> listListEffects = new List<EffectInfo>();
        Dictionary<string, Assembly> LoadedEffectAssembly = new Dictionary<string, Assembly>();
        Dictionary<string, Assembly> LoadingEffectAssembly = new Dictionary<string, Assembly>();
        Dictionary<WebClient, string> downloadingEffectDllReferences = new Dictionary<WebClient, string>();
        Dictionary<WebClient, string> downloadingEffectDllFilenames = new Dictionary<WebClient, string>();

        public MainPage()
        {
            InitializeComponent();
            propertiesGrid.SelectedObjectParent = designCanvas1.ControlContainer;
            designCanvas1.PositionChanged += new DesignCanvas.PositionChangedHander(designCanvas1_PositionChanged);
            designCanvas1.ZIndexChanged += new DesignCanvas.ZIndexChangedHandler(designCanvas1_ZIndexChanged);

            propertiesGrid.PropertyValueChange += new SL30PropertyGrid.PropertyGrid.OnPropertyValueChange(propertiesGrid_PropertyValueChange);
        }

        void propertiesGrid_PropertyValueChange(UIElement ui, string name, object value)
        {
            if (ui.Equals(designCanvas1.ControlContainer))
            {
                switch (name)
                {
                    case "Width":
                        designCanvas1.LayoutRoot.Width = designCanvas1.ControlContainer.Width;
                        designCanvas1.ControlContainer.UpdateChildrenPosition();
                        break;
                    case "Height":
                        designCanvas1.LayoutRoot.Height = designCanvas1.ControlContainer.Height;
                        designCanvas1.ControlContainer.UpdateChildrenPosition();
                        break;
                    default:
                        break;
                }
            }

            else
            {
                ProxyControl pc = designCanvas1.SelectedProxyControls[0];
                EffectableControl ec = designCanvas1.SelectedControls[0];
                DockCanvas.DockCanvas.DockType dt = DockCanvas.DockCanvas.GetDockType(ec);

                switch (name)
                {
                    case "Left":
                        if (dt == DockCanvas.DockCanvas.DockType.None)
                            pc.SetX(double.Parse((string)value));
                        break;
                    case "Top":
                        if (dt == DockCanvas.DockCanvas.DockType.None)
                            pc.SetY(double.Parse((string)value));
                        break;
                    case "ZIndex":
                        designCanvas1.SetZindex(pc, int.Parse((string)value));
                        propertiesGrid.UpdatePropertyValue("Left");
                        propertiesGrid.UpdatePropertyValue("Top");
                        propertiesGrid.UpdatePropertyValue("Width");
                        propertiesGrid.UpdatePropertyValue("Height");
                        break;
                    case "Width":
                        if (dt == DockCanvas.DockCanvas.DockType.None
                            || dt == DockCanvas.DockCanvas.DockType.Left
                            || dt == DockCanvas.DockCanvas.DockType.Right)
                        {
                            pc.SetWidth(double.Parse((string)value));
                            designCanvas1.ControlContainer.UpdateChildrenPosition();
                            designCanvas1.UpdateAllProxyControlPosition();
                        }
                        break;
                    case "Height":
                        if (dt == DockCanvas.DockCanvas.DockType.None
                            || dt == DockCanvas.DockCanvas.DockType.Top
                            || dt == DockCanvas.DockCanvas.DockType.Bottom)
                        {
                            pc.SetHeight(double.Parse((string)value));
                            designCanvas1.ControlContainer.UpdateChildrenPosition();
                            designCanvas1.UpdateAllProxyControlPosition();
                        }
                        break;
                    case "DockType":
                        DockCanvas.DockCanvas.SetDockType(designCanvas1.SelectedControls[0], (DockCanvas.DockCanvas.DockType)Enum.Parse(typeof(DockCanvas.DockCanvas.DockType), (string)value, true));
                        designCanvas1.ControlContainer.UpdateChildrenPosition();
                        designCanvas1.UpdateAllProxyControlPosition();
                        propertiesGrid.UpdatePropertyValue("Left");
                        propertiesGrid.UpdatePropertyValue("Top");
                        propertiesGrid.UpdatePropertyValue("Width");
                        propertiesGrid.UpdatePropertyValue("Height");
                        break;
                    default:
                        break;
                }
            }
        }

        void designCanvas1_ZIndexChanged(object sender, int zindex)
        {
            propertiesGrid.UpdatePropertyValue("ZIndex");
            propertiesGrid.UpdatePropertyValue("Left");
            propertiesGrid.UpdatePropertyValue("Top");
            propertiesGrid.UpdatePropertyValue("Width");
            propertiesGrid.UpdatePropertyValue("Height");
        }

        void designCanvas1_PositionChanged(object sender, bool multiControl)
        {
            if (multiControl)
                return;

            propertiesGrid.UpdatePropertyValue("Left");
            propertiesGrid.UpdatePropertyValue("Top");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            string absoluteUri = Application.Current.Host.Source.AbsoluteUri;
            int lastSlash = absoluteUri.LastIndexOf("/");
            clientRoot = absoluteUri.Substring(0, lastSlash + 1);

            DownloadControlInfo();
            DownloadEffectInfo();
            doubleClickTimer = new DispatcherTimer();
            doubleClickTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            doubleClickTimer.Tick += new EventHandler(DoubleClick_Timer);

            propertiesGrid.SelectedObject = designCanvas1.RootCanvas;
        }

        #region download Controls/info.xml and contruct control tree
        private void DownloadControlInfo()
        {
            //Get the Uri to ClientBin directory:
            string absoluteUri = Application.Current.Host.Source.AbsoluteUri;
            int lastSlash = absoluteUri.LastIndexOf("/");
            string assemblyPath = absoluteUri.Substring(0, lastSlash + 1);
            //And tack on name of assembly:
            assemblyPath += "Controls/info.xml";

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

        private void AddTreeViewItem(TreeView t, string group, object item)
        {
            foreach (TreeViewItem tv in t.Items)
                if (group == tv.Header.ToString())
                {
                    tv.Items.Add(item);
                    return;
                }
            TreeViewItem tvi = new TreeViewItem();
            tvi.Header = group;
            tvi.Items.Add(item);
            t.Items.Add(tvi);
        }

        private void AddControlToTree(ControlInfo ci)
        {
            ControlTreeViewItem item = new ControlTreeViewItem(ci, clientRoot);
            item.ControlIcon.Source = new BitmapImage(new Uri(clientRoot + @"Controls/Images/" + ci.IconName, UriKind.Absolute));
            item.MouseLeftButtonDown += new MouseButtonEventHandler(item_MouseLeftButtonDown);
            AddTreeViewItem(TreeControl, ci.Group, item);
        }
        #endregion

        #region download Effects/info.xml
        private void DownloadEffectInfo()
        {
            //Get the Uri to ClientBin directory:
            string absoluteUri = Application.Current.Host.Source.AbsoluteUri;
            int lastSlash = absoluteUri.LastIndexOf("/");
            string assemblyPath = absoluteUri.Substring(0, lastSlash + 1);
            //And tack on name of assembly:
            assemblyPath += "Effects/info.xml";

            Uri uri = new Uri(assemblyPath, UriKind.Absolute);
            //Start an async download:
            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_DownloadControlInfoCompleted1);
            webClient.OpenReadAsync(uri);
        }

        private void webClient_DownloadControlInfoCompleted1(object sender, OpenReadCompletedEventArgs e)
        {
            XmlReader reader = XmlReader.Create(e.Result);
            XDocument document = XDocument.Load(reader);

            foreach (XElement element in document.Descendants("Effect"))
            {
                EffectInfo ei = new EffectInfo(element);
                if (ei.Group == "List")
                    listListEffects.Add(ei);
                else
                    listSingleEffects.Add(ei);
            }
        }
        #endregion download Effects/info.xml

        #region control tree
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
                        {
                            LoadedAssembly.Add(controlName, assembly);
                            if (listControls[i].ControlName == downloadingControlName && bAdd == true)
                            {
                                AddControl(downloadingControlName);
                                HidePopup();
                            }
                        }
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
                            if (listControls[i].ControlName == downloadingControlName && bAdd == true)
                            {
                                AddControl(downloadingControlName);
                                HidePopup();
                            }
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
                {
                    DownloadControl(item.ControlInfo);
                    downloadingControlName = item.ControlInfo.ControlName;
                    ShowPopup();
                    return;
                }
                AddControl(item.ControlInfo.ControlName);
            }
            else
            {
                doubleClickTimer.Start();
            }
        }

        private void AddControl(string controlName)
        {
            BasicControl uc = LoadedAssembly[controlName].CreateInstance(controlName) as BasicControl;
            if (uc != null)
                designCanvas1.AddControl(uc, 100.0, 100.0, 400, 300);
        }

        void DoubleClick_Timer(object sender, EventArgs e)
        {
            doubleClickTimer.Stop();
        }

        private void TreeControl_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            doubleClickTimer.Stop();
        }
        #endregion control tree

        #region popup loading
        private void ShowPopup()
        {
            popupLoading.IsOpen = true;
            bAdd = true;
            bChange = true;
            IsEnabled = false;
        }

        private void HidePopup()
        {
            popupLoading.IsOpen = false;
            bAdd = false;
            bChange = false;
            IsEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HidePopup();
            downloadingControlName = "";
            downloadingEffectName = "";
        }
        #endregion popup loading

        #region select in design canvas
        private void designCanvas1_SelectPropertiesMenu(object sender, UIElement element)
        {
            propertiesGrid.SelectedObject = element;
            propertiesTabs.SelectedIndex = 0;
            if (propertiesTabs.Visibility == System.Windows.Visibility.Collapsed)
            {
                ExpandPropertiesGridPanelColumn.To = propertiesGridWidthBeforeCollapse;
                ExpandPropertiesGridPanel.Begin();
            }
        }

        private UIElement previousElement = null;
        private void designCanvas1_SelectionChanged(object sender, UIElement element)
        {
            if (!element.Equals(previousElement))
            {
                CleanEffectMenuItemsFromMenu();
                effectPropertiesGrid.SelectedObject = null;
            }

            if (element.Equals(designCanvas1.ControlContainer))
            {
                propertiesGrid.SelectedObject = element;
                previousElement = null;
            }
            else if (designCanvas1.SelectedControls.Count == 1)
            {
                if (!element.Equals(previousElement))
                {
                    propertiesGrid.SelectedObject = element;
                    AddEffectMenuItemsToMenu((EffectableControl)designCanvas1.SelectedControls[0]);
                }
                previousElement = designCanvas1.SelectedControls[0].Control;
            }
            else
            {
                propertiesGrid.SelectedObject = null;
                previousElement = null;
            }
        }

        private void CleanEffectMenuItemsFromMenu()
        {
            for (int i = 1; i < menu.Tabs.Items.Count; i++)
                menu.Tabs.Items.RemoveAt(i);
        }

        private void AddEffectMenuItemsToMenu(EffectableControl element)
        {
            PropertyInfo[] pis = element.Control.GetType().GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                if (typeof(BasicEffect).IsAssignableFrom(pi.PropertyType))
                {
                    AddEffectMenuItemToMenu(pi, true, element);
                }
                else if (typeof(BasicListEffect).IsAssignableFrom(pi.PropertyType))
                {
                    AddEffectMenuItemToMenu(pi, false, element);
                }
            }
        }

        private void AddEffectMenuItemToMenu(PropertyInfo pi, bool single, EffectableControl element)
        {
            RibbonSimpleListView listView = new RibbonSimpleListView();
            listView.Width = 600;
            listView.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            List<RibbonSimpleListViewSourceItem> list = new List<RibbonSimpleListViewSourceItem>();
            string str = "";
            int selectedIndex = -1;
            if (single)
            {
                BasicEffect be = (BasicEffect)element.Control.GetType().GetProperty(pi.Name).GetValue(element.Control, null); 
                if (be != null)
                    str = be.GetType().FullName;
                for (int i = 0; i < listSingleEffects.Count; i++)
                {
                    list.Add(new RibbonSimpleListViewSourceItem(listSingleEffects[i].DisplayName, clientRoot + @"Effects/Images/" + listSingleEffects[i].IconName, listSingleEffects[i]));
                    if (listSingleEffects[i].EffectName == str)
                        selectedIndex = i;
                }
            }
            else
            {
                BasicListEffect ble = (BasicListEffect)element.Control.GetType().GetProperty(pi.Name).GetValue(element.Control, null);
                if (ble != null)
                    str = ble.GetType().FullName;
                for (int i = 0; i < listListEffects.Count; i++)
                {
                    list.Add(new RibbonSimpleListViewSourceItem(listListEffects[i].DisplayName, clientRoot + @"Effects/Images/" + listListEffects[i].IconName, listListEffects[i]));
                    if (listSingleEffects[i].EffectName == str)
                        selectedIndex = i;
                }
            }
            listView.ListSourceItem = list;
            listView.Height = 60;
            listView.SelectedIndex = selectedIndex;
            listView.Tag = pi;
            listView.SelectionChanged += new SelectionChangedEventHandler(listView_SelectionChanged);

            RibbonButton rb = new RibbonButton();
            rb.ImageUrl = new BitmapImage(new Uri("Images/Delete.png", UriKind.Relative));
            rb.Text = "Effect options";
            rb.TooltipText = "Effect options";
            rb.Tag = pi;
            rb.OnClick += new RoutedEventHandler(rb_OnClick);

            RibbonButtonsGroup rbg = new RibbonButtonsGroup();
            rbg.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            rbg.Orientation = Orientation.Horizontal;
            rbg.Children.Add(listView);
            rbg.Children.Add(rb);

            RibbonItem ri = new RibbonItem();
            ri.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            if (pi.Name == "MainEffect")
                ri.Title = "Effects for this control";
            else
                ri.Title = "Effects for " + pi.Name;
            ri.Content = rbg;
            
            RibbonItems ris = new RibbonItems();
            ris.Children.Add(ri);
            ris.Margin = new Thickness(0, -2, 0, 0);

            TabsItem ti = new TabsItem();
            ti.Header = pi.Name;
            ti.Content = ris;
            ti.Tabs = menu.Tabs;
            ti.Tag = pi;

            menu.Tabs.Items.Add(ti);
        }

        void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RibbonSimpleListView listView = (RibbonSimpleListView)sender;
            RibbonSimpleListViewSourceItem item = (RibbonSimpleListViewSourceItem)e.AddedItems[0];
            EffectInfo ei = (EffectInfo)item.Data;
            piEffect = (PropertyInfo)listView.Tag;
            if (!LoadedEffectAssembly.ContainsKey(ei.EffectName))
            {
                DownloadEffect(ei);
                downloadingEffectName = ei.EffectName;
                ShowPopup();
                return;
            }
            ChangeEffect(ei.EffectName);
        }

        private void ChangeEffect(string effectName)
        {
            ((BasicControl)designCanvas1.SelectedControls[0].Control).ChangeEffect(piEffect.Name, LoadedEffectAssembly[effectName].GetType(effectName), designCanvas1.SelectedControls[0]);
            effectPropertiesGrid.SelectedObject = piEffect.GetValue(designCanvas1.SelectedControls[0].Control, null);
        }

        private void DownloadEffect(EffectInfo ei)
        {
            String assemblyPath = clientRoot + "Effects/EffectDll/" + ei.DllFilename;

            if (!ei.IsDllFileDownloaded)
            {
                if (!downloadingEffectDllFilenames.ContainsValue(assemblyPath))
                {
                    Uri uri = new Uri(assemblyPath, UriKind.Absolute);
                    //Start an async download:
                    WebClient webClient = new WebClient();
                    webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_DownloadEffectCompleted);
                    webClient.OpenReadAsync(uri);
                    downloadingEffectDllFilenames.Add(webClient, ei.EffectName);
                }
            }

            for (int i = 0; i < ei.DllReferences.Count; i++)
            {
                if (!ei.IsDllReferencesDownloaded[i])
                {
                    assemblyPath = clientRoot + "Effects/ReferenceDll/" + ei.DllReferences[i];
                    if (!downloadingEffectDllReferences.ContainsValue(ei.DllReferences[i]))
                    {
                        Uri uri = new Uri(assemblyPath, UriKind.Absolute);
                        //Start an async download:
                        WebClient webClient = new WebClient();
                        webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_DownloadEffectDllDependenceCompleted);
                        webClient.OpenReadAsync(uri);
                        downloadingEffectDllReferences.Add(webClient, ei.DllReferences[i]);
                    }
                }
            }
        }

        private void webClient_DownloadEffectCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                string effectName = downloadingEffectDllFilenames[(WebClient)sender];
                downloadingEffectDllFilenames.Remove((WebClient)sender);
                AssemblyPart assemblyPart = new AssemblyPart();
                Assembly assembly = assemblyPart.Load(e.Result);
                for (int i = 0; i < listSingleEffects.Count; i++)
                {
                    if (listSingleEffects[i].EffectName == effectName)
                    {
                        listSingleEffects[i].IsDllFileDownloaded = true;
                        if (listSingleEffects[i].IsReady)
                        {
                            LoadedEffectAssembly.Add(effectName, assembly);
                            if (listSingleEffects[i].EffectName == downloadingEffectName && bChange == true)
                            {
                                ChangeEffect(downloadingEffectName);
                                HidePopup();
                            }
                        }
                        else
                            LoadingEffectAssembly.Add(effectName, assembly);
                    }
                }

                for (int i = 0; i < listListEffects.Count; i++)
                {
                    if (listListEffects[i].EffectName == effectName)
                    {
                        listListEffects[i].IsDllFileDownloaded = true;
                        if (listListEffects[i].IsReady)
                        {
                            LoadedEffectAssembly.Add(effectName, assembly);
                            if (listListEffects[i].EffectName == downloadingEffectName && bChange == true)
                            {
                                ChangeEffect(downloadingEffectName);
                                HidePopup();
                            }
                        }
                        else
                            LoadingEffectAssembly.Add(effectName, assembly);
                    }
                }
            }
        }

        private void webClient_DownloadEffectDllDependenceCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                string dll = downloadingEffectDllReferences[(WebClient)sender];
                downloadingEffectDllReferences.Remove((WebClient)sender);
                AssemblyPart assemblyPart = new AssemblyPart();
                Assembly assembly = assemblyPart.Load(e.Result);

                for (int i = 0; i < listSingleEffects.Count; i++)
                {
                    string effectName = listSingleEffects[i].EffectName;
                    if (!LoadedEffectAssembly.ContainsKey(effectName))
                    {
                        listSingleEffects[i].CheckDllReferences(dll);
                        if (listSingleEffects[i].IsReady)
                        {
                            LoadedEffectAssembly.Add(effectName, LoadingEffectAssembly[effectName]);
                            LoadingEffectAssembly.Remove(effectName);
                            if (listSingleEffects[i].EffectName == downloadingEffectName && bChange == true)
                            {
                                ChangeEffect(downloadingEffectName);
                                HidePopup();
                            }
                        }
                    }
                }

                for (int i = 0; i < listListEffects.Count; i++)
                {
                    string effectName = listListEffects[i].EffectName;
                    if (!LoadedEffectAssembly.ContainsKey(effectName))
                    {
                        listListEffects[i].CheckDllReferences(dll);
                        if (listListEffects[i].IsReady)
                        {
                            LoadedEffectAssembly.Add(effectName, LoadingEffectAssembly[effectName]);
                            LoadingEffectAssembly.Remove(effectName);
                            if (listListEffects[i].EffectName == downloadingEffectName && bChange == true)
                            {
                                ChangeEffect(downloadingEffectName);
                                HidePopup();
                            }
                        }
                    }
                }
            }
        }

        void rb_OnClick(object sender, RoutedEventArgs e)
        {
            propertiesTabs.SelectedIndex = 1;
            if (propertiesTabs.Visibility == System.Windows.Visibility.Collapsed)
            {
                ExpandPropertiesGridPanelColumn.To = propertiesGridWidthBeforeCollapse;
                ExpandPropertiesGridPanel.Begin();
            }
        }

        private void menu_OnSelectionTabChanged(object sender, SelectionChangedEventArgs e)
        {
            TabsItem ti = (TabsItem)e.AddedItems[0];
            if (ti.Header.ToString() != "Home")
            {
                BasicControl ec = (BasicControl)designCanvas1.SelectedControls[0].Control;
                PropertyInfo pi = (PropertyInfo)ti.Tag;
                effectPropertiesGrid.SelectedObject = pi.GetValue(ec, null);
            }
        }
        #endregion select in design canvas

        #region animation for left and right panels
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
            if (propertiesTabs.Visibility == System.Windows.Visibility.Visible)
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
        #endregion animation for left and right panels
    }
}

