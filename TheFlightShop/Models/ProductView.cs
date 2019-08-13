using System;
using System.Collections.Generic;
namespace TheFlightShop.Models
{
    public class ProductView
    {
        public Guid CategoryId { get; set; }
        public string Category { get; set; }
        public Guid SubCategoryId { get; set; }
        public string SubCategory { get; set; }
        public string ProductCode { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public IEnumerable<Part> Parts { get; set; }
    }
}
