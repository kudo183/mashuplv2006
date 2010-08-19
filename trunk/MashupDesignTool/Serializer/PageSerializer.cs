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
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using BasicLibrary;

namespace MashupDesignTool
{
    public class PageSerializer
    {
        public delegate void DeserializeCompletedHandler();
        public event DeserializeCompletedHandler DeserializeCompleted; 
        
        private XElement canvasElement;
        private DockCanvas.DockCanvas dockCanvas;
        private DesignCanvas designCanvas;
        private ControlDownloader controlDownloader;
        private EffectDownloader effectDownloader;
        private List<string> effectDll, effectReferenceDll;
        private bool design;

        #region Serialize
        public static string Serialize(DesignCanvas designCanvas, List<string> controlDll, List<string> controlReferenceDll, List<string> effectDll, List<string> effectReferenceDll)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriter xm = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true });

            xm.WriteStartElement("Page");
            WriteNeccesaryDlls(xm, controlDll, controlReferenceDll, effectDll, effectReferenceDll);
            xm.WriteRaw(DockCanvasSerializer.Serialize(designCanvas));
            xm.WriteEndElement();

            xm.Flush();
            xm.Close();
            return sb.ToString();
        }

        private static void WriteNeccesaryDlls(XmlWriter xm, List<string> controlDll, List<string> controlReferenceDll, List<string> effectDll, List<string> effectReferenceDll)
        {
            xm.WriteStartElement("NeccesaryDlls");
            WriteDllList(xm, controlDll, "ControlDll");
            WriteDllList(xm, controlReferenceDll, "ControlReferenceDll");
            WriteDllList(xm, effectDll, "EffectDll");
            WriteDllList(xm, effectReferenceDll, "EffectReferenceDll");
            xm.WriteEndElement();
        }

        private static void WriteDllList(XmlWriter xm, List<string> dlls, string rootName)
        {
            xm.WriteStartElement(rootName);
            foreach (string str in dlls)
                xm.WriteElementString("Dll", str);
            xm.WriteEndElement();
        }
        #endregion Serialize

        #region Deserialize
        public void Deserialize(string xml, DockCanvas.DockCanvas dockCanvas)
        {
            design = false;

            List<string> controlDll = new List<string>();
            List<string> controlReferenceDll = new List<string>();
            List<string> effectDll = new List<string>();
            List<string> effectReferenceDll = new List<string>();

            XmlReader reader = XmlReader.Create(new StringReader(xml));
            XDocument doc = XDocument.Load(reader);
            XElement root = doc.Root;
            XElement designCanvasElement = null;

            foreach (XElement child in root.Elements())
            {
                if (child.Name == "DockCanvas")
                    designCanvasElement = child;
                else if (child.Name == "NeccesaryDlls")
                {
                    foreach (XElement childChild in child.Elements())
                    {
                        if (childChild.Name == "ControlDll")
                            DeserializeListDlls(childChild, controlDll);
                        else if (childChild.Name == "ControlReferenceDll")
                            DeserializeListDlls(childChild, controlReferenceDll);
                        else if (childChild.Name == "EffectDll")
                            DeserializeListDlls(childChild, effectDll);
                        else if (childChild.Name == "EffectReferenceDll")
                            DeserializeListDlls(childChild, effectReferenceDll);
                    }
                }
            }

            this.controlDownloader = new ControlDownloader();
            this.effectDownloader = new EffectDownloader();
            this.canvasElement = designCanvasElement;
            this.dockCanvas = dockCanvas;
            this.effectDll = effectDll;
            this.effectReferenceDll = effectReferenceDll;

            controlDownloader.DownloadCompleted += new ControlDownloader.DownloadCompletedHandler(controlDownloader_DownloadCompleted);
            controlDownloader.Download(controlDll, controlReferenceDll);
        }

        public void DeserializeInDesign(string xml, DesignCanvas designCanvas, ControlDownloader controlDownloader, EffectDownloader effectDownloader)
        {
            design = true;

            List<string> controlDll = new List<string>();
            List<string> controlReferenceDll = new List<string>();
            List<string> effectDll = new List<string>();
            List<string> effectReferenceDll = new List<string>();

            XmlReader reader = XmlReader.Create(new StringReader(xml));
            XDocument doc = XDocument.Load(reader);
            XElement root = doc.Root;
            XElement designCanvasElement = null;

            foreach (XElement child in root.Elements())
            {
                if (child.Name == "DockCanvas")
                    designCanvasElement = child;
                else if (child.Name == "NeccesaryDlls")
                {
                    foreach (XElement childChild in child.Elements())
                    {
                        if (childChild.Name == "ControlDll")
                            DeserializeListDlls(childChild, controlDll);
                        else if (childChild.Name == "ControlReferenceDll")
                            DeserializeListDlls(childChild, controlReferenceDll);
                        else if (childChild.Name == "EffectDll")
                            DeserializeListDlls(childChild, effectDll);
                        else if (childChild.Name == "EffectReferenceDll")
                            DeserializeListDlls(childChild, effectReferenceDll);
                    }
                }
            }

            this.controlDownloader = controlDownloader;
            this.effectDownloader = effectDownloader;
            this.canvasElement = designCanvasElement;
            this.designCanvas = designCanvas;
            this.effectDll = effectDll;
            this.effectReferenceDll = effectReferenceDll;

            controlDownloader.DownloadCompleted += new ControlDownloader.DownloadCompletedHandler(controlDownloader_DownloadCompleted);
            controlDownloader.Download(controlDll, controlReferenceDll);
        }

        void controlDownloader_DownloadCompleted()
        {
            controlDownloader.DownloadCompleted -= new ControlDownloader.DownloadCompletedHandler(controlDownloader_DownloadCompleted);

            effectDownloader.DownloadCompleted += new EffectDownloader.DownloadCompletedHandler(effectDownloader_DownloadCompleted);
            effectDownloader.Download(effectDll, effectReferenceDll);
        }

        void effectDownloader_DownloadCompleted()
        {
            DockCanvasSerializer dcs = new DockCanvasSerializer();
            dcs.DeserializeCompleted += new DockCanvasSerializer.DeserializeCompletedHandler(dcs_DeserializeCompleted);
            if (!design)
                dcs.Deserialize(canvasElement, dockCanvas);
            else
                dcs.DeserializeInDesign(canvasElement, designCanvas);
            effectDownloader.DownloadCompleted -= new EffectDownloader.DownloadCompletedHandler(effectDownloader_DownloadCompleted);
        }

        void dcs_DeserializeCompleted()
        {
            if (DeserializeCompleted != null)
                DeserializeCompleted();
        }

        private void DeserializeListDlls(XElement element, List<string> dlls)
        {
            foreach (XElement child in element.Elements())
            {
                if (child.Name == "Dll")
                    dlls.Add(child.Value);
            }
        }
        #endregion Deserialize
    }
}
