using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.Models
{
    public class MaintenanceSearchResult : SearchResult
    {
        public override bool IsCategoryHyperlinked => false;
        public override bool IsSubCategoryHyperlinked => false;

        public MaintenanceSearchResult(string name, string description, string imgFilename)
        {
            ProductId = null;
            Name = name;
            Description = description;
            Category = "Maintenance";
            SubCategory = null;
            ImgFilename = imgFilename;
        }
    }
}
