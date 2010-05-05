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

namespace Effect
{
    public class BasicMoveEffect
    {
        private static Dictionary<UIElement, Storyboard> storyboard = new Dictionary<UIElement, Storyboard>();

        public enum BasicMoveEffectSpeed
        {
            SLOW,
            NORMAL,
            FAST
        }

        public static Storyboard GetStoryboard(UIElement element)
        {
            if (!storyboard.ContainsKey(element))
                return null;
            return storyboard[element];
        }

        public static void AttachEffect(UIElement element, Point begin, Point end, BasicMoveEffectSpeed speed)
        {
            Storyboard sb;
            if (storyboard.ContainsKey(element))
            {
                sb = storyboard[element];
                sb.Children.Clear();
            }
            else
            {
                sb = new Storyboard();
                Storyboard.SetTarget(sb, element);
                storyboard.Add(element, sb);
            }

            DoubleAnimationUsingKeyFrames animationKeyFrames1 = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(animationKeyFrames1, new PropertyPath(@"(Canvas.Left)"));
            LinearDoubleKeyFrame PositionLeft = new LinearDoubleKeyFrame();
            PositionLeft.KeyTime = KeyTime.FromTimeSpan(CalculateTimeSpan(begin, end, speed));
            PositionLeft.Value = end.X;
            animationKeyFrames1.KeyFrames.Add(PositionLeft);
            sb.Children.Add(animationKeyFrames1);

            DoubleAnimationUsingKeyFrames animationKeyFrames2 = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(animationKeyFrames2, new PropertyPath(@"(Canvas.Top)"));
            LinearDoubleKeyFrame PositionTop = new LinearDoubleKeyFrame();
            PositionTop.KeyTime = KeyTime.FromTimeSpan(CalculateTimeSpan(begin, end, speed));
            PositionTop.Value = end.Y;
            animationKeyFrames2.KeyFrames.Add(PositionTop);
            sb.Children.Add(animationKeyFrames2);
        }

        public static bool DetachEffect(UIElement element)
        {
            if (!storyboard.ContainsKey(element))
                return false;
            storyboard.Remove(element);
            return true;
        }

        private static TimeSpan CalculateTimeSpan(Point begin, Point end, BasicMoveEffectSpeed speed)
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

        public static bool Start(UIElement element)
        {
            if (!storyboard.ContainsKey(element))
                return false;
            storyboard[element].Begin();
            return true;
        }

        public static bool Stop(UIElement element)
        {
            if (!storyboard.ContainsKey(element))
                return false;
            storyboard[element].Stop();
            return true;
        }
    }
}
