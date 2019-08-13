using System;
namespace TheFlightShop.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string ShortDescription { get; set; }
        public bool IsMostPopular { get; set; }
        public bool HasPricing { get; set; }
    }
}
