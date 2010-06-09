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
    public class BasicDataListItem : BasicControl
    {
        protected List<string> parameterCanBindingNameList = new List<string>();
        public string GetParameterCanBindingNames()
        {
            StringBuilder sb = new StringBuilder();
            XmlWriter xw = XmlWriter.Create(sb);
            xw.WriteStartDocument();
            xw.WriteStartElement("ParameterCanBindingNames");

            foreach (string name in parameterCanBindingNameList)
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

        public List<string> GetParameterCanBindingNameList()
        {
            return parameterCanBindingNameList;
        }

        public Type GetParameterCanBindingType(string parameterName)
        {
            if (parameterCanBindingNameList.Contains(parameterName))
                return this.GetType().GetProperty(parameterName).PropertyType;
            return null;
        }

        public object GetParameterCanBindingValue(string parameterName)
        {
            if (parameterCanBindingNameList.Contains(parameterName))
                return this.GetType().GetProperty(parameterName).GetValue(this, null);
            return null;
        }

        public bool SetParameterCanBindingValue(string parameterName, object value)
        {
            if (parameterCanBindingNameList.Contains(parameterName))
            {
                this.GetType().GetProperty(parameterName).SetValue(this, Convert.ChangeType(value, GetParameterCanBindingType(parameterName), null), null);
                return true;
            }
            return false;
        }
    }
}
