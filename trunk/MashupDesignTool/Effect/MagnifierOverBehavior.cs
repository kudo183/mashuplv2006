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
using System.Windows.Interactivity;

namespace Effect
{
    internal class MagnifierOverBehavior : Behavior<FrameworkElement>
    {
        private Magnifier magnifier;
        private double originalMagnification;

        public MagnifierOverBehavior() :
            base()
        {
            this.magnifier = new Magnifier();
            magnifier.InnerRadius = magnifier.InnerRadius + 90;
            magnifier.OuterRadius = magnifier.OuterRadius + 90;
        }

        public void ChangeMagnification(double magnification)
        {
            magnifier.Magnification = magnification;
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.MouseEnter += new MouseEventHandler( AssociatedObject_MouseEnter );
            this.AssociatedObject.MouseLeave += new MouseEventHandler( AssociatedObject_MouseLeave );
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.MouseEnter -= new MouseEventHandler( AssociatedObject_MouseEnter );
            this.AssociatedObject.MouseLeave -= new MouseEventHandler( AssociatedObject_MouseLeave );
        }

        private void AssociatedObject_MouseLeave( object sender, MouseEventArgs e )
        {
            this.AssociatedObject.MouseMove -= new MouseEventHandler( AssociatedObject_MouseMove );
            this.AssociatedObject.Effect = null;
        }

        private void AssociatedObject_MouseEnter( object sender, MouseEventArgs e )
        {
            this.AssociatedObject.MouseMove += new MouseEventHandler( AssociatedObject_MouseMove );
            this.AssociatedObject.Effect = this.magnifier;
        }

        private void AssociatedObject_MouseMove( object sender, MouseEventArgs e )
        {
            ( this.AssociatedObject.Effect as Magnifier ).Center =
                e.GetPosition( this.AssociatedObject );

            Point mousePosition = e.GetPosition( this.AssociatedObject );
            mousePosition.X /= this.AssociatedObject.ActualWidth;
            mousePosition.Y /= this.AssociatedObject.ActualHeight;
            this.magnifier.Center = mousePosition;

            Storyboard zoomInStoryboard = new Storyboard();
            DoubleAnimation zoomInAnimation = new DoubleAnimation();
            zoomInAnimation.To = this.magnifier.Magnification;
            zoomInAnimation.Duration = TimeSpan.FromSeconds( 0.5 );
            Storyboard.SetTarget( zoomInAnimation, this.AssociatedObject.Effect );
            Storyboard.SetTargetProperty( zoomInAnimation, new PropertyPath( Magnifier.MagnificationProperty ) );
            zoomInAnimation.FillBehavior = FillBehavior.HoldEnd;
            zoomInStoryboard.Children.Add( zoomInAnimation );
            zoomInStoryboard.Begin();
        }
    }
}
