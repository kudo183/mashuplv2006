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
using System.Windows.Controls.Primitives;

namespace AnimatedSliderControl
{

    [TemplatePart( Name = "LeftButton", Type = typeof( RepeatButton ) )]
    [TemplatePart( Name = "RightButton", Type = typeof( RepeatButton ) )]
    [TemplatePart( Name = "ToLeftStoryboard", Type = typeof( Storyboard ) )]
    [TemplatePart( Name = "ToRightStoryboard", Type = typeof( Storyboard ) )]
    [TemplatePart( Name = "View", Type = typeof( Canvas ) )]
    [TemplatePart( Name = "ItemsTemplate", Type = typeof( ItemsPresenter ) )]
    public class AnimatedSlider : ItemContainersControl
    {
        #region parts

        private RepeatButton PreviousButton;
        private RepeatButton NextButton;
        private Storyboard ToPreviousStoryboard;
        private Storyboard ToNextStoryboard;
        private DoubleAnimation ToPreviousAnimation;
        private DoubleAnimation ToNextAnimation;
        private Canvas View;
        private ItemsPresenter ItemsPresenter;

        #endregion

        #region private members

        private double _areaWidth;
        private Point _lastMousePos;
        private bool _IsMouseDrag = false;
        private bool _IsAnimationInProgress = false;
        //private MouseDragManager _dragManager;

        #endregion


        public AnimatedSlider()
        {
            DefaultStyleKey = typeof( AnimatedSlider );

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PreviousButton = ( RepeatButton )GetTemplateChild( "LeftButton" );
            if ( PreviousButton != null )
            {
                PreviousButton.Click += new RoutedEventHandler( LeftButton_Click );
                PreviousButton.Style = LeftButtonStyle;

            }
            NextButton = ( RepeatButton )GetTemplateChild( "RightButton" );
            if ( NextButton != null )
            {
                NextButton.Click += new RoutedEventHandler( RightButton_Click );
                NextButton.Style = RightButtonStyle;

            }
            ToPreviousStoryboard = ( Storyboard )GetTemplateChild( "ToLeftStoryboard" );
            ToPreviousStoryboard.Completed += new EventHandler( Storyboard_Completed );
            ToNextStoryboard = ( Storyboard )GetTemplateChild( "ToRightStoryboard" );
            ToNextStoryboard.Completed += new EventHandler( Storyboard_Completed );

            ToPreviousAnimation = ( DoubleAnimation )GetTemplateChild( "ToLeftAnimation" );
            ToPreviousAnimation.By = Height / ItemHeight * ItemWidth;
            PreviousButton.Interval = ToPreviousAnimation.Duration.TimeSpan.Milliseconds;
            ToNextAnimation = ( DoubleAnimation )GetTemplateChild( "ToRightAnimation" );
            ToNextAnimation.By = -Height / ItemHeight * ItemWidth;
            NextButton.Interval = ToNextAnimation.Duration.TimeSpan.Milliseconds;
            View = ( Canvas )GetTemplateChild( "View" );
            if ( View != null && IsMouseDraggingEnabled == true )
            {
                View.MouseLeftButtonDown += new MouseButtonEventHandler( Canvas_MouseLeftButtonDown );
                View.MouseLeftButtonUp += new MouseButtonEventHandler( Canvas_MouseLeftButtonUp );
                View.MouseMove += new MouseEventHandler( Canvas_MouseMove );
            }
            ItemsPresenter = ( ItemsPresenter )GetTemplateChild( "ItemsPresenter" );
            this.LayoutUpdated += new EventHandler( AnimatedSlider_LayoutUpdated );
        }

        public void SetWidthHeight(double width, double height, double itemWidth, double itemHeight)
        {
            Width = width;
            Height = height;
            ItemWidth = itemWidth;
            ItemHeight = itemHeight;
            if (ToPreviousAnimation != null)
            {
                ToPreviousAnimation.By = Height / ItemHeight * ItemWidth;
                ToNextAnimation.By = -Height / ItemHeight * ItemWidth;
            }
        }

