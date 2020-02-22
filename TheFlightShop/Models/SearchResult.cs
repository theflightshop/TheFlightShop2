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
        public string SubCategory { get; set; }
        public string ImgFilename { get; set; }

        public SearchResult(string name, Guid productId, string description, string category, string subCategory, string imgFilename)
        {
            ProductId = productId;
            Name = name;
            Description = description;
            Category = category;
            SubCategory = subCategory;
            ImgFilename = imgFilename;
        }
    }
}
