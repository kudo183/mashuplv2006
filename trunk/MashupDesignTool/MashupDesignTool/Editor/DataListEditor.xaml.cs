using System;
using System.Collections;
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
using System.Windows.Browser;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Reflection;
using MashupDesignTool;
using System.Windows.Threading;

namespace ItemCollectionEditor
{
    public partial class DataListEditor : UserControl
    {
        private Color borderColor = Color.FromArgb(255, 233, 236, 250);
        private double rowHeight = 25;
        FloatableWindow f = new FloatableWindow();
        Ultility u = new Ultility();
        BasicDataListControl _list;
        BasicDataListItem _listItem;
        List<int> _mapping = new List<int>();
        ListDataSource.DataSourceType _sourceType;
        List<ComboBox> _comboData = new List<ComboBox>();
        List<string> _data = new List<string>();
        List<string> _structure = new List<string>();
        Dictionary<string, Assembly> LoadedControlAssembly = new Dictionary<string, Assembly>();
        ControlDownloader controlDownloader;
        DispatcherTimer doubleClickTimer;
        string downloadingControlName = "";
        ControlInfo downloadControlInfo;
        bool bAdd = false;
        List<string> skipList = new List<string>() { "ControlName", "Visible", "Width", "Height" };
        public DataListEditor(Panel p, BasicDataListControl list, List<ControlInfo> listControlTemplate, ControlDownloader controlDownloader)
        {
            list.U = u;
            _list = list;
            InitializeComponent();
            f.ParentLayoutRoot = p;
            f.Content = this;
            f.Title = "Data list editor";
            f.Closed += new EventHandler(f_Closed);
            treeControl.ItemsSource = listControlTemplate;
            this.controlDownloader = controlDownloader;
        
            doubleClickTimer = new DispatcherTimer();
            doubleClickTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            doubleClickTimer.Tick += new EventHandler(DoubleClick_Timer);
        }

        public void ShowDialog()
        {
            f.Width = 700;
            f.Height = 600;
            f.ShowDialog();
        }

        void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            int index = _comboData.IndexOf(cb);
            if (index == -1)
                return;
            if (cb.SelectedIndex == -1)
                return;
            if (_data.Count == 0)
                return;
            _mapping[index] = cb.SelectedIndex - 1;
            if (cb.SelectedIndex == 0)
            {
                _listItem.SetParameterCanBindingValue(_listItem.GetParameterCanBindingNameList()[index], "");
                return;
            }
            _listItem.SetParameterCanBindingValue(_listItem.GetParameterCanBindingNameList()[index], _data[cb.SelectedIndex - 1]);
        }

        private void img1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            gridXmlBinding.RowDefinitions.Clear();
            gridXmlBinding.Children.Clear();

            foreach (ComboBox c in _comboData)
                c.SelectionChanged -= new SelectionChangedEventHandler(cb_SelectionChanged);
            _comboData.Clear();

            _mapping.Clear();
            BasicDataListItem item = sender as BasicDataListItem;

