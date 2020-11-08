using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheFlightShop.DAL.Schemas;
using TheFlightShop.Models;

namespace TheFlightShop.DAL
{
    public class OrderDAL : IOrderDAL
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public OrderDAL(string connectionString, ILogger logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }
        
        public async Task<bool> SaveNewOrder(ClientOrder clientOrder, IEnumerable<Part> parts)
        {
            bool succeeded = false;

            try
            {
                var stopwatch = Stopwatch.StartNew();
                var contactId = await SaveContact(clientOrder);
                var orderId = await CreateOrder(contactId, clientOrder);
                await SaveOrderLines(orderId, clientOrder.OrderLines, parts);
                stopwatch.Stop();
                _logger.LogInformation($"{nameof(OrderDAL)}.{nameof(SaveNewOrder)}-{clientOrder.OrderLines?.Count() ?? 0} orderlines in {stopwatch.ElapsedMilliseconds}ms");

                succeeded = true;
            }
            catch (Exception ex)
            {
                var error = ex.InnerException ?? ex;
                _logger.LogError(error, $"method={nameof(OrderDAL)}.{nameof(SaveNewOrder)},confirmation#={clientOrder.ConfirmationNumber},customerEmail={clientOrder.Email}.");
            }

            return succeeded;
        }

        private async Task SaveOrderLines(Guid orderId, IEnumerable<ClientOrderLine> orderLines, IEnumerable<Part> parts)
        {
            using (var db = new OrderContext(_connectionString))
            {
                foreach (var orderLine in orderLines)
                {
                    var existingPart = parts.FirstOrDefault(part => part.PartNumber.ToLower() == orderLine.PartNumber.ToLower());
                    var orderLineSaved = new OrderLine
                    {
                        Id = Guid.NewGuid(),
                        OrderId = orderId,
                        ProductId = orderLine.ProductId,
                        PartId = existingPart?.Id,
                        PartNumber = orderLine.PartNumber,
                        Quantity = orderLine.Quantity,
                        DateCreated = DateTime.UtcNow
                    };
                    db.OrderLines.Add(orderLineSaved);
                }
                await db.SaveChangesAsync();
            }
        }

        private async Task<Guid> CreateOrder(Guid contactId, ClientOrder clientOrder)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                ContactId = contactId,
                ConfirmationNumber = clientOrder.ConfirmationNumber,
                DateCreated = DateTime.UtcNow,
                ShippingType = (short)clientOrder.ShippingType,
                PaymentType = (short)clientOrder.PaymentType,
                PurchaseOrderNumber = clientOrder.PurchaseOrderNumber,
                Notes = clientOrder.Notes,
                AttentionTo = clientOrder.AttentionTo,
                CustomShippingType = clientOrder.CustomShippingType
            };
            
            using (var db = new OrderContext(_connectionString))
            {
                db.Orders.Add(order);
                await db.SaveChangesAsync();
            }

            return order.Id;
        }

        private async Task<Guid> SaveContact(ClientOrder clientOrder)
        {
            var formattedPhone = Regex.Replace(clientOrder.Phone?.Trim() ?? "", @"\D", "");
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                FirstName = clientOrder.FirstName,
                LastName = clientOrder.LastName,
                Email = clientOrder.Email,
                Phone = formattedPhone,
                Address1 = clientOrder.Address1,
                Address2 = clientOrder.Address2,
                City = clientOrder.City,
                State = clientOrder.State,
                Zip = clientOrder.Zip,
                CountryCode = clientOrder.CountryCode,
                BillingAddress1 = clientOrder.BillingAddress1,
                BillingAddress2 = clientOrder.BillingAddress2,
                BillingCity = clientOrder.BillingCity,
                BillingState = clientOrder.BillingState,
                BillingZip = clientOrder.BillingZip,
                BillingCountryCode = clientOrder.BillingCountryCode,
                CompanyName = clientOrder.CompanyName,
                DateCreated = DateTime.UtcNow
            };

            using (var db = new OrderContext(_connectionString))
            {
                db.Contacts.Add(contact);
                await db.SaveChangesAsync();
            }

            return contact.Id;
        }
    }
}
