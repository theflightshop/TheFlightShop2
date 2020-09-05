
using Microsoft.Extensions.Logging;
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
        private readonly string _adminAddress;
        private readonly ILogger _logger;

        public MailgunEmailClient(string apiKey, string fromUsername, string fromName, string domain, string adminAddress, ILogger logger)
        {
            _apiKey = apiKey;
            _fromUsername = fromUsername;
            _fromName = fromName;
            _domain = domain;
            _adminAddress = adminAddress;
            _logger = logger;
        }

        public async Task<bool> SendOrderConfirmation(ClientOrder order)
        {
            var succeeded = false;

            try
            {
                var clientBody = GetClientEmailBody(order, order.ConfirmationNumber);
                var adminBody = GetAdminEmailBody(order, order.ConfirmationNumber);
                var clientTask = SendEmail(order.Email, "Order Confirmation - The Flight Shop", clientBody);
                var adminTask = SendEmail(_adminAddress, $"Customer Order {order.ConfirmationNumber} - {order.Email}", adminBody);

                await Task.WhenAll(clientTask, adminTask);
                succeeded = clientTask.Result && adminTask.Result;
            }
            catch (Exception ex)
            {
                var hasApiKey = !string.IsNullOrEmpty(_apiKey);
                _logger.LogError(ex, $"method={nameof(MailgunEmailClient)}.{nameof(SendOrderConfirmation)},emailAdminUsername={_fromUsername},emailAdminAddress={_adminAddress},emailAdminDomain={_domain},hasApiKey={hasApiKey},confirmation#={order.ConfirmationNumber},customerEmail={order.Email}.");
            }

            return succeeded;
        }

        private async Task<bool> SendEmail(string toAddress, string subject, string body)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator = new HttpBasicAuthenticator("api", _apiKey);
            RestRequest request = new RestRequest();
            request.AddParameter("domain", _domain, ParameterType.UrlSegment);
            request.Resource = $"{_domain}/messages";
            request.AddParameter("from", $"{_fromName} <{_fromUsername}@{_domain}>");
            request.AddParameter("to", toAddress);
            request.AddParameter("subject", subject);
            request.AddParameter("html", body);
            request.AddParameter("text", "Your email provider does not support HTML messages. Please contact The Flight Shop to review your order.");
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
                    var hasApiKey = !string.IsNullOrEmpty(_apiKey);
                    _logger.LogWarning($"method={nameof(MailgunEmailClient)}.{nameof(SendEmail)} FAILED,responseStatus={response.StatusCode},responseContent={response.Content},emailAdminUsername={_fromUsername},emailAdminAddress={_adminAddress},emailAdminDomain={_domain},hasApiKey={hasApiKey},toAddress={toAddress}");
                    emailTaskCompletionSrc.SetResult(false);
                }
            });

            return await emailTaskCompletionSrc.Task;
        }

        private string GetOrderInformationMarkup(ClientOrder order, string confirmationNumber)
        {
            var orderLineItems = "";
            foreach (var orderLine in order.OrderLines)
            {
                orderLineItems += $"<tr><td style=\"border: 1px solid #ddd; text-align: center; padding: 0.25em 1em;\">{orderLine.PartNumber}</td>" + 
                    $"<td style=\"border: 1px solid #ddd; text-align: right; padding: 0.25em 1em;\">{orderLine.Quantity}</td></tr>";
            }

            var shipToNames = "";
            if (!string.IsNullOrWhiteSpace(order.CompanyName))
            {
                shipToNames += $"<span><strong>Company Name:</strong> {order.CompanyName}<span><br />";
            }
            if (!string.IsNullOrWhiteSpace(order.AttentionTo))
            {
                shipToNames += $"<span><strong>Attention To:</strong> {order.AttentionTo}<span><br />";
            }

            return $@"
<span><strong>Confirmation Number:&nbsp;</strong>{confirmationNumber}</span><br/>
<span><strong>Notes:&nbsp;</strong>{order.Notes ?? "(none)"}</span><br/>
<br/>
<span style=""font-size: 20px; font-weight: bold;"">Order Summary</span>
<table style=""margin-top: 0;"">
    <tr>
        <th style=""border: 1px solid #ddd; padding: 0.25em;"">Part #</th>
        <th style=""border: 1px solid #ddd; padding: 0.25em;"">Quantity</th>
    </tr>
{orderLineItems}
</table>
<br/>
<span style=""font-size: 20px; font-weight: bold;"">Shipping Information</span><br/>
{shipToNames}
<span><strong>Shipping Type:</strong>&nbsp;{GetShipTypeText(order.ShippingType, order.CustomShippingType)}</span><br/>
<span><strong>Address:</strong></span><br/>
{GetShippingAddressMarkup(order)}
<br/>";
        }

        private string GetClientEmailBody(ClientOrder order, string confirmationNumber)
        {
            var orderInfo = GetOrderInformationMarkup(order, confirmationNumber);

            return $@"
<div style=""font-family: 'sans-serif';"">
<span style=""font-size: 28px; font-weight: bold;"">Thank you for your order!</span><br/>
<br/>
<span>We will be in touch soon to review your order with you and receive your payment information. Additional fees may apply when the order is processed, including taxes and freight charges.</span><br/>
{orderInfo}
<br/>
<span>Sincerely,</span><br/><span>The Flight Shop</span><br/><br/>
<span><em>Please do not reply to this automated message. Emails sent to this address are not monitored.</em></span>
</div>
";
        }

        private string GetShipTypeText(int shipType, string otherShipType)
        {
            var text = $"{(ShippingType)shipType}";
            if (shipType == (int)ShippingType.Other)
            {
                text += $" - {otherShipType}";
            }
            return text;
        }

        private string GetShippingAddressMarkup(ClientOrder order)
        {
            string address;
            if (string.IsNullOrWhiteSpace(order.InternationalShippingAddress))
            {
                address = $@"
<span>{order.Address1}&nbsp;{order.Address2}</span><br/>
<span>{order.City}, {order.State} {order.Zip}</span><br/>";
            }
            else
            {
                address = order.InternationalShippingAddress.Replace("\n", "<br/>");
            }
            return address;
        }

        private string GetBillingAddressMarkup(ClientOrder order)
        {
            string address;
            if (string.IsNullOrWhiteSpace(order.InternationalBillingAddress))
            {
                address = $@"
<span>{order.BillingAddress1}&nbsp;{order.BillingAddress2}</span><br/>
<span>{order.BillingCity}, {order.BillingState} {order.BillingZip}</span><br/>";
            }
            else
            {
                address = order.InternationalBillingAddress.Replace("\n", "<br/>");
            }
            return address;
        }

        private string GetAdminEmailBody(ClientOrder order, string confirmationNumber)
        {
            var orderInfo = GetOrderInformationMarkup(order, confirmationNumber);
            string billingAddress = order.UseShippingAddressForBilling ? "<span>(same as shipping address)</span>" : GetBillingAddressMarkup(order);

            return $@"
<div style=""font-family: 'sans-serif';"">
<span style=""font-size: 20px; font-weight: bold;"">Customer Contact Information</span><br/>
<span><strong>Name:&nbsp;</strong>{order.FirstName}&nbsp;{order.LastName}</span><br/>
<span><strong>Email:&nbsp;</strong>{order.Email}</span><br/>
<span><strong>Phone:&nbsp;</strong>{order.Phone}</span><br/>
<span><strong>PO Number:&nbsp;</strong>{order.PurchaseOrderNumber ?? "(none)"}</span><br/>
{orderInfo}
<span style=""font-size: 20px; font-weight: bold;"">Billing Address</span><br/>
{billingAddress}
</div>
";
        }
    }
}
