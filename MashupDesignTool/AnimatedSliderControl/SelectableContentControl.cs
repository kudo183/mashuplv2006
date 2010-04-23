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
using System.Collections.Generic;
using System.Collections;

namespace AnimatedSliderControl
{
    [TemplatePart( Name = "ContentPresenter", Type = typeof( ContentControl ) )]
    [TemplatePart( Name = "ContentContainer", Type = typeof( FrameworkElement ) )]


    [TemplateVisualState( Name = "Normal", GroupName = "CommonStates" )]

    [TemplateVisualState( Name = "MouseOver", GroupName = "CommonStates" )]

    [TemplateVisualState( Name = "Pressed", GroupName = "CommonStates" )]

    [TemplateVisualState( Name = "Selected", GroupName = "SelectionStates" )]

    [TemplateVisualState( Name = "Deselected", GroupName = "SelectionStates" )]
    public class SelectableContentControl : ContentControl, ISelectable
    {

        #region private members

        private bool isMouseOver, isPressed;
        private ContentControl ContentPresenter; //ContentPresenter
        private FrameworkElement ContentContainer;
        public event EventHandler Selected;

        #endregion

        public SelectableContentControl()
        {
            DefaultStyleKey = typeof( SelectableContentControl );
            ContentWidth = double.NaN;
            ContentHeight = double.NaN;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ContentPresenter = ( ContentControl )GetTemplateChild( "ContentPresenter" );
            if ( ContentPresenter != null )
            {
                ContentPresenter.MouseEnter += new MouseEventHandler( ContentPresenter_MouseEnter );
                ContentPresenter.MouseLeave += new MouseEventHandler( ContentPresenter_MouseLeave );
                ContentPresenter.MouseLeftButtonDown += new MouseButtonEventHandler( ContentPresenter_MouseLeftButtonDown );
                ContentPresenter.MouseLeftButtonUp += new MouseButtonEventHandler( ContentPresenter_MouseLeftButtonUp );

                ContentPresenter.Style = ContentStyle;

            }
            ContentContainer = ( FrameworkElement )GetTemplateChild( "ContentContainer" );
            if ( ContentContainer != null )
            {
                UpdateContainerWidth();
                UpdateContainerHeight();
                //ContentContainer.Style = ContentStyle;

            }
            // Go to normal state without using any transitions
            GoToState( false );
        }

        #region private methods

        private void UpdateContainerWidth()
        {
            ContentContainer.Width = ContentWidth;
        }
        private void UpdateContainerHeight()
        {
            ContentContainer.Height = ContentHeight;
        }

        private void GoToState( bool useTransitions )
        {
            if ( isPressed )
            {
                VisualStateManager.GoToState( this, "Pressed", useTransitions );
            }
            else if ( isMouseOver )
            {
                VisualStateManager.GoToState( this, "MouseOver", useTransitions );
            }
            else
            {
                VisualStateManager.GoToState( this, "Normal", useTransitions );
            }
            if ( IsSelected )
            {
                VisualStateManager.GoToState( this, "Selected", useTransitions );
            }
            else
            {
                VisualStateManager.GoToState( this, "Deselected", useTransitions );
            }
        }

        #endregion

        #region event handlers

        void ContentPresenter_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            ContentPresenter.ReleaseMouseCapture();
            isPressed = false;
            GoToState( true );

        }

        void ContentPresenter_MouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {
            ContentPresenter.CaptureMouse();
            isPressed = true;
            IsSelected = true;
            GoToState( true );

        }

        void ContentPresenter_MouseLeave( object sender, MouseEventArgs e )
        {
            this.ReleaseMouseCapture();

            isMouseOver = false;
            this.SetValue( Canvas.ZIndexProperty, 0 );

            GoToState( true );
        }

        void ContentPresenter_MouseEnter( object sender, MouseEventArgs e )
        {
            this.CaptureMouse();

            isMouseOver = true;
            this.SetValue( Canvas.ZIndexProperty, 10 );
            
            GoToState( true );
        }

