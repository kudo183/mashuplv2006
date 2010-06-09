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
    public class ChangeFillColor : BasicEffect
    {
        #region attributes
        private Storyboard sb1, sb2;
        private TimeSpan duration = new TimeSpan();
        Brush oldBackground;
        Color fillColor = Colors.Gray;
        #endregion attributes

        #region properties
        public Color FillColor
        {
            get { return fillColor; }
            set
            {
                fillColor = value;
                InitStoryboard();
            }
        }

        public TimeSpan Duration
        {
            get { return duration; }
            set
            {
                duration = value;
                InitStoryboard();
            }
        }
        #endregion properties

        public ChangeFillColor(EffectableControl control)
            : base(control)
        {
            parameterNameList.Add("Duration");
            parameterNameList.Add("FillColor");

            duration = TimeSpan.FromMilliseconds(800);
            oldBackground = control.CanvasRoot.Background;
            InitStoryboard();

            control.MouseEnter += new MouseEventHandler(control_MouseEnter);
            control.MouseLeave += new MouseEventHandler(control_MouseLeave);
        }

        void control_MouseLeave(object sender, MouseEventArgs e)
        {
            sb2.Begin();
        }

        void control_MouseEnter(object sender, MouseEventArgs e)
        {
            sb1.Begin();
        }

        #region create storyboard
        private void InitStoryboard()
        {
            InitStoryboard1();
            InitStoryboard2();
        }

        private void InitStoryboard1()
        {
            sb1 = new Storyboard();
            ObjectAnimationUsingKeyFrames oaufk1 = new ObjectAnimationUsingKeyFrames() { BeginTime = TimeSpan.FromMilliseconds(0), Duration = duration };
            DiscreteObjectKeyFrame dokf1 = new DiscreteObjectKeyFrame()
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                Value = new SolidColorBrush(fillColor)
            };
            oaufk1.KeyFrames.Add(dokf1);
            Storyboard.SetTarget(oaufk1, control.CanvasRoot);
            Storyboard.SetTargetProperty(oaufk1, new PropertyPath("(Canvas.Background)"));
            sb1.Children.Add(oaufk1);
        }

        private void InitStoryboard2()
        {
            sb2 = new Storyboard();
            ObjectAnimationUsingKeyFrames oaufk1 = new ObjectAnimationUsingKeyFrames() { BeginTime = TimeSpan.FromMilliseconds(0), Duration = duration };
            DiscreteObjectKeyFrame dokf1 = new DiscreteObjectKeyFrame()
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                Value = oldBackground
            };
            oaufk1.KeyFrames.Add(dokf1);
            Storyboard.SetTarget(oaufk1, control.CanvasRoot);
            Storyboard.SetTargetProperty(oaufk1, new PropertyPath("(Canvas.Background)"));
            sb2.Children.Add(oaufk1);
        }
        #endregion create storyboard

        #region override methods
        public override void Start()
        {
        }

        public override void Stop()
        {
        }

        public override void DetachEffect()
        {
            Canvas.SetLeft(control.Control, 0);
            Canvas.SetTop(control.Control, 0);
            control.CanvasRoot.Background = oldBackground;
        }

        protected override void SetSelfHandle()
        {
        }
        #endregion override methods

    }
}
