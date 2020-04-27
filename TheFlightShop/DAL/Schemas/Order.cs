using System;

namespace TheFlightShop.DAL.Schemas
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
        public bool Completed { get; set; }
        public bool Shipped { get; set; }
        public DateTime DateCreated { get; set; }
        public short ShippingType { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string Notes { get; set; }
    }
}
