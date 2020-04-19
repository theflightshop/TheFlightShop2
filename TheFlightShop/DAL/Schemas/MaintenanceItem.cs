using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.DAL.Schemas
{
    public class MaintenanceItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string ImageFilename { get; set; }
        public string Keywords { get; set; }
        public bool IsActive { get; set; }
    }
}
