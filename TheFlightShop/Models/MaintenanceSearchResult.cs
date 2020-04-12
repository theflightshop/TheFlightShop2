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

        public override string ControllerName { get; }
        public override string ActionName { get; }

        public MaintenanceSearchResult(string name, string description, string controller, string action, string imgFilename)
        {
            Id = null;
            Name = name;
            Description = description;
            Category = "Maintenance";
            SubCategory = null;
            ControllerName = controller;
            ActionName = action;
            ImgFilename = imgFilename;
        }
    }
}
