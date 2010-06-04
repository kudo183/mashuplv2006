using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BasicLibrary;
using System.Reflection;

namespace BasicLibrary
{
    public partial class EffectableControl : UserControl
    {
        private FrameworkElement control;

        public EffectableControl()
        {
            InitializeComponent();
        }

        public EffectableControl(FrameworkElement control) : this()
        {
            control.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            control.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            LayoutRoot.Children.Add(control);
            control.SizeChanged += new SizeChangedEventHandler(control_SizeChanged);
            this.control = control;
            this.Width = control.Width;
            this.Height = control.Height;
            this.SizeChanged += new SizeChangedEventHandler(LayoutRoot_SizeChanged);

            if (typeof(BasicControl).IsAssignableFrom(control.GetType()))
                ((BasicControl)control).CallEffect += new BasicControl.CallEffectHandle(EffectableControl_CallEffect);
        }

        void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (control.Width != e.NewSize.Width || control.Height != e.NewSize.Height)
            {
                control.Width = e.NewSize.Width;
                control.Height = e.NewSize.Height;
                LayoutRoot.Clip = new RectangleGeometry() { Rect = new Rect(new Point(0, 0), e.NewSize) };
            }
        }

        void control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (LayoutRoot.Width != e.NewSize.Width || LayoutRoot.Height != e.NewSize.Height)
            {
                LayoutRoot.Width = e.NewSize.Width;
                LayoutRoot.Height = e.NewSize.Height;
                this.Width = LayoutRoot.Width;
                this.Height = LayoutRoot.Height;
            }
        }

        public FrameworkElement Control
        {
            get { return control; }
        }

        public Canvas CanvasRoot
        {
            get { return LayoutRoot; }
        }

        protected BasicEffect mainEffect;

        public BasicEffect MainEffect
        {
            get { return mainEffect; }
        }

        public void ChangeEffect(string propertyName, Type effectType)
        {
            if (propertyName == "MainEffect")
            {
                if (mainEffect != null)
                    mainEffect.DetachEffect();
                ConstructorInfo ci = effectType.GetConstructor(new Type[] { typeof(EffectableControl) });
                mainEffect = (BasicEffect)ci.Invoke(new object[] { this });
            }
            else
            {
                if (typeof(BasicControl).IsAssignableFrom(control.GetType()))
                {
                    ((BasicControl)control).ChangeEffect(propertyName, effectType, this);
                }
            }
        }

        void EffectableControl_CallEffect(object sender)
        {
            if (mainEffect != null)
                mainEffect.Start();
        }

        public List<string> GetListEffectPropertyName()
        {
            List<string> list = new List<string>();
            list.Add("MainEffect");

            if (typeof(BasicControl).IsAssignableFrom(control.GetType()))
            {
                List<string> temp = ((BasicControl)control).GetListEffectPropertyName();
                foreach (string str in temp)
                    list.Add(str);
            }
            return list;
        }

        public Type GetEffectType(string effectName)
        {
            if (effectName == "MainEffect")
            {
                if (mainEffect == null)
                    return typeof(BasicEffect);
                else
                    return mainEffect.GetType();
            }
            else if (typeof(BasicControl).IsAssignableFrom(control.GetType()))
            {
                return ((BasicControl)control).GetEffectType(effectName);
            }
            return null;
        }

        public IBasic GetEffect(string effectName)
        {
            if (effectName == "MainEffect")
            {
                return mainEffect;
            }
            else if (typeof(BasicControl).IsAssignableFrom(control.GetType()))
            {
                return ((BasicControl)control).GetEffect(effectName);
            }
            return null;
        }
    }
}
