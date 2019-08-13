using System;
using System.Collections.Generic;

namespace TheFlightShop.Models
{
    public class ProductCategoryCount
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public IEnumerable<ProductSubCategoryCount> Counts { get; set; }
    }
}
