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
using System.Windows.Interactivity;

namespace Effect
{
    public class MagnifierOverEffect
    {
        private static Dictionary<UIElement, MagnifierOverBehavior> behaviors = new Dictionary<UIElement, MagnifierOverBehavior>();

        public static void AttachEffect(UIElement element, double magnification)
        {
            if (behaviors.ContainsKey(element))
            {
                MagnifierOverBehavior behavior = behaviors[element];
                behavior.ChangeMagnification(magnification);
            }

            else
            {
                MagnifierOverBehavior behavior = new MagnifierOverBehavior();
                behavior.ChangeMagnification(magnification);
                Interaction.GetBehaviors(element).Add(behavior);
                behaviors.Add(element, behavior);
            }
        }

        public static bool DetachEffect(UIElement element)
        {
            if (!behaviors.ContainsKey(element))
                return false;
            Interaction.GetBehaviors(element).Remove(behaviors[element]);
            behaviors.Remove(element);
            return true;
        }

        public static bool ChangeMagnification(UIElement element, double magnification)
        {
            if (behaviors.ContainsKey(element))
            {
                MagnifierOverBehavior behavior = behaviors[element];
                behavior.ChangeMagnification(magnification);
                return true;
            }
            return false;
        }
    }
}
