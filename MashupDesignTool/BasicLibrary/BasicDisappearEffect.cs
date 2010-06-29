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

namespace BasicLibrary
{
    public abstract class BasicDisappearEffect : BasicEffect
    {
        public BasicDisappearEffect(EffectableControl control)
            : base(control)
        {
        }
    }
}
