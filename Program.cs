using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Problem1
{
    public class Entry
    {
        public string API { get; set; }
        public string Description { get; set; }
        public string Auth { get; set; }
        public bool HTTPS { get; set; }
        public string Cors { get; set; }
        public string Link { get; set; }
        public string Category { get; set; }
    }

    public class Root
    {
        public int Count { get; set; }
        public List<Entry> Entries { get; set; }
    }

    class Program
    {

        static void Main(string[] args)
        {
            string currentdatetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string LogFolder = @"E:Problem1";

            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var json = "";
                using (WebClient wc = new WebClient())
                {
                    json = wc.DownloadString("https://api.publicapis.org/entries");
                
                }

                Root root = JsonConvert.DeserializeObject<Root>(json);

                foreach (Entry entry in root.Entries)
                {
                    string connectionString = @"Data Source = SEIF; Initial Catalog = Problem1; Integrated Security = True; ";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                       
                        connection.Open();

                        string insertStatement = "INSERT INTO JSON_DATA(API, [Description], Auth, HTTPS, Cors, Link, Category) " +
                            "VALUES('" + entry.API.Replace("'", "''") + "', '" + entry.Description.Replace("'", "''") + "', '" + entry.Auth + "', '" + entry.HTTPS + "', '" + entry.Cors + "', '" + entry.Link + "', '" + entry.Category + "')";

                        SqlCommand command = new SqlCommand(insertStatement, connection);

                        int rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }

            catch (Exception exception)
            {
                using (StreamWriter sw = File.CreateText(LogFolder + "\\" + "ErrorLog_" + currentdatetime + ".log"))
                {
                    sw.WriteLine(exception.ToString());
                }

            }
        }
    }
}
