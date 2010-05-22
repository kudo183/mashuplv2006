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
    public class Ripple : BasicEffect
    {
        int _speed;
        double _progressFrom;
        double _progressTo;

        public double ProgressTo
        {
            get { return _progressTo; }
            set 
            { 
                _progressTo = value;
                ((EasingDoubleKeyFrame)((DoubleAnimationUsingKeyFrames)sbEnter.Children[0]).KeyFrames[1]).Value = _progressTo;
            }
        }
        public double ProgressFrom
        {
            get { return _progressFrom; }
            set 
            { 
                _progressFrom = value;
                ((EasingDoubleKeyFrame)((DoubleAnimationUsingKeyFrames)sbEnter.Children[0]).KeyFrames[0]).Value = _progressFrom;
            }
        }
        
        public int Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                ((EasingDoubleKeyFrame)((DoubleAnimationUsingKeyFrames)sbEnter.Children[0]).KeyFrames[1]).KeyTime = TimeSpan.FromMilliseconds(_speed);                
            }
        }
        
        
        public override void Start()
        {
            sbEnter.Begin();
        }

        public override void Stop()
        {
            
        }

        public override void DetachEffect()
        {
            control.Effect = null;
            IsSelfHandle = false;
        }

        protected override void SetSelfHandle()
        {
            if (_isSelfHandle == true)
            {
                control.MouseLeftButtonDown += new MouseButtonEventHandler(control_MouseLeftButtonDown);
            }
            else
            {
                control.MouseLeftButtonDown -= new MouseButtonEventHandler(control_MouseLeftButtonDown);
            }
        }
        Storyboard sbEnter;
       
        public Ripple(EffectableControl control)
            : base(control)
        {
            parameterNameList.Add("Speed");
            parameterNameList.Add("ProgressFrom");
            parameterNameList.Add("ProgressTo");
            
            sbEnter = new Storyboard();
            
            CustomPixelShader.RippleShader rs = control.Effect as CustomPixelShader.RippleShader;
            if (rs == null)
            {
                rs = new CustomPixelShader.RippleShader();
                control.Effect = rs;
            }
            _speed = 500;
            _progressFrom = 0;
            _progressTo = 1;
            sbEnter.Children.Add(CreateDoubleAnimationUsingKeyFrames(rs, "Progress", _progressFrom, _progressTo, _speed));                                         
        }

        void control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            sbEnter.Begin();
        }

        private static DoubleAnimationUsingKeyFrames CreateDoubleAnimationUsingKeyFrames(
            DependencyObject element,
            string property,
            double from,
            double to,
            double milisecond)
        {
            DoubleAnimationUsingKeyFrames dakf = new DoubleAnimationUsingKeyFrames();
            EasingDoubleKeyFrame edkf;

            edkf = new EasingDoubleKeyFrame();
            edkf.Value = from;
            edkf.KeyTime = TimeSpan.FromMilliseconds(0);

            dakf.KeyFrames.Add(edkf);

            edkf = new EasingDoubleKeyFrame();
            edkf.Value = to;
            edkf.KeyTime = TimeSpan.FromMilliseconds(milisecond);

            dakf.KeyFrames.Add(edkf);

            Storyboard.SetTargetProperty(dakf, new PropertyPath(property));
            Storyboard.SetTarget(dakf, element);

            return dakf;
        }
    }
}
