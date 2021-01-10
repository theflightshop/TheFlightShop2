using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TheFlightShop.Payment;

namespace TheFlightShop.Models
{
    public class ClientOrder
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string AttentionTo { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string CountryCode { get; set; }
        [JsonIgnore]
        public string Country => TheFlightShop.Country.CODES.First(kvp => kvp.Value == CountryCode).Key;
        public string BillingCompanyName { get; set; }
        public string BillingAddress1 { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZip { get; set; }
        public string BillingCountryCode { get; set; }
        [JsonIgnore]
        public string BillingCountry => BillingCountryCode == null ? null : TheFlightShop.Country.CODES.First(kvp => kvp.Value == BillingCountryCode).Key;
        public int ShippingType { get; set; }
        public PaymentType PaymentType { get; set; }
        public string CustomShippingType { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string Notes { get; set; }
        public IEnumerable<ClientOrderLine> OrderLines { get; set; }
        public string ConfirmationNumber { get; set; }

        [JsonIgnore]
        public bool UseShippingAddressForBilling => BillingAddress1 == Address1 && BillingCity == City && BillingState == State && BillingZip == Zip;

        private const int CONF_NR_RANDOM_CHARS_LENGTH = 4;
        
        public static ClientOrder FromNmiGatewayResponse(NmiGatewayResponse gatewayResponse)
        {
            var orderLines = gatewayResponse.LineItems.Select(lineItem => new ClientOrderLine
            {
                PartNumber = lineItem.PartNumber,
                ProductId = lineItem.ProductId,
                Description = lineItem.Description,
                Quantity = (int)lineItem.Quantity,
                Price = lineItem.UnitCost == 0 ? (decimal?)null : lineItem.UnitCost
            });
            return new ClientOrder
            {
                ConfirmationNumber = gatewayResponse.TransactionId,
                FirstName = gatewayResponse.ShippingAddress.FirstName,
                LastName = gatewayResponse.ShippingAddress.LastName,
                Phone = gatewayResponse.ShippingAddress.PhoneNumber,
                Email = gatewayResponse.ShippingAddress.EmailAddress,
                AttentionTo = gatewayResponse.AttentionTo,
                PurchaseOrderNumber = gatewayResponse.PurchaseOrderNumber,
                ShippingType = Models.ShippingType.FromName(gatewayResponse.ShippingType).Value,
                CustomShippingType  = gatewayResponse.CustomShippingType,
                PaymentType = PaymentType.CreditCard, // NOTE: as of this writing (Jan 10, 20201), only orders with CreditCard payment type are sent to NMI for validation/pre-auth
                Notes = gatewayResponse.Notes,
                CompanyName = gatewayResponse.ShippingAddress.CompanyName,
                Address1 = gatewayResponse.ShippingAddress.Address1,
                Address2 = gatewayResponse.ShippingAddress.Address2,
                City = gatewayResponse.ShippingAddress.City,
                State = gatewayResponse.ShippingAddress.State,
                Zip = gatewayResponse.ShippingAddress.PostalCode,
                CountryCode = gatewayResponse.ShippingAddress.CountryCode,
                BillingCompanyName = gatewayResponse.BillingAddress.CompanyName,
                BillingAddress1 = gatewayResponse.BillingAddress.Address1,
                BillingAddress2 = gatewayResponse.BillingAddress.Address2,
                BillingCity = gatewayResponse.BillingAddress.City,
                BillingState = gatewayResponse.BillingAddress.State,
                BillingZip = gatewayResponse.BillingAddress.PostalCode,
                BillingCountryCode = gatewayResponse.BillingAddress.CountryCode,
                OrderLines = orderLines
            };
        }

        public void GenerateConfirmationNumber()
        {
            var possibleCharacters = new char[]
            {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 
                'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '2', '3', '4', '5', '6', '7', '8', '9'
            };
            var now = DateTime.UtcNow;

            var confirmationNumber = now.Year.ToString() + possibleCharacters[now.Month].ToString() + possibleCharacters[now.Day].ToString() + 
                possibleCharacters[now.Hour].ToString() + now.Minute.ToString("00") + now.Second.ToString("00");

            var random = new Random();
            for (var i = 0; i < CONF_NR_RANDOM_CHARS_LENGTH; i++)
            {
                var nextRandomChar = possibleCharacters[random.Next(possibleCharacters.Length)];
                confirmationNumber += nextRandomChar;
            }

            ConfirmationNumber = confirmationNumber;
        }
    }
}
