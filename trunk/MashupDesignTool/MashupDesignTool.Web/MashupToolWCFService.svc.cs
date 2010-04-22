using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Net;

namespace MashupDesignTool.Web
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class MashupToolWCFService
    {
        [OperationContract]
        public void DoWork()
        {
            // Add your operation implementation here
            return;
        }

        // Add more operations here and mark them with [OperationContract]

        [OperationContract]
        public string GetStringFromURL(string url)
        {
            WebClient webClient = new WebClient();
            return webClient.DownloadString(url);
        }

        [OperationContract]
        public byte[] GetDataFromURL(string url)
        {
            WebClient webClient = new WebClient();
            return webClient.DownloadData(url);
        }
    }
}