        void Storyboard_Completed( object sender, EventArgs e )
        {
            _IsAnimationInProgress = false;
        }

        void AnimatedSlider_LayoutUpdated( object sender, EventArgs e )
        {
            this.LayoutUpdated -= new EventHandler( AnimatedSlider_LayoutUpdated );
            _areaWidth = this.ActualWidth - PreviousButton.ActualWidth - NextButton.ActualWidth;
            RectangleGeometry visibleArea = new RectangleGeometry();
            Rect clip = new Rect( 0, 0, _areaWidth, this.ActualHeight );
            visibleArea.Rect = clip;
            View.Clip = visibleArea;
        }


        #region event handlers

        private void LeftButton_Click( object sender, RoutedEventArgs e )
        {

            // skip to the end if the animation goes out of boundary
            if ( ( double )ItemsPresenter.GetValue( Canvas.LeftProperty ) + Height / ItemHeight * ItemWidth / 4 >= 0 )
            {
                ItemsPresenter.SetValue( Canvas.LeftProperty, ( double )0 );
            }
            else if ( ToPreviousStoryboard != null )
            {
                if ( _IsAnimationInProgress == false )
                {
                    ToPreviousStoryboard.Begin();
                    _IsAnimationInProgress = true;
                }
            }
            else // Strange bug calls recursively LeftButton_Click when Canvas.LeftProperty is set to positive
                ItemsPresenter.SetValue( Canvas.LeftProperty, ( double )ItemsPresenter.GetValue( Canvas.LeftProperty ) + ItemWidth );

        }
        // && IsMouseDraggingEnabled
        void RightButton_Click( object sender, RoutedEventArgs e )
        {
            double dist = ( double )ItemsPresenter.GetValue( Canvas.LeftProperty ) - _areaWidth - NextButton.ActualWidth;
            // skip to the end if the animation goes out of boundary
            if ( dist + 20 <= -( ItemsPresenter.ActualWidth ) )
            {
                ItemsPresenter.SetValue( Canvas.LeftProperty, _areaWidth - ( ItemsPresenter.ActualWidth ) );
            }
            else if ( ToNextStoryboard != null )
            {
                if ( _IsAnimationInProgress == false )
                {
                    ToNextStoryboard.Begin();
                    _IsAnimationInProgress = true;
                }
            }
            else // Strange bug calls recursively RightButton_Click when Canvas.LeftProperty is set to positive
                ItemsPresenter.SetValue( Canvas.LeftProperty, ( double )ItemsPresenter.GetValue( Canvas.LeftProperty ) - ItemWidth );
        }

        private void Canvas_MouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {
            _IsMouseDrag = true;
            _lastMousePos = e.GetPosition( View );
            FrameworkElement c = sender as FrameworkElement;
            c.CaptureMouse();
        }

        private void Canvas_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            //double itemS = ( Height / ItemHeight * ItemWidth );
            //int dist = -(int)((double)ItemsPresenter.GetValue( Canvas.LeftProperty ) - itemS / 2) ;
            //int itemN = ( int )( dist / itemS );
            //ItemsPresenter.SetValue( Canvas.LeftProperty, - itemN * itemS );
            _IsMouseDrag = false;
            FrameworkElement c = sender as FrameworkElement;
            c.ReleaseMouseCapture();
        }


        private void Canvas_MouseMove( object sender, MouseEventArgs e )
        {
            if ( _IsMouseDrag )
            {
                Point toViewCoordinates = e.GetPosition( View );
                double delta = toViewCoordinates.X - _lastMousePos.X;
                double newLeft = ( double )ItemsPresenter.GetValue( Canvas.LeftProperty ) + delta;
                if ( newLeft <= 0 && newLeft >= -( ItemsPresenter.ActualWidth - _areaWidth ) )
                    ItemsPresenter.SetValue( Canvas.LeftProperty, newLeft );
                _lastMousePos = toViewCoordinates;
            }
        }

