using System;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MicrosoftGraphHttpApiExample
{
    class Program
    {
        static void Main(string[] args)
        {
            CommonDataServiceExample.RunCdsExample();

        }


        static void OnBehalfOfUserExample()
        {

            /////////   INPUT   ////////////
            string client_id = "e32b77a3-67df-411b-927b-f05cc6fe8d5d"; //The registered Client ID (same as "Application ID") that you can find in the Azure portal.
            string tenant_id = "48bfdaf9-31eb-482c-970d-8dc9e97c26bc"; //Your tenant ID (also can find under "Azure Active Directory" in the Azure portal).
            /////////////////////////////////


            //Get the URL to direct the user to (where they authenticate into)
            //The Redirect URI from this link must match one of the Redirect URI's that are registered for this application in the Azure portal.
            //The response mode equalling query means that the return "code" piece will be part of the address that the user will see.
            //The scope paramater are the permissions you want, separated by space ("%20" since it is in the URL)
            //The offline access scope is what causes the graph api to return a refresh token - a value we can hold onto to get another access token at a point in the future (after expiration of the access token)
            string auth_url = "https://login.microsoftonline.com/" + tenant_id + "/oauth2/v2.0/authorize?client_id=" + client_id + "&response_type=code&redirect_uri=https://www.google.com/&response_mode=query&scope=offline_access%20user.read";
            



            //Show the message
            Console.WriteLine("Authenticate at this URL: ");
            Console.WriteLine();
            Console.WriteLine(auth_url);
            Console.WriteLine();

            //Ask for the address bar after
            Console.WriteLine("After authenticating at the URL above, please copy and paste the web address that you are redirected to.");
            Console.WriteLine("Enter it below:");
            string redirect_url_full = Console.ReadLine();

            //get the code piece out of it
            int loc1 = redirect_url_full.IndexOf("code=");
            loc1 = redirect_url_full.IndexOf("=", loc1 + 1);
            int loc2 = redirect_url_full.IndexOf("&", loc1 + 1);
            string code = redirect_url_full.Substring(loc1 + 1, loc2 - loc1 - 1);
            

            //Set up a POST call to get an access token
            string token_url = "https://login.microsoftonline.com/" + tenant_id + "/oauth2/v2.0/token";
            List<KeyValuePair<string, string>> post_data = new List<KeyValuePair<string, string>>();
            post_data.Add(new KeyValuePair<string, string>("client_id", client_id));
            post_data.Add(new KeyValuePair<string, string>("scope", "user.read"));
            post_data.Add(new KeyValuePair<string, string>("code",code));
            post_data.Add(new KeyValuePair<string, string>("redirect_uri", "https://www.google.com/")); //The redirect URL should match one of the redirect URL's that it has registered with it (and also the redirect URL that was used in the authentication flow above).
            post_data.Add(new KeyValuePair<string, string>("grant_type","authorization_code"));
            FormUrlEncodedContent content = new FormUrlEncodedContent(post_data.ToArray());
            HttpRequestMessage req = new HttpRequestMessage();
            req.Method = HttpMethod.Post;
            req.Content = content;
            req.RequestUri = new Uri(token_url);

            //Make the call
            Console.WriteLine(); //Blank Space
            Console.WriteLine("Requesting access from Graph API...");
            HttpClient hc = new HttpClient();
            HttpResponseMessage hrm = hc.SendAsync(req).Result;
            string web = hrm.Content.ReadAsStringAsync().Result;


            //Get the tokens
            string AccessToken = GetJsonValue(web, "access_token");
            string RefreshToken = GetJsonValue(web, "refresh_token");

            Console.WriteLine(); //Blank Space
            Console.WriteLine("Access token: " + AccessToken);
            Console.WriteLine(); //Blank Space
            Console.WriteLine("Refresh token: " + RefreshToken);
            Console.WriteLine(); //Blank Space

        
        }

        static string GetJsonValue(string from, string value)
        {
            int loc1 = 0;
            loc1 = from.IndexOf(value);
            loc1 = from.IndexOf(":", loc1 + 1);
            loc1 = from.IndexOf("\"", loc1 + 1);
            int loc2 = from.IndexOf("\"", loc1 + 1);
            string tr = from.Substring(loc1 + 1, loc2 - loc1 - 1);
            return tr;
        }

    }
}
