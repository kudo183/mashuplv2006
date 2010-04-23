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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Data;

namespace ControlsLibrary
{
    public class UniformGridNew : ItemsSelector
    {
        #region private members

        private Dictionary<object, ItemContainer> _objectToItemContainer;
        private ItemsPresenter ItemsPresenter;
        private int _lastColumnIndex = 0;
        private int _lastRowIndex = 0;

        #endregion

        protected ICollection<ItemContainer> GetContainers()
        {
            ICollection<ItemContainer> r = ObjectToItemContainer.Values;
            return r;

        }
        public UniformGridNew()
        {
            DefaultStyleKey = typeof( UniformGridNew );
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
        }

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

        public static readonly DependencyProperty AutoFillProperty =
                               DependencyProperty.Register( "AutoFillProperty",
                                                            typeof( bool ),
                                                            typeof( UniformGridNew ),
                                                            new PropertyMetadata( UniformGridNew.OnAutoFillChanged ) );

        public bool AutoFill
        {
            get
            {
                return ( bool )this.GetValue( AutoFillProperty );
            }
            set
            {
                base.SetValue( UniformGridNew.AutoFillProperty, value );
            }
        }

        protected static void OnAutoFillChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as UniformGridNew ).OnAutoFillChanged( ( bool )e.OldValue, ( bool )e.NewValue );
        }

        internal virtual void OnAutoFillChanged( bool oldValue, bool newValue )
        {
            UpdateMeasure();
            ArrangePanel();
        }

        #endregion

        #region adapter Rows property

        public static readonly DependencyProperty RowsProperty =
                               DependencyProperty.Register( "RowsProperty",
                                                             typeof( int ),
                                                             typeof( UniformGridNew ),
                                                             new PropertyMetadata( UniformGridNew.OnRowsChanged ) );

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
                base.SetValue( UniformGridNew.RowsProperty, value );
            }
        }

        protected static void OnRowsChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as UniformGridNew ).OnRowsChanged( ( int )e.OldValue, ( int )e.NewValue );
        }

        internal virtual void OnRowsChanged( int oldValue, int newValue )
        {
            UpdateMeasure();
        }

        #endregion

        #region adapter Columns property

        public static readonly DependencyProperty ColumnsProperty =
                               DependencyProperty.Register( "ColumnsProperty",
                                                            typeof( int ),
                                                            typeof( UniformGridNew ),
                                                            new PropertyMetadata( UniformGridNew.OnColumnsChanged ) );

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
                base.SetValue( UniformGridNew.ColumnsProperty, value );
            }
        }

        protected static void OnColumnsChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as UniformGridNew ).OnColumnsChanged( ( int )e.OldValue, ( int )e.NewValue );
        }

        internal virtual void OnColumnsChanged( int oldValue, int newValue )
        {
            UpdateMeasure();
        }

        #endregion

        #region ChildrenFlow property

        public static readonly DependencyProperty ChildrenFlowProperty =
                               DependencyProperty.Register( "ChildrenFlowProperty",
                                                             typeof( Orientation ),
                                                             typeof( UniformGridNew ),
                                                             new PropertyMetadata( UniformGridNew.OnChildrenFlowChanged ) );

        public Orientation ChildrenFlow
        {
            get
            {
                return ( Orientation )this.GetValue( ChildrenFlowProperty );
            }
            set
            {
                base.SetValue( UniformGridNew.ChildrenFlowProperty, value );
            }
        }

        private static void OnChildrenFlowChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            UniformGridNew grid = ( UniformGridNew )d;
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

        public static readonly DependencyProperty ItemContainerStyleProperty =
                               DependencyProperty.Register( "ItemContainerStyleProperty",
                                                            typeof( Style ),
                                                            typeof( UniformGridNew ),
                                                            new PropertyMetadata(
                                                                new PropertyChangedCallback(
                                                                    UniformGridNew.OnItemContainerStyleChanged ) ) );

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
            ( d as UniformGridNew ).OnItemContainerStyleChanged( ( Style )e.OldValue, ( Style )e.NewValue );
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
                        ItemContainerForObject.ContentStyle = newItemContainerStyle;
                    }
                }
        }

        #endregion

        #region ItemAnimationContainerStyle property

        public static readonly DependencyProperty ItemAnimationContainerStyleProperty =
                               DependencyProperty.Register( "ItemAnimationContainerStyleProperty",
                                                            typeof( Style ),
                                                            typeof( UniformGridNew ),
                                                            new PropertyMetadata(
                                                                new PropertyChangedCallback(
                                                                    UniformGridNew.OnItemAnimationContainerStyleChanged ) ) );

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
            ( d as UniformGridNew ).OnItemAnimationContainerStyleChanged( ( Style )e.OldValue, ( Style )e.NewValue );
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

        protected ItemContainer GetItemContainerForObject( object value )
        {
            ItemContainer item = value as ItemContainer;
            if ( item == null )
            {
                this.ObjectToItemContainer.TryGetValue( value, out item );
            }
            return item;
        }

        protected IDictionary<object, ItemContainer> ObjectToItemContainer
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

            if ( this.ItemAnimationContainerStyle != null )
            {
                item.Style = this.ItemAnimationContainerStyle;
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
                this.ArrangeItem( item2 );
                this.ObjectToItemContainer[ item ] = item2;

            }

            if ( ( this.ItemAnimationContainerStyle != null ) && ( item2.Style == null ) )
            {
                item2.Style = this.ItemAnimationContainerStyle;
            }
            if ( ( this.ItemContainerStyle != null ) && ( item2.ContentStyle == null ) )
            {
                item2.ContentStyle = this.ItemContainerStyle;
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
                this.ObjectToItemContainer.Remove( item );
                GoToPreviousIndex();
            }
        }

        #endregion
    }
}
