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
    internal class Utility
    {
        public static DoubleAnimationUsingKeyFrames CreateDoubleAnimationUsingKeyFrames(string targetProperty)
        {
            DoubleAnimationUsingKeyFrames animationKeyFrames = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(animationKeyFrames, new PropertyPath(targetProperty));
            return animationKeyFrames;
        }

        public static LinearDoubleKeyFrame CreateLinearDoubleKeyFrame(double value, KeyTime keyTime)
        {
            LinearDoubleKeyFrame keyFrame = new LinearDoubleKeyFrame();
            keyFrame.KeyTime = keyTime;
            keyFrame.Value = value;
            return keyFrame;
        }

        public static SplineDoubleKeyFrame CreateSplineDoubleKeyFrame(double value, KeyTime keyTime, KeySpline keySpline)
        {
            SplineDoubleKeyFrame keyFrame = new SplineDoubleKeyFrame();
            keyFrame.KeyTime = keyTime;
            keyFrame.Value = value;
            keyFrame.KeySpline = keySpline;
            return keyFrame;
        }

        public static SplineDoubleKeyFrame CreateSplineDoubleKeyFrame(double value, KeyTime keyTime, double x1, double y1, double x2, double y2)
        {
            KeySpline keySpline = new KeySpline();
            keySpline.ControlPoint1 = new Point(x1, y1);
            keySpline.ControlPoint2 = new Point(x2, y2);
            return CreateSplineDoubleKeyFrame(value, keyTime, keySpline);
        }
    }
}
