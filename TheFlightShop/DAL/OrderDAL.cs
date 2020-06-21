using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheFlightShop.DAL.Schemas;
using TheFlightShop.Models;

namespace TheFlightShop.DAL
{
    public class OrderDAL : DbContext, IOrderDAL
    {
        public DbSet<Order>  Orders { get; set; }        
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        
        private readonly string _connectionString;

        public OrderDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connectionString);
        }

        public bool SaveNewOrder(ClientOrder clientOrder, IEnumerable<Part> parts)
        {
            bool succeeded = false;

            try
            {
                var contactId = SaveContact(clientOrder);
                var orderId = CreateOrder(contactId, clientOrder);
                SaveOrderLines(orderId, clientOrder.OrderLines, parts);

                succeeded = true;
            }
            catch (Exception ex)
            {
                // todo: logging 
            }

            return succeeded;
        }

        private void SaveOrderLines(Guid orderId, IEnumerable<ClientOrderLine> orderLines, IEnumerable<Part> parts)
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
                OrderLines.Add(orderLineSaved);
            }
            SaveChanges();
        }

        private Guid CreateOrder(Guid contactId, ClientOrder clientOrder)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                ContactId = contactId,
                DateCreated = DateTime.UtcNow,
                ShippingType = (short)clientOrder.ShippingType,
                PurchaseOrderNumber = clientOrder.PurchaseOrderNumber,
                Notes = clientOrder.Notes,
                AttentionTo = clientOrder.AttentionTo,
                CustomShippingType = clientOrder.CustomShippingType
            };
            Orders.Add(order);
            SaveChanges();

            return order.Id;
        }

        private Guid SaveContact(ClientOrder clientOrder)
        {
            var formattedPhone = Regex.Replace(clientOrder.Phone?.Trim() ?? "", @"\D", "");
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                FirstName = clientOrder.FirstName?.Trim(),
                LastName = clientOrder.LastName?.Trim(),
                Email = clientOrder.Email?.ToLower().Trim(),
                Phone = formattedPhone,
                Address1 = clientOrder.Address1?.Trim(),
                Address2 = clientOrder.Address2?.Trim(),
                City = clientOrder.City?.ToLower().Trim(),
                State = clientOrder.State?.ToUpper().Trim(),
                Zip = clientOrder.Zip?.ToLower().Trim(),
                InternationalShippingAddress = clientOrder.InternationalShippingAddress,
                BillingAddress1 = clientOrder.BillingAddress1?.Trim(),
                BillingAddress2 = clientOrder.BillingAddress2?.Trim(),
                BillingCity = clientOrder.BillingCity?.ToLower().Trim(),
                BillingState = clientOrder.BillingState?.ToUpper().Trim(),
                BillingZip = clientOrder.BillingZip?.ToLower().Trim(),
                CompanyName = clientOrder.CompanyName?.Trim(),
                InternationalBillingAddress = clientOrder.InternationalBillingAddress,
                DateCreated = DateTime.UtcNow
            };

            // todo: save new ea. order for auditing? and then have user w/ contactId //--- var contactSaved =GetExistingContact(contactQuery);

            Contacts.Add(contact);
            SaveChanges();

            return contact.Id;
        }
    }
}