            Border bd = null;
            int i = 0;
            foreach (string s in item.GetParameterCanBindingNameList())
            {
                TextBlock tb = new TextBlock();
                tb.Text = s;
                bd = new Border() { Child = tb, BorderBrush = new SolidColorBrush(borderColor), BorderThickness = new Thickness(1) };
                Grid.SetColumn(bd, 0);
                Grid.SetRow(bd, i);
                gridXmlBinding.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(rowHeight, GridUnitType.Pixel) });
                gridXmlBinding.Children.Add(bd);

                ComboBox cb = new ComboBox();
                cb.Items.Add(new TextBlock() { Text = "***Not use this property***", FontStyle = FontStyles.Italic });
                cb.SelectedIndex = 0;

                cb.SelectionChanged += new SelectionChangedEventHandler(cb_SelectionChanged);

                foreach (string str in _structure)
                {
                    // cb.Items.Add(new TextBlock() { Text = str });
                    cb.Items.Add(str);
                }

                bd = new Border() { Child = cb, BorderBrush = new SolidColorBrush(borderColor), BorderThickness = new Thickness(1) };
                Grid.SetColumn(bd, 1);
                Grid.SetRow(bd, i);
                gridXmlBinding.Children.Add(bd);
                _comboData.Add(cb);

                _mapping.Add(-1);

                i++;
            }
            _listItem = Activator.CreateInstance(item.GetType()) as BasicDataListItem;
            brdPreview.Child = _listItem;
        }

        bool _isMouseDownOnGridTitleGridSplitter;
        private void GridSplitter_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDownOnGridTitleGridSplitter == false)
                return;
            gridXmlBinding.ColumnDefinitions[0].Width = gridTitle.ColumnDefinitions[0].Width;
            //textBlock3.Text = gridXmlBinding.ColumnDefinitions[0].Width.ToString() + "  " + gridTitle.ColumnDefinitions[0].Width.ToString();
        }

        private void GridSplitter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseDownOnGridTitleGridSplitter = true;
        }

        private void GridSplitter_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDownOnGridTitleGridSplitter = false;
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            _list.Datasource.SourceType = _sourceType;
            _list.ListItemXMlString = BasicControlSerializer.Serialize(_listItem);
            if (_sourceType == ListDataSource.DataSourceType.XML)
            {
                _list.Datasource.XmlURL = textXmlURL.Text;
                _list.Datasource.ElementName = textXmlElement.Text;
            }
            else if (_sourceType == ListDataSource.DataSourceType.MYSQL)
            {
                _list.Datasource.Server = textServerUrl.Text;
                _list.Datasource.Username = textUserName.Text;
                _list.Datasource.Password = textPassword.Password;
                _list.Datasource.Db = textDBName.Text;
                _list.Datasource.Table = textSQLString.Text;
            }
            _list.ControlDataMapping = _mapping;
            _list.BindingData();
            f.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            f.Close();
        }

        private void Get_Click(object sender, RoutedEventArgs e)
        {
            u.OnGetListDataFromXmlStructureAsyncCompleted += new Ultility.GetListDataFromXmlStructureAsyncCompletedHandler(u_OnGetListDataFromXmlStructureAsyncCompleted);
            u.GetListDataFromXmlStructureAsync(textXmlURL.Text, textXmlElement.Text);
            _sourceType = ListDataSource.DataSourceType.XML;
        }

        void u_OnGetListDataFromXmlStructureAsyncCompleted(string xmlString)
        {
            u.OnGetListDataFromXmlStructureAsyncCompleted -= new Ultility.GetListDataFromXmlStructureAsyncCompletedHandler(u_OnGetListDataFromXmlStructureAsyncCompleted);
            LoadComboboxData(xmlString);
        }

        private void btnGetDatabase_Click(object sender, RoutedEventArgs e)
        {
            u.OnGetListDataFromDatabaseStructureAsyncCompleted += new Ultility.GetListDataFromDatabaseStructureAsyncCompletedHandler(u_OnGetListDataFromDatabaseStructureAsyncCompleted);
            u.GetListDataFromDatabaseStructureAsync(textServerUrl.Text, textUserName.Text, textPassword.Password, textDBName.Text, textSQLString.Text);
            if (comboServerType.SelectedIndex == 0)
            {
                _sourceType = ListDataSource.DataSourceType.MYSQL;
            }
        }

        void u_OnGetListDataFromDatabaseStructureAsyncCompleted(string xmlString)
        {
            u.OnGetListDataFromDatabaseStructureAsyncCompleted += new Ultility.GetListDataFromDatabaseStructureAsyncCompletedHandler(u_OnGetListDataFromDatabaseStructureAsyncCompleted);
            LoadComboboxData(xmlString);
        }

        private void LoadComboboxData(string xmlString)
        {
            XDocument xDoc = XDocument.Parse(xmlString);

            _structure.Clear();
            _data.Clear();
            XElement eRoot = xDoc.Element("Root");
            foreach (XElement e in eRoot.Elements())
            {
                _structure.Add(e.Name.ToString());
                _data.Add(e.Value);
            }
            foreach (ComboBox cb in _comboData)
            {
                cb.Items.Clear();
                cb.Items.Add(new TextBlock() { Text = "***Not use this property***", FontStyle = FontStyles.Italic });
                // cb.SelectedIndex = 0;

                foreach (string s in _structure)
                {
                    //cb.Items.Add(new TextBlock() { Text = s });
                    cb.Items.Add(s);
                }
                cb.UpdateLayout();
                cb.SelectedIndex = 0;
            }
        }

        #region Download control
        private void Control_Click(object sender, RoutedEventArgs e)
        {
            ControlInfo ci = (ControlInfo)((RadioButton)sender).DataContext;
            if (doubleClickTimer.IsEnabled)
            {
                doubleClickTimer.Stop();
                if (!LoadedControlAssembly.ContainsKey(ci.ControlName))
                {
                    if (controlDownloader.GetAssembly(ci) != null)
                    {
                        LoadedControlAssembly.Add(ci.ControlName, controlDownloader.GetAssembly(ci));
                    }
                    else
                    {
                        downloadingControlName = ci.ControlName;
                        downloadControlInfo = ci;
                        ShowPopup();
                        return;
                    }
                }
                AddControl(ci.ControlName);
            }
            else
            {
                doubleClickTimer.Start();
            }
        }

        void DoubleClick_Timer(object sender, EventArgs e)
        {
            doubleClickTimer.Stop();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HidePopup();
        }

        private void ShowPopup()
        {
            controlDownloader.DownloadControlCompleted += new ControlDownloader.DownloadControlCompletedHandler(controlDownloader_DownloadControlCompleted);
            openPopup.Begin();
            LoadingPopup.Visibility = System.Windows.Visibility.Visible;
            openPopup.Completed += new EventHandler(openPopup_Completed);
            bAdd = true;
        }

        void controlDownloader_DownloadControlCompleted(ControlInfo ci, Assembly assembly)
        {
            if (!LoadedControlAssembly.ContainsKey(ci.ControlName))
                LoadedControlAssembly.Add(ci.ControlName, assembly);
            if (ci.ControlName == downloadingControlName && bAdd == true)
            {
                AddControl(downloadingControlName);
                HidePopup();
            }
        }

        void openPopup_Completed(object sender, EventArgs e)
        {
            controlDownloader.DownloadControl(downloadControlInfo);
        }

        private void HidePopup()
        {
            controlDownloader.DownloadControlCompleted -= new ControlDownloader.DownloadControlCompletedHandler(controlDownloader_DownloadControlCompleted);
            closePopup.Begin();
            closePopup.Completed += new EventHandler(closePopup_Completed);
            bAdd = false;
        }

        void closePopup_Completed(object sender, EventArgs e)
        {
            LoadingPopup.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void AddControl(string controlName)
        {
            gridXmlBinding.RowDefinitions.Clear();
            gridXmlBinding.Children.Clear();

            foreach (ComboBox c in _comboData)
                c.SelectionChanged -= new SelectionChangedEventHandler(cb_SelectionChanged);
            _comboData.Clear();
            _mapping.Clear();

            BasicDataListItem item = LoadedControlAssembly[controlName].CreateInstance(controlName) as BasicDataListItem;
            if (item == null)
                return;

            Border bd = null;
            int i = 0;
            foreach (string s in item.GetParameterCanBindingNameList())
            {
                TextBlock tb = new TextBlock();
                tb.Text = s;
                bd = new Border() { Child = tb, BorderBrush = new SolidColorBrush(borderColor), BorderThickness = new Thickness(1) };
                Grid.SetColumn(bd, 0);
                Grid.SetRow(bd, i);
                gridXmlBinding.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(rowHeight, GridUnitType.Pixel) });
                gridXmlBinding.Children.Add(bd);

                ComboBox cb = new ComboBox();
                cb.Items.Add(new TextBlock() { Text = "***Not use this property***", FontStyle = FontStyles.Italic });
                cb.SelectedIndex = 0;

                cb.SelectionChanged += new SelectionChangedEventHandler(cb_SelectionChanged);

                foreach (string str in _structure)
                {
                    // cb.Items.Add(new TextBlock() { Text = str });
                    cb.Items.Add(str);
                }

                bd = new Border() { Child = cb, BorderBrush = new SolidColorBrush(borderColor), BorderThickness = new Thickness(1) };
                Grid.SetColumn(bd, 1);
                Grid.SetRow(bd, i);
                gridXmlBinding.Children.Add(bd);
                _comboData.Add(cb);

                _mapping.Add(-1);

                i++;
            }
            _listItem = Activator.CreateInstance(item.GetType()) as BasicDataListItem;
            brdPreview.Child = _listItem;
        }
        
        void f_Closed(object sender, EventArgs e)
        {
            controlDownloader.DownloadControlCompleted -= new ControlDownloader.DownloadControlCompletedHandler(controlDownloader_DownloadControlCompleted); 
        }
        #endregion Download control

        private void brdPreview_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (brdPreview.Child == null)
                return;
            
            BasicControl bc = brdPreview.Child as BasicControl;
            if (bc == null)
                return;
            List<string> properties = bc.GetParameterNameList();
            foreach(string s in skipList)
                properties.Remove(s);
            propertyGrid1.SetSelectedObject(bc, properties);
        }
    }
}
