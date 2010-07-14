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
            int index = int.Parse(context.Request["INDEX"]);
            int count = int.Parse(context.Request["COUNT"]);
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
                List<List<string>> result = new List<List<string>>();
                int i = 0;
                while (reader.Read())
                {
                    if (i < index)
                    {
                        i++;
                        continue;
                    }
                    List<string> temp = new List<string>();
                    for (int j = 0; j < reader.FieldCount; j++)
                        temp.Add(reader[j].ToString());
                    result.Add(temp);
                    if (result.Count == count)
                        break;
                }
                conn.Close();
                XmlSerializer xm = new XmlSerializer(typeof(List<List<string>>));
                xm.Serialize(context.Response.OutputStream, result);
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