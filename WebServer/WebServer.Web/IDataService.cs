using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WebServer.Web
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDataService" in both code and config file together.
    [ServiceContract]
    public interface IDataService
    {
        [OperationContract]
        List<DesignedApplicationData> GetDesignedApplicationList(string userName);

        [OperationContract]
        DesignedApplicationData Insert(DesignedApplicationData data);

        [OperationContract]
        DesignedApplicationData Update(DesignedApplicationData data);

        [OperationContract]
        DesignedApplicationData Delete(Guid id);

        [OperationContract]
        DesignedApplicationData GetDesignedApplication(Guid id);

        [OperationContract]
        Guid GetUserIdFromName(string userName);
    }

    [DataContract]
    public class DesignedApplicationData
    {
        Guid id;
        Guid userId;
        string applicationName;
        byte[] xmlString;
        DateTime lastUpdate = new DateTime();

        [DataMember]
        public Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public Guid UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        [DataMember]
        public string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        [DataMember]
        public byte[] XmlString
        {
            get { return xmlString; }
            set { xmlString = value; }
        }

        [DataMember]
        public DateTime LastUpdate
        {
            get { return lastUpdate; }
            set { lastUpdate = value; }
        }
    }
}
