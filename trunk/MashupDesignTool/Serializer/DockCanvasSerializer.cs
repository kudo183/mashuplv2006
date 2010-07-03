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
        public delegate void DeserializeCompletedHandler();
        public event DeserializeCompletedHandler DeserializeCompleted;
        
        private DockCanvas.DockCanvas dockCanvas;
        private DesignCanvas designCanvas;
        private XElement eventsElement = null;
        private List<XElement> controlElements;
        private int controlCount;
        private int numControl;
        private bool design;

        public static string Serialize(DesignCanvas designCanvas)
        {
            List<EffectableControl> controls = designCanvas.Controls;
            StringBuilder sb = new StringBuilder();
            XmlWriter xm = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true });

            xm.WriteStartElement("DockCanvas");

            xm.WriteElementString("Width", designCanvas.ControlContainerCanvas.Width.ToString());
            xm.WriteElementString("Height", designCanvas.ControlContainerCanvas.Height.ToString());
            xm.WriteStartElement("Background");
            xm.WriteRaw(MyXmlSerializer.Serialize(designCanvas.ControlContainerCanvas.Background));
            xm.WriteEndElement();

            xm.WriteStartElement("Controls");
            foreach (EffectableControl ec in controls)
            {
                xm.WriteRaw(EffectableObjectXmlSerializer.Serialize(ec));
            }
            xm.WriteEndElement();

            xm.WriteStartElement("Events");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            for (int i = 0; i < controls.Count; i++)
                dictionary.Add(controls[i].Control.Name, i.ToString());
            for (int i = 0; i < controls.Count; i++)
            {
                if (typeof(BasicControl).IsAssignableFrom(controls[i].Control.GetType()))
                {
                    List<MDTEventInfo> list = MDTEventManager.GetListEventInfoRaiseBy((BasicControl)controls[i].Control);
                    foreach (MDTEventInfo mei in list)
                    {
                        int count = mei.HandleControls.Count;
                        xm.WriteStartElement("Event");
                        xm.WriteElementString("RaiseControlIndex", i.ToString());
                        xm.WriteElementString("EventName", mei.EventName);
                        xm.WriteStartElement("Handles");
                        for (int j = 0; j < count; j++)
                        {
                            xm.WriteStartElement("Handle");
                            xm.WriteElementString("HandleControlIndex", dictionary[mei.HandleControls[j].Name]);
                            xm.WriteElementString("HandleOperation", mei.HandleOperations[j]);
                            xm.WriteEndElement();
                        }
                        xm.WriteEndElement();
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

        #region deserialize
        public void Deserialize(string xml, DockCanvas.DockCanvas canvas)
        {
            XmlReader reader = XmlReader.Create(new StringReader(xml));
            XDocument doc = XDocument.Load(reader);
            XElement root = doc.Root;
            Deserialize(root, canvas);
        }

        public void Deserialize(XElement root, DockCanvas.DockCanvas canvas)
        {
            design = false;
            dockCanvas = canvas;
            controlCount = 0;
            numControl = 0;
            eventsElement = null;
            XElement controlsElement = null;

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
                        controlsElement = element;
                        break;
                    case "Events":
                        eventsElement = element;
                        break;
                    default:
                        break;
                }
            }

            if (controlsElement != null)
            {
                controlElements = new List<XElement>();
                foreach (XElement child in controlsElement.Elements())
                {
                    numControl++;
                    controlElements.Add(child);
                }
                if (numControl > 0)
                {
                    EffectableObjectXmlSerializer eoxs = new EffectableObjectXmlSerializer();
                    eoxs.DeserializeCompleted += new EffectableObjectXmlSerializer.DeserializeCompletedHandler(eoxs_DeserializeCompleted);
                    eoxs.Deserialize(controlElements[0]);
                }
            }
        }

        public void DeserializeInDesign(string xml, DesignCanvas designCanvas)
        {
            XmlReader reader = XmlReader.Create(new StringReader(xml));
            XDocument doc = XDocument.Load(reader);
            XElement root = doc.Root;

            DeserializeInDesign(root, designCanvas);
        }

        public void DeserializeInDesign(XElement root, DesignCanvas designCanvas)
        {
            design = true;
            DockCanvas.DockCanvas canvas = designCanvas.ControlContainerCanvas;
            this.designCanvas = designCanvas;
            controlCount = 0;
            numControl = 0;
            eventsElement = null;
            XElement controlsElement = null;

            foreach (XElement element in root.Elements())
            {
                switch (element.Name.LocalName)
                {
                    case "Width":
                        canvas.Width = double.Parse(element.Value);
                        //designCanvas.LayoutRoot.Width = canvas.Width;
                        break;
                    case "Height":
                        canvas.Height = double.Parse(element.Value);
                        //designCanvas.LayoutRoot.Height = canvas.Height;
                        break;
                    case "Background":
                        canvas.Background = (Brush)MyXmlSerializer.Deserialize(element.FirstNode.ToString());
                        break;
                    case "Controls":
                        controlsElement = element;
                        break;
                    case "Events":
                        eventsElement = element;
                        break;
                    default:
                        break;
                }
            }

            if (controlsElement != null)
            {
                controlElements = new List<XElement>();
                foreach (XElement child in controlsElement.Elements())
                {
                    numControl++;
                    controlElements.Add(child);
                }
                if (numControl > 0)
                {
                    EffectableObjectXmlSerializer eoxs = new EffectableObjectXmlSerializer();
                    eoxs.DeserializeCompleted += new EffectableObjectXmlSerializer.DeserializeCompletedHandler(eoxs_DeserializeCompleted);
                    eoxs.Deserialize(controlElements[0]);
                }
            }
        }

        void eoxs_DeserializeCompleted(EffectableControl control)
        {
            if (design)
                designCanvas.AddEffectableControl(control);
            else
                dockCanvas.Children.Add(control);
            controlCount++;
            if (controlCount != numControl)
            {
                EffectableObjectXmlSerializer eoxs = new EffectableObjectXmlSerializer();
                eoxs.DeserializeCompleted += new EffectableObjectXmlSerializer.DeserializeCompletedHandler(eoxs_DeserializeCompleted);
                eoxs.Deserialize(controlElements[controlCount]);
            }
            else
            {
                dockCanvas.UpdateChildrenPosition();
                if (eventsElement != null)
                {
                    foreach (XElement child in eventsElement.Elements("Event"))
                    {
                        BasicControl raiseControl;
                        List<BasicControl> handleControls = new List<BasicControl>();
                        string eventName;
                        List<string> handleOperations = new List<string>();
                        try
                        {
                            raiseControl = (BasicControl)((EffectableControl)dockCanvas.Children[int.Parse(child.Element("RaiseControlIndex").Value)]).Control;
                            eventName = child.Element("EventName").Value;
                            XElement handlesElement = child.Element("Handles");
                            foreach (XElement handle in handlesElement.Elements("Handle"))
                            {
                                BasicControl handleControl = (BasicControl)((EffectableControl)dockCanvas.Children[int.Parse(handle.Element("HandleControlIndex").Value)]).Control;
                                string handleOperation = handle.Element("HandleOperation").Value;
                                handleControls.Add(handleControl);
                                handleOperations.Add(handleOperation);
                            }
                            MDTEventManager.RegisterEvent(raiseControl, eventName, handleControls, handleOperations);
                        }
                        catch { }
                    }
                }
                if (DeserializeCompleted != null)
                    DeserializeCompleted();
            }
        }
        #endregion deserialize
    }
}
