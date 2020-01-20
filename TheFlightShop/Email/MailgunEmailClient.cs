
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
    public class MailgunEmailClient : IEmailClient
    {
        private readonly string _apiKey;
        private readonly string _fromUsername;
        private readonly string _fromName;
        private readonly string _domain;

        public MailgunEmailClient(string apiKey, string fromUsername, string fromName, string domain)
        {
            _apiKey = apiKey;
            _fromUsername = fromUsername;
            _fromName = fromName;
            _domain = domain;
        }

        public async Task<bool> SendOrderConfirmation(ClientOrder order)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator = new HttpBasicAuthenticator("api", _apiKey);
            RestRequest request = new RestRequest();
            request.AddParameter("domain", _domain, ParameterType.UrlSegment);
            request.Resource = $"{_domain}/messages";
            request.AddParameter("from", $"{_fromName} <{_fromUsername}@{_domain}>");
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
