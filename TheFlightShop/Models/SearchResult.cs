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
        public string Category { get; set; }
        public string ImgSrc { get; set; }

        public SearchResult(Part part, string category, string drawingUrl)
        {
            ProductId = part.ProductId;
            Name = part.PartNumber;
            Description = part.Description;
            Category = category;
            ImgSrc = drawingUrl;
        }

        public SearchResult(Product product, string category, string drawingUrl)
        {
            ProductId = product.Id;
            Name = product.Code;
            Description = product.ShortDescription;
            Category = category;
            ImgSrc = drawingUrl;
        }
    }
}
