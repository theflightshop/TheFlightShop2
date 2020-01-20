
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.Email
{
    public class BasicEmail
    {
        public void SendMail()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                new HttpBasicAuthenticator("api",
                                            "6f24297e0995057cb359dd2a02608afc-9dfbeecd-fb8e9520");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "sandbox57f57bb9bbe84da588461d609b8c5985.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "sandbox57f57bb9bbe84da588461d609b8c5985.mailgun.org/messages";
            request.AddParameter("from", "The Nick <mailgun@sandbox57f57bb9bbe84da588461d609b8c5985.mailgun.org>");
            request.AddParameter("to", "thatoneguynick@mailinator.com");
            request.AddParameter("subject", "Hello, World! ;)");
            request.AddParameter("text", "Testing some Mailgun awesomness! yeah buddy, rollin like a big shot. chevy tuned up like a nascar pitstop.");
            request.Method = Method.POST;
            client.ExecuteAsync(request, (response, handle) =>
            {
                int x = 0;
            });
        }
    }
}
