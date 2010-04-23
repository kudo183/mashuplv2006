using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Collections.Generic;
using Selector;
using System.Collections.ObjectModel;

namespace ControlsLibrary
{

    public class UniformGrid : ItemsControl
    {
        #region private members

        //private SelectManager _selectManager;
        private ObservableCollection<ISelectable> _selectableCollection;
        private SelectionManager _selectManager;
        public event EventHandler<SelectionChangedEventArgs> SelectionChange;

        private Dictionary<object, ItemContainer> _objectToItemContainer;
        //private ObservableDictionary<object, ItemContainer> _objectToItemContainer;
        //private Collection<object> _objectToItemContainer;

        private ItemsPresenter ItemsPresenter;
        private int _lastColumnIndex = 0;
        private int _lastRowIndex = 0;

        #endregion

        public UniformGrid()
        {
            DefaultStyleKey = typeof( UniformGrid );
            _selectableCollection = new ObservableCollection<ISelectable>();
            _selectManager = new SelectionManager();
            //ObservableCollection<ISelectable> e = ObjectToItemContainer.Values as ObservableCollection<ItemContainer>;
            //_selectManager.HookOnSelectedEvent(  e );

            _selectManager.HookOnSelectionChangedEvent( _selectableCollection );
            _selectManager.SelectionChange += new EventHandler<SelectionChangedEventArgs>( _selectManager_SelectionChange );
            ItemWidth = double.NaN;
            ItemHeight = double.NaN;
        }

        void _selectManager_SelectionChange( object sender, SelectionChangedEventArgs e )
        {
            if( this.SelectionChange != null )
                this.SelectionChange( this, e );
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ItemsPresenter = ( ItemsPresenter )GetTemplateChild( "ItemsPresenter" );
            ItemsPresenter.LayoutUpdated += new EventHandler( ItemsPresenter_LayoutUpdated );
        }

        void ItemsPresenter_LayoutUpdated( object sender, EventArgs e )
        {
            ItemsPresenter.LayoutUpdated -= new EventHandler( ItemsPresenter_LayoutUpdated );
            ItemsPresenter = ( ItemsPresenter )GetTemplateChild( "ItemsPresenter" );
            UpdateMeasure();
            //UpdateSizes();
        }

        #region ItemWidth property

        public static readonly DependencyProperty           ItemWidthProperty =
                               DependencyProperty.Register( "ItemWidthProperty",
                                                            typeof( double ),
                                                            typeof( UniformGrid ),
                                                            new PropertyMetadata( UniformGrid.OnItemWidthChanged ) );

        public double ItemWidth
        {
            get
            {
                return ( double )this.GetValue( ItemWidthProperty );
            }
            set
            {
                base.SetValue( UniformGrid.ItemWidthProperty, value );
            }
        }

