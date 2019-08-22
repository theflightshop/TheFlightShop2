using System;
using System.Collections.Generic;

namespace TheFlightShop.Models
{
    public class ProductCategoryViewModel
    {
        public string CategoryName { get; set; }
        public IEnumerable<string> SubCategories { get; set; }
        public IEnumerable<ProductViewModel> Products { get; set; }
    }
}
