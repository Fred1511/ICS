using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NOutils
{
    public class WebExpert
    {
        // *** PUBLIC ***************************

        static public Licence[] ReadLicencesFromWeb()
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

        // *** RESTRICTED ***********************

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

    }
}
