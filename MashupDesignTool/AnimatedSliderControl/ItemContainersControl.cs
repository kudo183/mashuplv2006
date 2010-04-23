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

namespace AnimatedSliderControl
{
    public class ItemContainersControl : ItemsSelector
    {
        private Dictionary<object, ItemContainer> _objectToItemContainer;
     
        public ItemContainersControl()
        {
            ItemWidth = double.NaN;
            ItemHeight = double.NaN;
        }
        
        #region  ItemWidth property

        public static readonly DependencyProperty ItemWidthProperty =
                              DependencyProperty.Register( "ItemWidthProperty",
                                                           typeof( double ),
                                                           typeof( ItemContainersControl ),
                                                           new PropertyMetadata( ItemContainersControl.OnItemWidthChanged ) );

        public double ItemWidth
        {
            get
            {
                return ( double )this.GetValue( ItemWidthProperty );
            }
            set
            {
                base.SetValue( ItemContainersControl.ItemWidthProperty, value );
            }
        }

        protected static void OnItemWidthChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as ItemContainersControl ).OnItemWidthChanged( ( double )e.OldValue, ( double )e.NewValue );
        }

        internal virtual void OnItemWidthChanged( double oldValue, double newValue )
        {
            UpdateContainderWidth();
        }

        protected void UpdateContainderWidth()
        {
            foreach ( object obj in ObjectToItemContainer.Values )//GetContainers()
            {
                ItemContainer ItemContainerForObject = this.GetItemContainerForObject( obj );
                ItemContainerForObject.ContentWidth = ItemWidth;
            }
        }

        #endregion

        #region ItemHeight property

        public static readonly DependencyProperty ItemHeightProperty =
                               DependencyProperty.Register( "ItemHeightProperty",
                                                            typeof( double ),
                                                            typeof( ItemContainersControl ),
                                                            new PropertyMetadata( ItemContainersControl.OnItemHeightChanged ) );

        public double ItemHeight
        {
            get
            {
                return ( double )this.GetValue( ItemHeightProperty );
            }
            set
            {
                base.SetValue( ItemContainersControl.ItemHeightProperty, value );
            }
        }

        protected static void OnItemHeightChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as ItemContainersControl ).OnItemHeightChanged( ( double )e.OldValue, ( double )e.NewValue );
        }

        internal virtual void OnItemHeightChanged( double oldValue, double newValue )
        {
            foreach ( object obj in ObjectToItemContainer.Values )//GetContainers()
            {
                ItemContainer ItemContainerForObject = this.GetItemContainerForObject( obj );
                ItemContainerForObject.ContentHeight = ItemHeight;
            }
        }

        #endregion

        #region ItemStyle property

        public static readonly DependencyProperty ItemStyleProperty =
                               DependencyProperty.Register( "ItemContentStyleProperty",
                                                            typeof( Style ),
                                                            typeof( ItemContainersControl ),
                                                            new PropertyMetadata(
                                                                new PropertyChangedCallback(
                                                                    ItemContainersControl.OnItemStyleChanged ) ) );

        public Style ItemStyle
        {
            get
            {
                return ( Style )this.GetValue( ItemStyleProperty );
            }
            set
            {
                base.SetValue( ItemStyleProperty, ( DependencyObject )value );
            }
        }

        private static void OnItemStyleChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as ItemContainersControl ).OnItemStyleChanged( ( Style )e.OldValue, ( Style )e.NewValue );
        }

        internal virtual void OnItemStyleChanged( Style oldItemContainerStyle, Style newItemContainerStyle )
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

        #region ContainerStyle property

        public static readonly DependencyProperty ContainerStyleProperty =
                               DependencyProperty.Register( "ItemAnimationContainerStyleProperty",
                                                            typeof( Style ),
                                                            typeof( ItemContainersControl ),
                                                            new PropertyMetadata(
                                                                new PropertyChangedCallback(
                                                                    ItemContainersControl.OnContainerStyleChanged ) ) );

        public Style ContainerStyle
        {
            get
            {
                return ( Style )this.GetValue( ContainerStyleProperty );
            }
            set
            {
                base.SetValue( ContainerStyleProperty, ( DependencyObject )value );
            }
        }

        private static void OnContainerStyleChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as ItemContainersControl ).OnContainerStyleChanged( ( Style )e.OldValue, ( Style )e.NewValue );
        }

        internal virtual void OnContainerStyleChanged( Style oldItemAnimationContainerStyle, Style newItemAnimationContainerStyle )
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

        #region overridden methods

        protected override DependencyObject GetContainerForItemOverride()
        {
            ItemContainer item = new ItemContainer();

            if ( this.ContainerStyle != null )
            {
                item.Style = this.ContainerStyle;
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
                
                this.ObjectToItemContainer[ item ] = item2;
                
            }

            if ( ( this.ContainerStyle != null ) && ( item2.Style == null ) )
            {
                item2.Style = this.ContainerStyle;
            }
            if ( ( this.ItemStyle != null ) && ( item2.ContentStyle == null ) )
            {
                item2.ContentStyle = this.ItemStyle;
            }
            if ( item2 != null )
            {
                item2.ContentWidth = ItemWidth;
                item2.Width = Height * ( ItemWidth / ItemHeight );
                item2.Height = Height;
                item2.ContentHeight = ItemHeight;
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
            }
        }

        #endregion
    }
}
