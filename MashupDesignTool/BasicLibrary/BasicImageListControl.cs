using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using System.Xml;

namespace BasicLibrary
{
    public class BasicImageListControl : BasicListControl
    {
        private ObservableCollection<string> _ItemsData = new ObservableCollection<string>();
        public ObservableCollection<string> ItemsData
        {
            get
            {
                IsManual = true;
                _ItemsData.Clear();
                foreach (EffectableControl ec in Items)
                {
                    ImageListControlItems item = ec.Control as ImageListControlItems;
                    _ItemsData.Add(item.ImageUrl);
                    _ItemsData.Add(item.Description);
                    _ItemsData.Add(item.Title);
                }
                IsManual = false;
                return _ItemsData;
            }
        }

        private bool _IsShowTitle;

        public bool IsShowTitle
        {
            get { return _IsShowTitle; }
            set
            {
                if (_IsShowTitle == value)
                    return;
                _IsShowTitle = value;
                foreach (EffectableControl ec in Items)
                {
                    (ec.Control as ImageListControlItems).IsShowTitle = _IsShowTitle;
                }
            }
        }

        private bool IsManual = false;
        public BasicImageListControl()
            : base()
        {
            parameterNameList.Add("ItemsData");
            parameterNameList.Add("IsShowTitle");
            _ItemsData.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_ItemsData_CollectionChanged);

            int count = 4;
            Width = Height = 150;
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            // Width = Height = 150;
            for (int i = 0; i < count; i++)
                AddItem(new EffectableControl(new ImageListControlItems() { Width = 50, Height = 50, }));
        }

        int i = 0;
        bool firstCall = true;
        ImageListControlItems temp = new ImageListControlItems();

        void _ItemsData_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (IsManual)
                return;
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                if (firstCall == true)
                {
                    firstCall = false;
                    RemoveAllItem();
                }
                i++;
                if (e.NewItems[0] != null)
                {
                    switch (i)
                    {
                        case 1:
                            temp.ImageUrl = e.NewItems[0].ToString();
                            break;
                        case 2:
                            temp.Description = e.NewItems[0].ToString();
                            break;
                        case 3:
                            temp.Title = e.NewItems[0].ToString();
                            break;
                    }
                }
                if (i == 3)
                {
                    AddItem(new EffectableControl(temp));
                    temp = new ImageListControlItems();
                    i = 0;
                }
            }

        }
    }
}
