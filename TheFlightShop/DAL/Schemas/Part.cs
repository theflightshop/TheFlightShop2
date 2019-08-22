using System;
namespace TheFlightShop.DAL.Schemas
{
    public class Part
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
