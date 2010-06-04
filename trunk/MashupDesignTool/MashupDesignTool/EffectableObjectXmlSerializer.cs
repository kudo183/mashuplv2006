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
using BasicLibrary;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;

namespace MashupDesignTool
{
    public class EffectableObjectXmlSerializer
    {
        public static string Serialize(EffectableControl control)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings setting = new XmlWriterSettings() { OmitXmlDeclaration = true };
            XmlWriter xm = XmlWriter.Create(sb, setting);
            xm.WriteStartElement(control.GetType().Name);

            xm.WriteElementString("Top", DockCanvas.DockCanvas.GetTop(control).ToString());
            xm.WriteElementString("Left", DockCanvas.DockCanvas.GetLeft(control).ToString());
            xm.WriteElementString("ZIndex", DockCanvas.DockCanvas.GetZIndex(control).ToString());
            xm.WriteElementString("DockType", DockCanvas.DockCanvas.GetDockType(control).ToString());
            xm.WriteElementString("Width", control.Width.ToString());
            xm.WriteElementString("Height", control.Height.ToString());

            xm.WriteStartElement("Control");
            xm.WriteRaw(MyXmlSerializer.Serialize(control.Control));
            xm.WriteEndElement();

            xm.WriteStartElement("Effects");
            List<string> effectNameList = control.GetListEffectPropertyName();
            foreach (string effectName in effectNameList)
            {
                IBasic effect = control.GetEffect(effectName);
                if (effect != null)
                {
                    xm.WriteStartElement(effectName);
                    xm.WriteAttributeString("Type", effect.GetType().AssemblyQualifiedName);
                    xm.WriteRaw(MyXmlSerializer.Serialize(effect));
                    xm.WriteEndElement();
                }
            }
            xm.WriteEndElement();

            xm.WriteEndElement();
            xm.Flush();
            xm.Close();
            return sb.ToString();
        }

        public static EffectableControl Load(string xml)
        {
            XmlReader reader = XmlReader.Create(new StringReader(xml));
            XDocument doc = XDocument.Load(reader);
            XElement root = doc.Root;

            return Load(root);
        }

        public static EffectableControl Load(XElement root)
        {
            double top = 0, left = 0, width = 2, height = 2;
            int zindex = 1;
            DockCanvas.DockCanvas.DockType dockType = DockCanvas.DockCanvas.DockType.None;
            FrameworkElement fe = null;
            BasicEffect mainEffect = null;

            foreach (XElement element in root.Elements())
            {
                switch (element.Name.LocalName)
                {
                    case "Top":
                        top = double.Parse(element.Value);
                        break;
                    case "Left":
                        left = double.Parse(element.Value);
                        break;
                    case "ZIndex":
                        zindex = int.Parse(element.Value);
                        break;
                    case "DockType":
                        dockType = (DockCanvas.DockCanvas.DockType)Enum.Parse(typeof(DockCanvas.DockCanvas.DockType), element.Value, true);
                        break;
                    case "Width":
                        width = double.Parse(element.Value);
                        break;
                    case "Height":
                        height = double.Parse(element.Value);
                        break;
                    case "Control":
                        fe = (FrameworkElement)MyXmlSerializer.Load(element.FirstNode.ToString());
                        break;
                    default:
                        break;
                }
            }

            EffectableControl control = new EffectableControl(fe);
            foreach (XElement element in root.Elements())
            {
                if (element.Name == "Effects")
                {
                    foreach (XElement child in element.Elements())
                    {
                        string effectName = child.Name.LocalName;
                        Type effectType = Type.GetType(child.Attribute("Type").Value);
                        control.ChangeEffect(effectName, effectType);
                        MyXmlSerializer.Load(child.FirstNode.ToString(), control.GetEffect(effectName));
                    }
                }
            }

            control.Control.Width = width;
            control.Control.Height = height;
            DockCanvas.DockCanvas.SetLeft(control, left);
            DockCanvas.DockCanvas.SetTop(control, top);
            DockCanvas.DockCanvas.SetZIndex(control, zindex);
            DockCanvas.DockCanvas.SetDockType(control, dockType);

            return control;
        }
    }
}
