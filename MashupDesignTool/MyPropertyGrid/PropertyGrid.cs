
namespace SL30PropertyGrid
{
    #region Using Directives
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Animation;

    #endregion

    #region PropertyGrid
    /// <summary>
    /// PropertyGrid
    /// </summary>
    public partial class PropertyGrid : ContentControl
    {

        public delegate void OnPropertyValueChange(UIElement ui, string name, object value);
        public event OnPropertyValueChange PropertyValueChange;

        private Dictionary<PropertyGridLabel, Storyboard> listStoryboardMouseEnter = new Dictionary<PropertyGridLabel, Storyboard>();
        private Dictionary<PropertyGridLabel, Storyboard> listStoryboardMouseLeave = new Dictionary<PropertyGridLabel, Storyboard>();

        #region Fields

        //#99B4D1
        //internal static Color backgroundColor = Color.FromArgb(127, 153, 180, 209);

        //#E9ECFA
        internal static Color backgroundColor = Color.FromArgb(255, 233, 236, 250);
        //internal static Color backgroundColor = Colors.Transparent;
        //internal static Color backgroundColorFocused = Color.FromArgb(255, 94, 170, 255);

        public static Color labelBackgroundColor = Colors.White;
        public static Color labelBackgroundColorFocused = Color.FromArgb(255, 254, 201, 0);
        public static Color labelForegroundColor = Colors.Black;
        public static Color labelForegroundColorReadOnly = Colors.Gray;
        public static Color labelForegroundColorFocused = Colors.White;

        static Type thisType = typeof(PropertyGrid);

        List<PropertyItem> props;
        List<ValueEditorBase> editors;

        ValueEditorBase selectedEditor;
        ScrollViewer LayoutRoot;
        Grid MainGrid;
        bool loaded = false;
        bool resetLoadedObject;
        ValueEditorBase _editorLeft;
        ValueEditorBase _editorTop;

        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        public PropertyGrid()
        {
            base.DefaultStyleKey = typeof(PropertyGrid);
            this.Loaded += new RoutedEventHandler(PropertyGrid_Loaded);
            //MainGrid.Background = new SolidColorBrush( Colors.Red);
        }
        #endregion

        #region Properties

        private FrameworkElement _SelectedObjectParent;

        public FrameworkElement SelectedObjectParent
        {
            get { return _SelectedObjectParent; }
            set { _SelectedObjectParent = value; }
        }

        #region SelectedObject
        private object _SelectedObject;

        public object SelectedObject
        {
            get { return _SelectedObject; }
            set 
            { 
                _SelectedObject = value;
                _propertyNames = null;
                if (loaded == false)
                {
                    resetLoadedObject = true;
                    return;
                }

                if (_SelectedObject != null)
                    this.ResetObject(_SelectedObject);
                else
                    this.ResetMainGrid();
            }
        }
        //public static readonly DependencyProperty SelectedObjectProperty =
        //  DependencyProperty.Register("SelectedObject", typeof(object), thisType, new PropertyMetadata(null, OnSelectedObjectChanged));

        //public object SelectedObject
        //{
        //    get { return base.GetValue(SelectedObjectProperty); }
        //    set { base.SetValue(SelectedObjectProperty, value); }
        //}

        //private static void OnSelectedObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    PropertyGrid propertyGrid = d as PropertyGrid;
        //    if (propertyGrid != null)
        //    {
        //        if (!propertyGrid.loaded)
        //            propertyGrid.resetLoadedObject = true;
        //        else if (null != e.NewValue)
        //            propertyGrid.ResetObject(e.NewValue);
        //        else
        //            propertyGrid.ResetMainGrid();
        //    }
        //}
        #endregion

