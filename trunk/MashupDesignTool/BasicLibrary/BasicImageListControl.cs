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
                    _ItemsData.Add(item.Link);
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

        private double titleSize = 12;
        private Brush titleColor = new SolidColorBrush(Colors.Black);
        private FontFamily titleFontFamily = new FontFamily("Arial");

        public double TitleSize
        {
            get { return titleSize; }
            set
            {
                if (titleSize == value)
                    return;
                titleSize = value;
                foreach (EffectableControl ec in Items)
                {
                    (ec.Control as ImageListControlItems).TitleSize = titleSize;
                }
            }
        }

        public Brush TitleColor
        {
            get { return titleColor; }
            set
            {
                if (titleColor == value)
                    return;
                titleColor = value;
                foreach (EffectableControl ec in Items)
                {
                    (ec.Control as ImageListControlItems).TitleColor = titleColor;
                }
            }
        }

        public FontFamily TitleFontFamily
        {
            get { return titleFontFamily; }
            set
            {
                if (titleFontFamily == value)
                    return;
                titleFontFamily = value;
                foreach (EffectableControl ec in Items)
                {
                    (ec.Control as ImageListControlItems).TitleFontFamily = titleFontFamily;
                }
            }
        }

        private bool IsManual = false;
        public BasicImageListControl()
            : base()
        {
            parameterNameList.Add("ItemsData");
            parameterNameList.Add("IsShowTitle");
            parameterNameList.Add("TitleSize");
            parameterNameList.Add("TitleColor");
            parameterNameList.Add("TitleFontFamily");
            _ItemsData.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_ItemsData_CollectionChanged);

            int count = 4;
            Width = Height = 150;
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            // Width = Height = 150;
            for (int i = 0; i < count; i++)
                AddItem(new EffectableControl(new ImageListControlItems() { Width = 50, Height = 50, }));
            Loaded += new RoutedEventHandler(BasicImageListControl_Loaded);
        }

        void BasicImageListControl_Loaded(object sender, RoutedEventArgs e)
        {
            TitleColor = TitleColor;
            TitleFontFamily = TitleFontFamily;
            TitleSize = TitleSize;
            if (_ItemsData.Count == 0)
                RemoveAllItem();
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
                        case 4:
                            temp.Link = e.NewItems[0].ToString();
                            break;
                    }
                }
                if (i == 4)
                {
                    temp.TitleSize = titleSize;
                    temp.TitleColor = titleColor;
                    temp.TitleFontFamily = titleFontFamily;
                    AddItem(new EffectableControl(temp));
                    temp = new ImageListControlItems();
                    i = 0;
                }
            }

        }
    }
}
