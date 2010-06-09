using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Xml.Linq;
using System.Xml.Serialization;

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

                string xmlString = webClient.DownloadString(uri);

                XDocument xDoc = XDocument.Parse(xmlString);
                List<string> result = new List<string>();

                foreach (XElement element in xDoc.Descendants(elementName))
                {
                    foreach (XElement e in element.Elements())
                        result.Add(e.Name.ToString());
                    break;
                }

                XmlSerializer xm = new XmlSerializer(typeof(List<string>));
                xm.Serialize(context.Response.OutputStream, result);
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