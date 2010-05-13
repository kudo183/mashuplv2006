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
    public class Jump : BasicEffect
    {
        public enum DIRECTION
        {
            UP,
            DOWN,
            LEFT,
            RIGHT
        }

        int _speed;
        int _bounces;
        DIRECTION _direction;
        double _distant;

        public int Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                ((EasingDoubleKeyFrame)((DoubleAnimationUsingKeyFrames)sbEnter.Children[0]).KeyFrames[0]).KeyTime = TimeSpan.FromMilliseconds(_speed / 5);
                ((EasingDoubleKeyFrame)((DoubleAnimationUsingKeyFrames)sbEnter.Children[0]).KeyFrames[1]).KeyTime = TimeSpan.FromMilliseconds(_speed);
            }
        }

        public int Bounces
        {
            get { return _bounces; }
            set
            {
                _bounces = value;
                EasingDoubleKeyFrame edkf = ((DoubleAnimationUsingKeyFrames)sbEnter.Children[0]).KeyFrames[1] as EasingDoubleKeyFrame;

                ((BounceEase)edkf.EasingFunction).Bounces = _bounces;
            }
        }

        public DIRECTION Direction
        {
            get { return _direction; }
            set
            {
                _direction = value;

                double temp = (_direction == DIRECTION.LEFT || _direction == DIRECTION.UP) ? -_distant : _distant;

                ((EasingDoubleKeyFrame)((DoubleAnimationUsingKeyFrames)sbEnter.Children[0]).KeyFrames[0]).Value = temp;

                DoubleAnimationUsingKeyFrames dakf = sbEnter.Children[0] as DoubleAnimationUsingKeyFrames;
                if (_direction == DIRECTION.UP || _direction == DIRECTION.DOWN)
                    Storyboard.SetTargetProperty(dakf, new PropertyPath("Y"));
                else if (_direction == DIRECTION.LEFT || _direction == DIRECTION.RIGHT)
                    Storyboard.SetTargetProperty(dakf, new PropertyPath("X"));
            }
        }

        public double Distant
        {
            get { return (_distant < 0) ? -_distant : _distant; }
            set
            {
                _distant = value;
                double temp = (_direction == DIRECTION.LEFT || _direction == DIRECTION.UP) ? -_distant : _distant;

                ((EasingDoubleKeyFrame)((DoubleAnimationUsingKeyFrames)sbEnter.Children[0]).KeyFrames[0]).Value = temp;
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
            IsSelfHande = false;
            control.RenderTransform = null;
        }

        protected override void SetSelfHandle()
        {
            if (_isSelfHande == true)
            {
                control.MouseLeftButtonDown += new MouseButtonEventHandler(control_MouseLeftButtonDown);
            }
            else
            {
                control.MouseLeftButtonDown -= new MouseButtonEventHandler(control_MouseLeftButtonDown);
            }
        }

        void control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            sbEnter.Begin();
        }
        Storyboard sbEnter;
        TranslateTransform tt;

        public Jump(EffectableControl control)
            : base(control)
        {
            parameterNameList.Add("Speed");
            parameterNameList.Add("Bounces");
            parameterNameList.Add("Direction");
            parameterNameList.Add("Distant");

            sbEnter = new Storyboard();

            tt = control.RenderTransform as TranslateTransform;
            if (tt == null)
            {
                tt = new TranslateTransform();
                control.RenderTransform = tt;
            }
           
            sbEnter = CreateStoryboard();           
        }        

        private Storyboard CreateStoryboard()
        {
            _distant = 50;
            _speed = 3000;
            _bounces = 5;
            _direction = DIRECTION.UP;

            Storyboard sb = new Storyboard();

            DoubleAnimationUsingKeyFrames dakf = new DoubleAnimationUsingKeyFrames();
            EasingDoubleKeyFrame edkf;

            edkf = new EasingDoubleKeyFrame();
            edkf.Value = -50;
            edkf.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut };
            edkf.KeyTime = TimeSpan.FromMilliseconds(3000 / 5);
            dakf.KeyFrames.Add(edkf);

            edkf = new EasingDoubleKeyFrame();

            edkf.EasingFunction = new BounceEase { Bounces = 5, EasingMode = EasingMode.EaseOut };
            edkf.Value = 0;
            edkf.KeyTime = TimeSpan.FromMilliseconds(3000);

            dakf.KeyFrames.Add(edkf);
            
            Storyboard.SetTargetProperty(dakf, new PropertyPath("Y"));            
            Storyboard.SetTarget(dakf, tt);

            sb.Children.Add(dakf);
            return sb;
        }
    }
}
