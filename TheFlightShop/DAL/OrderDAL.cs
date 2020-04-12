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
                var contactId = GetOrSaveContact(clientOrder);
                var orderId = CreateOrder(contactId);
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

        private Guid CreateOrder(Guid contactId)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                ContactId = contactId,
                DateCreated = DateTime.UtcNow
            };
            Orders.Add(order);
            SaveChanges();

            return order.Id;
        }

        private Guid GetOrSaveContact(ClientOrder clientOrder)
        {
            var formattedPhone = Regex.Replace(clientOrder.Phone?.Trim() ?? "", @"\D", "");
            var contactQuery = new Contact
            {
                FirstName = clientOrder.FirstName?.ToLower().Trim(),
                LastName = clientOrder.LastName?.ToLower().Trim(),
                Email = clientOrder.Email?.ToLower().Trim(),
                Phone = formattedPhone,
                Address1 = clientOrder.Address1?.Trim(),
                Address2 = clientOrder.Address2?.Trim(),
                City = clientOrder.City?.ToLower().Trim(),
                State = clientOrder.State?.ToUpper().Trim(),
                Zip = clientOrder.Zip?.ToLower().Trim()
            };

            var contactSaved = GetExistingContact(contactQuery);
            if (contactSaved == null)
            {
                contactSaved = contactQuery;
                contactSaved.Id = Guid.NewGuid();
                contactSaved.DateCreated = DateTime.UtcNow;

                Contacts.Add(contactSaved);
                SaveChanges();
            }

            return contactSaved.Id;
        }

        private Contact GetExistingContact(Contact contactQuery)
        {
            return Contacts.FirstOrDefault(contact =>
                contact.FirstName == contactQuery.FirstName &&
                contact.LastName == contactQuery.LastName &&
                contact.Email == contactQuery.Email &&
                contact.Phone == contactQuery.Phone &&
                (
                    contact.Address1 == null || contactQuery.Address1 == null ||
                    contact.Address1.ToLower() == contactQuery.Address1.ToLower()
                ) &&
                (
                    contact.Address2 == null || contactQuery.Address2 == null || 
                    contact.Address2.ToLower() == contactQuery.Address2.ToLower()
                ) &&
                contact.City == contactQuery.City &&
                contact.State == contactQuery.State &&
                contact.Zip == contactQuery.Zip);
        }
    }
}
