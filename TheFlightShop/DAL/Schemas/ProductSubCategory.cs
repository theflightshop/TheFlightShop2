using System;
namespace TheFlightShop.DAL.Schemas
{
    public class ProductSubCategory
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
    }
}
