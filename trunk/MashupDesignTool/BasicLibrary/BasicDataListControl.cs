using System;
using System.Net;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BasicLibrary
{
    public class BasicDataListControl : BasicListControl
    {
        private Type _ListItem;

        public Type ListItem
        {
            get { return _ListItem; }
            set { _ListItem = value; }
        }

        List<int> _ControlDataMapping = new List<int>();

        public List<int> ControlDataMapping
        {
            get { return _ControlDataMapping; }
            set { _ControlDataMapping = value; }
        }

        private ListDataSource _Datasource;

        public ListDataSource Datasource
        {
            get { return _Datasource; }
            set { _Datasource = value; }
        }

        public BasicDataListControl():base()
        {
        }

        public void BindingData()
        {
            Ultility u = new Ultility();
            if (_Datasource.SourceType == ListDataSource.DataSourceType.XML)
            {
                u.OnGetListDataFromXmlAsyncCompleted += new Ultility.GetListDataFromXmlAsyncCompletedHandler(u_OnGetListDataCompleted);
                u.GetListDataFromXmlAsync(_Datasource.XmlURL, _Datasource.ElementName, 0, int.MaxValue);
            }
        }

        void u_OnGetListDataCompleted(List<List<string>> result)
        {
            RemoveAllItem();
            foreach (List<string> lstString in result)
            {
                BasicDataListItem fe = Activator.CreateInstance(_ListItem) as BasicDataListItem;
                
                AddItem(new EffectableControl(fe));
            }
        }
    }

    /*
    public class BasicDataListControl : BasicListControl
    {
        private Type _ListItemPanelType;

        public Type ListItemPanelType
        {
            get { return _ListItemPanelType; }
            set { _ListItemPanelType = value; }
        }
        private List<Type> _ListItemType = new List<Type>();

        public List<Type> ListItemType
        {
            get { return _ListItemType; }
            set { _ListItemType = value; }
        }

        private ListDataSource _Datasource;

        public ListDataSource Datasource
        {
            get { return _Datasource; }
            set { _Datasource = value; }
        }

        List<int> _ControlDataMapping = new List<int>();
        List<List<string>> _Data = new List<List<string>>();

        public List<List<string>> Data
        {
            get { return _Data; }
            set { _Data = value; }
        }
        public List<int> ControlDataMapping
        {
            get { return _ControlDataMapping; }
            set { _ControlDataMapping = value; }
        }

        public virtual void BindingData()
        {
            RemoveAllItem();
            int count = Math.Min(_ListItemType.Count, _ControlDataMapping.Count);
            for (int i = 0; i < _Data.Count; i++)
            {
                Panel p = Activator.CreateInstance(_ListItemPanelType) as Panel;
                for (int j = 0; j < count; j++)
                {
                    UIElement element = Activator.CreateInstance(_ListItemType[j]) as UIElement;
                    p.Children.Add(element);

                    TextBlock t = element as TextBlock;
                    if (t != null)
                    {
                        t.Text = _Data[i][j];
                        continue;
                    }
                    Image img = element as Image;
                    if (img != null)
                    {
                        BitmapImage bm = new BitmapImage(new Uri(_Data[i][j], UriKind.RelativeOrAbsolute));
                        img.Source = bm;
                        continue;
                    }
                }
                AddItem(new EffectableControl(p));
            }
        }

        public class SupportedType
        {
            string _DisplayName;

            public string DisplayName
            {
                get { return _DisplayName; }
                set { _DisplayName = value; }
            }
            Type _ControlType;

            public Type ControlType
            {
                get { return _ControlType; }
                set { _ControlType = value; }
            }

        }

        public virtual ComboBox GetSupportedTypeCombobox()
        {
            ComboBox combo = new ComboBox();
            combo.DisplayMemberPath = "DisplayName";
            combo.Items.Add(new SupportedType() { DisplayName = "TextBlock", ControlType = typeof(TextBlock) });
            combo.Items.Add(new SupportedType() { DisplayName = "Image", ControlType = typeof(Image) });
            combo.SelectedIndex = 0;
            return combo;
        }

        public virtual ComboBox GetSupportedPanelCombobox()
        {
            ComboBox combo = new ComboBox();
            combo.DisplayMemberPath = "DisplayName";
            combo.Items.Add(new SupportedType() { DisplayName = "Canvas", ControlType = typeof(Canvas) });
            combo.Items.Add(new SupportedType() { DisplayName = "StackPanel", ControlType = typeof(StackPanel) });
            combo.Items.Add(new SupportedType() { DisplayName = "Grid", ControlType = typeof(Grid) });
            combo.SelectedIndex = 0;
            return combo;
        }
    }*/
}
