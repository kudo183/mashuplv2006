using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Net;

namespace WcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class Service1 : IService1
    {
        public string GetStringFromURL(string url)
        {
            WebClient webClient = new WebClient();
            return webClient.DownloadString(url);
        }

        public byte[] GetDataFromURL(string url)
        {
            WebClient webClient = new WebClient();
            return webClient.DownloadData(url);
        }
    }
}
