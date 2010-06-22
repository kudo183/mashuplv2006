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
            List<EffectableControl> controls = designCanvas.Controls;
            StringBuilder sb = new StringBuilder();
            XmlWriter xm = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true });

            xm.WriteStartElement("DockCanvas");

            xm.WriteElementString("Width", designCanvas.ControlContainer.Width.ToString());
            xm.WriteElementString("Height", designCanvas.ControlContainer.Height.ToString());
            xm.WriteStartElement("Background");
            xm.WriteRaw(MyXmlSerializer.Serialize(designCanvas.ControlContainer.Background));
            xm.WriteEndElement();

            xm.WriteStartElement("Controls");
            foreach (EffectableControl ec in controls)
            {
                xm.WriteRaw(EffectableObjectXmlSerializer.Serialize(ec));
            }
            xm.WriteEndElement();

            xm.WriteStartElement("Events");
            Dictionary<FrameworkElement, string> dictionary = new Dictionary<FrameworkElement, string>();
            for (int i = 0; i < controls.Count; i++)
                dictionary.Add(controls[i].Control, i.ToString());
            for (int i = 0; i < controls.Count; i++)
            {
                if (typeof(BasicControl).IsAssignableFrom(controls[i].Control.GetType()))
                {
                    List<MDTEventInfo> list = MDTEventManager.GetListEventInfoRaiseBy((BasicControl)controls[i].Control);
                    foreach (MDTEventInfo mei in list)
                    {
                        xm.WriteStartElement("Event");
                        xm.WriteAttributeString("RaiseControlIndex", i.ToString());
                        xm.WriteAttributeString("EventName", mei.EventName);
                        xm.WriteAttributeString("HandleControlIndex", dictionary[mei.HandleControl]);
                        xm.WriteAttributeString("HandleOperation", mei.HandleOperation);
                        xm.WriteEndElement();
                    }
                }
            }
            xm.WriteEndElement();

            xm.WriteEndElement();
            xm.Flush();
            xm.Close();
            return sb.ToString();
        }

        public static void Deserialize(string xml, DockCanvas.DockCanvas canvas)
        {
            XmlReader reader = XmlReader.Create(new StringReader(xml));
            XDocument doc = XDocument.Load(reader);
            XElement root = doc.Root;
            XElement eventsElement = null;

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
                        canvas.Background = (Brush)MyXmlSerializer.Deserialize(element.FirstNode.ToString());
                        break;
                    case "Controls":
                        foreach (XElement child in element.Elements())
                            canvas.Children.Add(EffectableObjectXmlSerializer.Deserialize(child));
                        canvas.UpdateChildrenPosition();
                        break;
                    case "Events":
                        eventsElement = element;
                        break;
                    default:
                        break;
                }
            }

            if (eventsElement != null)
            {
                foreach (XElement child in eventsElement.Elements("Event"))
                {
                    try
                    {
                        BasicControl raiseControl = (BasicControl)((EffectableControl)canvas.Children[int.Parse(child.Attribute("RaiseControlIndex").Value)]).Control;
                        BasicControl handleControl = (BasicControl)((EffectableControl)canvas.Children[int.Parse(child.Attribute("HandleControlIndex").Value)]).Control;
                        string eventName = child.Attribute("EventName").Value;
                        string handleOperation = child.Attribute("HandleOperation").Value;
                        MDTEventManager.RegisterEvent(raiseControl, eventName, handleControl, handleOperation);
                    }
                    catch { }
                }
            }
        }

        public static void LoadInDesign(string xml, DesignCanvas designCanvas)
        {
            DockCanvas.DockCanvas canvas = designCanvas.ControlContainer;
            XmlReader reader = XmlReader.Create(new StringReader(xml));
            XDocument doc = XDocument.Load(reader);
            XElement root = doc.Root;
            XElement eventsElement = null;

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
                        canvas.Background = (Brush)MyXmlSerializer.Deserialize(element.FirstNode.ToString());
                        break;
                    case "Controls":
                        foreach (XElement child in element.Elements())
                            designCanvas.AddControl(EffectableObjectXmlSerializer.Deserialize(child));
                        break;
                    case "Events":
                        eventsElement = element;
                        break;
                    default:
                        break;
                }
            }

            if (eventsElement != null)
            {
                foreach (XElement child in eventsElement.Elements("Event"))
                {
                    try
                    {
                        BasicControl raiseControl = (BasicControl)designCanvas.Controls[int.Parse(child.Attribute("RaiseControlIndex").Value)].Control;
                        BasicControl handleControl = (BasicControl)designCanvas.Controls[int.Parse(child.Attribute("HandleControlIndex").Value)].Control;
                        string eventName = child.Attribute("EventName").Value;
                        string handleOperation = child.Attribute("HandleOperation").Value;
                        MDTEventManager.RegisterEvent(raiseControl, eventName, handleControl, handleOperation);
                    }
                    catch { }
                }
            }
        }
    }
}
