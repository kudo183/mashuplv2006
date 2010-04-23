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

namespace AnimatedSliderControl
{

    public class MouseDragManager
    {
        private ObservableCollection<FrameworkElement> _items;
        private FrameworkElement _view;

        public int FrontZIndex = 10;
        public int OldZIndex;
        private Point _oldMousePos;
        private Point _delta;
        private bool _isMouseDrag = false;
        private bool _isMouseInView = false;

        public MouseDragManager()
        {
        }

        public void LockInBounds( FrameworkElement view )
        {
            _view = view;
        }

        public void SetCollection( ObservableCollection<FrameworkElement> items )
        {
            if ( _items != null )
                foreach ( UIElement element in _items )
                {
                    element.MouseLeftButtonDown -= new MouseButtonEventHandler( element_MouseLeftButtonDown );
                    element.MouseLeftButtonUp -= new MouseButtonEventHandler( element_MouseLeftButtonUp );
                    element.MouseMove -= new MouseEventHandler( element_MouseMove );
                }
            _items = items;
            foreach ( UIElement element in _items )
            {
                element.MouseLeftButtonDown += new MouseButtonEventHandler( element_MouseLeftButtonDown );
                element.MouseLeftButtonUp += new MouseButtonEventHandler( element_MouseLeftButtonUp );
                element.MouseMove += new MouseEventHandler( element_MouseMove );
            }
        }

        void element_MouseMove( object sender, MouseEventArgs e )
        {
            if ( _isMouseDrag )
            {
                FrameworkElement element = ( sender as FrameworkElement );
                GeneralTransform childTransform = element.TransformToVisual( ( element.Parent as FrameworkElement ) );
                Point elementCoords = childTransform.Transform( new Point( 0, 0 ) );

                GeneralTransform viewTransform = _view.TransformToVisual( ( element.Parent as FrameworkElement ) );
                Point viewCoords = viewTransform.Transform( new Point( 0, 0 ) );

                Point current = e.GetPosition( element.Parent as FrameworkElement );

                _delta.X = current.X - _oldMousePos.X;
                _delta.Y = current.Y - _oldMousePos.Y;


                TranslateTransform translation = element.RenderTransform as TranslateTransform;
                if ( translation == null )
                {
                    translation = new TranslateTransform();
                    element.RenderTransform = translation;
                }

                if ( elementCoords.X <= viewCoords.X - _delta.X )
                {
                    translation.X = viewCoords.X - _delta.X;
                }
                if ( elementCoords.X + _delta.X >= viewCoords.X + _view.Width - element.Width )
                {
                    translation.X = viewCoords.X + _view.Width - element.Width;
                }
                else
                {
                    translation.X += _delta.X;
                }

                if ( elementCoords.Y <= viewCoords.Y - _delta.Y )
                {
                    translation.Y = viewCoords.Y - _delta.Y;
                }
                if ( elementCoords.Y + _delta.Y >= viewCoords.Y + _view.Height - element.Height )
                {
                    translation.Y = viewCoords.Y + _view.Height - element.Height;
                }
                else
                {
                    translation.Y += _delta.Y;
                }

                _oldMousePos = e.GetPosition( element.Parent as FrameworkElement );
            }
        }

        void element_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            FrameworkElement element = ( sender as FrameworkElement );
            element.SetValue( Canvas.ZIndexProperty, OldZIndex );
            element.ReleaseMouseCapture();
            _isMouseDrag = false;

        }

        void element_MouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {
            _isMouseDrag = true;
            FrameworkElement element = ( sender as FrameworkElement );
            OldZIndex = ( int )element.GetValue( Canvas.ZIndexProperty );
            element.SetValue( Canvas.ZIndexProperty, FrontZIndex );
            _oldMousePos = e.GetPosition( element.Parent as FrameworkElement );
            element.CaptureMouse();

        }
    }
}
