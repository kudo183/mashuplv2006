using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using System.Net;
using System.Xml.Serialization;

namespace MashupDesignTool.Web
{
    /// <summary>
    /// Summary description for GetListDataFromXml
    /// </summary>
    public class GetListDataFromXml : IHttpHandler
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
                List<List<string>> result = new List<List<string>>();
                foreach (XElement element in xDoc.Descendants(elementName))
                {
                    List<string> temp = new List<string>();
                    foreach (XElement e in element.Elements())
                    {
                        temp.Add(e.Value);
                    }
                    result.Add(temp);
                }

                XmlSerializer xm = new XmlSerializer(typeof(List<List<string>>));
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