        protected static void OnItemWidthChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as UniformGrid ).OnItemWidthChanged( ( double )e.OldValue, ( double )e.NewValue );
        }

        internal virtual void OnItemWidthChanged( double oldValue, double newValue )
        {
            UpdateContainderWidth();
        }

        protected void UpdateContainderWidth()
        {
            foreach ( object obj in _selectableCollection )
            {
                ItemContainer ItemContainerForObject = this.GetItemContainerForObject( obj );
                ItemContainerForObject.ContentWidth = ItemWidth;
            }
        }

        #endregion

        #region ItemHeight property

        public static readonly DependencyProperty           ItemHeightProperty =
                               DependencyProperty.Register( "ItemHeightProperty",
                                                            typeof( double ),
                                                            typeof( UniformGrid ),
                                                            new PropertyMetadata( UniformGrid.OnItemHeightChanged ) );
        
        public double ItemHeight
        {
            get
            {
                return ( double )this.GetValue( ItemHeightProperty );
            }
            set
            {
                base.SetValue( UniformGrid.ItemHeightProperty, value );
            }
        }

        protected static void OnItemHeightChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as UniformGrid ).OnItemHeightChanged( ( double )e.OldValue, ( double )e.NewValue );
        }

        internal virtual void OnItemHeightChanged( double oldValue, double newValue )
        {
            foreach ( object obj in _selectableCollection )
            {
                ItemContainer ItemContainerForObject = this.GetItemContainerForObject( obj );
                ItemContainerForObject.ContentHeight = ItemHeight;
            }
        }

        #endregion

        #region arrangement methods

        protected void UpdateMeasure()
        {
            int ss = 0;
            if ( ItemsPresenter != null )
                ss = VisualTreeHelper.GetChildrenCount( ItemsPresenter );

            Grid g = new Grid();
            for ( int i = 0 ; i < ss ; i++ )
            {
                DependencyObject target = VisualTreeHelper.GetChild( ItemsPresenter, i );
                if ( target is Grid )
                {
                    g = target as Grid;

                    g.RowDefinitions.Clear();
                    for ( int r = 0 ; r < Rows ; r++ )
                        g.RowDefinitions.Add( new RowDefinition() );

                    g.ColumnDefinitions.Clear();
                    for ( int c = 0 ; c < Columns ; c++ )
                        g.ColumnDefinitions.Add( new ColumnDefinition() );
                }
            }
        }

        protected void ArrangePanel()
        {
            _lastColumnIndex = 0;
            _lastRowIndex = 0;
            foreach ( object obj in base.Items )
            {
                ItemContainer ItemContainerForObject = this.GetItemContainerForObject( obj );
                ArrangeItem( ItemContainerForObject );
            }
        }

        protected void ArrangeItem( ItemContainer item )
        {
            item.SetValue( Grid.ColumnProperty, _lastColumnIndex );
            item.SetValue( Grid.RowProperty, _lastRowIndex );
            GoToNextIndex();
        }

        protected void GoToNextIndex()
        {
            if ( ChildrenFlow == Orientation.Horizontal )
            {
                if ( AutoFill == true && _lastRowIndex >= Rows )
                    Rows++;
                if ( _lastColumnIndex < Columns - 1 )
                    _lastColumnIndex++;
                else
                {
                    _lastColumnIndex = 0;
                    _lastRowIndex++;
                }

            }
            else if ( ChildrenFlow == Orientation.Vertical )
            {
                if ( AutoFill == true && _lastColumnIndex >= Columns )
                    Columns++;


                if ( _lastRowIndex < Rows - 1 )
                    _lastRowIndex++;
                else
                {
                    _lastRowIndex = 0;
                    _lastColumnIndex++;
                }
            }
        }

        protected void GoToPreviousIndex()
        {
            if ( ChildrenFlow == Orientation.Horizontal )
            {
                _lastColumnIndex--;
                if ( _lastColumnIndex < 0 )
                {
                    _lastColumnIndex = Columns - 1;
                    _lastRowIndex--;
                }
                if ( AutoFill == true && _lastColumnIndex == 0 )
                    Rows--;
            }
            else if ( ChildrenFlow == Orientation.Vertical )
            {
                _lastRowIndex--;
                if ( _lastRowIndex < 0 )
                {
                    _lastRowIndex = Rows - 1;
                    _lastColumnIndex--;
                }
                if ( AutoFill == true && _lastRowIndex == 0 )
                    Columns--;
            }
        }

        #endregion

        #region AutoFill property

        public static readonly DependencyProperty           AutoFillProperty =
                               DependencyProperty.Register( "AutoFillProperty",
                                                            typeof( bool ),
                                                            typeof( UniformGrid ),
                                                            new PropertyMetadata( UniformGrid.OnAutoFillChanged ) );

        public bool AutoFill
        {
            get
            {
                return ( bool )this.GetValue( AutoFillProperty );
            }
            set
            {
                base.SetValue( UniformGrid.AutoFillProperty, value );
            }
        }

        protected static void OnAutoFillChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as UniformGrid ).OnAutoFillChanged( ( bool )e.OldValue, ( bool )e.NewValue );
        }

        internal virtual void OnAutoFillChanged( bool oldValue, bool newValue )
        {
            // Here could implement better logic to change the number of rows/columns to fill the viewer's area
            //UpdateMeasurement();
            //ArrangePanel();
        }

        #endregion

        #region adapter Rows property

        public static readonly DependencyProperty            RowsProperty =
                               DependencyProperty.Register( "RowsProperty",
                                                             typeof( int ),
                                                             typeof( UniformGrid ),
                                                             new PropertyMetadata( UniformGrid.OnRowsChanged ) );

        public int Rows
        {
            get
            {
                return ( int )this.GetValue( RowsProperty );
            }
            set
            {
                if ( value < 1 )
                    value = 1;
                base.SetValue( UniformGrid.RowsProperty, value );
            }
        }

        protected static void OnRowsChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as UniformGrid ).OnRowsChanged( ( int )e.OldValue, ( int )e.NewValue );
        }

        internal virtual void OnRowsChanged( int oldValue, int newValue )
        {
            UpdateMeasure();
        }

        #endregion

        #region adapter Columns property

        public static readonly DependencyProperty           ColumnsProperty =
                               DependencyProperty.Register( "ColumnsProperty",
                                                            typeof( int ),
                                                            typeof( UniformGrid ),
                                                            new PropertyMetadata( UniformGrid.OnColumnsChanged ) );

        public int Columns
        {
            get
            {
                return ( int )this.GetValue( ColumnsProperty );
            }
            set
            {
                if ( value < 1 )
                    value = 1;
                base.SetValue( UniformGrid.ColumnsProperty, value );
            }
        }

        protected static void OnColumnsChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as UniformGrid ).OnColumnsChanged( ( int )e.OldValue, ( int )e.NewValue );
        }

        internal virtual void OnColumnsChanged( int oldValue, int newValue )
        {
            UpdateMeasure();
        }

        #endregion

        #region ChildrenFlow property

        public static readonly DependencyProperty            ChildrenFlowProperty =
                               DependencyProperty.Register( "ChildrenFlowProperty",
                                                             typeof( Orientation ),
                                                             typeof( UniformGrid ),
                                                             new PropertyMetadata( UniformGrid.OnChildrenFlowChanged ) );

        public Orientation ChildrenFlow
        {
            get
            {
                return ( Orientation )this.GetValue( ChildrenFlowProperty );
            }
            set
            {
                base.SetValue( UniformGrid.ChildrenFlowProperty, value );
            }
        }

        private static void OnChildrenFlowChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            UniformGrid grid = ( UniformGrid )d;
            grid.OnChildrenFlowChanged( ( Orientation )e.OldValue, ( Orientation )e.NewValue );
        }

        internal virtual void OnChildrenFlowChanged( Orientation oldItemContainerOrientation, Orientation newItemContainerOrientation )
        {
            if ( oldItemContainerOrientation != newItemContainerOrientation )
            {
                ArrangePanel();
            }
        }

        #endregion

        #region ItemContainerStyle property

        public static readonly DependencyProperty           ItemContainerStyleProperty =
                               DependencyProperty.Register( "ItemContainerStyleProperty",
                                                            typeof( Style ),
                                                            typeof( UniformGrid ),
                                                            new PropertyMetadata(
                                                                new PropertyChangedCallback(
                                                                    UniformGrid.OnItemContainerStyleChanged ) ) );

        public Style ItemContainerStyle
        {
            get
            {
                return ( Style )this.GetValue( ItemContainerStyleProperty );
            }
            set
            {
                base.SetValue( ItemContainerStyleProperty, ( DependencyObject )value );
            }
        }

        private static void OnItemContainerStyleChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as UniformGrid ).OnItemContainerStyleChanged( ( Style )e.OldValue, ( Style )e.NewValue );
        }

        internal virtual void OnItemContainerStyleChanged( Style oldItemContainerStyle, Style newItemContainerStyle )
        {
            if ( oldItemContainerStyle != newItemContainerStyle )
                foreach ( object obj2 in base.Items )
                {
                    ItemContainer ItemContainerForObject = this.GetItemContainerForObject( obj2 );
                    if ( ( ItemContainerForObject != null ) && ( ( ItemContainerForObject.Style == null ) || ( oldItemContainerStyle == ItemContainerForObject.Style ) ) )
                    {
                        if ( ItemContainerForObject.Style != null )
                        {
                            throw new NotSupportedException( null );
                        }
                        (ItemContainerForObject.Content as FrameworkElement).Style = newItemContainerStyle;
                    }
                }
        }

        #endregion

        #region ItemAnimationContainerStyle property

        public static readonly DependencyProperty ItemAnimationContainerStyleProperty =
                               DependencyProperty.Register( "ItemAnimationContainerStyleProperty",
                                                            typeof( Style ),
                                                            typeof( UniformGrid ),
                                                            new PropertyMetadata(
                                                                new PropertyChangedCallback(
                                                                    UniformGrid.OnItemAnimationContainerStyleChanged ) ) );

        public Style ItemAnimationContainerStyle
        {
            get
            {
                return ( Style )this.GetValue( ItemAnimationContainerStyleProperty );
            }
            set
            {
                base.SetValue( ItemAnimationContainerStyleProperty, ( DependencyObject )value );
            }
        }

        private static void OnItemAnimationContainerStyleChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as UniformGrid ).OnItemAnimationContainerStyleChanged( ( Style )e.OldValue, ( Style )e.NewValue );
        }

        internal virtual void OnItemAnimationContainerStyleChanged( Style oldItemAnimationContainerStyle, Style newItemAnimationContainerStyle )
        {
            if ( oldItemAnimationContainerStyle != newItemAnimationContainerStyle )
                foreach ( object obj2 in base.Items )
                {
                    ItemContainer ItemContainerForObject = this.GetItemContainerForObject( obj2 );
                    if ( ( ItemContainerForObject != null ) && ( ( ItemContainerForObject.Style == null ) || ( oldItemAnimationContainerStyle == ItemContainerForObject.Style ) ) )
                    {
                        if ( ItemContainerForObject.Style != null )
                        {
                            throw new NotSupportedException( null );
                        }
                        ItemContainerForObject.Style = newItemAnimationContainerStyle;
                    }
                }
        }

        #endregion

        #region ItemContainer methods

        private ItemContainer GetItemContainerForObject( object value )
        {
            ItemContainer item = value as ItemContainer;
            if ( item == null )
            {
                this.ObjectToItemContainer.TryGetValue( value, out item );
            }
            return item;
        }

        private IDictionary<object, ItemContainer> ObjectToItemContainer
        {
            get
            {
                if ( this._objectToItemContainer == null )
                {
                    this._objectToItemContainer = new Dictionary<object, ItemContainer>();
                }
                return this._objectToItemContainer;
            }
        }

        #endregion

        #region overriden methods

        protected override DependencyObject GetContainerForItemOverride()
        {
            ItemContainer item = new ItemContainer();
            
            if ( this.ItemContainerStyle != null )
            {
                item.Style = this.ItemContainerStyle;
            }
            return item;
        }

        protected override bool IsItemItsOwnContainerOverride( object item )
        {
            return ( item is ItemContainer );
        }

        protected override void PrepareContainerForItemOverride( DependencyObject element, object item )
        {
            base.PrepareContainerForItemOverride( element, item );
            ItemContainer item2 = element as ItemContainer;
            bool flag = true;
            if ( item2 != item )
            {
                if ( base.ItemTemplate != null )
                {
                    item2.ContentTemplate = base.ItemTemplate;
                }
                else if ( !string.IsNullOrEmpty( base.DisplayMemberPath ) )
                {
                    Binding binding = new Binding( base.DisplayMemberPath );
                    item2.SetBinding( ContentControl.ContentProperty, binding );
                    flag = false;
                }
                if ( flag )
                {
                    item2.Content = item;
                }
                //item2.Width = ItemWidth + 20;
                item2.ContentWidth = ItemWidth;
                item2.Width = Height * ( ItemWidth / ItemHeight );
                item2.Height = Height;

                item2.ContentHeight = ItemHeight;
                this.ArrangeItem( item2 );
                this._selectableCollection.Add( item2 );
                this.ObjectToItemContainer[ item ] = item2;
                
            }
            if ( ( this.ItemContainerStyle != null ) && ( item2.Style == null ) )
            {
                item2.Style = this.ItemContainerStyle;
            }
        }

        protected override void ClearContainerForItemOverride( DependencyObject element, object item )
        {
            base.ClearContainerForItemOverride( element, item );
            ItemContainer item2 = element as ItemContainer;
            if ( item == null )
            {
                item = ( item2.Content == null ) ? item2 : item2.Content;
            }
            if ( item2 != item )
            {
                this._selectableCollection.Remove( ObjectToItemContainer[ item ] );
                this.ObjectToItemContainer.Remove( item );
                GoToPreviousIndex();
            }
        }

        #endregion
    }
}
