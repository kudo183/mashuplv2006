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
using System.Text;
using System.Xml;
using BasicLibrary;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;

namespace BasicLibrary
{
    public class BasicControlSerializer
    {
        public static string Serialize(BasicControl control)
        {
            Type type = control.GetType();
            if (!typeof(BasicControl).IsAssignableFrom(type))
                return "";

            BasicControl basicControl = (BasicControl)control;
            StringBuilder sb = new StringBuilder();
            XmlWriter xm = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true });

            xm.WriteStartElement(type.Name);
            xm.WriteAttributeString("Type", type.AssemblyQualifiedName);
            List<string> propertyList = basicControl.GetParameterNameList();
            foreach (string propertyName in propertyList)
            {
                object value = basicControl.GetParameterValue(propertyName);
                if (value != null)
                    xm.WriteRaw(MyXmlSerializer.Serialize(value, propertyName));
            }

            xm.WriteEndElement();
            xm.Flush();
            xm.Close();
            return sb.ToString();
        }

        public static BasicControl Deserialize(string xml)
        {
            XmlReader reader = XmlReader.Create(new StringReader(xml));
            XDocument doc = XDocument.Load(reader);
            XElement root = doc.Root;

            return (BasicControl)MyXmlSerializer.Load(root);
        }
    }
}