        #region Default LabelWidth
        /// <summary>
        /// The DefaultLabelWidth DependencyProperty
        /// </summary>
        public static readonly DependencyProperty DefaultLabelWidthProperty =
          DependencyProperty.Register("DefaultLabelWidth", typeof(int), thisType, new PropertyMetadata(75));
        /// <summary>
        /// Gets or sets the Default Width for the labels
        /// </summary>
        public int DefaultLabelWidth
        {
            get { return (int)base.GetValue(DefaultLabelWidthProperty); }
            set { base.SetValue(DefaultLabelWidthProperty, value); }
        }
        #endregion

        #region Grid BorderBrush

        public static readonly DependencyProperty GridBorderBrushProperty =
            DependencyProperty.Register("GridBorderBrush", typeof(Brush), thisType, new PropertyMetadata(new SolidColorBrush(Colors.LightGray), OnGridBorderBrushChanged));

        /// <summary>
        /// Gets or sets the Border Brush of the Property Grid
        /// </summary>
        public Brush GridBorderBrush
        {
            get { return (Brush)base.GetValue(GridBorderBrushProperty); }
            set { base.SetValue(GridBorderBrushProperty, value); }
        }

        private static void OnGridBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid propertyGrid = d as PropertyGrid;
            if (propertyGrid != null && null != propertyGrid.LayoutRoot && null != e.NewValue)
                propertyGrid.LayoutRoot.BorderBrush = (SolidColorBrush)e.NewValue;
        }

        public static readonly DependencyProperty GridBackgroundBrushProperty =
            DependencyProperty.Register("GridBackgroundBrush", typeof(Brush), thisType, new PropertyMetadata(new SolidColorBrush(Colors.LightGray), OnGridBackgroundBrushChanged));

        /// <summary>
        /// Gets or sets the Background Brush of the Property Grid
        /// </summary>
        public Brush GridBackgroundBrush
        {
            get { return (Brush)base.GetValue(GridBackgroundBrushProperty); }
            set { base.SetValue(GridBackgroundBrushProperty, value); }
        }

