using System.Collections.Generic;
namespace TheFlightShop.Models
{
    public class ProductView
    {
        public string Description { get; set; }
        public IEnumerable<Part> Parts { get; set; }
    }
}
