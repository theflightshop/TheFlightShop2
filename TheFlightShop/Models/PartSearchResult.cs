using System;

namespace TheFlightShop.Models
{
    public class PartSearchResult : SearchResult
    {
        public override bool IsCategoryHyperlinked => true;
        public override bool IsSubCategoryHyperlinked => true;

        public override string ControllerName => "Products";
        public override string ActionName => "ProductDetail";

        public PartSearchResult(string name, Guid productId, string description, string category, string subCategory, string imgFilename)
        {
            Id = productId;
            Name = name;
            Description = description;
            Category = category;
            SubCategory = subCategory;
            ImgFilename = imgFilename;
        }
    }
}
