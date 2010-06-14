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
        public DataListEditor(Panel p, BasicDataListControl list)
        {
            list.U = u;
            _list = list;
            InitializeComponent();
            f.ParentLayoutRoot = p;
            f.Content = this;
            f.Title = "Data list editor";
        }

        public void ShowDialog()
        {
            f.Width = f.Height = 600;
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
            textBlock3.Text = gridXmlBinding.ColumnDefinitions[0].Width.ToString() + "  " + gridTitle.ColumnDefinitions[0].Width.ToString();
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
            if(comboServerType.SelectedIndex == 0)
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
        /*      public DataListEditor(Panel p, BasicDataListControl list)
              {
                  InitializeComponent();

                  u = new Ultility() { ServerURL = new Uri("http://localhost:1646/", UriKind.Absolute) };

                  StackItemPanel.Children.Add(list.GetSupportedPanelCombobox());
                  previewer = new DataListItemPanel();
                  canvasPreview.Children.Add(previewer);
                  _list = list;
                  f.ParentLayoutRoot = p;
                  f.Content = this;
                  f.Title = "Data list editor";
                  f.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(f_Closing);
                  u.OnGetListDataFromXmlAsyncCompleted += new Ultility.GetListDataFromXmlAsyncCompletedHandler(u_OnGetListDataFromXmlAsyncCompleted);
                  u.OnGetXmlStructureAsyncCompleted += new Ultility.GetXmlStructureAsyncCompletedHandler(u_OnGetXmlStructureAsyncCompleted);
              }

              void f_Closing(object sender, System.ComponentModel.CancelEventArgs e)
              {
                  _list.BindingData();
              }

              void u_OnGetXmlStructureAsyncCompleted(List<string> result)
              {
                  int index = 0;
                  foreach (string s in result)
                      AddDataRow(s, ref index);
                  u.GetListDataFromXmlAsync(textXmlURL.Text, textXmlElement.Text,0,1);
              }

              public void ShowDialog()
              {
                  f.ShowDialog();
              }

              private void Get_Click(object sender, RoutedEventArgs e)
              {
                  //u.GetListDataFromXmlAsync(textXmlURL.Text, textXmlElement.Text);
                  u.GetXmlStructureAsync(textXmlURL.Text, textXmlElement.Text);
              }

              void u_OnGetListDataFromXmlAsyncCompleted(List<List<string>> result)
              {
                  _list.Data = result;
                  _list.ListItemPanelType = typeof(StackPanel);
                  _list.ListItemType.Add(typeof(TextBlock));
                  _list.ListItemType.Add(typeof(TextBlock));
                  _list.ListItemType.Add(typeof(TextBlock));
                  _list.ListItemType.Add(typeof(TextBlock));
                  _list.ControlDataMapping.Add(0);
                  _list.ControlDataMapping.Add(1);
                  _list.ControlDataMapping.Add(2);
                  _list.ControlDataMapping.Add(3);

                  previewer.Title = result[0][0];
                  previewer.ImageURL = "http://vnexpress.net/Files/Subject/3B/A1/C9/5F/a4.jpg";
              }

              private void btnPreview_Click(object sender, RoutedEventArgs e)
              {

              }

              private void AddDataRow(string data, ref int index)
              {
                  gridData.RowDefinitions.Add(new RowDefinition());

                  CheckBox c = new CheckBox();
                  Grid.SetColumn(c, 0);
                  Grid.SetRow(c, index);
                  gridData.Children.Add(c);

                  TextBlock t = new TextBlock() { Text = data };
                  Grid.SetColumn(t, 1);
                  Grid.SetRow(t, index);
                  gridData.Children.Add(t);

                  ComboBox combo = _list.GetSupportedTypeCombobox();
            
                  Grid.SetColumn(combo, 2);
                  Grid.SetRow(combo, index);
                  gridData.Children.Add(combo);

                  index++;
              }

              public object Clone(object o)
              {
                  Type t = o.GetType();
                  object result = null;

                  if (t.IsPrimitive == true)
                  {
                      result = o;
                      return result;
                  }

                  if (t == typeof(string))
                  {
                      result = o;
                      return result;
                  }
                  try
                  {
                      result = Activator.CreateInstance(t);
                  }
                  catch (Exception ex)
                  {
                      return null;
                  }
            
                  foreach (PropertyInfo pi in t.GetProperties())
                  {
                      if (pi.PropertyType.IsArray == true)
                      {
                          continue;
                      }

                      if (typeof(IList).IsAssignableFrom(pi.PropertyType) == true)
                      {
                          continue;
                      }

                      if (pi.CanWrite == false || pi.GetSetMethod() == null)
                          continue;

                      if (pi.GetIndexParameters().Length > 0)
                          continue;

                      if (typeof(DependencyObject).IsAssignableFrom(pi.PropertyType) == true)
                          continue;

                      object temp = pi.GetValue(o, null);
                      if (temp == null)
                          continue;

                      object obj = Clone(temp);
                      result.GetType().GetProperty(pi.Name).SetValue(result, obj, null);
                  }
                  return result;
              }*/
    }
}
