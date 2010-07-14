using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

namespace MashupDesignTool.Web
{
    /// <summary>
    /// Summary description for GetBinaryData
    /// </summary>
    public class GetBinaryData : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            WebClient webClient = new WebClient();

            try
            {
                Uri uri  = new Uri(context.Request["URL"], UriKind.RelativeOrAbsolute);
                context.Response.BinaryWrite(webClient.DownloadData(uri));
            }
            catch(Exception ex)
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