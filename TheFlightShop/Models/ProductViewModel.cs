using System;
namespace TheFlightShop.Models
{
    public class ProductViewModel
    {
        public Guid Id { get; set; }
        public string SubCategory { get; set; }
        public string Code { get; set; }
        public string ShortDescription { get; set; }
        public string ImageSource { get; set; }
        public bool IsMostPopular { get; set; }
        public bool HasPricing { get; set; }
    }
}
