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
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace BasicLibrary
{
    public class BasicControl : UserControl, IBasic
    {
        #region IBasic Members
        protected List<string> parameterNameList = new List<string>();
        public string GetParameterNames()
        {
            if (!parameterNameList.Contains("Width"))
                parameterNameList.Add("Width");
            if (!parameterNameList.Contains("Height"))
                parameterNameList.Add("Height"); 
            
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

        public List<string> GetParameterNameList()
        {
            if (!parameterNameList.Contains("Width"))
                parameterNameList.Add("Width");
            if (!parameterNameList.Contains("Height"))
                parameterNameList.Add("Height");
            return parameterNameList;
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

        public delegate void CallEffectHandle(object sender);
        public event CallEffectHandle CallEffect;

        protected List<string> effectPropertyNameList = new List<string>();

        public virtual void ChangeEffect(string propertyName, Type effectType, EffectableControl owner)
        {
        }

        public List<string> GetListEffectPropertyName()
        {
            return effectPropertyNameList;
        }

        public Type GetEffectType(string effectName)
        {
            if (!effectPropertyNameList.Contains(effectName))
                return null;
            return this.GetType().GetProperty(effectName).PropertyType;
        }

        public IBasic GetEffect(string effectName)
        {
            if (!effectPropertyNameList.Contains(effectName))
                return null;
            return (IBasic)this.GetType().GetProperty(effectName).GetValue(this, null);
        }

        public BasicControl()
            : base()
        {
        }

        protected void StartMainEffect()
        {
            if (CallEffect != null)
                CallEffect(this);
        }

        public virtual void Dispose()
        {
        }
    }   
}
