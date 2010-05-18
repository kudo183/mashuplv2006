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

        public bool SendMail(string fromName, string fromAddress, string toAddresses, string subject, string body)
        {
            try
            {
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
                msg.From = new System.Net.Mail.MailAddress(fromName + "<" + fromAddress + ">");
                msg.ReplyTo = new System.Net.Mail.MailAddress(fromName + "<" + fromAddress + ">");
                string[] array = toAddresses.Split(',');
                foreach (string toAddress in array)
                    msg.To.Add(new System.Net.Mail.MailAddress(toAddress));
                msg.Subject = subject;
                msg.Body = body;
                msg.IsBodyHtml = true;

                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                smtp.Send(msg);
                return true;
            }
            catch
            {
                return false;
            }        
        }
    }
}
