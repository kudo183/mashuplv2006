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
using BasicLibrary;

namespace EffectLibrary
{
    public class Fade : BasicEffect
    {
        #region attributes
        private Storyboard sb;
        private TimeSpan duration = new TimeSpan();
        double width, height;
        Brush oldBackground;
        double oldOpacity;
        #endregion attributes

        #region properties
        #endregion properties

        public Fade(EffectableControl control)
            : base(control)
        {
            width = control.Width;
            height = control.Height;
            if (((double.IsNaN(width) && double.IsNaN(height)) || (width == 0 && height == 0)) && !double.IsNaN(control.ActualWidth) && !double.IsNaN(control.ActualHeight))
            {
                width = control.ActualWidth;
                height = control.ActualHeight;
            }

            duration = TimeSpan.FromMilliseconds(800);
            oldBackground = control.CanvasRoot.Background;
            oldOpacity = control.Control.Opacity;
            InitStoryboard();
        }

        private void InitStoryboard()
        {
            sb = new Storyboard();
            sb.Completed += new EventHandler(sb_Completed);

            DoubleAnimation doubleAnimation = new DoubleAnimation() { BeginTime = TimeSpan.FromSeconds(0), Duration = duration, From = 0, To = 1 };
            Storyboard.SetTarget(doubleAnimation, control.Control);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(UIElement.Opacity)"));

            sb.Children.Add(doubleAnimation);
        }

        void sb_Completed(object sender, EventArgs e)
        {
            control.CanvasRoot.Background = oldBackground;
        }

        #region override methods
        public override void Start()
        {
            control.CanvasRoot.Background = new SolidColorBrush(Colors.Black);
            sb.Begin();
        }

        public override void Stop()
        {
            sb.Stop();
        }

        public override void DetachEffect()
        {
            Canvas.SetLeft(control.Control, 0);
            Canvas.SetTop(control.Control, 0);
            control.Control.Opacity = oldOpacity;
            control.CanvasRoot.Background = oldBackground;
        }

        protected override void SetSelfHandle()
        {
        }
        #endregion override methods
    }
}
