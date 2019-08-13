using System;
namespace TheFlightShop.Models
{
    public class ProductSubCategoryCount
    {
        public Guid SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public int Count { get; set; }
    }
}
