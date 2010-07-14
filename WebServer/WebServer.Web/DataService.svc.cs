using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WebServer.Web.linqtosql;
using System.ServiceModel.Activation;

namespace WebServer.Web
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DataService" in code, svc and config file together.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DataService : IDataService
    {
        public List<DesignedApplicationData> GetList(string userName)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            aspnet_User user = dc.GetUserByName(userName);
            var dads = from p in dc.DesignedApplications
                       where p.UserId == user.UserId
                       select p;
            List<DesignedApplicationData> list = new List<DesignedApplicationData>();
            foreach (DesignedApplication p in dads)
            {
                list.Add(Convert(p));
            }
            return list;
        }

        public DesignedApplicationData Insert(DesignedApplicationData data)
        {
            DesignedApplication da = Convert(data);
            da.Id = Guid.NewGuid();
            DataClasses1DataContext dc = new DataClasses1DataContext();
            dc.DesignedApplications.InsertOnSubmit(da);
            dc.SubmitChanges();
            data.Id = da.Id;
            return data;
        }

        public DesignedApplicationData Update(DesignedApplicationData data)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            DesignedApplication da = dc.DesignedApplications.Single(p => p.Id == data.Id);
            da.ApplicationName = data.ApplicationName;
            da.XmlString = data.XmlString;
            dc.SubmitChanges();
            data.UserId = da.UserId;
            return data;
        }

        public DesignedApplicationData Delete(Guid id)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            DesignedApplication da = dc.DesignedApplications.Single(p => p.Id == id);
            DesignedApplicationData data = Convert(da);
            dc.DesignedApplications.DeleteOnSubmit(da);
            dc.SubmitChanges();
            return data;
        }

        public DesignedApplicationData GetData(Guid id)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            DesignedApplication da1 = dc.DesignedApplications.Single(p => p.Id == id);
            return Convert(da1);
        }

        public Guid GetUserIdFromName(string userName)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return dc.GetUserByName(userName).UserId;
        }

        private DesignedApplicationData Convert(DesignedApplication data)
        {
            DesignedApplicationData dad = new DesignedApplicationData()
            {
                Id = data.Id,
                UserId = data.UserId,
                ApplicationName = data.ApplicationName,
                XmlString = data.XmlString
            };
            return dad;
        }

        private DesignedApplication Convert(DesignedApplicationData data)
        {
            DesignedApplication da = new DesignedApplication()
            {
                Id = data.Id,
                UserId = data.UserId,
                ApplicationName = data.ApplicationName,
                XmlString = data.XmlString
            };
            return da;
        }
    }
}
