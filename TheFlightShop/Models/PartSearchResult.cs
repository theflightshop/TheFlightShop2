using System;

namespace TheFlightShop.Models
{
    public class PartSearchResult : SearchResult
    {
        public override bool IsCategoryHyperlinked => true;
        public override bool IsSubCategoryHyperlinked => true;

        public PartSearchResult(string name, Guid productId, string description, string category, string subCategory, string imgFilename)
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
