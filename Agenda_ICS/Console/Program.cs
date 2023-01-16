using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading.Tasks;

namespace NConsole
{
    class Program
    {
        static HttpClient client;

        static async Task<string> GetGlobalDataAsync()
        {
            var data = string.Empty;
            var response = await client.GetAsync("https://www.forsim.net/rest/api.html");

            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return data;
        }

        static string[] ReadDatasFromWeb()
        {
            client = new HttpClient();
            var result = GetGlobalDataAsync().GetAwaiter().GetResult();

            var lines = result.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            return lines;
        }

        static void Main(string[] args)
        {
            var datas = ReadDatasFromWeb();
            foreach(var data in datas)
                System.Console.WriteLine("Datas from web : [" + data + "]");

            //var address = GetPublicIpAddress();
            //System.Console.WriteLine("IP address : " + address);

            //GetAllHddDatas();
            //System.Console.WriteLine("/n/n");

            //System.Console.Write("Drive letter : ");
            //var letter = System.Console.ReadLine();

            //var serialNo = GetHddDatas(letter + ":");
            //System.Console.WriteLine("Serial n° of " + letter + " : [" + serialNo + "]");

            System.Console.WriteLine("Press any key to exit.....");
            System.Console.ReadKey();
        }

        static string GetHddDatas(string driveLetter)
        {
            try
            {
                using (var partitions = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_LogicalDisk.DeviceID='" + driveLetter +
                                                    "'} WHERE ResultClass=Win32_DiskPartition"))
                {
                    foreach (var partition in partitions.Get())
                    {
                        using (var drives = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" +
                                                                partition["DeviceID"] +
                                                                "'} WHERE ResultClass=Win32_DiskDrive"))
                        {
                            foreach (var drive in drives.Get())
                            {
                                return (string)drive["SerialNumber"];
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return "<unknown>";
        }

        static void GetAllHddDatas()
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");

            int i = 0;
            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                // get the hardware serial no.
                if (wmi_HD["SerialNumber"] == null)
                    System.Console.WriteLine("None");
                else
                    System.Console.WriteLine(wmi_HD["SerialNumber"].ToString());

                ++i;
            }
        }

        static string GetPublicIpAddress()
        {
            String address = "";
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                address = stream.ReadToEnd();
            }

            int first = address.IndexOf("Address: ") + 9;
            int last = address.LastIndexOf("</body>");
            address = address.Substring(first, last - first);
            address = address.Split(':')[1].Trim();
            return address;
        }

        private static void TestSpeechRecogniser()
        {
            SpeechRecognizer sr = new SpeechRecognizer();
            GrammarBuilder gb = new GrammarBuilder("hello computer");
            Grammar gr = new Grammar(gb);
            sr.LoadGrammar(gr);

            //sr.SpeechRecognized += new EventHandler(sr_SpeechRecognized);
            sr.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sr_SpeechRecognized);

            while (true)
            {
                System.Console.ReadLine();
            }

        }

// Handle the SpeechRecognized event.  
        private static void sr_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            System.Console.WriteLine("Recognized text: " + e.Result.Text);
        }
        
        private static void ManageBddLinq()
        {
            var con = new SqlConnection();
            con.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; 
                            AttachDbFilename = C:\Users\Utilisateur\Documents\ICS\ArchiveDbBatigest\BTG_DOS_ICS.mdf; 
                            Integrated Security = True";
            con.Open();

            using (SqlCommand command = new SqlCommand("SELECT Code, Nom, Date, Adr, TotalHT FROM Devis WHERE Date > '2022-01-01'", con))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var idDevis = reader.GetString(0);
                    var nom = reader.GetString(1);
                    var date = reader.GetDateTime(2);
                    var address = reader.GetString(3);
                    var totalHT = reader.GetDouble(4);
                    System.Console.WriteLine("{0} {1} {2} {3} {4}", idDevis, nom, date, address, totalHT);
                }
            }

            con.Close();
        }

        private static void ExtractDatasFromAccess()
        {
            string myConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                           @"Data Source=C:\Users\Utilisateur\Documents\Batigest\SOC01\BatiGDevis.mdb;" +
                           "Persist Security Info=True;" +
                           "Jet OLEDB:Database Password=;";
            try
            {
                // Open OleDb Connection
                OleDbConnection myConnection = new OleDbConnection();
                myConnection.ConnectionString = myConnectionString;
                myConnection.Open();

                // Execute Queries
                OleDbCommand cmd = myConnection.CreateCommand();
                cmd.CommandText = "SELECT * FROM `Devis` WHERE `CodeClient` = \"373\" ORDER BY `Date`";
                OleDbDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection); // close conn after complete

                // Load the result into a DataTable
                DataTable myDataTable = new DataTable();
                myDataTable.Load(reader);

                foreach (DataRow row in myDataTable.Rows)
                {
                    System.Console.WriteLine(row["Code"] + " - " + row["Nom"]);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("OLEDB Connection FAILED: " + ex.Message);
            }
        }
    }
}
