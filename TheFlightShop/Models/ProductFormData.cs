using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.Models
{
    public class ProductFormData
    {
        public string Code { get; set; }
        public IFormFile Image { get; set; }
        public IFormFile Drawing { get; set; }
    }
}
