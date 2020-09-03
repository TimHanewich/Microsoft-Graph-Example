using System;
using System.Net.Http;
using System.Text;
using System.Text.Encodings;

namespace MicrosoftGraphHttpApiExample
{
    public class CommonDataServiceExample
    {
        public static void RunCdsExample()
        {

            //To do BEFORE this
            //1 - Set up a registered application in the Azure portal. 
            //2 - Add CDS permissions to that application
            //3 - Change the value "oauth2AllowImplicitFlow" to true in the application manifest (you will have to download it, edit it, and re-upload it)



            ///////INPUTS///////
            string env_url = "https://org159a2adc0.crm.dynamics.com"; //The URL of your CDS environment.
            string client_id = "a5ad2463-0a21-4e98-bae8-dc36bdf432ba"; //From your registered application in the Azure portal.
            
            //Construct the authorization URL that we will send the user to
            string baseURL = "https://login.microsoftonline.com/common/oauth2/authorize";
            string param1 = "resource=" + env_url;
            string param2 = "client_id=" + client_id;
            string param3 = "response_type=token"; //We are asking for an access token.
            string AUTH_URL = baseURL + "?" + param1 + "&" + param2 + "&" + param3;
            
            

            //Have the user go to this endpoint in a browser and authenticate here
            Console.WriteLine("Please navigate to this URL in a browser and authenticate with an account that has permission to access the environment '" + env_url + "'");
            Console.WriteLine(AUTH_URL);

            //After the user authenticates into that URL, it will redirect them to the webpage that you have set as the Redirect URL in the registered app in the Azure Portal.
            //The access token will be in the URL header as a paramater.
            //In an actual production app, you will want that redirect endpoint to be one of your API's that then receives that access token as an input.
            //And that will be your access token that we will use moving forward.
            //Since we did not do that (this is just a demo), we will ask the user to copy + paste that redirect address into this app and parse the token that we need out of it.

            //Ask for the address
            Console.WriteLine();
            Console.WriteLine("After authenticating, please copy + paste the URL from the address bar that you are redirected to");
            Console.WriteLine("Paste below:");
            string redirected_to_address = Console.ReadLine();
            Console.WriteLine();

            //Parse the access token out
            int loc1 = redirected_to_address.IndexOf("access_token");
            loc1 = redirected_to_address.IndexOf("=", loc1 + 1);
            int loc2 = redirected_to_address.IndexOf("&", loc1 + 1);
            string access_token = redirected_to_address.Substring(loc1 + 1, loc2 - loc1 - 1);


            //Construct the web api URl - This will be the endpoint that we use for all of our API calls to the CDS
            string web_api_url = env_url + "/api/data/v9.0/";
            
            //Give options as to what they want to do
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("What do you want to do?");
                Console.WriteLine("1 - Test and prove conneciton with 'WhoAmI'");
                Console.WriteLine("2 - Retrieve an Account record");
                Console.WriteLine("3 - Retrieve a custom record");
                Console.WriteLine("4 - Update a custom entity name");
                Console.WriteLine("5 - Update a custom entity's Lookup field");
                Console.WriteLine("6 - Add new Account record");
                string ToDo = Console.ReadLine();
                Console.WriteLine();
                if (ToDo == "1")
                {
                    //Construct the request
                    HttpRequestMessage req = new HttpRequestMessage();
                    req.Method = HttpMethod.Get;
                    req.Headers.Add("Authorization", "Bearer " + access_token); //Add the access token as a header
                    req.RequestUri = new Uri(web_api_url + "WhoAmI");
                    
                    //Make the call
                    HttpClient hc = new HttpClient();
                    HttpResponseMessage resp = hc.SendAsync(req).Result;
                    string resp_txt = resp.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(resp_txt);
                }
                else if (ToDo == "2")
                {                    
                    
                    //Construct the account query endpoint
                    //To see the "entity setter" keywords (for example, the "accounts" is the keyword to reference the account entity) for all entities, go to https://<<Your org name>>.crm.dynamics.com/api/data/v9.0/ in an already-authenticated browser session. (sign into power apps, then go to that end point)
                    string accountGUID = "752603e3-13d0-ea11-a812-000d3a3b7d88"; //This is the GUID value (unique identifier of that record) of the account record in CDS
                    string query_endpoint = "accounts(" + accountGUID + ")";
                    
                    //Construct the request
                    HttpRequestMessage req = new HttpRequestMessage();
                    req.Method = HttpMethod.Get;
                    req.Headers.Add("Authorization", "Bearer " + access_token);
                    req.RequestUri = new Uri(web_api_url + query_endpoint);

                    //Make the call
                    HttpClient hc = new HttpClient();
                    HttpResponseMessage resp = hc.SendAsync(req).Result;
                    string resp_txt = resp.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(resp_txt);
                }
                else if (ToDo == "3")
                {
                    //Construct the animal query endpoint
                    //To see the "entity setter" keywords (for example, the "accounts" is the keyword to reference the account entity) for all entities, go to https://<<Your org name>>.crm.dynamics.com/api/data/v9.0/ in an already-authenticated browser session. (sign into power apps, then go to that end point)
                    string animalGUID = "12b673d8-11ee-ea11-a817-000d3a3b7d88"; //This is the GUID value (unique identifier of that record) of the account record in CDS
                    string query_endpoint = "crda6_animals(" + animalGUID + ")";
                    
                    //Construct the request
                    HttpRequestMessage req = new HttpRequestMessage();
                    req.Method = HttpMethod.Get;
                    req.Headers.Add("Authorization", "Bearer " + access_token);
                    req.RequestUri = new Uri(web_api_url + query_endpoint);

                    //Make the call
                    HttpClient hc = new HttpClient();
                    HttpResponseMessage resp = hc.SendAsync(req).Result;
                    string resp_txt = resp.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(resp_txt);
                }
                else if (ToDo == "4")
                {
                    string update_body = "{\"crda6_name\":\"Johnny the Evolved Jaguar\"}";

                    //Construct the query
                    string animalGUID = "12b673d8-11ee-ea11-a817-000d3a3b7d88";
                    string query_endpoint = "crda6_animals(" + animalGUID + ")";

                    //Construct the request
                    HttpRequestMessage req = new HttpRequestMessage();
                    req.Method = HttpMethod.Patch; //Patch method used for record updates
                    req.Headers.Add("Authorization", "Bearer " + access_token);
                    req.RequestUri = new Uri(web_api_url + query_endpoint);
                    req.Content = new StringContent(update_body, Encoding.UTF8, "application/json");

                    //Make the call
                    HttpClient hc = new HttpClient();
                    HttpResponseMessage resp = hc.SendAsync(req).Result;
                    Console.WriteLine("Response status code: " + resp.StatusCode.ToString());
                    string content = resp.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("Response content: " + content);
                    Console.WriteLine("Call complete!");
                }
                else if (ToDo == "5")
                {
                    //Make the update body
                    //You only have to specify the properites that you want to update
                    //The name of the field is the SCHEMA name, which you can find in the Dynamics 365 Solution explorer when you enter into a field. Note that this IS case sensitive.
                    //Since this is a lookup value, you also must include two things that you typically wouldn't include for all other fields
                    //  1 - You need to include the "@odata.bind" at the end of the field schema name (still inside of the quotations)
                    //  2 - Instead of just plugging in the GUID value, you must wrap it inside the entity setter for that entity type ("crda6_animals" below).
                    string update_body = "{\"crda6_ParentAnimal@odata.bind\":\"crda6_animals(e8d51151-12ee-ea11-a817-000d3a3b7d88)\"}";

                    //Construct the query
                    string animalGUID = "12b673d8-11ee-ea11-a817-000d3a3b7d88";
                    string query_endpoint = "crda6_animals(" + animalGUID + ")";

                    //Construct the request
                    HttpRequestMessage req = new HttpRequestMessage();
                    req.Method = HttpMethod.Patch; //Patch method used for record updates
                    req.Headers.Add("Authorization", "Bearer " + access_token);
                    req.RequestUri = new Uri(web_api_url + query_endpoint);
                    req.Content = new StringContent(update_body, Encoding.UTF8, "application/json");

                    //Make the call
                    HttpClient hc = new HttpClient();
                    HttpResponseMessage resp = hc.SendAsync(req).Result;
                    Console.WriteLine("Response status code: " + resp.StatusCode.ToString());
                    string content = resp.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("Response content: " + content);
                    Console.WriteLine("Call complete!");
                }
                else if (ToDo == "6")
                {
                    string body = "{\"name\":\"API Industries\"}";

                    //Construct the request
                    HttpRequestMessage req = new HttpRequestMessage();
                    req.Content = new StringContent(body, Encoding.UTF8, "application/json");
                    req.Method = HttpMethod.Post;
                    req.RequestUri = new Uri(web_api_url + "accounts");
                    req.Headers.Add("Authorization", "Bearer " + access_token);

                    //Make the call
                    HttpClient hc = new HttpClient();
                    HttpResponseMessage msg = hc.SendAsync(req).Result;
                    Console.WriteLine("Status Code: " + msg.StatusCode.ToString());
                    Console.WriteLine("Return Content: " + msg.Content.ReadAsStringAsync().Result);
                    Console.WriteLine("Call complete!");
                    
                }
                else
                {
                    Console.WriteLine("That wasn't an option.");
                }
            }
            

        }
    }
}