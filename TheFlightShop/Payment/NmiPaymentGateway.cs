using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TheFlightShop.DAL.Schemas;
using TheFlightShop.Models;

namespace TheFlightShop.Payment
{
    public class NmiPaymentGateway
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger _logger;

        public NmiPaymentGateway(IHttpClientFactory clientFactory, string gatewayUrl, string apiKey, ILogger logger) 
        {
            _apiKey = apiKey;
            _logger = logger;
            _httpClient = clientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(gatewayUrl);
        }

        public async Task<PaymentGatewayFormUrlResult> RetrievePaymentAuthUrl(ClientOrder order, IEnumerable<Part> parts, string redirectUrl)
        {
            // TODO: see if this returns errors for invalid addresses??
            var formUrlResult = new PaymentGatewayFormUrlResult();

            try
            {
                var xmlBody = SerializeOrderXml(order, parts, redirectUrl);
                var content = new StringContent(xmlBody, Encoding.UTF8, "text/xml");
                var response = await _httpClient.PostAsync(string.Empty, content);

                var formUrlResponse = await ParseGatewayResponse(response);
                if (Enum.TryParse(formUrlResponse.Result, out NmiGatewayResponseStatus responseStatus))
                {
                    if (responseStatus == NmiGatewayResponseStatus.Approved)
                    {
                        // TODO: on fail set CanRetry and/or ErrorReason

                        if (string.IsNullOrWhiteSpace(formUrlResponse.PaymentAuthFormUrl))
                        {
                            // todo: log warn couldn't parse NMI payment gateway form URL for method, print response STRING
                        }
                        else
                        {
                            formUrlResult.Succeeded = true;
                            formUrlResult.PaymentAuthFormUrl = formUrlResponse.PaymentAuthFormUrl;
                        }
                    }
                    else
                    {
                        // todo: gracefully handle response failure using result text/code
                    }
                }
                else
                {
                    // todo: log warn couldn't parse NMI payment gateway response for method, print response STRING
                }
                        
                    // todo: httpclientfactory send text/xml message
                    // process response if success or not
                    // NOTE: if payment auth fails, show specific error if possible
                    // add review screen (don't post CC info ;)), allow to edit 
                    // submit to NMI from there
                    // redirect to action to submit token/auth
                    // handle errors, should return JSON response 
                    // allow to edit if errors
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $""); // todo: fill this out
            }

            return formUrlResult;
        }

        private async Task<NmiGatewayResponse> ParseGatewayResponse(HttpResponseMessage responseMessage)
        {
            var gatewayResponse = new NmiGatewayResponse();
            using (var responseStream = await responseMessage.Content.ReadAsStreamAsync())
            {
                try
                {
                    var responseSerializer = new XmlSerializer(typeof(NmiGatewayResponse));
                    gatewayResponse = (NmiGatewayResponse)responseSerializer.Deserialize(responseStream); 
                }
                catch (Exception ex)
                {
                    responseStream.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(responseStream))
                    {
                        var responseBody = reader.ReadToEnd();
                        _logger.LogError(ex, $"{nameof(NmiPaymentGateway)}.{nameof(ParseGatewayResponse)}-error parsing response from NMI. responseBody={responseBody}");
                        throw;
                    }
                }
            }
            return gatewayResponse;
    }

        private string SerializeOrderXml(ClientOrder order, IEnumerable<Part> parts, string redirectUrl)
        {
            NmiAddress billingAddress;
            if (order.UseShippingAddressForBilling)
            {
                billingAddress = new NmiAddress
                {
                    Address1 = order.Address1,
                    Address2 = order.Address2,
                    City = order.City,
                    State = order.State,
                    PostalCode = order.Zip,
                    CountryCode = order.CountryCode
                };
            }
            else
            {
                billingAddress = new NmiAddress
                {
                    Address1 = order.BillingAddress1,
                    Address2 = order.BillingAddress2,
                    City = order.BillingCity,
                    State = order.BillingState,
                    PostalCode = order.BillingZip,
                    CountryCode = order.BillingCountryCode
                };
            }
            billingAddress.FirstName = order.FirstName;
            billingAddress.LastName = order.LastName;
            billingAddress.CompanyName = order.CompanyName;

            var amountToAuthorize = order.OrderLines.Sum(line => GetLineAmount(line, parts));
            var customerInfo = new NmiCustomerInfo
            {
                Amount = amountToAuthorize,
                ApiKey = _apiKey,
                RedirectUrl = redirectUrl,
                BillingAddress = billingAddress
            };

            var rootElementName = amountToAuthorize > 0.00m ? "auth" : "validate";
            var requestSerializer = new XmlSerializer(typeof(NmiCustomerInfo), new XmlRootAttribute(rootElementName));
            var emptyNamespaces = new XmlSerializerNamespaces();
            emptyNamespaces.Add(string.Empty, string.Empty);

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, bufferSize: 1024, leaveOpen: true))
                {
                    requestSerializer.Serialize(streamWriter, customerInfo, emptyNamespaces);
                    stream.Position = 0;
                }
                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        private decimal GetLineAmount(ClientOrderLine line, IEnumerable<Part> parts)
        {
            var price = parts.FirstOrDefault(part => line.PartNumber == part.PartNumber)?.Price ?? 0;
            return price * line.Quantity;
        }
    }
}
