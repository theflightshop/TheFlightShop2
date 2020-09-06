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

        public async Task<NmiGatewayResponse> AuthorizeOrValidatePaymentAmount(string tokenId)
        {
            var authResponse = new NmiGatewayResponse();

            try
            {
                var paymentAuth = new NmiPaymentAuth
                {
                    ApiKey = _apiKey,
                    TokenId = tokenId
                };
                var xmlBody = SerializeXml(paymentAuth);
                authResponse = await PostXml(xmlBody);
                LogGatewayResult(authResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(NmiPaymentGateway)}.{nameof(AuthorizeOrValidatePaymentAmount)}- error validating token {tokenId}.");
            }

            return authResponse;
        }

        public async Task<NmiGatewayResponse> RetrievePaymentAuthUrl(ClientOrder order, IEnumerable<Part> parts, string redirectUrl)
        {
            // TODO: see if this returns errors for invalid addresses??
            var response = new NmiGatewayResponse();

            try
            {
                var xmlBody = SerializeOrderXml(order, parts, redirectUrl);
                response = await PostXml(xmlBody); 
                LogGatewayResult(response);

                if (response.Succeeded && string.IsNullOrWhiteSpace(response.PaymentAuthFormUrl))
                {
                    response = new NmiGatewayResponse();
                    _logger.LogWarning($"{nameof(NmiPaymentGateway)}.{nameof(RetrievePaymentAuthUrl)}- couldn't parse NMI gateway form URL, but response was successful. " + 
                        $"status={response.Result},code={response.ResultCode},resultText={response.ResultText},transactionId={response.TransactionId}");
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(NmiPaymentGateway)}.{nameof(RetrievePaymentAuthUrl)}- error for order {order?.ConfirmationNumber}."); 
            }

            return response;
        }

        private void LogGatewayResult(NmiGatewayResponse response)
        {
            var responseStatus = response.GetResponseStatus();

            if (responseStatus == null)
            {
                _logger.LogWarning($"{nameof(NmiPaymentGateway)}.{nameof(LogGatewayResult)}- couldn't parse NMI gateway response status. status={response.Result},code={response.ResultCode},resultText={response.ResultText},transactionId={response.TransactionId}");
            }
            else if (responseStatus == NmiGatewayResponseStatus.Approved)
            {
                _logger.LogInformation($"{nameof(NmiPaymentGateway)}.{nameof(LogGatewayResult)}- customer submission approved. transactionId={response.TransactionId},confirmationNumber={response.ConfirmationNumber ?? "unknown/unavailable"}");
            }
            else if (responseStatus == NmiGatewayResponseStatus.Declined)
            {
                _logger.LogInformation($"{nameof(NmiPaymentGateway)}.{nameof(LogGatewayResult)}- customer card declined. transactionId={response.TransactionId},confirmationNumber={response.ConfirmationNumber ?? "unknown/unavailable"}");
            }
            else // error
            {
                _logger.LogWarning($"{nameof(NmiPaymentGateway)}.{nameof(LogGatewayResult)}- error processing customer submission. status={response.Result},code={response.ResultCode},resultText={response.ResultText},transactionId={response.TransactionId},confirmationNumber={response.ConfirmationNumber ?? "unknown/unavailable"}");
            }
        }

        private async Task<NmiGatewayResponse> PostXml(string xmlBody)
        {
            var content = new StringContent(xmlBody, Encoding.UTF8, "text/xml");
            var response = await _httpClient.PostAsync(string.Empty, content);
            return await ParseGatewayResponse(response);
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

            var lineItems = order.OrderLines.Select(line => new NmiLineItem
            {
                PartNumber = line.PartNumber,
                ProductId = line.ProductId.ToString(),
                Quantity = line.Quantity
            }).ToArray();

            var amountToAuthorize = order.OrderLines.Sum(line => GetLineAmount(line, parts));
            var customerInfo = new NmiCustomerInfo
            {
                Amount = amountToAuthorize,
                ApiKey = _apiKey,
                RedirectUrl = redirectUrl,
                ConfirmationNumber = order.ConfirmationNumber,
                AttentionTo = order.AttentionTo,
                ShippingType = order.ShippingType.ToString(),
                CustomShippingType = order.ShippingType == (int)ShippingType.Other ? order.CustomShippingType : null,
                PurchaseOrderNumber = order.PurchaseOrderNumber,
                Notes = order.Notes,
                LineItems = lineItems,
                BillingAddress = billingAddress,
                ShippingAddress = new NmiAddress
                {
                    FirstName = order.FirstName,
                    LastName = order.LastName,
                    CompanyName = order.CompanyName,
                    EmailAddress = order.Email,
                    PhoneNumber = order.Phone,
                    Address1 = order.Address1,
                    Address2 = order.Address2,
                    City = order.City,
                    State = order.State,
                    PostalCode = order.Zip,
                    CountryCode = order.CountryCode
                }
            };

            var rootElementName = amountToAuthorize > 0.00m ? "auth" : "validate";
            return SerializeXml(customerInfo, rootElementName);
        }

        private string SerializeXml<T>(T obj, string rootElementName = null)
        {
            var requestSerializer = rootElementName == null ? new XmlSerializer(typeof(T)) : new XmlSerializer(typeof(T), new XmlRootAttribute(rootElementName));
            var emptyNamespaces = new XmlSerializerNamespaces();
            emptyNamespaces.Add(string.Empty, string.Empty);

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, bufferSize: 1024, leaveOpen: true))
                {
                    requestSerializer.Serialize(streamWriter, obj, emptyNamespaces);
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
