using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

using BasicLibrary;

namespace EffectLibrary.CustomPixelShader
{
    public class CustomPixelShaderBase : ShaderEffect, IBasic 
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

        public CustomPixelShaderBase():base()
        {
        }

        protected List<string> parameterNameList = new List<string>();
    }
}