        private static void OnGridBackgroundBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid propertyGrid = d as PropertyGrid;
            if (propertyGrid != null && null != propertyGrid.LayoutRoot && null != e.NewValue)
            {
                propertyGrid.LayoutRoot.Background = (SolidColorBrush)e.NewValue;
            }
        }
        #endregion

        #region Grid BorderThickness

        public static readonly DependencyProperty GridBorderThicknessProperty =
            DependencyProperty.Register("GridBorderThickness", typeof(Thickness), thisType, new PropertyMetadata(new Thickness(1), OnGridBorderThicknessChanged));

        /// <summary>
        /// Gets or sets the Border Thickness of the Property Grid
        /// </summary>
        public Thickness GridBorderThickness
        {
            get { return (Thickness)base.GetValue(GridBorderThicknessProperty); }
            set { base.SetValue(GridBorderThicknessProperty, value); }
        }


        private static void OnGridBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid propertyGrid = d as PropertyGrid;
            if (propertyGrid != null && null != propertyGrid.LayoutRoot && null != e.NewValue)
                propertyGrid.LayoutRoot.BorderThickness = (Thickness)e.NewValue;
        }

        #endregion
        private List<string> _propertyNames = new List<string>();
        #endregion

        #region Overrides
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.LayoutRoot = (ScrollViewer)this.GetTemplateChild("LayoutRoot");
            this.MainGrid = (Grid)this.GetTemplateChild("MainGrid");

            loaded = true;

            if (resetLoadedObject)
            {
                resetLoadedObject = false;
                if (_propertyNames == null)
                    this.ResetObject(_SelectedObject);
                else
                    this.SetSelectedObject(_SelectedObject, _propertyNames);
            }
        }
        #endregion

        #region Methods
        public void SetSelectedObject(object obj, List<string> propertyNames)
        {
            _SelectedObject = obj;
            _propertyNames = propertyNames;
            if (loaded == false)
            {               
                resetLoadedObject = true;
                return;
            }
            this.ResetMainGrid();
            if (obj == null)
                return;            

            int rowCount = this.SetObject(obj, propertyNames);

            if (rowCount > 0)
                AddGridSplitter(rowCount);
        }
        int SetObject(object obj, List<string> propertyNames)
        {
            props = new List<PropertyItem>();
            editors = new List<ValueEditorBase>();

            int rowCount = -1;

            // Parse the objects properties
            props = ParseObject(obj, propertyNames);

            #region Render the Grid

            var categories = (from p in props
                              orderby p.Category
                              select p.Category).Distinct();

            foreach (string category in categories)
            {

                this.AddHeaderRow(category, ref rowCount);

                var items = from p in props
                            where p.Category == category
                            orderby p.Name
                            select p;

                foreach (var item in items)
                    this.AddPropertyRow(item, ref rowCount);

            }
            #endregion

            return rowCount++;

        }

        int SetObject(object obj)
        {
            props = new List<PropertyItem>();
            editors = new List<ValueEditorBase>();

            int rowCount = -1;

            // Parse the objects properties
            props = ParseObject(obj);

            #region Render the Grid

            var categories = (from p in props
                              orderby p.Category
                              select p.Category).Distinct();

            foreach (string category in categories)
            {

                this.AddHeaderRow(category, ref rowCount);

                var items = from p in props
                            where p.Category == category
                            orderby p.Name
                            select p;

                foreach (var item in items)
                    this.AddPropertyRow(item, ref rowCount);

            }
            #endregion

            return rowCount++;

        }

        void ResetObject(object obj)
        {
            this.ResetMainGrid();

            int rowCount = this.SetObject(obj);

            if (rowCount > 0)
                AddGridSplitter(rowCount);
        }
        void ResetMainGrid()
        {
            this.MainGrid.Children.Clear();
            this.MainGrid.RowDefinitions.Clear();
        }
        void AddHeaderRow(string category, ref int rowIndex)
        {
            rowIndex++;
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(21) });

            #region Column 0 - Margin

            Border brd = GetCategoryMargin(category, GetHideImage(Visibility.Visible), GetShowImage(Visibility.Collapsed));
            MainGrid.Children.Add(brd);
            Grid.SetRow(brd, rowIndex);
            Grid.SetColumn(brd, 0);

            #endregion

            #region Column 1 & 2 - Category Header

            brd = GetCategoryHeader(category);
            MainGrid.Children.Add(brd);
            Grid.SetRow(brd, rowIndex);
            Grid.SetColumn(brd, 1);
            Grid.SetColumnSpan(brd, 2);

            #endregion
        }
        void AddPropertyRow(PropertyItem item, ref int rowIndex)
        {
            if (item.DisplayName == "Projection")
                return;
            item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
            #region Create Display Objects
            PropertyGridLabel label = CreateLabel(item.Name, item.DisplayName);
            ValueEditorBase editor = EditorService.GetEditor(item, label);
            if (null == editor)
                return;
            editor.GotFocus += new RoutedEventHandler(this.Editor_GotFocus);
            editors.Add(editor);
            #endregion

            #region create storyboard
            Storyboard sb1 = new Storyboard();
            ObjectAnimationUsingKeyFrames colorAnimation1 = new ObjectAnimationUsingKeyFrames() { BeginTime = TimeSpan.FromSeconds(0) };
            Storyboard.SetTarget(colorAnimation1, label);
            Storyboard.SetTargetProperty(colorAnimation1, new PropertyPath("Background"));
            colorAnimation1.KeyFrames.Add(new DiscreteObjectKeyFrame() { KeyTime = TimeSpan.FromMilliseconds(200), Value = new SolidColorBrush(Color.FromArgb(00, 254, 201, 0)) });
            sb1.Children.Add(colorAnimation1);
            sb1.Begin();
            listStoryboardMouseLeave.Add(label, sb1);

            Storyboard sb2 = new Storyboard();
            ObjectAnimationUsingKeyFrames colorAnimation2 = new ObjectAnimationUsingKeyFrames() { BeginTime = TimeSpan.FromSeconds(0) };
            Storyboard.SetTarget(colorAnimation2, label);
            Storyboard.SetTargetProperty(colorAnimation2, new PropertyPath("Background"));
            colorAnimation2.KeyFrames.Add(new DiscreteObjectKeyFrame() { KeyTime = TimeSpan.FromSeconds(0), Value = new SolidColorBrush(Color.FromArgb(255, 254, 201, 0)) });
            colorAnimation2.KeyFrames.Add(new DiscreteObjectKeyFrame() { KeyTime = TimeSpan.FromMilliseconds(200), Value = new SolidColorBrush(Color.FromArgb(255, 254, 219, 96)) });
            sb2.Children.Add(colorAnimation2);
            listStoryboardMouseEnter.Add(label, sb2);

            label.MouseEnter += new MouseEventHandler(label_MouseEnter);
            label.MouseLeave += new MouseEventHandler(label_MouseLeave);
            #endregion create storyboard

            rowIndex++;
            MainGrid.RowDefinitions.Add(new RowDefinition());
            string tagValue = item.Category;

            #region Column 0 - Margin
            Border brd = GetItemMargin(tagValue);
            MainGrid.Children.Add(brd);
            Grid.SetRow(brd, rowIndex);
            Grid.SetColumn(brd, 0);
            #endregion

            #region Column 1 - Label
            brd = GetItemLabel(label, tagValue);
            MainGrid.Children.Add(brd);
            Grid.SetRow(brd, rowIndex);
            Grid.SetColumn(brd, 1);
            #endregion

            #region Column 2 - Editor
            brd = GetItemEditor(editor, tagValue);
            MainGrid.Children.Add(brd);
            Grid.SetRow(brd, rowIndex);
            Grid.SetColumn(brd, 2);
            #endregion

            if (item.DisplayName == "Left")
            {
                _editorLeft = editor;
            }
            else if (item.DisplayName == "Top")
            {
                _editorTop = editor;
            }
        }

        void label_MouseLeave(object sender, MouseEventArgs e)
        {
            PropertyGridLabel label = (PropertyGridLabel)sender;
            if (selectedEditor != null)
            {
                if (!label.Equals(selectedEditor.Label))
                    listStoryboardMouseLeave[(PropertyGridLabel)sender].Begin();
            }
            else
                listStoryboardMouseLeave[(PropertyGridLabel)sender].Begin();
        }

        void label_MouseEnter(object sender, MouseEventArgs e)
        {
            PropertyGridLabel label = (PropertyGridLabel)sender;
            if (selectedEditor != null)
            {
                if (!label.Equals(selectedEditor.Label))
                    listStoryboardMouseEnter[(PropertyGridLabel)sender].Begin();
            }
            else
                listStoryboardMouseEnter[(PropertyGridLabel)sender].Begin();
        }

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyValueChange != null)
            {
                PropertyItem pi = sender as PropertyItem;
                UIElement ui = pi.Instant as UIElement;
                PropertyValueChange(ui, pi.Name, pi.Value);

                if (pi.Name == "DockType")
                {
                    string dock = pi.Value.ToString();

                    if (dock == "Left" || dock == "Right")
                    {
                        SetPropertyReadonly("Height", true);
                        SetPropertyReadonly("Width", false);
                    }
                    else if (dock == "Top" || dock == "Bottom")
                    {
                        SetPropertyReadonly("Width", true);
                        SetPropertyReadonly("Height", false);
                    }
                    else if (dock == "Fill")
                    {
                        SetPropertyReadonly("Height", true);
                        SetPropertyReadonly("Width", true);
                    }
                    else
                    {
                        SetPropertyReadonly("Height", false);
                        SetPropertyReadonly("Width", false);
                    }
                }

            }

        }
        void AddGridSplitter(int rowCount)
        {
            GridSplitter gsp = new GridSplitter()
            {
                IsTabStop = false,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = new SolidColorBrush(Colors.Transparent),
                ShowsPreview = false,
                Width = 2
            };
            Grid.SetColumn(gsp, 2);
            Grid.SetRowSpan(gsp, rowCount);
            Canvas.SetZIndex(gsp, 1);
            MainGrid.Children.Add(gsp);

        }
        void ToggleCategoryVisible(bool show, string tagValue)
        {
            foreach (FrameworkElement element in this.MainGrid.Children)
            {
                object value = element.Tag;
                if (null != value)
                {
                    string tag = (string)value;
                    if (tagValue == tag)
                        element.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }
        void AttachWheelEvents()
        {
            HtmlPage.Window.AttachEvent("DOMMouseScroll", OnMouseWheel);
            HtmlPage.Window.AttachEvent("onmousewheel", OnMouseWheel);
            HtmlPage.Document.AttachEvent("onmousewheel", OnMouseWheel);
        }
        void DetachWheelEvents()
        {
            HtmlPage.Window.DetachEvent("DOMMouseScroll", OnMouseWheel);
            HtmlPage.Window.DetachEvent("onmousewheel", OnMouseWheel);
            HtmlPage.Document.DetachEvent("onmousewheel", OnMouseWheel);
        }
        Image GetHideImage(Visibility visibility)
        {
            Image img = GetImage("Images/minus.png");
            img.Visibility = visibility;
            img.MouseLeftButtonUp += new MouseButtonEventHandler(this.CategoryHide_MouseLeftButtonUp);
            return img;
        }
        Image GetShowImage(Visibility visibility)
        {
            Image img = GetImage("Images/plus.png");
            img.Visibility = visibility;
            img.MouseLeftButtonUp += new MouseButtonEventHandler(this.CategoryShow_MouseLeftButtonUp);
            return img;
        }

        private List<PropertyItem> ParseObject(object objItem)
        {
            if (null == objItem)
                return new List<PropertyItem>();

            List<PropertyItem> pc = new List<PropertyItem>();
            Type t = objItem.GetType();
            var props = t.GetProperties();

            foreach (PropertyInfo pinfo in props)
            {

                bool isBrowsable = true;
                BrowsableAttribute b = PropertyItem.GetAttribute<BrowsableAttribute>(pinfo);
                if (null != b)
                    isBrowsable = b.Browsable;
                if (isBrowsable)
                {
                    EditorBrowsableAttribute eb = PropertyItem.GetAttribute<EditorBrowsableAttribute>(pinfo);
                    if (null != eb && eb.State == EditorBrowsableState.Never)
                        isBrowsable = false;
                }
                if (isBrowsable)
                {
                    bool readOnly = (pinfo.CanWrite == false) | (pinfo.GetSetMethod() == null);
                    ReadOnlyAttribute attr = PropertyItem.GetAttribute<ReadOnlyAttribute>(pinfo);
                    if (attr != null)
                        readOnly = attr.IsReadOnly;

                    try
                    {
                        object value = pinfo.GetValue(objItem, null);
                        PropertyItem prop = new PropertyItem(objItem, null, value, pinfo, readOnly, false, null, null, null);
                        pc.Add(prop);
                    }
                    catch (Exception ex) { }
                }
            }

            #region attached property
            FrameworkElement fe = objItem as FrameworkElement;
            while (fe != null && fe.Parent != _SelectedObjectParent)
            {
                fe = fe.Parent as FrameworkElement;
            }

            int beginIndex = pc.Count;
            if (fe != null)
            {
                _SelectedObjectParent = fe.Parent as FrameworkElement;
                if (_SelectedObjectParent != null)
                {
                    Type parentType = _SelectedObjectParent.GetType();

                    while (parentType != null)
                    {
                        MethodInfo[] mi = parentType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                        FieldInfo[] fields = parentType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                        foreach (FieldInfo field in fields)
                        {
                            if (field.FieldType == typeof(DependencyProperty))
                            {
                                string propertyName = field.Name.Substring(0, field.Name.Length - ("Property").Length);
                                int matchCount = 0;
                                MethodInfo get, set;
                                get = set = null;
                                foreach (MethodInfo m in mi)
                                {
                                    if (m.Name == "Get" + propertyName)
                                    {
                                        ParameterInfo[] methodParams = m.GetParameters();
                                        if (methodParams.Count() == 1)
                                        {
                                            matchCount += 1;
                                            get = m;
                                        }
                                    }
                                    if (m.Name == "Set" + propertyName)
                                    {
                                        ParameterInfo[] methodParams = m.GetParameters();
                                        if (methodParams.Count() == 2)
                                        {
                                            matchCount += 2;
                                            set = m;
                                        }
                                    }
                                }
                                if (matchCount == 0)
                                    continue;
                                matchCount = 0;
                                for (int i = beginIndex - 1; i < pc.Count; i++)
                                {
                                    if (pc[i].Name == propertyName)
                                    {
                                        matchCount = 1;
                                        break;
                                    }
                                }
                                if (matchCount == 1)
                                    continue;

                                bool readOnly = false;
                                if (matchCount == 1)
                                    readOnly = true;

                                try
                                {
                                    //get the default value
                                    DependencyProperty dp = (DependencyProperty)field.GetValue(_SelectedObjectParent);

                                    object currentValue = fe.GetValue(dp);
                                    //now have both the value and the default, we're away!

                                    PropertyItem propAttached = new PropertyItem(objItem, _SelectedObjectParent, currentValue, null, readOnly, true, propertyName, get, set);
                                    pc.Add(propAttached);
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }
                        //rinse and repeat for each base type
                        //parentType = parentType.BaseType;
                        parentType = null;
                    }
                }
            }
            #endregion

            return pc;
        }

        private List<PropertyItem> ParseObject(object objItem, List<string> propertyNames)
        {
            if (null == objItem)
                return new List<PropertyItem>();

            List<PropertyItem> pc = new List<PropertyItem>();
            Type t = objItem.GetType();

            foreach (string name in propertyNames)
            {
                PropertyInfo pinfo = t.GetProperty(name);
                if (pinfo == null)
                    continue;
                bool isBrowsable = true;
                BrowsableAttribute b = PropertyItem.GetAttribute<BrowsableAttribute>(pinfo);
                if (null != b)
                    isBrowsable = b.Browsable;
                if (isBrowsable)
                {
                    EditorBrowsableAttribute eb = PropertyItem.GetAttribute<EditorBrowsableAttribute>(pinfo);
                    if (null != eb && eb.State == EditorBrowsableState.Never)
                        isBrowsable = false;
                }
                if (isBrowsable)
                {
                    bool readOnly = (pinfo.CanWrite == false) | (pinfo.GetSetMethod() == null);
                    ReadOnlyAttribute attr = PropertyItem.GetAttribute<ReadOnlyAttribute>(pinfo);
                    if (attr != null)
                        readOnly = attr.IsReadOnly;

                    try
                    {
                        object value = pinfo.GetValue(objItem, null);
                        PropertyItem prop = new PropertyItem(objItem, null, value, pinfo, readOnly, false, null, null, null);
                        pc.Add(prop);
                    }
                    catch (Exception ex) { }
                }
            }

            #region attached property
            FrameworkElement fe = objItem as FrameworkElement;
            while (fe != null && fe.Parent != _SelectedObjectParent)
            {
                fe = fe.Parent as FrameworkElement;
            }

            int beginIndex = pc.Count;
            if (fe != null)
            {
                _SelectedObjectParent = fe.Parent as FrameworkElement;
                if (_SelectedObjectParent != null)
                {
                    Type parentType = _SelectedObjectParent.GetType();


                    foreach (string name in propertyNames)
                    {
                        FieldInfo field = parentType.GetField(name + "Property", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                        if (field == null)
                            continue;
                        if (field.FieldType == typeof(DependencyProperty))
                        {                            
                            MethodInfo get = null;
                            get = parentType.GetMethod("Get" + name, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                            MethodInfo set = null;
                            set = parentType.GetMethod("Set" + name, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);


                            if (get == null && set == null)
                                continue;

                            ParameterInfo[] getMethodParams = get.GetParameters();
                            if (getMethodParams.Length != 1)
                            {
                                continue;
                            }

                            ParameterInfo[] setMethodParams = set.GetParameters();
                            bool readOnly = true;
                            if (setMethodParams.Length == 2)
                            {
                                readOnly = false;
                            }

                            //loại bỏ property trùng
                            int matchCount = 0;
                            for (int i = beginIndex - 1; i < pc.Count; i++)
                            {
                                if (pc[i].Name == name)
                                {
                                    matchCount = 1;
                                    break;
                                }
                            }
                            if (matchCount == 1)
                                continue;

                            try
                            {
                                //get the default value
                                DependencyProperty dp = (DependencyProperty)field.GetValue(_SelectedObjectParent);

                                object currentValue = fe.GetValue(dp);
                                //now have both the value and the default, we're away!

                                PropertyItem propAttached = new PropertyItem(objItem, _SelectedObjectParent, currentValue, null, readOnly, true, name, get, set);
                                pc.Add(propAttached);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
            }
            #endregion

            return pc;
        }

        static PropertyGridLabel CreateLabel(string name, string displayName)
        {
            TextBlock txt = new TextBlock()
            {
                Text = displayName,
                Margin = new Thickness(0)
            };
            return new PropertyGridLabel()
            {
                Name = Guid.NewGuid().ToString("N"),
                Content = txt
            };
        }
        static Border GetCategoryMargin(string tagValue, Image hide, Image show)
        {
            StackPanel stp = new StackPanel()
            {
                Name = Guid.NewGuid().ToString("N"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            stp.Tag = tagValue;
            stp.Children.Add(hide);
            stp.Children.Add(show);

            Border brd = new Border() { Background = new SolidColorBrush(backgroundColor) };
            brd.Child = stp;

            return brd;
        }
        static Border GetCategoryHeader(string category)
        {
            TextBlock txt = new TextBlock()
            {
                Name = Guid.NewGuid().ToString("N"),
                Text = category,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Foreground = new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(3, 0, 0, 0),
                FontWeight = FontWeights.Bold,
                FontFamily = new FontFamily("Portable User Interface")
            };

            Border brd = new Border();
            brd.Background = new SolidColorBrush(backgroundColor);
            brd.Child = txt;
            Canvas.SetZIndex(brd, 1);
            return brd;
        }
        static Border GetItemMargin(string tagValue)
        {
            return new Border()
            {
                Name = Guid.NewGuid().ToString("N"),
                Margin = new Thickness(0),
                BorderThickness = new Thickness(0),
                Background = new SolidColorBrush(backgroundColor),
                Tag = tagValue
            };
        }
        static Border GetItemLabel(PropertyGridLabel label, string tagValue)
        {
            return new Border()
            {
                Name = Guid.NewGuid().ToString("N"),
                Margin = new Thickness(0),
                BorderBrush = new SolidColorBrush(backgroundColor),
                BorderThickness = new Thickness(0, 0, 1, 1),
                Child = label,
                Tag = tagValue
            };
        }
        static Border GetItemEditor(ValueEditorBase editor, string tagValue)
        {
            Border brd = new Border()
            {
                Name = Guid.NewGuid().ToString("N"),
                Margin = new Thickness(1, 0, 0, 0),
                BorderThickness = new Thickness(0, 0, 0, 1),
                BorderBrush = new SolidColorBrush(backgroundColor)
            };
            brd.Child = editor;
            brd.Tag = tagValue;
            return brd;
        }
        static Image GetImage(string imageUri)
        {
            //
            Image img = new Image()
            {
                Name = Guid.NewGuid().ToString("N"),
                Source = new BitmapImage(new Uri(imageUri, UriKind.Relative)),
                Height = 9,
                Width = 9,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            return img;
        }

        #endregion

        #region Event Handlers
        private void PropertyGrid_Loaded(object sender, RoutedEventArgs e)
        {
            this.MouseEnter += new MouseEventHandler(PropertyGrid_MouseEnter);
            this.MouseLeave += new MouseEventHandler(PropertyGrid_MouseLeave);
        }
        private void Editor_GotFocus(object sender, RoutedEventArgs e)
        {
            if (null != selectedEditor)
                selectedEditor.IsSelected = false;
            selectedEditor = sender as ValueEditorBase;
            if (null != selectedEditor)
            {
                listStoryboardMouseLeave[selectedEditor.Label].Stop();
                selectedEditor.IsSelected = true;

                //double editorX = ((UIElement)selectedEditor.Parent).RenderTransformOrigin.X;
                //Debug.WriteLine("editorX: " + editorX.ToString());
                //double editorY = ((UIElement)selectedEditor.Parent).RenderTransformOrigin.Y;
                //Debug.WriteLine("editorY: " + editorY.ToString());

                //double thisWidth = this.RenderSize.Width;
                //Debug.WriteLine("thisWidth: " + thisWidth.ToString());
                //double thisHeight = this.RenderSize.Height;
                //Debug.WriteLine("thisHeight: " + thisHeight.ToString());

            }
        }
        private void PropertyGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            this.AttachWheelEvents();
        }
        private void PropertyGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            this.DetachWheelEvents();
        }
        private void OnMouseWheel(object sender, HtmlEventArgs args)
        {
            double mouseDelta = 0;
            ScriptObject e = args.EventObject;

            // Mozilla and Safari    
            if (e.GetProperty("detail") != null)
            {
                mouseDelta = ((double)e.GetProperty("detail"));
            }

                // IE and Opera    
            else if (e.GetProperty("wheelDelta") != null)
                mouseDelta = ((double)e.GetProperty("wheelDelta"));

            mouseDelta = Math.Sign(mouseDelta);
            mouseDelta = mouseDelta * 1;
            mouseDelta = mouseDelta * 40; // Just a guess at an acceleration
            mouseDelta = this.LayoutRoot.VerticalOffset + mouseDelta;
            this.LayoutRoot.ScrollToVerticalOffset(mouseDelta);
        }
        private void CategoryHide_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement ctl = sender as FrameworkElement;
            Panel stp = ctl.Parent as Panel;
            string tagValue = (string)stp.Tag;
            stp.Children[0].Visibility = Visibility.Collapsed;
            stp.Children[1].Visibility = Visibility.Visible;
            this.Dispatcher.BeginInvoke(delegate()
            {
                ToggleCategoryVisible(false, tagValue);
            });
        }
        private void CategoryShow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement ctl = sender as FrameworkElement;
            Panel stp = ctl.Parent as Panel;
            string tagValue = (string)stp.Tag;
            stp.Children[0].Visibility = Visibility.Visible;
            stp.Children[1].Visibility = Visibility.Collapsed;
            this.Dispatcher.BeginInvoke(delegate()
            {
                ToggleCategoryVisible(true, tagValue);
            });
        }
        #endregion

        public void SetPropertyReadonly(string propertyName, bool isReadonly)
        {
            for (int i = 0; i < editors.Count; i++)
            {
                if (editors[i].Property.Name == propertyName)
                {
                    editors[i].Property.ReadOnly = isReadonly;
                    editors[i].UpdateLabelColor();
                    break;
                }
            }
        }

        public void UpdatePropertyValue(string propertyName)
        {
            if (propertyName == "Left")
            {
                _editorLeft.UpdatePropertyValue();
                return;
            }
            if (propertyName == "Top")
            {
                _editorTop.UpdatePropertyValue();
                return;
            }
            for (int i = 0; i < editors.Count; i++)
            {
                if (editors[i].Property.Name == propertyName)
                {
                    editors[i].UpdatePropertyValue();
                    break;
                }
            }
        }

        public void UpdateAllPropertyValue()
        {
            for (int i = 0; i < editors.Count; i++)
            {
                Border brd = GetItemEditor(editors[i], editors[i].Property.Category);
                MainGrid.Children[i * 3 - 1] = brd;
            }
        }
    }
    #endregion
}
