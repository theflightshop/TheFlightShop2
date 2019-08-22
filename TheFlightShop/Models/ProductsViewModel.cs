using System.Collections.Generic;
using TheFlightShop.DAL.Schemas;

namespace TheFlightShop.Models
{
    public class ProductsViewModel
    {
        public IEnumerable<ProductCategory> Categories { get; set; }
    }
}
