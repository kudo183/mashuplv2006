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
using BasicLibrary;

namespace EffectLibrary
{
    public class Zoom : BasicEffect
    {
        int _speed;
        double _scaleFactorX;
        double _scaleFactorY;
        
        public int Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                ((EasingDoubleKeyFrame)((DoubleAnimationUsingKeyFrames)sbEnter.Children[0]).KeyFrames[0]).KeyTime = TimeSpan.FromMilliseconds(_speed);
                ((EasingDoubleKeyFrame)((DoubleAnimationUsingKeyFrames)sbEnter.Children[1]).KeyFrames[0]).KeyTime = TimeSpan.FromMilliseconds(_speed);
                ((EasingDoubleKeyFrame)((DoubleAnimationUsingKeyFrames)sbLeave.Children[0]).KeyFrames[0]).KeyTime = TimeSpan.FromMilliseconds(_speed);
                ((EasingDoubleKeyFrame)((DoubleAnimationUsingKeyFrames)sbLeave.Children[1]).KeyFrames[0]).KeyTime = TimeSpan.FromMilliseconds(_speed);
            }
        }
        public double ScaleFactorX
        {
            get { return _scaleFactorX; }
            set 
            { 
                _scaleFactorX = value;
                ((EasingDoubleKeyFrame)((DoubleAnimationUsingKeyFrames)sbEnter.Children[0]).KeyFrames[0]).Value = _scaleFactorX;
            }
        }
        public double ScaleFactorY
        {
            get { return _scaleFactorY; }
            set
            {
                _scaleFactorY = value;
                ((EasingDoubleKeyFrame)((DoubleAnimationUsingKeyFrames)sbEnter.Children[1]).KeyFrames[0]).Value = _scaleFactorX;
            }
        }
        public override void Start()
        {
            sbEnter.Begin();
        }

        public override void Stop()
        {
            sbLeave.Begin();
        }

        public override void DetachEffect()
        {
            IsSelfHandle = false;
            control.RenderTransform = null;
        }

        protected override void SetSelfHandle()
        {
            if (_isSelfHandle == true)
            {
                control.MouseEnter += new MouseEventHandler(control_MouseEnter);
                control.MouseLeave += new MouseEventHandler(control_MouseLeave);
                control.SizeChanged += new SizeChangedEventHandler(control_SizeChanged);
            }
            else
            {
                control.MouseEnter -= new MouseEventHandler(control_MouseEnter);
                control.MouseLeave -= new MouseEventHandler(control_MouseLeave);
                control.SizeChanged -= new SizeChangedEventHandler(control_SizeChanged);
            }
        }
        Storyboard sbEnter, sbLeave;
        ScaleTransform tt;

        public Zoom(EffectableControl control)
            : base(control)
        {
            parameterNameList.Add("Speed");
            parameterNameList.Add("ScaleFactorX");
            parameterNameList.Add("ScaleFactorY");
            sbEnter = new Storyboard();
            sbLeave = new Storyboard();
            tt = control.RenderTransform as ScaleTransform;
            if (tt == null)
            {
                tt = new ScaleTransform();
                control.RenderTransform = tt;
            }
            _speed = 500;
            _scaleFactorX = _scaleFactorY = 1.5;
            sbEnter.Children.Add(CreateDoubleAnimationUsingKeyFrames(tt, "ScaleX", _scaleFactorX, _speed));
            sbEnter.Children.Add(CreateDoubleAnimationUsingKeyFrames(tt, "ScaleY", _scaleFactorY, _speed));

            sbLeave.Children.Add(CreateDoubleAnimationUsingKeyFrames(tt, "ScaleX", 1, _speed));
            sbLeave.Children.Add(CreateDoubleAnimationUsingKeyFrames(tt, "ScaleY", 1, _speed));
        }

        void control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            tt.CenterX = control.ActualWidth / 2;
            tt.CenterY = control.ActualHeight / 2;
        }

        void control_MouseLeave(object sender, MouseEventArgs e)
        {
            sbLeave.Begin();
        }

        void control_MouseEnter(object sender, MouseEventArgs e)
        {
            sbEnter.Begin();
        }

        private static DoubleAnimationUsingKeyFrames CreateDoubleAnimationUsingKeyFrames(
            DependencyObject element,
            string property,
            double value,
            double milisecond)
        {
            DoubleAnimationUsingKeyFrames dakf = new DoubleAnimationUsingKeyFrames();
            EasingDoubleKeyFrame edkf;

            edkf = new EasingDoubleKeyFrame();
            edkf.Value = value;
            edkf.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut };
            edkf.KeyTime = TimeSpan.FromMilliseconds(milisecond);

            dakf.KeyFrames.Add(edkf);

            Storyboard.SetTargetProperty(dakf, new PropertyPath(property));
            Storyboard.SetTarget(dakf, element);

            return dakf;
        }
    }
}
