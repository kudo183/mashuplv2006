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
using System.Text;
using System.Xml;

namespace BasicLibrary
{
    public abstract class BasicEffect : IBasic
    {
        #region IBasic Members
        public string GetParameterNames()
        {
            StringBuilder sb = new StringBuilder();
            XmlWriter xw = XmlWriter.Create(sb);
            xw.WriteStartDocument();
            xw.WriteStartElement("ParameterNames");

            foreach (string name in parameterNameList)
            {
                xw.WriteStartElement("ParameterName");
                xw.WriteValue(name);
                xw.WriteEndElement();
            }

            xw.WriteEndElement();
            xw.WriteEndDocument();
            xw.Flush();
            xw.Close();
            return sb.ToString();
        }

        public Type GetParameterType(string parameterName)
        {
            if (parameterNameList.Contains(parameterName))
                return this.GetType().GetProperty(parameterName).PropertyType;
            return null;
        }

        public object GetParameterValue(string parameterName)
        {
            if (parameterNameList.Contains(parameterName))
                return this.GetType().GetProperty(parameterName).GetValue(this, null);
            return null;
        }

        public bool SetParameterValue(string parameterName, object value)
        {
            if (parameterNameList.Contains(parameterName))
            {
                this.GetType().GetProperty(parameterName).SetValue(this, Convert.ChangeType(value, GetParameterType(parameterName), null), null); 
                return true;
            }
            return false;
        }
        #endregion

        public BasicEffect(EffectableControl control)
        {
            this.control = control;
            IsSelfHandle = true;
        }

        protected EffectableControl control;
        protected List<string> parameterNameList = new List<string>();
        protected bool _isSelfHandle;

        public bool IsSelfHandle
        {
            get { return _isSelfHandle; }
            set 
            {
                if (_isSelfHandle == value)
                    return;

                _isSelfHandle = value;
                SetSelfHandle();
            }
        }

        public abstract void Start();
        public abstract void Stop();
        public abstract void DetachEffect();
        protected abstract void SetSelfHandle();
    }
}