        #endregion

        #region ISelectable Members

        public void Select()
        {
            IsSelected = true;
        }

        public void Deselect()
        {
            IsSelected = false;
        }

        #endregion

        #region IsSelected property

        public static DependencyProperty IsSelectedProperty =
                      DependencyProperty.Register( "IsSelectedProperty",
                                                    typeof( bool ),
                                                    typeof( SelectableContentControl ),
                                                    new PropertyMetadata( SelectableContentControl.OnIsSelectedChange ) );
        public bool IsSelected
        {
            get
            {
                return ( bool )GetValue( SelectableContentControl.IsSelectedProperty );
            }
            set
            {
                SetValue( SelectableContentControl.IsSelectedProperty, value );

            }
        }

        private static void OnIsSelectedChange( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            SelectableContentControl c = d as SelectableContentControl;
            c.OnIsSelectedChange( ( bool )e.OldValue, ( bool )e.NewValue );
        }

        private void OnIsSelectedChange( bool oldValue, bool newValue )
        {
            EventHandler handler = Selected;

            if ( handler != null && newValue == true )
            {
                handler( this, new EventArgs() );
            }
            GoToState( true );
        }

        #endregion

        #region ContentWidth property

        public static readonly DependencyProperty ContentWidthProperty =
                               DependencyProperty.Register( "ContentWidthProperty",
                                                            typeof( double ),
                                                            typeof( SelectableContentControl ),
                                                            new PropertyMetadata( SelectableContentControl.OnContentWidthChanged ) );

        public double ContentWidth
        {
            get
            {
                return ( double )this.GetValue( ContentWidthProperty );
            }
            set
            {
                base.SetValue( SelectableContentControl.ContentWidthProperty, value );
            }
        }

        protected static void OnContentWidthChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as SelectableContentControl ).OnContentWidthChanged( ( double )e.OldValue, ( double )e.NewValue );
        }

        internal virtual void OnContentWidthChanged( double oldValue, double newValue )
        {
            if ( ContentContainer != null )
                UpdateContainerWidth();
        }

        #endregion

        #region ContentHeight property

        public static readonly DependencyProperty ContentHeightProperty =
                               DependencyProperty.Register( "ContentHeightProperty",
                                                            typeof( double ),
                                                            typeof( SelectableContentControl ),
                                                            new PropertyMetadata( SelectableContentControl.OnContentHeightChanged ) );

        public double ContentHeight
        {
            get
            {
                return ( double )this.GetValue( ContentHeightProperty );
            }
            set
            {
                base.SetValue( SelectableContentControl.ContentHeightProperty, value );
            }
        }

        protected static void OnContentHeightChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as SelectableContentControl ).OnContentHeightChanged( ( double )e.OldValue, ( double )e.NewValue );
        }

        internal virtual void OnContentHeightChanged( double oldValue, double newValue )
        {
            if ( ContentContainer != null )
                UpdateContainerHeight();
        }

        #endregion

        #region ContentStyle property

        public static readonly DependencyProperty ContentStyleProperty =
                               DependencyProperty.Register( "ContentStyleProperty",
                                                            typeof( Style ),
                                                            typeof( SelectableContentControl ),
                                                            new PropertyMetadata( SelectableContentControl.OnContentStyleChanged ) );

        public Style ContentStyle
        {
            get
            {
                return ( Style )this.GetValue( ContentStyleProperty );
            }
            set
            {
                base.SetValue( SelectableContentControl.ContentStyleProperty, value );
            }
        }

        protected static void OnContentStyleChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as SelectableContentControl ).OnContentStyleChanged( ( Style )e.OldValue, ( Style )e.NewValue );
        }

        internal virtual void OnContentStyleChanged( Style oldValue, Style newValue )
        {
            if ( ContentPresenter != null )
                ContentPresenter.Style = ContentStyle;
        }

        #endregion
    }
}
