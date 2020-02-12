using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.DAL.Schemas
{
    public class OrderLine
    {        
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? PartId { get; set; }
        public string PartNumber { get; set; }
        public decimal Quantity { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
