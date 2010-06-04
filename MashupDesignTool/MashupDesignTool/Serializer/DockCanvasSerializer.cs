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
    public class DockCanvasSerializer
    {
        public static string Serialize(DesignCanvas designCanvas)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriter xm = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true });

            xm.WriteStartElement("DockCanvas");

            xm.WriteElementString("Width", designCanvas.ControlContainer.Width.ToString());
            xm.WriteElementString("Height", designCanvas.ControlContainer.Height.ToString());
            xm.WriteStartElement("Background");
            xm.WriteRaw(MyXmlSerializer.Serialize(designCanvas.ControlContainer.Background));
            xm.WriteEndElement();

            xm.WriteStartElement("Controls");
            foreach (EffectableControl ec in designCanvas.Controls)
            {
                xm.WriteRaw(EffectableObjectXmlSerializer.Serialize(ec));
            }
            xm.WriteEndElement();

            xm.WriteEndElement();
            xm.Flush();
            xm.Close();
            return sb.ToString();
        }

        public static void Load(string xml, DockCanvas.DockCanvas canvas)
        {
            XmlReader reader = XmlReader.Create(new StringReader(xml));
            XDocument doc = XDocument.Load(reader);
            XElement root = doc.Root;

            foreach (XElement element in root.Elements())
            {
                switch (element.Name.LocalName)
                {
                    case "Width":
                        canvas.Width = double.Parse(element.Value);
                        break;
                    case "Height":
                        canvas.Height = double.Parse(element.Value);
                        break;
                    case "Background":
                        canvas.Background = (Brush)MyXmlSerializer.Load(element.FirstNode.ToString());
                        break;
                    case "Controls":
                        foreach (XElement child in element.Elements())
                            canvas.Children.Add(EffectableObjectXmlSerializer.Load(child));
                        canvas.UpdateChildrenPosition();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
