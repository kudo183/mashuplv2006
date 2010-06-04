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

namespace MashupDesignTool
{
    public class ControlSerializer
    {
        public static string Serialize(FrameworkElement control)
        {
            if (Utility.IsFrameworkControl(control))
                return MyXmlSerializer.Serialize(control);

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

        public static FrameworkElement Load(string xml)
        {
            XmlReader reader = XmlReader.Create(new StringReader(xml));
            XDocument doc = XDocument.Load(reader);
            XElement root = doc.Root;

            return (FrameworkElement)MyXmlSerializer.Load(root);
        }
    }
}
