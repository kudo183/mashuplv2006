using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MySql.Data.MySqlClient;
using System.Xml.Serialization;

namespace MashupDesignTool.Web
{
    /// <summary>
    /// Summary description for GetListDataFromDatabase
    /// </summary>
    public class GetListDataFromDatabase : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //string server = "SQL09.FREEMYSQL.NET";
            //string user_name = "kudo183";
            //string password = "nobita";
            //string db = "kudo001";
            //string table = "text";

            string server = context.Request["SERVER"];
            string user_name = context.Request["USER"];
            string password = context.Request["PASS"];
            string db = context.Request["DB"];
            string table = context.Request["TABLE"];
            try
            {
                MySqlConnection conn = new MySqlConnection();
                MySqlCommand cmd = new MySqlCommand();
                conn.ConnectionString = "server=" + server + ";uid=" + user_name + ";pwd=" + password + ";database=" + db + ";";
                conn.Open();

                cmd.CommandText = table;
                cmd.Connection = conn;
                cmd.CommandType = CommandType.TableDirect;
                MySql.Data.MySqlClient.MySqlDataReader reader = cmd.ExecuteReader();
                List<List<string>> result = new List<List<string>>();
                while (reader.Read())
                {
                    List<string> temp = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                        temp.Add(reader[i].ToString());
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