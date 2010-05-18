using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        // TODO: Add your service operations here
        [OperationContract]
        string GetStringFromURL(string url);

        [OperationContract]
        byte[] GetDataFromURL(string url);

        [OperationContract]
        bool SendMail(string fromName, string fromAddress, string toAddresses, string subject, string body);
    }
}