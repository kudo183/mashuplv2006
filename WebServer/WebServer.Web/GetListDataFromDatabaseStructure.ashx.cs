using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MySql.Data.MySqlClient;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
namespace MashupDesignTool.Web
{
    /// <summary>
    /// Summary description for GetDatabaseStructure
    /// </summary>
    public class GetDatabaseStructure : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string server = context.Request["SERVER"];
            string user_name = context.Request["USER"];
            string password = context.Request["PASS"];
            string db = context.Request["DB"];
            string table = context.Request["TABLE"];
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection();
                MySqlCommand cmd = new MySqlCommand();
                conn.ConnectionString = "server=" + server + ";uid=" + user_name + ";pwd=" + password + ";database=" + db + ";";
                conn.Open();

                cmd.CommandText = table;
                cmd.Connection = conn;
                cmd.CommandType = CommandType.TableDirect;
                MySqlDataReader reader = cmd.ExecuteReader();

                StringBuilder sb = new StringBuilder();
                XmlWriter xResult = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true });

                if (reader.Read())
                {
                    xResult.WriteStartElement("Root");
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        xResult.WriteStartElement(reader.GetName(i));
                        xResult.WriteRaw(reader[i].ToString());
                        xResult.WriteEndElement();
                    }
                    xResult.WriteEndElement();
                }
                conn.Close();

                xResult.Flush();
                xResult.Close();
                context.Response.Write(sb.ToString());
            }
            catch (Exception ex)
            {
                if (conn != null)
                    conn.Close();
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