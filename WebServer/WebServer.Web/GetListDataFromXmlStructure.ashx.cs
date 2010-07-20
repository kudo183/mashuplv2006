using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Text;
using System.IO;
using System.Security;

namespace ItemCollectionEditor.Web
{
    /// <summary>
    /// Summary description for GetXMLStructure
    /// </summary>
    public class GetXMLStructure : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            WebClient webClient = new WebClient();
            try
            {
                Uri uri = new Uri(context.Request["URL"], UriKind.RelativeOrAbsolute);
                string elementName = context.Request["ELEMENT"];
                
                //string xmlString = webClient.DownloadString(uri);
                //xmlString = HttpUtility.UrlDecode(xmlString);

                StreamReader sr = new StreamReader(webClient.OpenRead(uri));
                string xmlString = sr.ReadToEnd();
                sr.Close();
                //XmlDocument xoc = new XmlDocument();
                
                //xoc.Load(webClient.OpenRead(uri));
                //StringBuilder sb1 = new StringBuilder();
                //XmlWriter xw = XmlWriter.Create(sb1);
                //xoc.WriteContentTo(xw);
                //xw.Close();
                //string temp = sb1.ToString();

                XDocument xDoc = XDocument.Parse(xmlString);
                string temp;
                StringBuilder sb = new StringBuilder();
                XmlWriter xResult = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true });
                foreach (XElement element in xDoc.Descendants(elementName))
                {
                    xResult.WriteStartElement("Root");
                    foreach (XElement e in element.Elements())
                    {
                        xResult.WriteStartElement(e.Name.ToString());
                        temp = SecurityElement.Escape(e.Value);
                        xResult.WriteRaw(temp);
                        xResult.WriteEndElement();
                    }
                    xResult.WriteEndElement();
                    break;
                }
                xResult.Flush();
                xResult.Close();
                context.Response.Write(sb.ToString());
            }
            catch (Exception ex)
            {
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}