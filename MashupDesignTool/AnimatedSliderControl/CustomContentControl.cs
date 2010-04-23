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
using ControlsLibrary;

namespace Selector
{
    [TemplatePart( Name = "ContentPresenter", Type = typeof( ContentPresenter ) )]
    [TemplatePart( Name = "ContentContainer", Type = typeof( FrameworkElement ) )]


    [TemplateVisualState( Name = "Normal", GroupName = "CommonStates" )]

    [TemplateVisualState( Name = "MouseOver", GroupName = "CommonStates" )]

    [TemplateVisualState( Name = "Pressed", GroupName = "CommonStates" )]

    [TemplateVisualState( Name = "Selected", GroupName = "SelectionStates" )]

    [TemplateVisualState( Name = "Deselected", GroupName = "SelectionStates" )]
    public class CustomContentControl : ContentControl,ISelectable
    {

        #region private members

        private bool isMouseOver, isPressed;
        private ContentPresenter ContentPresenter;
        private FrameworkElement ContentContainer;
        public event EventHandler Selected;

        #endregion

        public CustomContentControl()
        {
            DefaultStyleKey = typeof( CustomContentControl );
            ContentWidth = double.NaN;
            ContentHeight = double.NaN;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ContentPresenter = ( ContentPresenter )GetTemplateChild( "ContentPresenter" );
            if ( ContentPresenter != null )
            {
                ContentPresenter.MouseEnter += new MouseEventHandler( ContentPresenter_MouseEnter );
                ContentPresenter.MouseLeave += new MouseEventHandler( ContentPresenter_MouseLeave );
                ContentPresenter.MouseLeftButtonDown += new MouseButtonEventHandler( ContentPresenter_MouseLeftButtonDown );
                ContentPresenter.MouseLeftButtonUp += new MouseButtonEventHandler( ContentPresenter_MouseLeftButtonUp );

            }
            ContentContainer = ( FrameworkElement )GetTemplateChild( "ContentContainer" );
            if ( ContentContainer != null )
            {
                UpdateContainerWidth();
                UpdateContainerHeight();
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
            isPressed = false;
            GoToState( true );

        }

        void ContentPresenter_MouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {
            isPressed = true;
            IsSelected = true;
            GoToState( true );
        }

        void ContentPresenter_MouseLeave( object sender, MouseEventArgs e )
        {
            isMouseOver = false;
            GoToState( true );
        }

        void ContentPresenter_MouseEnter( object sender, MouseEventArgs e )
        {
            isMouseOver = true;
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
                                                    typeof( CustomContentControl ),
                                                    new PropertyMetadata( CustomContentControl.OnIsSelectedChange ) );
        public bool IsSelected
        {
            get
            {
                return ( bool )GetValue( CustomContentControl.IsSelectedProperty );
            }
            set
            {
                SetValue( CustomContentControl.IsSelectedProperty, value );

            }
        }

        private static void OnIsSelectedChange( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            CustomContentControl c = d as CustomContentControl;
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
                                                            typeof( CustomContentControl ),
                                                            new PropertyMetadata( CustomContentControl.OnContentWidthChanged ) );

        public double ContentWidth
        {
            get
            {
                return ( double )this.GetValue( ContentWidthProperty );
            }
            set
            {
                base.SetValue( CustomContentControl.ContentWidthProperty, value );
            }
        }

        protected static void OnContentWidthChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as CustomContentControl ).OnContentWidthChanged( ( double )e.OldValue, ( double )e.NewValue );
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
                                                            typeof( CustomContentControl ),
                                                            new PropertyMetadata( CustomContentControl.OnContentHeightChanged ) );

        public double ContentHeight
        {
            get
            {
                return ( double )this.GetValue( ContentHeightProperty );
            }
            set
            {
                base.SetValue( CustomContentControl.ContentHeightProperty, value );
            }
        }

        protected static void OnContentHeightChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as CustomContentControl ).OnContentHeightChanged( ( double )e.OldValue, ( double )e.NewValue );
        }

        internal virtual void OnContentHeightChanged( double oldValue, double newValue )
        {
            if ( ContentContainer != null )
                UpdateContainerHeight();
        }

        #endregion
    }
}
