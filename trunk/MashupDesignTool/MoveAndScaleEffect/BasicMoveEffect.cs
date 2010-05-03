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

namespace MoveAndScaleEffect
{
    public class BasicMoveEffect
    {
        public delegate void EffectCompleteHandler(object sender, UIElement element);
        public event EffectCompleteHandler EffectComplete;
        
        public enum BasicMoveEffectSpeed
        {
            SLOW,
            NORMAL,
            FAST
        }

        private Storyboard sb;
        UIElement element;

        public BasicMoveEffect(UIElement element, Point begin, Point end, BasicMoveEffectSpeed speed)
        {
            this.element = element;
            sb = new Storyboard();
            Storyboard.SetTarget(sb, element);

            DoubleAnimationUsingKeyFrames animationKeyFrames3 = Utility.CreateDoubleAnimationUsingKeyFrames(@"(Canvas.Left)");
            LinearDoubleKeyFrame PositionLeft = Utility.CreateLinearDoubleKeyFrame(end.X, KeyTime.FromTimeSpan(CalculateTimeSpan(begin, end, speed)));
            animationKeyFrames3.KeyFrames.Add(PositionLeft);
            sb.Children.Add(animationKeyFrames3);

            DoubleAnimationUsingKeyFrames animationKeyFrames4 = Utility.CreateDoubleAnimationUsingKeyFrames(@"(Canvas.Top)");
            LinearDoubleKeyFrame PositionTop = Utility.CreateLinearDoubleKeyFrame(end.Y, KeyTime.FromTimeSpan(CalculateTimeSpan(begin, end, speed)));
            animationKeyFrames4.KeyFrames.Add(PositionTop);
            sb.Children.Add(animationKeyFrames4);

            sb.Completed += new EventHandler(sb_Completed);
        }

        void sb_Completed(object sender, EventArgs e)
        {
            if (EffectComplete != null)
                EffectComplete(sender, element);
        }

        public void Start()
        {
            sb.Begin();
        }

        public void Stop()
        {
            sb.Stop();
        }

        private TimeSpan CalculateTimeSpan(Point begin, Point end, BasicMoveEffectSpeed speed)
        {
            Point disPoint = new Point(Math.Abs(begin.X - end.X), Math.Abs(begin.Y - end.Y));
            double distance = disPoint.X < disPoint.Y ? disPoint.Y : disPoint.X;

            if (speed == BasicMoveEffectSpeed.SLOW)
                return new TimeSpan(0, 0, 0, 1, 100);
            else if (speed == BasicMoveEffectSpeed.NORMAL)
                return new TimeSpan(0, 0, 0, 0, 700);
            else
                return new TimeSpan(0, 0, 0, 0, 300);
        }
    }
}
