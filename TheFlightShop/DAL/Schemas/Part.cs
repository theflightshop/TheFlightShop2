using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheFlightShop.DAL.Schemas
{
    public class Part
    {
        public Guid Id { get; set; }
        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
