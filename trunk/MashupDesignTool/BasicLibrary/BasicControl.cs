﻿using System;
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
    public abstract class BasicControl : UserControl, IBasic
    {
        #region IBasic Members
        public abstract string GetParameterNames();
        public abstract Type GetParameterType(string parameterName);
        public abstract object GetParameterValue(string parameterName);
        public abstract bool SetParameterValue(string parameterName, object value);
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
    }   
}