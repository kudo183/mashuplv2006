using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Xml.Serialization;

namespace MashupDesignTool.Web
{
    /// <summary>
    /// Summary description for GetDataFromURL
    /// </summary>
    public class GetStringDataFromURL : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            WebClient webClient = new WebClient();

            try
            {
                Uri uri = new Uri(context.Request["URL"], UriKind.RelativeOrAbsolute);
                string result = webClient.DownloadString(uri);

                XmlSerializer xm = new XmlSerializer(typeof(string));
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