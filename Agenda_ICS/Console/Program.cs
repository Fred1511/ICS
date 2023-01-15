using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Net.Http;
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

        static string ReadDatasFromWeb()
        {
            client = new HttpClient();
            var result = GetGlobalDataAsync().GetAwaiter().GetResult();

            return result;
        }

        static void Main(string[] args)
        {

            //var test = new ReadDatasOnFile();
            //test.Core();
            ManageBddLinq();

            System.Console.WriteLine("Press any key to exit.....");
            System.Console.ReadKey();
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

            using (SqlCommand command = new SqlCommand("SELECT Nom FROM Devis WHERE Code='00001011'", con))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    System.Console.WriteLine("{0}",
                        reader.GetString(0));
                }
            }

            //SqlCommand cmd = new SqlCommand("INSERT INTO Employees (EmployeID, Beginning, End) VALUES (@V1, @V2, @V3");

            //DateTime beginning = DateTime.Now;

            //cmd.Parameters.AddWithValue("@V1", 1);
            //cmd.Parameters.AddWithValue("@V2", beginning);
            //cmd.Parameters.AddWithValue("@V3", Convert.ToDateTime(txtDatum.Text));

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
