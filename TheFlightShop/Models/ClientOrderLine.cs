using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.Models
{
    public class ClientOrderLine
    {
        public string PartNumber { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
