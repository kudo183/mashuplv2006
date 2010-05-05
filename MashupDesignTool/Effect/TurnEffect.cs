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
    public class TurnEffect
    {
        public enum TurnEffectOrientation
        {
            VERTICAL,
            HORIZONTAL
        }

        private static Dictionary<UIElement, Storyboard> storyboard = new Dictionary<UIElement, Storyboard>();

        public static Storyboard GetStoryboard(UIElement element)
        {
            if (!storyboard.ContainsKey(element))
                return null;
            return storyboard[element];
        }

        public static void AttachEffect(UIElement element, double from, double to, TimeSpan beginTime, TimeSpan duration, double centerOfRotation, TurnEffectOrientation orientation)
        {
            Storyboard sb;
            DoubleAnimation doubleAnimation;
            if (storyboard.ContainsKey(element))
            {
                sb = storyboard[element];
                doubleAnimation = (DoubleAnimation)sb.Children[0];
            }
            else
            {
                sb = new Storyboard();
                storyboard.Add(element, sb);
                doubleAnimation = new DoubleAnimation();
                sb.Children.Add(doubleAnimation);
                if (orientation == TurnEffectOrientation.VERTICAL)
                {
                    if (element.Projection is PlaneProjection == false)
                    {
                        element.Projection = new PlaneProjection() { CenterOfRotationY = centerOfRotation };
                    }
                    Storyboard.SetTarget(doubleAnimation, element.Projection);
                    Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("RotationX"));
                }
                else
                {
                    if (element.Projection is PlaneProjection == false)
                    {
                        element.Projection = new PlaneProjection() { CenterOfRotationX = centerOfRotation };
                    }
                    Storyboard.SetTarget(doubleAnimation, element.Projection);
                    Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("RotationY"));
                }
            }

            doubleAnimation.From = from;
            doubleAnimation.To = to;
            doubleAnimation.BeginTime = beginTime;
            doubleAnimation.Duration = duration;
        }

        public static void Update(UIElement element, double from, double to, TimeSpan beginTime, TimeSpan duration, double centerOfRotation, TurnEffectOrientation orientation)
        {
            AttachEffect(element, from, to, beginTime, duration, centerOfRotation, orientation);
        }

        public static bool DetachEffect(UIElement element)
        {
            if (!storyboard.ContainsKey(element))
                return false;
            storyboard.Remove(element);
            return true;
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
