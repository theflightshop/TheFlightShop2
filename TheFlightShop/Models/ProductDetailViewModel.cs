using System;
using System.Collections.Generic;
namespace TheFlightShop.Models
{
    public class ProductDetailViewModel
    {
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }
        public string Category { get; set; }
        public string ProductCode { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string DrawingUrl { get; set; }
        public string ImageSource { get; set; }
        public int NumberOfInstallationExamples { get; set; }
        public string InstallationExamplesPath { get; set; }
        public IEnumerable<PartViewModel> Parts { get; set; }
    }
}
