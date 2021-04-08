using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace MicrosoftGraphHttpApiExample
{
    public class CdsWithPasswordExample
    {
        //This demo ONLY shows how to authenticate with the CDS api service (Dataverse)
        public static async Task DemoAsync()
        {
            //Inputs
            Guid ClientId = Guid.Parse("a5ad2463-0a21-4e98-bae8-dc36bdf432ba");
            string Resource = "https://ccdscannedbatch20210331.crm.dynamics.com/"; //The Dataverse url to your database
            string username = ""; //Your username
            string password = ""; //Your password

            //Gather username and password for demo
            if (username == "")
            {
                Console.Write("Username? >");
                username = Console.ReadLine();
            }
            if (password == "")
            {
                Console.Write("Password? >");
                password = Console.ReadLine();
            }

            List<KeyValuePair<string, string>> KVPs = new List<KeyValuePair<string, string>>();
            KVPs.Add(new KeyValuePair<string, string>("grant_type", "password")); //Defines the flow we are using. We are supplying a username + password
            KVPs.Add(new KeyValuePair<string, string>("client_id", ClientId.ToString()));
            KVPs.Add(new KeyValuePair<string, string>("resource", Resource));
            KVPs.Add(new KeyValuePair<string, string>("username", username));
            KVPs.Add(new KeyValuePair<string, string>("password", password));
            string body = await new FormUrlEncodedContent(KVPs.ToArray()).ReadAsStringAsync();

            //Make the call
            HttpRequestMessage req = new HttpRequestMessage();
            req.RequestUri = new Uri("https://login.windows.net/common/oauth2/token");
            req.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpClient hc = new HttpClient();
            HttpResponseMessage resp = await hc.SendAsync(req);
            
            string respcontent = await resp.Content.ReadAsStringAsync();
            Console.WriteLine(resp.StatusCode.ToString());
            Console.WriteLine(respcontent);
        }
    }
}