using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheFlightShop.DAL.Schemas;

namespace TheFlightShop.Models
{
    public class SearchResult
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public SearchResult(Part part)
        {
            ProductId = part.ProductId;
            Name = part.PartNumber;
            Description = part.Description;
        }

        public SearchResult(Product product)
        {
            ProductId = product.Id;
            Name = product.Code;
            Description = product.ShortDescription;
        }
    }
}