        #endregion

        #region  LeftButtonStyle

        public static readonly DependencyProperty LeftButtonStyleProperty =
                              DependencyProperty.Register( "LeftButtonStyleProperty",
                                                           typeof( Style ),
                                                           typeof( AnimatedSlider ),
                                                           new PropertyMetadata( AnimatedSlider.OnLeftButtonStyleChanged ) );

        public Style LeftButtonStyle
        {
            get
            {
                return ( Style )this.GetValue( LeftButtonStyleProperty );
            }
            set
            {
                base.SetValue( AnimatedSlider.LeftButtonStyleProperty, value );
            }
        }

        protected static void OnLeftButtonStyleChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as AnimatedSlider ).OnLeftButtonStyleChanged( ( Style )e.OldValue, ( Style )e.NewValue );
        }

        internal virtual void OnLeftButtonStyleChanged( Style oldValue, Style newValue )
        {
            if ( PreviousButton != null )
            {
                PreviousButton.Style = newValue;
            }
        }
        #endregion

        #region  RightButtonStyle

        public static readonly DependencyProperty RightButtonStyleProperty =
                              DependencyProperty.Register( "RightButtonStyleProperty",
                                                           typeof( Style ),
                                                           typeof( AnimatedSlider ),
                                                           new PropertyMetadata( AnimatedSlider.OnRightButtonStyleChanged ) );

        public Style RightButtonStyle
        {
            get
            {
                return ( Style )this.GetValue( RightButtonStyleProperty );
            }
            set
            {
                base.SetValue( AnimatedSlider.RightButtonStyleProperty, value );
            }
        }

        protected static void OnRightButtonStyleChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as AnimatedSlider ).OnRightButtonStyleChanged( ( Style )e.OldValue, ( Style )e.NewValue );
        }

        internal virtual void OnRightButtonStyleChanged( Style oldValue, Style newValue )
        {
            if ( NextButton != null )
            {
                NextButton.Style = newValue;
            }
        }

        #endregion

        #region  IsMouseDraggingEnabled

        public static readonly DependencyProperty IsMouseDraggingEnabledProperty =
                              DependencyProperty.Register( "IsMouseDraggingEnabledProperty",
                                                           typeof( bool ),
                                                           typeof( AnimatedSlider ),
                                                           new PropertyMetadata( AnimatedSlider.OnIsMouseDraggingEnabledChanged ) );

        public bool IsMouseDraggingEnabled
        {
            get
            {
                return ( bool )this.GetValue( IsMouseDraggingEnabledProperty );
            }
            set
            {
                base.SetValue( AnimatedSlider.IsMouseDraggingEnabledProperty, value );
            }
        }

        protected static void OnIsMouseDraggingEnabledChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( d as AnimatedSlider ).OnIsMouseDraggingEnabledChanged( ( bool )e.OldValue, ( bool )e.NewValue );
        }

        internal virtual void OnIsMouseDraggingEnabledChanged( bool oldValue, bool newValue )
        {
            if ( View != null && newValue == true )
            {
                View.MouseLeftButtonDown += new MouseButtonEventHandler( Canvas_MouseLeftButtonDown );
                View.MouseLeftButtonUp += new MouseButtonEventHandler( Canvas_MouseLeftButtonUp );
                View.MouseMove += new MouseEventHandler( Canvas_MouseMove );
            }
            if ( View != null && newValue == false )
            {
                View.MouseLeftButtonDown -= new MouseButtonEventHandler( Canvas_MouseLeftButtonDown );
                View.MouseLeftButtonUp -= new MouseButtonEventHandler( Canvas_MouseLeftButtonUp );
                View.MouseMove -= new MouseEventHandler( Canvas_MouseMove );
            }
        }

        #endregion
    }
}
