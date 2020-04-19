using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheFlightShop.DAL.Schemas;

namespace TheFlightShop.Models
{
    public abstract class SearchResult
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string ImgFilename { get; set; }
        public abstract string ControllerName { get; }
        public abstract string ActionName { get; }
    }
}
