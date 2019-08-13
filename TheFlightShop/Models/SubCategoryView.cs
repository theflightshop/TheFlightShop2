using System;
using System.Collections.Generic;

namespace TheFlightShop.Models
{
    public class SubCategoryView
    {
        public Guid CategoryId { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
