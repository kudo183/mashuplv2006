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
    public class BasicControl : UserControl, IBasic
    {
        #region IBasic Members
        public virtual string GetParameterNames()
        {
            return "";
        }

        public virtual Type GetParameterType(string parameterName)
        {
            return null;
        }

        public virtual object GetParameterValue(string parameterName)
        {
            return null;
        }

        public virtual bool SetParameterValue(string parameterName, object value)
        {
            return false;
        }
        #endregion

        protected BasicEffect mainEffect;
        public BasicEffect MainEffect
        {
            get { return mainEffect; }
            set { ChangeMainEffect(value); }
        }

        public void ChangeMainEffect(BasicEffect be)
        {
            if (mainEffect != null)
                mainEffect.DetachEffect();
            mainEffect = be;
        }

        public BasicControl()
            : base()
        {
        }
    }   
}
