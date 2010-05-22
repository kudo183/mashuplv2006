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

namespace MapulRibbon
{
    public partial class RibbonSimpleListView : UserControl
    {
        public event SelectionChangedEventHandler SelectionChanged;

        List<RibbonSimpleListViewSourceItem> list = new List<RibbonSimpleListViewSourceItem>();
        public RibbonSimpleListView()
        {
            InitializeComponent();
            listBox.ItemsSource = list;
        }

        public void AddListItem(RibbonSimpleListViewSourceItem item)
        {
            list.Add(item);
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged(this, e);
        }

        public List<RibbonSimpleListViewSourceItem> ListSourceItem
        {
            get { return list; }
            set
            {
                list = value;
                listBox.ItemsSource = list;
            }
        }

        public int SelectedIndex
        {
            get { return listBox.SelectedIndex; }
            set { listBox.SelectedIndex = value; }
        }

        public RibbonSimpleListViewSourceItem SelectedItem
        {
            get { return (RibbonSimpleListViewSourceItem)listBox.SelectedItem; }
            set { listBox.SelectedItem = value; }
        }
    }
}
