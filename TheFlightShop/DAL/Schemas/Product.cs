using System;
namespace TheFlightShop.DAL.Schemas
{
    public class Product
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public Guid SubCategoryId { get; set; }
        public string Code { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public bool MostPopular { get; set; }
        public int NumberOfInstallationExamples { get; set; }
        public string DrawingUrl { get; set; }
    }
}
