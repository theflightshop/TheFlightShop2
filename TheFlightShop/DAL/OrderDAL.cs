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

        //todo:
        //public async Task<ClientOrder> GetClientOrderByConfirmationNumber(string confirmationNumber)
        //{
        //    try
        //    {
        //        ClientOrder result = null;
        //        using (var db = new OrderContext(_connectionString))
        //        {
        //            var matchingOrders = await db.Orders.Where(order => order.ConfirmationNumber == confirmationNumber.Trim()).ToArrayAsync();
        //            if (!matchingOrders.Any())
        //            {
        //                _logger.LogWarning($"method={nameof(OrderDAL)}.{nameof(GetClientOrderByConfirmationNumber)}- no order found by confirmation # {confirmationNumber}.");
        //            }
        //            else if (matchingOrders.Length > 1)
        //            {
        //                var orderIds = string.Join(',', matchingOrders.Select(order => order.Id));
        //                _logger.LogWarning($"method={nameof(OrderDAL)}.{nameof(GetClientOrderByConfirmationNumber)}- multiple orders found by confirmation # {confirmationNumber}. orderIds={orderIds}.");
        //            }
        //            else
        //            {
        //                var order = matchingOrders.First();
        //                var contactTask = db.Contacts.FindAsync(order.ContactId);
        //                var orderLinesTask = db.OrderLines.Where(line => line.OrderId == order.Id).ToArrayAsync();
        //                await Task.WhenAll(contactTask, orderLinesTask);
        //                var contact = contactTask.Result;
        //                result = new ClientOrder
        //                {
        //                    ConfirmationNumber = order.ConfirmationNumber,
        //                    AttentionTo = order.AttentionTo,
        //                    PurchaseOrderNumber = order.PurchaseOrderNumber,
        //                    CustomShippingType = order.CustomShippingType,
        //                    ShippingType = order.ShippingType,
        //                    Notes = order.Notes,
        //                    OrderLines = orderLinesTask.Result.Select(line => new ClientOrderLine
        //                    {
        //                        PartNumber = line.PartNumber,
        //                        ProductId = line.ProductId,
        //                        Quantity = (int)line.Quantity
        //                    }),
        //                    FirstName = contact.FirstName,
        //                    LastName = contact.LastName,
        //                    CompanyName = contact.CompanyName,
        //                    Email = contact.Email,
        //                    Phone = contact.Phone,
        //                    Address1 = contact.Address1,
        //                    Address2 = contact.Address2,
        //                    City = contact.City,
        //                    State = contact.State,
        //                    Zip = contact.Zip,
        //                    CountryCode = contact.CountryCode,
        //                    BillingAddress1 = contact.BillingAddress1,
        //                    BillingAddress2 = contact.BillingAddress2,
        //                    BillingCity = contact.BillingCity,
        //                    BillingState = contact.BillingState,
        //                    BillingZip = contact.BillingZip,
        //                    BillingCountryCode = contact.BillingCountryCode
        //                };
        //            }
        //        }
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"method={nameof(OrderDAL)}.{nameof(GetClientOrderByConfirmationNumber)}- error retrieving order by confirmation # {confirmationNumber}.");
        //        throw;
        //    }
        //}

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

            // todo: save new ea. order for auditing? and then have user w/ contactId //--- var contactSaved =GetExistingContact(contactQuery);

            using (var db = new OrderContext(_connectionString))
            {
                db.Contacts.Add(contact);
                await db.SaveChangesAsync();
            }

            return contact.Id;
        }
    }
}
