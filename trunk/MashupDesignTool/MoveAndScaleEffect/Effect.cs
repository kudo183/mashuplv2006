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
using System.Windows.Threading;
using System.Collections.Generic;

namespace MoveAndScaleEffect
{
    public class Effect
    {
        public delegate void EffectCompleteHandler(object sender, UIElement element);
        public event EffectCompleteHandler EffectComplete;
        
        public enum MoveAndScaleEffectSpeed
        {
            SLOW,
            NORMAL,
            FAST
        }

        private Storyboard sb;
        UIElement element;

        public Effect(UIElement element, Point begin, Point end, Point scaleFrom, Point scaleTo, MoveAndScaleEffectSpeed speed)
        {
            this.element = element;
            sb = new Storyboard();
            Storyboard.SetTarget(sb, element);

            string scaletranformspath = "";

            if (element.RenderTransform == null)
            {
                element.RenderTransform = new ScaleTransform();
            }
            else if (element.RenderTransform is TransformGroup)
            {
                TransformGroup tg = (TransformGroup)element.RenderTransform;
                Transform transform = null;
                int i = 0;
                for (; i < tg.Children.Count; i++)
                {
                    Transform t = tg.Children[i];
                    if (t is ScaleTransform)
                    {
                        transform = (ScaleTransform)t;
                        break;
                    }
                }
                if (transform == null)
                {
                    transform = new ScaleTransform();
                    tg.Children.Add(transform);
                }
                scaletranformspath = string.Format(".(TransformGroup.Children)[{0}]", i);
            }
            else
            {
                TransformGroup tg = new TransformGroup();
                Transform t = element.RenderTransform;
                tg.Children.Add(t);
                tg.Children.Add(new ScaleTransform());
                element.RenderTransform = tg;
                scaletranformspath = string.Format(".(TransformGroup.Children)[{0}]", tg.Children.Count - 1);
            }

            DoubleAnimationUsingKeyFrames animationKeyFrames1 = Utility.CreateDoubleAnimationUsingKeyFrames(@"(UIElement.RenderTransform)" + scaletranformspath + @".(ScaleTransform.ScaleX)");
            LinearDoubleKeyFrame ScaleXFrom = Utility.CreateLinearDoubleKeyFrame(scaleFrom.X, KeyTime.FromTimeSpan(new TimeSpan()));
            animationKeyFrames1.KeyFrames.Add(ScaleXFrom);
            LinearDoubleKeyFrame ScaleXTo = Utility.CreateLinearDoubleKeyFrame(scaleTo.X, KeyTime.FromTimeSpan(CalculateTimeSpan(begin, end, speed)));
            animationKeyFrames1.KeyFrames.Add(ScaleXTo);
            sb.Children.Add(animationKeyFrames1);

            DoubleAnimationUsingKeyFrames animationKeyFrames2 = Utility.CreateDoubleAnimationUsingKeyFrames(@"(UIElement.RenderTransform)" + scaletranformspath + @".(ScaleTransform.ScaleY)");
            LinearDoubleKeyFrame ScaleYFrom = Utility.CreateLinearDoubleKeyFrame(scaleFrom.Y, KeyTime.FromTimeSpan(new TimeSpan()));
            animationKeyFrames2.KeyFrames.Add(ScaleYFrom);
            LinearDoubleKeyFrame ScaleYTo = Utility.CreateLinearDoubleKeyFrame(scaleTo.Y, KeyTime.FromTimeSpan(CalculateTimeSpan(begin, end, speed)));
            animationKeyFrames2.KeyFrames.Add(ScaleYTo);
            sb.Children.Add(animationKeyFrames2);

            DoubleAnimationUsingKeyFrames animationKeyFrames3 = Utility.CreateDoubleAnimationUsingKeyFrames(@"(Canvas.Left)");
            SplineDoubleKeyFrame PositionLeft = Utility.CreateSplineDoubleKeyFrame(end.X, KeyTime.FromTimeSpan(CalculateTimeSpan(begin, end, speed)), 0.14, 0.52, 0.15, 0.98);
            animationKeyFrames3.KeyFrames.Add(PositionLeft);
            sb.Children.Add(animationKeyFrames3);

            DoubleAnimationUsingKeyFrames animationKeyFrames4 = Utility.CreateDoubleAnimationUsingKeyFrames(@"(Canvas.Top)");
            SplineDoubleKeyFrame PositionTop = Utility.CreateSplineDoubleKeyFrame(end.Y, KeyTime.FromTimeSpan(CalculateTimeSpan(begin, end, speed)), 0.14, 0.52, 0.15, 0.98);
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

        void timer_Tick(object sender, EventArgs e)
        {
            //ScaleTransform transform = null;
            //if (element.RenderTransform == null)
            //{
            //    transform = new ScaleTransform();
            //    element.RenderTransform = transform;
            //}
            //else if (element.RenderTransform is TransformGroup)
            //{
            //    TransformGroup tg = (TransformGroup)element.RenderTransform;
            //    foreach (Transform t in tg.Children)
            //    {
            //        if (t is ScaleTransform)
            //        {
            //            transform = (ScaleTransform)t;
            //            break;
            //        }
            //    }
            //    if (transform == null)
            //    {
            //        transform = new ScaleTransform();
            //        tg.Children.Add(transform);
            //    }
            //}
            //else if (element.RenderTransform is ScaleTransform)
            //{
            //    transform = (ScaleTransform)element.RenderTransform;
            //}
            //else
            //{
            //    TransformGroup tg = new TransformGroup();
            //    Transform t = element.RenderTransform;
            //    tg.Children.Add(t);
            //    transform = new ScaleTransform();
            //    tg.Children.Add(transform);
            //    element.RenderTransform = tg;
            //}
            //transform.ScaleX += deltaScale.X;
            //transform.ScaleY += deltaScale.Y;
        }

        private TimeSpan CalculateTimeSpan(Point begin, Point end, MoveAndScaleEffectSpeed speed)
        {
            Point disPoint = new Point(Math.Abs(begin.X - end.X), Math.Abs(begin.Y - end.Y));
            double distance = disPoint.X < disPoint.Y ? disPoint.Y : disPoint.X;

            if (speed == MoveAndScaleEffectSpeed.SLOW)
                return new TimeSpan(0, 0, 0, 1, 100);
            else if (speed == MoveAndScaleEffectSpeed.NORMAL)
                return new TimeSpan(0, 0, 0, 0, 700);
            else
                return new TimeSpan(0, 0, 0, 0, 300);
        }
    }
}
