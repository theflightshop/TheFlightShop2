
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TheFlightShop.Models;

namespace TheFlightShop.Email
{
    public class BasicEmail
    {
        private string _apiKey;

        public BasicEmail(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<bool> SendMail(ClientOrder order)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator = new HttpBasicAuthenticator("api", _apiKey);
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "sandbox57f57bb9bbe84da588461d609b8c5985.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "sandbox57f57bb9bbe84da588461d609b8c5985.mailgun.org/messages";
            request.AddParameter("from", "The Flight Shop <mailgun@sandbox57f57bb9bbe84da588461d609b8c5985.mailgun.org>");
            request.AddParameter("to", order.Email);
            request.AddParameter("subject", "Order Confirmation");
            request.AddParameter("text", "Testing some Mailgun awesomness! yeah buddy, rollin like a big shot. chevy tuned up like a nascar pitstop.");
            request.Method = Method.POST;

            var emailTaskCompletionSrc = new TaskCompletionSource<bool>();
            var handle = client.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    emailTaskCompletionSrc.SetResult(true);
                }
                else
                {
                    // TODO: add logging
                    emailTaskCompletionSrc.SetResult(false);
                }
            });

            return await emailTaskCompletionSrc.Task;
        }
    }
}
