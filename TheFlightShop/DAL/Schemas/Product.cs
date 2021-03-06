using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheFlightShop.DAL.Schemas
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public Guid CategoryId { get; set; }
        public Guid SubCategoryId { get; set; }
        public string Code { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public bool MostPopular { get; set; }
        public int NumberOfInstallationExamples { get; set; }
        public string ImageFilename { get; set; }
        public string DrawingFilename { get; set; }

        public IEnumerable<Part> Parts { get; set; }

        public Product()
        {
            Parts = new List<Part>();
        }
    }
}
