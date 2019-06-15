using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MomoApiTester
{
    class Program
    {
        //when you subscribe for a product, you get 2 keys...a primary and a secondary
        //put the primary key here (i hear either can work)
        private const string SubscriptionKey = "c091e5095e484fdea549fa23f40b11ea";

        //once you have an API user, you can then use the create API key endpoint
        //at to create an API key...put it here
        private const string ApiKey = "0f194c7ed6ce4b59928abb9e72abeb1e";

        //before using the API, you must register an API user by firing a json request
        //to the create API user endpoint. I used postman to fire. Details here
        //
        private const string ApiUser = "4e1ec7a2-9339-438b-855f-55472183e3ef";

        //to create an API key you must supply a call back host e.g localhost or example.com etc
        //so your call back url must be a sub url in that call back host otherwise it wont work
        private const string CallBackURL = "http://localhost/";

        static void Main(string[] args)
        {
            //before you can use the API methods you need a session token.
            //u use the token API to create one.
            string token = GenerateToken(SubscriptionKey);

            Console.WriteLine($"Token: {token}");

            //for the MTN API..the actual transaction ID is a UUID generated for
            //each and every request. Its what MTN requires to be unique for each request
            string transactionID = GenerateUUID();

            Console.WriteLine($"TransactionID: {transactionID}");

            //once you have the token you can now send a request to pay.
            //this API returns an empty response with http status code 202 (Accepted)
            string payResposne = SendRequestToPay(transactionID, token, SubscriptionKey, CallBackURL);

            Console.WriteLine($"RequestToPayResponse (Empty is good): {payResposne}");

            //you need this last API to get the final status of a previously submitted
            //transaction. Remember the actual transaction ID is the UUID generated
            string tranStatus = GetTransactionStatus(transactionID, token, SubscriptionKey);

            Console.WriteLine($"Transaction Status: {tranStatus}");

            //leave
            Console.ReadLine();
        }

        private static string GetTransactionStatus(string uuid, string token, string subscriptionKey)
        {
            string url = $"https://ericssonbasicapi2.azure-api.net/collection/v1_0/requesttopay/" + uuid;//d18ab8ea-a1d8-454b-8fcb-1580441515f4";


            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                {"Authorization", $"Bearer {token}" },
                {"Ocp-Apim-Subscription-Key", subscriptionKey },
                {"X-Target-Environment", "sandbox" },
            };

            string statusResponse = Task.Run(() => SendHttpGetRequest(url, headers)).Result;
            return statusResponse;
        }

        private static string SendRequestToPay(string uuid, string token, string subscriptionKey, string callbackURL)
        {
            string url = "https://ericssonbasicapi2.azure-api.net/collection/v1_0/requesttopay";
            string body = "{" +
                              "\"amount\": \"1000\"," +
                              "\"currency\": \"EUR\"," +
                              "\"externalId\": \"125677889945656675659678\"," +
                              "\"payer\": {" +
                                            "\"partyIdType\": \"MSISDN\"," +
                                            "\"partyId\": \"256785975800\"" +
                              "}," +
                              "\"payerMessage\": \"Test Payment 5\"," +
                              "\"payeeNote\": \"Test Payment 6\"" +
                            "}";

            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                {"X-Reference-Id", uuid },
                {"Authorization", $"Bearer {token}" },
                {"Ocp-Apim-Subscription-Key", subscriptionKey },
                {"X-Callback-Url", callbackURL },
                {"X-Target-Environment", "sandbox" },
            };

            string payResponse = Task.Run(() => SendHttpPostRequest(url, body, headers)).Result;
            return payResponse;
        }

        private static string GenerateToken(string subscriptionKey)
        {
            string url = "https://ericssonbasicapi2.azure-api.net/collection/token/";
            string body = "";

            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "Authorization", "Basic " + GetBase64String($"{ApiUser}:{ApiKey}") },
                { "Ocp-Apim-Subscription-Key", subscriptionKey }
            };

            string tokenJson = Task.Run(() => SendHttpPostRequest(url, body, headers)).Result;
            dynamic token = JsonConvert.DeserializeObject(tokenJson);
            return token.access_token.ToString();
        }

        private static string GetBase64String(string plainstring)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainstring));
        }

        private static async Task<string> SendHttpGetRequest(string url, Dictionary<string, string> headers)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Authorization", $"{headers["Authorization"]}");
            client.DefaultRequestHeaders.Add("X-Target-Environment", "sandbox");
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);

            var uri = "https://ericssonbasicapi2.azure-api.net/collection/v1_0/requesttopay/{referenceId}?" + queryString;

            var response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        private static async Task<string> SendHttpPostRequest(string url, string body, Dictionary<string, string> headers)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            client.DefaultRequestHeaders.Add("Authorization", headers["Authorization"]);
            headers.Remove("Authorization");

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(body);

            using (var content = new ByteArrayContent(byteData))
            {
                foreach (var key in headers.Keys)
                {
                    content.Headers.Add(key, headers[key]);
                }
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(url, content);
            }

            return await response?.Content?.ReadAsStringAsync();

        }

        private static string GenerateUUID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
