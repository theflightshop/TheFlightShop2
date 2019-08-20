using System;
using System.Collections.Generic;
namespace TheFlightShop.Models
{
    public class ProductView
    {
        public Guid CategoryId { get; set; }
        public string Category { get; set; }
        public string ProductCode { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string ImageSource { get; set; }
        public string DrawingUrl { get; set; }
        public string ComparisonTableUrl { get; set; }
        public IEnumerable<Part> Parts { get; set; }
    }
}
