using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.DAL.Schemas
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
        public bool Completed { get; set; }
        public bool Shipped { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
