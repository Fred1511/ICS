using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NOutils
{
    public class WebExpert
    {
        // *** PUBLIC ***************************

        public static Licence[] ReadLicencesFromWeb_Asynchrone()
        {
            client = new HttpClient();
            var result = GetGlobalDataAsync().GetAwaiter().GetResult();
            var lines = result.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            var output = new List<Licence>();
            foreach (var line in lines)
            {
                var components = line.Split('=');
                var licence = new Licence(components[0], components[1]);
                output.Add(licence);
            }

            return output.ToArray();
        }

        public static Licence[] ReadLicencesFromWeb_Synchrone()
        {
            client = new HttpClient();
            try
            {
                var result = client.GetAsync("https://www.forsim.net/rest/api.html").Result;

                if (result.IsSuccessStatusCode)
                {
                    var data = result.Content.ReadAsStringAsync().Result;
                    var lines = data.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                    var output = new List<Licence>();
                    foreach (var line in lines)
                    {
                        var components = line.Split('=');
                        var licence = new Licence(components[0], components[1]);
                        output.Add(licence);
                    }

                    return output.ToArray();
                }
            }
            catch
            {

            }

            return null;
        }


        // *** RESTRICTED ***********************

        private static HttpClient client;

        private static async Task<string> GetGlobalDataAsync()
        {
            var data = string.Empty;
            var response = await client.GetAsync("https://www.forsim.net/rest/api.html");

            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return data;
        }
    }
}
