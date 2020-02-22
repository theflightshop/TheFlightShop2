using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TheFlightShop.DAL.Schemas;
using TheFlightShop.Models;

namespace TheFlightShop.DAL
{
    public class ProductDAL : DbContext, IProductDAL
    {
        private const string NON_WORD_CHARACTER_PATTERN = @"[\W]+";
        private const string NUMBER_PATTERN = @"[\d]+";

        private class ProductCategoryMapping
        {
            public int ProductId { get; set; }
            public int CategoryId { get; set; }
            public bool IsActive { get; set; }
            public string CategoryName { get; set; }
            public string Code { get; set; }
            public string ShortDescription { get; set; }
            public decimal Price { get; set; }
        }

        private class CategoryMapping
        {
            public int uid { get; set; }
            public bool IsActive { get; set; }
            public int ParentID { get; set; }
            public string Name { get; set; }
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Part> Parts { get; set; }

        private readonly string _connectionString;

        public ProductDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connectionString);
        }        

        public void InitializeFrom()
        {
            if (!(Products.Any() || Parts.Any() || Categories.Any()))
            {
                //var productsPath = Path.Combine("product-static-files", "json", "dbexport-oct-19", "productInfo.json");
                //var productListJson = File.ReadAllText(productsPath);
                var productListJson = @"
[
  {
    ""ProductId"": 5,
    ""IsActive"": 1,
    ""CategoryId"": 50,
    ""CategoryName"": ""Adhesive Dispensers"",
    ""Code"": ""CB100-81"",
    ""ShortDescription"": ""Manual dispenser (fits CB200-40 cartridge)"",
    ""Price"": ""49.5""
  },
  {
    ""ProductId"": 6,
    ""IsActive"": 1,
    ""CategoryId"": 50,
    ""CategoryName"": ""Adhesive Dispensers"",
    ""Code"": ""CB100-21"",
    ""ShortDescription"": ""Manual Dispenser (fits CB359-50 cartridge)"",
    ""Price"": ""48.50""
  },
  {
    ""ProductId"": 2,
    ""IsActive"": 1,
    ""CategoryId"": 51,
    ""CategoryName"": ""Adhesive Kits"",
    ""Code"": ""CB92"",
    ""ShortDescription"": ""Adhesive mix kit (estimate one kit per five fasteners) Includes: 1 pk. CB200, 1 mixing plate, 1 mixing stick, 1 alhocol wipe, 1 abrasive pad"",
    ""Price"": ""2.25""
  },
  {
    ""ProductId"": 1,
    ""IsActive"": 1,
    ""CategoryId"": 49,
    ""CategoryName"": ""Adhesive Packets and Cartridges"",
    ""Code"": ""CB200"",
    ""ShortDescription"": ""3.5 gram Adhesive packet"",
    ""Price"": ""1.36""
  },
  {
    ""ProductId"": 3,
    ""IsActive"": 1,
    ""CategoryId"": 49,
    ""CategoryName"": ""Adhesive Packets and Cartridges"",
    ""Code"": ""CB200-40"",
    ""ShortDescription"": ""Acrylic Adhesive 40ml. Dual *cartridge"",
    ""Price"": ""13.40""
  },
  {
    ""ProductId"": 4,
    ""IsActive"": 1,
    ""CategoryId"": 49,
    ""CategoryName"": ""Adhesive Packets and Cartridges"",
    ""Code"": ""CB359-50"",
    ""ShortDescription"": ""Epoxy Adhesive 50ml. Dual *cartridge"",
    ""Price"": ""24.75""
  },
  {
    ""ProductId"": 1,
    ""IsActive"": 1,
    ""CategoryId"": 48,
    ""CategoryName"": ""Adhesives and Adhesives Kits"",
    ""Code"": ""CB200"",
    ""ShortDescription"": ""3.5 gram Adhesive packet"",
    ""Price"": ""1.36""
  },
  {
    ""ProductId"": 1,
    ""IsActive"": 1,
    ""CategoryId"": 48,
    ""CategoryName"": ""Adhesives and Adhesives Kits"",
    ""Code"": ""CB200"",
    ""ShortDescription"": ""3.5 gram Adhesive packet"",
    ""Price"": ""1.36""
  },
  {
    ""ProductId"": 3,
    ""IsActive"": 1,
    ""CategoryId"": 48,
    ""CategoryName"": ""Adhesives and Adhesives Kits"",
    ""Code"": ""CB200-40"",
    ""ShortDescription"": ""Acrylic Adhesive 40ml. Dual *cartridge"",
    ""Price"": ""13.40""
  },
  {
    ""ProductId"": 4,
    ""IsActive"": 1,
    ""CategoryId"": 48,
    ""CategoryName"": ""Adhesives and Adhesives Kits"",
    ""Code"": ""CB359-50"",
    ""ShortDescription"": ""Epoxy Adhesive 50ml. Dual *cartridge"",
    ""Price"": ""24.75""
  },
  {
    ""ProductId"": 5,
    ""IsActive"": 1,
    ""CategoryId"": 48,
    ""CategoryName"": ""Adhesives and Adhesives Kits"",
    ""Code"": ""CB100-81"",
    ""ShortDescription"": ""Manual dispenser (fits CB200-40 cartridge)"",
    ""Price"": ""49.5""
  },
  {
    ""ProductId"": 6,
    ""IsActive"": 1,
    ""CategoryId"": 48,
    ""CategoryName"": ""Adhesives and Adhesives Kits"",
    ""Code"": ""CB100-21"",
    ""ShortDescription"": ""Manual Dispenser (fits CB359-50 cartridge)"",
    ""Price"": ""48.50""
  },
  {
    ""ProductId"": 7,
    ""IsActive"": 1,
    ""CategoryId"": 48,
    ""CategoryName"": ""Adhesives and Adhesives Kits"",
    ""Code"": ""CB106"",
    ""ShortDescription"": ""Mixing Tips"",
    ""Price"": ""0.95""
  },
  {
    ""ProductId"": 7,
    ""IsActive"": 1,
    ""CategoryId"": 52,
    ""CategoryName"": ""Adhesives Mixing Tips"",
    ""Code"": ""CB106"",
    ""ShortDescription"": ""Mixing Tips"",
    ""Price"": ""0.95""
  },
  {
    ""ProductId"": 185,
    ""IsActive"": 1,
    ""CategoryId"": 93,
    ""CategoryName"": ""CB100"",
    ""Code"": ""CB100"",
    ""ShortDescription"": ""Manual Dispenser"",
    ""Price"": ""38.5""
  },
  {
    ""ProductId"": 195,
    ""IsActive"": 1,
    ""CategoryId"": 638,
    ""CategoryName"": ""CB100"",
    ""Code"": ""CB-11"",
    ""ShortDescription"": ""Manual powered Dispenser and Individual Slides"",
    ""Price"": ""10.0""
  },
  {
    ""ProductId"": 196,
    ""IsActive"": 1,
    ""CategoryId"": 638,
    ""CategoryName"": ""CB100"",
    ""Code"": ""CB-21"",
    ""ShortDescription"": ""Manual powered Dispenser and Individual Slides"",
    ""Price"": ""10.00""
  },
  {
    ""ProductId"": 197,
    ""IsActive"": 0,
    ""CategoryId"": 638,
    ""CategoryName"": ""CB100"",
    ""Code"": ""CB-32"",
    ""ShortDescription"": ""Manual powered Dispenser and Individual Slides"",
    ""Price"": ""10""
  },
  {
    ""ProductId"": 198,
    ""IsActive"": 1,
    ""CategoryId"": 638,
    ""CategoryName"": ""CB100"",
    ""Code"": ""CB-41"",
    ""ShortDescription"": ""Manual powered Dispenser and Individual Slides"",
    ""Price"": ""10.00""
  },
  {
    ""ProductId"": 199,
    ""IsActive"": 1,
    ""CategoryId"": 638,
    ""CategoryName"": ""CB100"",
    ""Code"": ""CB-81"",
    ""ShortDescription"": ""Manual powered Dispenser and Individual Slides"",
    ""Price"": ""11""
  },
  {
    ""ProductId"": 185,
    ""IsActive"": 1,
    ""CategoryId"": 638,
    ""CategoryName"": ""CB100"",
    ""Code"": ""CB100"",
    ""ShortDescription"": ""Manual Dispenser"",
    ""Price"": ""38.5""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 432,
    ""CategoryName"": ""CB100 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 6,
    ""IsActive"": 1,
    ""CategoryId"": 301,
    ""CategoryName"": ""CB100-21"",
    ""Code"": ""CB100-21"",
    ""ShortDescription"": ""Manual Dispenser (fits CB359-50 cartridge)"",
    ""Price"": ""48.50""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 614,
    ""CategoryName"": ""CB100-21 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 5,
    ""IsActive"": 1,
    ""CategoryId"": 300,
    ""CategoryName"": ""CB100-81"",
    ""Code"": ""CB100-81"",
    ""ShortDescription"": ""Manual dispenser (fits CB200-40 cartridge)"",
    ""Price"": ""49.5""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 613,
    ""CategoryName"": ""CB100-81 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 7,
    ""IsActive"": 1,
    ""CategoryId"": 101,
    ""CategoryName"": ""CB106"",
    ""Code"": ""CB106"",
    ""ShortDescription"": ""Mixing Tips"",
    ""Price"": ""0.95""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 440,
    ""CategoryName"": ""CB106 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 414,
    ""CategoryName"": ""CB124 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 446,
    ""CategoryName"": ""CB1509 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 444,
    ""CategoryName"": ""CB1901 - RGQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 441,
    ""CategoryName"": ""CB1906 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 445,
    ""CategoryName"": ""CB1907 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 447,
    ""CategoryName"": ""CB1916 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 1,
    ""IsActive"": 1,
    ""CategoryId"": 299,
    ""CategoryName"": ""CB200"",
    ""Code"": ""CB200"",
    ""ShortDescription"": ""3.5 gram Adhesive packet"",
    ""Price"": ""1.36""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 612,
    ""CategoryName"": ""CB200 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 556,
    ""CategoryName"": ""CB2000 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 541,
    ""CategoryName"": ""CB2001 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 3,
    ""IsActive"": 1,
    ""CategoryId"": 88,
    ""CategoryName"": ""CB200-40"",
    ""Code"": ""CB200-40"",
    ""ShortDescription"": ""Acrylic Adhesive 40ml. Dual *cartridge"",
    ""Price"": ""13.40""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 427,
    ""CategoryName"": ""CB200-40 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 585,
    ""CategoryName"": ""CB2009 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 587,
    ""CategoryName"": ""CB2011 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 122,
    ""IsActive"": 1,
    ""CategoryId"": 275,
    ""CategoryName"": ""CB2031"",
    ""Code"": ""CB2031CR08CRP"",
    ""ShortDescription"": ""8/32\"" stainless steel/primed"",
    ""Price"": ""8""
  },
  {
    ""ProductId"": 123,
    ""IsActive"": 1,
    ""CategoryId"": 275,
    ""CategoryName"": ""CB2031"",
    ""Code"": ""CB2031CR3CRP"",
    ""ShortDescription"": ""10/32\"" stainless steel/primed"",
    ""Price"": ""8.75""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 589,
    ""CategoryName"": ""CB2031 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 183,
    ""IsActive"": 1,
    ""CategoryId"": 89,
    ""CategoryName"": ""CB250-50"",
    ""Code"": ""CB250-50"",
    ""ShortDescription"": ""Acrylic Adhesive 50ml. Dual *cartridge"",
    ""Price"": ""10""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 428,
    ""CategoryName"": ""CB250-50 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 182,
    ""IsActive"": 1,
    ""CategoryId"": 243,
    ""CategoryName"": ""CB3000"",
    ""Code"": ""CB3000AA3-20"",
    ""ShortDescription"": ""10/32 - 1.25\"" Tall Aluminum/Anodized"",
    ""Price"": ""5.9""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 557,
    ""CategoryName"": ""CB3000 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 542,
    ""CategoryName"": ""CB3001 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 561,
    ""CategoryName"": ""CB3003 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 543,
    ""CategoryName"": ""CB3004 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 105,
    ""IsActive"": 1,
    ""CategoryId"": 141,
    ""CategoryName"": ""CB3005"",
    ""Code"": ""CB3005CR08-10"",
    ""ShortDescription"": ""8/32--5/8\"" grip length"",
    ""Price"": ""8.5""
  },
  {
    ""ProductId"": 106,
    ""IsActive"": 1,
    ""CategoryId"": 141,
    ""CategoryName"": ""CB3005"",
    ""Code"": ""CB3005CR08-16"",
    ""ShortDescription"": ""10/32-1.00\"" grip length"",
    ""Price"": ""8.5""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 141,
    ""CategoryName"": ""CB3005"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 105,
    ""IsActive"": 1,
    ""CategoryId"": 152,
    ""CategoryName"": ""CB3005"",
    ""Code"": ""CB3005CR08-10"",
    ""ShortDescription"": ""8/32--5/8\"" grip length"",
    ""Price"": ""8.5""
  },
  {
    ""ProductId"": 106,
    ""IsActive"": 1,
    ""CategoryId"": 152,
    ""CategoryName"": ""CB3005"",
    ""Code"": ""CB3005CR08-16"",
    ""ShortDescription"": ""10/32-1.00\"" grip length"",
    ""Price"": ""8.5""
  },
  {
    ""ProductId"": 105,
    ""IsActive"": 1,
    ""CategoryId"": 481,
    ""CategoryName"": ""CB3005 - RFQ"",
    ""Code"": ""CB3005CR08-10"",
    ""ShortDescription"": ""8/32--5/8\"" grip length"",
    ""Price"": ""8.5""
  },
  {
    ""ProductId"": 106,
    ""IsActive"": 1,
    ""CategoryId"": 481,
    ""CategoryName"": ""CB3005 - RFQ"",
    ""Code"": ""CB3005CR08-16"",
    ""ShortDescription"": ""10/32-1.00\"" grip length"",
    ""Price"": ""8.5""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 488,
    ""CategoryName"": ""CB3005 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 586,
    ""CategoryName"": ""CB3009 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 588,
    ""CategoryName"": ""CB3011 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 193,
    ""IsActive"": 1,
    ""CategoryId"": 184,
    ""CategoryName"": ""CB3019"",
    ""Code"": ""CB3019AA3N"",
    ""ShortDescription"": ""Aluminum/Anodized-360° Swivel Mount"",
    ""Price"": ""2.25""
  },
  {
    ""ProductId"": 70,
    ""IsActive"": 1,
    ""CategoryId"": 184,
    ""CategoryName"": ""CB3019"",
    ""Code"": ""CB3019AA5N"",
    ""ShortDescription"": ""Aluminum/Anodized-360° Swivel Mount"",
    ""Price"": ""2.25""
  },
  {
    ""ProductId"": 69,
    ""IsActive"": 1,
    ""CategoryId"": 184,
    ""CategoryName"": ""CB3019"",
    ""Code"": ""CB3019A5N"",
    ""ShortDescription"": ""Aluminum/Anodized-360° Swivel Mount"",
    ""Price"": ""2.25""
  },
  {
    ""ProductId"": 204,
    ""IsActive"": 1,
    ""CategoryId"": 184,
    ""CategoryName"": ""CB3019"",
    ""Code"": ""CB3019A3N"",
    ""ShortDescription"": """",
    ""Price"": ""2.05""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 499,
    ""CategoryName"": ""CB3019 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 559,
    ""CategoryName"": ""CB3021 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 545,
    ""CategoryName"": ""CB3033 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 184,
    ""IsActive"": 1,
    ""CategoryId"": 90,
    ""CategoryName"": ""CB309-50"",
    ""Code"": ""CB309-50"",
    ""ShortDescription"": ""Epoxy Adhesive 50ml. Dual *cartridge"",
    ""Price"": ""25.25""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 429,
    ""CategoryName"": ""CB309-50 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 558,
    ""CategoryName"": ""CB3200 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 544,
    ""CategoryName"": ""CB3201 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 4,
    ""IsActive"": 1,
    ""CategoryId"": 91,
    ""CategoryName"": ""CB359-50"",
    ""Code"": ""CB359-50"",
    ""ShortDescription"": ""Epoxy Adhesive 50ml. Dual *cartridge"",
    ""Price"": ""24.75""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 430,
    ""CategoryName"": ""CB359-50 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 431,
    ""CategoryName"": ""CB394-43 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 548,
    ""CategoryName"": ""CB4000 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 534,
    ""CategoryName"": ""CB4001 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 532,
    ""CategoryName"": ""CB4002 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 606,
    ""CategoryName"": ""CB4002 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 553,
    ""CategoryName"": ""CB4003 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 535,
    ""CategoryName"": ""CB4004 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 554,
    ""CategoryName"": ""CB4005 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 600,
    ""CategoryName"": ""CB4009 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 601,
    ""CategoryName"": ""CB4011 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 504,
    ""CategoryName"": ""CB4013 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 498,
    ""CategoryName"": ""CB4014 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 533,
    ""CategoryName"": ""CB4014 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 549,
    ""CategoryName"": ""CB4017 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 536,
    ""CategoryName"": ""CB4018 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 71,
    ""IsActive"": 1,
    ""CategoryId"": 188,
    ""CategoryName"": ""CB4019"",
    ""Code"": ""CB4019G3N"",
    ""ShortDescription"": ""Glass/Epoxy, 360° F Swivel Mount"",
    ""Price"": ""4.5""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 500,
    ""CategoryName"": ""CB4019 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 74,
    ""IsActive"": 1,
    ""CategoryId"": 189,
    ""CategoryName"": ""CB4020"",
    ""Code"": ""CB4020V3N6"",
    ""ShortDescription"": ""Glass/Polyetherimide-3/8\"" length"",
    ""Price"": ""3.5""
  },
  {
    ""ProductId"": 75,
    ""IsActive"": 1,
    ""CategoryId"": 189,
    ""CategoryName"": ""CB4020"",
    ""Code"": ""CB4020V3N8"",
    ""ShortDescription"": ""Glass/Polyetherimide-1/2\"" length"",
    ""Price"": ""3.6""
  },
  {
    ""ProductId"": 76,
    ""IsActive"": 1,
    ""CategoryId"": 189,
    ""CategoryName"": ""CB4020"",
    ""Code"": ""CB4020V3N16"",
    ""ShortDescription"": ""Glass/Polyetherimide-1\"" length"",
    ""Price"": ""3.8""
  },
  {
    ""ProductId"": 77,
    ""IsActive"": 1,
    ""CategoryId"": 189,
    ""CategoryName"": ""CB4020"",
    ""Code"": ""CB4020V3N24"",
    ""ShortDescription"": ""Glass/Polyetherimide-1 1/2\"" length"",
    ""Price"": ""3.9""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 501,
    ""CategoryName"": ""CB4020 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 82,
    ""IsActive"": 1,
    ""CategoryId"": 190,
    ""CategoryName"": ""CB4021"",
    ""Code"": ""CB4021V3N8"",
    ""ShortDescription"": ""1/2\"" tall - Glass/PEI"",
    ""Price"": ""6""
  },
  {
    ""ProductId"": 83,
    ""IsActive"": 1,
    ""CategoryId"": 190,
    ""CategoryName"": ""CB4021"",
    ""Code"": ""CB4021V3N12"",
    ""ShortDescription"": ""3/4\"" tall - Glass/PEI"",
    ""Price"": ""6.1""
  },
  {
    ""ProductId"": 173,
    ""IsActive"": 1,
    ""CategoryId"": 190,
    ""CategoryName"": ""CB4021"",
    ""Code"": ""CB4021G3N16"",
    ""ShortDescription"": ""1\"" tall - Glass/Epoxy"",
    ""Price"": ""12""
  },
  {
    ""ProductId"": 84,
    ""IsActive"": 1,
    ""CategoryId"": 190,
    ""CategoryName"": ""CB4021"",
    ""Code"": ""CB4021V3N16"",
    ""ShortDescription"": ""1\"" tall - Glass/PEI"",
    ""Price"": ""6.3""
  },
  {
    ""ProductId"": 85,
    ""IsActive"": 1,
    ""CategoryId"": 190,
    ""CategoryName"": ""CB4021"",
    ""Code"": ""CB4021V3N24"",
    ""ShortDescription"": ""1 1/2\"" tall - Glass/PEI"",
    ""Price"": ""6.3""
  },
  {
    ""ProductId"": 86,
    ""IsActive"": 1,
    ""CategoryId"": 190,
    ""CategoryName"": ""CB4021"",
    ""Code"": ""CB4021V3N32"",
    ""ShortDescription"": ""2\"" tall - Glass/PEI"",
    ""Price"": ""6.4""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 502,
    ""CategoryName"": ""CB4021 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 78,
    ""IsActive"": 1,
    ""CategoryId"": 181,
    ""CategoryName"": ""CB4022"",
    ""Code"": ""CB4022V08CR8"",
    ""ShortDescription"": ""8/32-1/2\"" length"",
    ""Price"": ""7.6""
  },
  {
    ""ProductId"": 79,
    ""IsActive"": 1,
    ""CategoryId"": 181,
    ""CategoryName"": ""CB4022"",
    ""Code"": ""CB4022V08CR16"",
    ""ShortDescription"": ""8/32-1\"" length"",
    ""Price"": ""7.8""
  },
  {
    ""ProductId"": 80,
    ""IsActive"": 1,
    ""CategoryId"": 181,
    ""CategoryName"": ""CB4022"",
    ""Code"": ""CB4022V3CR8"",
    ""ShortDescription"": ""10/32-1/2\"" length"",
    ""Price"": ""7.6""
  },
  {
    ""ProductId"": 172,
    ""IsActive"": 1,
    ""CategoryId"": 181,
    ""CategoryName"": ""CB4022"",
    ""Code"": ""CB4022V3CR12"",
    ""ShortDescription"": ""10/32-3/4\"" length"",
    ""Price"": ""7.8""
  },
  {
    ""ProductId"": 81,
    ""IsActive"": 1,
    ""CategoryId"": 181,
    ""CategoryName"": ""CB4022"",
    ""Code"": ""CB4022V3CR16"",
    ""ShortDescription"": ""10/32-1\"" length"",
    ""Price"": ""7.8""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 496,
    ""CategoryName"": ""CB4022 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 174,
    ""IsActive"": 1,
    ""CategoryId"": 182,
    ""CategoryName"": ""CB4023"",
    ""Code"": ""CB4023V08CR8"",
    ""ShortDescription"": ""8/32-1/2\"" tall"",
    ""Price"": ""9""
  },
  {
    ""ProductId"": 87,
    ""IsActive"": 1,
    ""CategoryId"": 182,
    ""CategoryName"": ""CB4023"",
    ""Code"": ""CB4023V3CR-8"",
    ""ShortDescription"": ""10/32-1/2\"" tall"",
    ""Price"": ""9""
  },
  {
    ""ProductId"": 88,
    ""IsActive"": 1,
    ""CategoryId"": 182,
    ""CategoryName"": ""CB4023"",
    ""Code"": ""CB4023V3CR-12"",
    ""ShortDescription"": ""10/32-3/4\"" tall"",
    ""Price"": ""9.1""
  },
  {
    ""ProductId"": 89,
    ""IsActive"": 1,
    ""CategoryId"": 182,
    ""CategoryName"": ""CB4023"",
    ""Code"": ""CB4023V3CR-16"",
    ""ShortDescription"": ""10/32-1\"" tall"",
    ""Price"": ""9.2""
  },
  {
    ""ProductId"": 90,
    ""IsActive"": 1,
    ""CategoryId"": 182,
    ""CategoryName"": ""CB4023"",
    ""Code"": ""CB4023V3CR-24"",
    ""ShortDescription"": ""10/32-1 1/2\"" tall"",
    ""Price"": ""9.3""
  },
  {
    ""ProductId"": 91,
    ""IsActive"": 1,
    ""CategoryId"": 182,
    ""CategoryName"": ""CB4023"",
    ""Code"": ""CB4023V3CR-32"",
    ""ShortDescription"": ""10/32-2\"" tall"",
    ""Price"": ""9.4""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 497,
    ""CategoryName"": ""CB4023 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 505,
    ""CategoryName"": ""CB4024 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 538,
    ""CategoryName"": ""CB4033 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 521,
    ""CategoryName"": ""CB4034 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 522,
    ""CategoryName"": ""CB4035 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 616,
    ""CategoryName"": ""CB4062 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 617,
    ""CategoryName"": ""CB4063 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 618,
    ""CategoryName"": ""CB4064 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 552,
    ""CategoryName"": ""CB4101 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 495,
    ""CategoryName"": ""CB4132 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 607,
    ""CategoryName"": ""CB4132 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 550,
    ""CategoryName"": ""CB4200 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 537,
    ""CategoryName"": ""CB4201 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 494,
    ""CategoryName"": ""CB4233 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 609,
    ""CategoryName"": ""CB4233 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 555,
    ""CategoryName"": ""CB5000 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 539,
    ""CategoryName"": ""CB5001 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 560,
    ""CategoryName"": ""CB5003 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 540,
    ""CategoryName"": ""CB5004 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 479,
    ""CategoryName"": ""CB5005 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 480,
    ""CategoryName"": ""CB5006 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 477,
    ""CategoryName"": ""CB5007 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 101,
    ""IsActive"": 1,
    ""CategoryId"": 113,
    ""CategoryName"": ""CB5906"",
    ""Code"": ""CB5906C06B300"",
    ""ShortDescription"": ""#10--.300\"" stainless steel"",
    ""Price"": ""4.62""
  },
  {
    ""ProductId"": 102,
    ""IsActive"": 1,
    ""CategoryId"": 113,
    ""CategoryName"": ""CB5906"",
    ""Code"": ""CB5906C08B300"",
    ""ShortDescription"": ""#1/4-.300\"" stainless steel"",
    ""Price"": ""4.62""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 452,
    ""CategoryName"": ""CB5906 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 450,
    ""CategoryName"": ""CB5907 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 454,
    ""CategoryName"": ""CB5908 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 453,
    ""CategoryName"": ""CB5946 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 451,
    ""CategoryName"": ""CB5947 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 455,
    ""CategoryName"": ""CB5948 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 574,
    ""CategoryName"": ""CB6007 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 591,
    ""CategoryName"": ""CB6008 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 17,
    ""IsActive"": 1,
    ""CategoryId"": 261,
    ""CategoryName"": ""CB6009"",
    ""Code"": ""CB6009CR08-1P"",
    ""ShortDescription"": ""8/32\"" stainless steel/primed"",
    ""Price"": ""4""
  },
  {
    ""ProductId"": 18,
    ""IsActive"": 1,
    ""CategoryId"": 261,
    ""CategoryName"": ""CB6009"",
    ""Code"": ""CB6009CR3-1P"",
    ""ShortDescription"": ""10/32\"" stainless steel/primed"",
    ""Price"": ""4""
  },
  {
    ""ProductId"": 114,
    ""IsActive"": 1,
    ""CategoryId"": 261,
    ""CategoryName"": ""CB6009"",
    ""Code"": ""CB6009CR4-1P"",
    ""ShortDescription"": ""1/4-28\"" stainless steel/primed"",
    ""Price"": ""4.93""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 575,
    ""CategoryName"": ""CB6009 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 593,
    ""CategoryName"": ""CB6010 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 577,
    ""CategoryName"": ""CB6011 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 579,
    ""CategoryName"": ""CB6012 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 580,
    ""CategoryName"": ""CB6013 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 201,
    ""IsActive"": 1,
    ""CategoryId"": 283,
    ""CategoryName"": ""CB6014"",
    ""Code"": ""CB6014CR06-1P"",
    ""ShortDescription"": ""6/32 stainless steel/primed"",
    ""Price"": ""3.50""
  },
  {
    ""ProductId"": 115,
    ""IsActive"": 1,
    ""CategoryId"": 283,
    ""CategoryName"": ""CB6014"",
    ""Code"": ""CB6014CR04-1P"",
    ""ShortDescription"": ""4/40 stainless steel/primed"",
    ""Price"": ""3.75""
  },
  {
    ""ProductId"": 116,
    ""IsActive"": 1,
    ""CategoryId"": 283,
    ""CategoryName"": ""CB6014"",
    ""Code"": ""CB6014CR08-1P"",
    ""ShortDescription"": ""8/32 stainless steel/primed"",
    ""Price"": ""3.50""
  },
  {
    ""ProductId"": 117,
    ""IsActive"": 1,
    ""CategoryId"": 283,
    ""CategoryName"": ""CB6014"",
    ""Code"": ""CB6014CR3-1P"",
    ""ShortDescription"": ""10/32 stainless steel/primed"",
    ""Price"": ""3.50""
  },
  {
    ""ProductId"": 118,
    ""IsActive"": 1,
    ""CategoryId"": 283,
    ""CategoryName"": ""CB6014"",
    ""Code"": ""CB6014CR4-1P"",
    ""ShortDescription"": ""1/4-28  stainless steel/primed"",
    ""Price"": ""4.5""
  },
  {
    ""ProductId"": 119,
    ""IsActive"": 1,
    ""CategoryId"": 283,
    ""CategoryName"": ""CB6014"",
    ""Code"": ""CB6014CR5-1P"",
    ""ShortDescription"": ""5/16-24 stainless steel/primed"",
    ""Price"": ""9.53""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 597,
    ""CategoryName"": ""CB6014 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 415,
    ""CategoryName"": ""CB602 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 416,
    ""CategoryName"": ""CB603 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 417,
    ""CategoryName"": ""CB607 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 615,
    ""CategoryName"": ""CB6080 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 418,
    ""CategoryName"": ""CB609 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 571,
    ""CategoryName"": ""CB6109 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 572,
    ""CategoryName"": ""CB6209 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 419,
    ""CategoryName"": ""CB625 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 420,
    ""CategoryName"": ""CB629 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 563,
    ""CategoryName"": ""CB6307 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 566,
    ""CategoryName"": ""CB6309 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 570,
    ""CategoryName"": ""CB6310 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 565,
    ""CategoryName"": ""CB6311 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 564,
    ""CategoryName"": ""CB6347 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 567,
    ""CategoryName"": ""CB6349 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 421,
    ""CategoryName"": ""CB750 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 422,
    ""CategoryName"": ""CB751 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 423,
    ""CategoryName"": ""CB752 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 424,
    ""CategoryName"": ""CB753 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 592,
    ""CategoryName"": ""CB8008 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 576,
    ""CategoryName"": ""CB8009 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 594,
    ""CategoryName"": ""CB8010 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 578,
    ""CategoryName"": ""CB8011 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 107,
    ""IsActive"": 1,
    ""CategoryId"": 146,
    ""CategoryName"": ""CB9029"",
    ""Code"": ""CB9029AA3-500"",
    ""ShortDescription"": ""10/32\"" Anodized-Aluminum .500 long"",
    ""Price"": ""6.75""
  },
  {
    ""ProductId"": 176,
    ""IsActive"": 1,
    ""CategoryId"": 146,
    ""CategoryName"": ""CB9029"",
    ""Code"": ""CB9029AA3-813"",
    ""ShortDescription"": ""10/32\"" Anodized-Aluminum .813 long"",
    ""Price"": ""6.75""
  },
  {
    ""ProductId"": 177,
    ""IsActive"": 1,
    ""CategoryId"": 146,
    ""CategoryName"": ""CB9029"",
    ""Code"": ""CB9029A3-375"",
    ""ShortDescription"": ""10/32\"" Aluminum .375 long"",
    ""Price"": ""6.75""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 485,
    ""CategoryName"": ""CB9029 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 425,
    ""CategoryName"": ""CB9032 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 492,
    ""CategoryName"": ""CB9060 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 487,
    ""CategoryName"": ""CB9061 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 486,
    ""CategoryName"": ""CB9077 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 448,
    ""CategoryName"": ""CB9080 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 449,
    ""CategoryName"": ""CB9081 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 482,
    ""CategoryName"": ""CB9084 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 483,
    ""CategoryName"": ""CB9085 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 581,
    ""CategoryName"": ""CB9099 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 433,
    ""CategoryName"": ""CB91 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 493,
    ""CategoryName"": ""CB9101 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 583,
    ""CategoryName"": ""CB9102 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 438,
    ""CategoryName"": ""CB911 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 484,
    ""CategoryName"": ""CB9112 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 67,
    ""IsActive"": 1,
    ""CategoryId"": 193,
    ""CategoryName"": ""CB9120"",
    ""Code"": ""CB9120V5"",
    ""ShortDescription"": ""Glass/Thermoplastic"",
    ""Price"": ""2.58""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 506,
    ""CategoryName"": ""CB9120 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 590,
    ""CategoryName"": ""CB9121 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 599,
    ""CategoryName"": ""CB9121 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 478,
    ""CategoryName"": ""CB9122 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 68,
    ""IsActive"": 1,
    ""CategoryId"": 194,
    ""CategoryName"": ""CB9151"",
    ""Code"": ""CB9151V5"",
    ""ShortDescription"": ""Glass/Thermoplastic"",
    ""Price"": ""2.35""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 507,
    ""CategoryName"": ""CB9151 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 551,
    ""CategoryName"": ""CB9159 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 514,
    ""CategoryName"": ""CB9170 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 108,
    ""IsActive"": 1,
    ""CategoryId"": 201,
    ""CategoryName"": ""CB9173"",
    ""Code"": ""CB9173V3-12S"",
    ""ShortDescription"": ""UItem Thermoplastic 3/4\"" tall"",
    ""Price"": ""1.1""
  },
  {
    ""ProductId"": 109,
    ""IsActive"": 1,
    ""CategoryId"": 201,
    ""CategoryName"": ""CB9173"",
    ""Code"": ""CB9173V3-24S"",
    ""ShortDescription"": ""UItem Thermoplastic 1 1/2\"" tall"",
    ""Price"": ""1.25""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 515,
    ""CategoryName"": ""CB9173 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 110,
    ""IsActive"": 1,
    ""CategoryId"": 204,
    ""CategoryName"": ""CB9174"",
    ""Code"": ""CB9174V3-6"",
    ""ShortDescription"": ""UItem Thermoplastic 3/8\"" tall"",
    ""Price"": ""0.6""
  },
  {
    ""ProductId"": 111,
    ""IsActive"": 1,
    ""CategoryId"": 204,
    ""CategoryName"": ""CB9174"",
    ""Code"": ""CB9174V3-8"",
    ""ShortDescription"": ""UItem Thermoplastic 1/2\"" tall"",
    ""Price"": ""0.6""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 518,
    ""CategoryName"": ""CB9174 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 21,
    ""IsActive"": 1,
    ""CategoryId"": 157,
    ""CategoryName"": ""CB9176"",
    ""Code"": ""CB9176CR08CRP"",
    ""ShortDescription"": ""8/32\"" stainless steel"",
    ""Price"": ""7.8""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 490,
    ""CategoryName"": ""CB9176 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 584,
    ""CategoryName"": ""CB9186 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 546,
    ""CategoryName"": ""CB9188 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 598,
    ""CategoryName"": ""CB9197 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 2,
    ""IsActive"": 1,
    ""CategoryId"": 95,
    ""CategoryName"": ""CB92"",
    ""Code"": ""CB92"",
    ""ShortDescription"": ""Adhesive mix kit (estimate one kit per five fasteners) Includes: 1 pk. CB200, 1 mixing plate, 1 mixing stick, 1 alhocol wipe, 1 abrasive pad"",
    ""Price"": ""2.25""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 434,
    ""CategoryName"": ""CB92 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 523,
    ""CategoryName"": ""CB9201 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 525,
    ""CategoryName"": ""CB9205 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 516,
    ""CategoryName"": ""CB9206 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 519,
    ""CategoryName"": ""CB9208 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 517,
    ""CategoryName"": ""CB9210 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 547,
    ""CategoryName"": ""CB9212 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 66,
    ""IsActive"": 1,
    ""CategoryId"": 196,
    ""CategoryName"": ""CB9257"",
    ""Code"": ""CB9257V"",
    ""ShortDescription"": ""Glass/Thermoplastic, 350° F"",
    ""Price"": ""3.5""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 509,
    ""CategoryName"": ""CB9257 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 426,
    ""CategoryName"": ""CB9268 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 471,
    ""CategoryName"": ""CB9296 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 526,
    ""CategoryName"": ""CB9297 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 435,
    ""CategoryName"": ""CB93 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 65,
    ""IsActive"": 1,
    ""CategoryId"": 195,
    ""CategoryName"": ""CB9302"",
    ""Code"": ""CB9302V3"",
    ""ShortDescription"": ""Glass/Thermoplastic"",
    ""Price"": ""2.73""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 508,
    ""CategoryName"": ""CB9302 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 442,
    ""CategoryName"": ""CB9308 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 491,
    ""CategoryName"": ""CB9312 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 520,
    ""CategoryName"": ""CB9322 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 595,
    ""CategoryName"": ""CB9356 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 443,
    ""CategoryName"": ""CB9366 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 568,
    ""CategoryName"": ""CB9382 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 569,
    ""CategoryName"": ""CB9392 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 103,
    ""IsActive"": 1,
    ""CategoryId"": 313,
    ""CategoryName"": ""CB9393"",
    ""Code"": ""CB9393V5"",
    ""ShortDescription"": ""Polyetherimide/Glass, 350°   F"",
    ""Price"": ""3.8""
  },
  {
    ""ProductId"": 203,
    ""IsActive"": 1,
    ""CategoryId"": 313,
    ""CategoryName"": ""CB9393"",
    ""Code"": ""CB9393V7"",
    ""ShortDescription"": ""Quarter Receptacle 4002"",
    ""Price"": ""4.3""
  },
  {
    ""ProductId"": 178,
    ""IsActive"": 1,
    ""CategoryId"": 313,
    ""CategoryName"": ""CB9393"",
    ""Code"": ""CB9393V5C"",
    ""ShortDescription"": ""Polyetherimide/Glass, 350°   F w/Cap"",
    ""Price"": ""7.3""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 620,
    ""CategoryName"": ""CB9393 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 436,
    ""CategoryName"": ""CB94 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 527,
    ""CategoryName"": ""CB9421 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 530,
    ""CategoryName"": ""CB9421 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 528,
    ""CategoryName"": ""CB9422 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 531,
    ""CategoryName"": ""CB9422 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 72,
    ""IsActive"": 1,
    ""CategoryId"": 198,
    ""CategoryName"": ""CB9459"",
    ""Code"": ""CB9459P3"",
    ""ShortDescription"": ""PEEK"",
    ""Price"": ""1.25""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 512,
    ""CategoryName"": ""CB9459 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 73,
    ""IsActive"": 1,
    ""CategoryId"": 199,
    ""CategoryName"": ""CB9513P5"",
    ""Code"": ""CB9513P5-10"",
    ""ShortDescription"": ""10\"" Long/PEEK"",
    ""Price"": ""5.75""
  },
  {
    ""ProductId"": 170,
    ""IsActive"": 1,
    ""CategoryId"": 199,
    ""CategoryName"": ""CB9513P5"",
    ""Code"": ""CB9513P5-15"",
    ""ShortDescription"": ""15\"" Long/PEEK"",
    ""Price"": ""8.25""
  },
  {
    ""ProductId"": 171,
    ""IsActive"": 1,
    ""CategoryId"": 199,
    ""CategoryName"": ""CB9513P5"",
    ""Code"": ""CB9513P5-18"",
    ""ShortDescription"": ""18\"" Long/PEEK"",
    ""Price"": ""9.75""
  },
  {
    ""ProductId"": 180,
    ""IsActive"": 1,
    ""CategoryId"": 199,
    ""CategoryName"": ""CB9513P5"",
    ""Code"": ""CB9513P5-8"",
    ""ShortDescription"": ""8\"" Long/PEEK"",
    ""Price"": ""4.75""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 513,
    ""CategoryName"": ""CB9513P5 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 524,
    ""CategoryName"": ""CB9514 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 511,
    ""CategoryName"": ""CB9519 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 573,
    ""CategoryName"": ""CB9530 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 596,
    ""CategoryName"": ""CB9530 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 529,
    ""CategoryName"": ""CB9532 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 582,
    ""CategoryName"": ""CB9595 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 437,
    ""CategoryName"": ""CB96 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 96,
    ""IsActive"": 1,
    ""CategoryId"": 298,
    ""CategoryName"": ""CG596C"",
    ""Code"": ""CG596C55"",
    ""ShortDescription"": ""Stainless Steel/ for #8 screw"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 97,
    ""IsActive"": 1,
    ""CategoryId"": 298,
    ""CategoryName"": ""CG596C"",
    ""Code"": ""CG596C06"",
    ""ShortDescription"": ""Stainless Steel/ for #10 screw"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 98,
    ""IsActive"": 1,
    ""CategoryId"": 298,
    ""CategoryName"": ""CG596C"",
    ""Code"": ""CG596C08"",
    ""ShortDescription"": ""Stainless Steel/ for #1/4 screw"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 99,
    ""IsActive"": 1,
    ""CategoryId"": 298,
    ""CategoryName"": ""CG596C"",
    ""Code"": ""CG596C10"",
    ""ShortDescription"": ""Stainless Steel/ for #5/16 screw"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 100,
    ""IsActive"": 1,
    ""CategoryId"": 298,
    ""CategoryName"": ""CG596C"",
    ""Code"": ""CG596C12"",
    ""ShortDescription"": ""Stainless Steel/ for #3/8 screw"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 611,
    ""CategoryName"": ""CG596C - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 96,
    ""IsActive"": 1,
    ""CategoryId"": 393,
    ""CategoryName"": ""CG596C06"",
    ""Code"": ""CG596C55"",
    ""ShortDescription"": ""Stainless Steel/ for #8 screw"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 97,
    ""IsActive"": 1,
    ""CategoryId"": 393,
    ""CategoryName"": ""CG596C06"",
    ""Code"": ""CG596C06"",
    ""ShortDescription"": ""Stainless Steel/ for #10 screw"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 98,
    ""IsActive"": 1,
    ""CategoryId"": 393,
    ""CategoryName"": ""CG596C06"",
    ""Code"": ""CG596C08"",
    ""ShortDescription"": ""Stainless Steel/ for #1/4 screw"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 99,
    ""IsActive"": 1,
    ""CategoryId"": 393,
    ""CategoryName"": ""CG596C06"",
    ""Code"": ""CG596C10"",
    ""ShortDescription"": ""Stainless Steel/ for #5/16 screw"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 100,
    ""IsActive"": 1,
    ""CategoryId"": 393,
    ""CategoryName"": ""CG596C06"",
    ""Code"": ""CG596C12"",
    ""ShortDescription"": ""Stainless Steel/ for #3/8 screw"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 621,
    ""CategoryName"": ""CG596C06 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 96,
    ""IsActive"": 1,
    ""CategoryId"": 395,
    ""CategoryName"": ""CG596C10"",
    ""Code"": ""CG596C55"",
    ""ShortDescription"": ""Stainless Steel/ for #8 screw"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 97,
    ""IsActive"": 1,
    ""CategoryId"": 395,
    ""CategoryName"": ""CG596C10"",
    ""Code"": ""CG596C06"",
    ""ShortDescription"": ""Stainless Steel/ for #10 screw"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 98,
    ""IsActive"": 1,
    ""CategoryId"": 395,
    ""CategoryName"": ""CG596C10"",
    ""Code"": ""CG596C08"",
    ""ShortDescription"": ""Stainless Steel/ for #1/4 screw"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 99,
    ""IsActive"": 1,
    ""CategoryId"": 395,
    ""CategoryName"": ""CG596C10"",
    ""Code"": ""CG596C10"",
    ""ShortDescription"": ""Stainless Steel/ for #5/16 screw"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 100,
    ""IsActive"": 1,
    ""CategoryId"": 395,
    ""CategoryName"": ""CG596C10"",
    ""Code"": ""CG596C12"",
    ""ShortDescription"": ""Stainless Steel/ for #3/8 screw"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 623,
    ""CategoryName"": ""CG596C10 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 96,
    ""IsActive"": 1,
    ""CategoryId"": 394,
    ""CategoryName"": ""CG59C08"",
    ""Code"": ""CG596C55"",
    ""ShortDescription"": ""Stainless Steel/ for #8 screw"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 97,
    ""IsActive"": 1,
    ""CategoryId"": 394,
    ""CategoryName"": ""CG59C08"",
    ""Code"": ""CG596C06"",
    ""ShortDescription"": ""Stainless Steel/ for #10 screw"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 98,
    ""IsActive"": 1,
    ""CategoryId"": 394,
    ""CategoryName"": ""CG59C08"",
    ""Code"": ""CG596C08"",
    ""ShortDescription"": ""Stainless Steel/ for #1/4 screw"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 99,
    ""IsActive"": 1,
    ""CategoryId"": 394,
    ""CategoryName"": ""CG59C08"",
    ""Code"": ""CG596C10"",
    ""ShortDescription"": ""Stainless Steel/ for #5/16 screw"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 100,
    ""IsActive"": 1,
    ""CategoryId"": 394,
    ""CategoryName"": ""CG59C08"",
    ""Code"": ""CG596C12"",
    ""ShortDescription"": ""Stainless Steel/ for #3/8 screw"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 622,
    ""CategoryName"": ""CG59C08 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 100,
    ""IsActive"": 1,
    ""CategoryId"": 396,
    ""CategoryName"": ""CG59C12"",
    ""Code"": ""CG596C12"",
    ""ShortDescription"": ""Stainless Steel/ for #3/8 screw"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 624,
    ""CategoryName"": ""CG59C12 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 189,
    ""IsActive"": 1,
    ""CategoryId"": 60,
    ""CategoryName"": ""CN109"",
    ""Code"": ""CN109"",
    ""ShortDescription"": ""Two Lug, Bracket Retained"",
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 400,
    ""CategoryName"": ""CN109 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 187,
    ""IsActive"": 1,
    ""CategoryId"": 61,
    ""CategoryName"": ""CN111"",
    ""Code"": ""CN111"",
    ""ShortDescription"": ""Nutplate, Bracket Retained, one lug"",
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 401,
    ""CategoryName"": ""CN111 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 56,
    ""IsActive"": 1,
    ""CategoryId"": 70,
    ""CategoryName"": ""CN125"",
    ""Code"": ""CN125-632-8CR"",
    ""ShortDescription"": ""6/32-1/2\"" long/stainless steel"",
    ""Price"": ""3.75""
  },
  {
    ""ProductId"": 169,
    ""IsActive"": 1,
    ""CategoryId"": 70,
    ""CategoryName"": ""CN125"",
    ""Code"": ""CN125-1032-16GCR"",
    ""ShortDescription"": ""10/32-1\"" long/composite base"",
    ""Price"": ""6.00""
  },
  {
    ""ProductId"": 62,
    ""IsActive"": 1,
    ""CategoryId"": 70,
    ""CategoryName"": ""CN125"",
    ""Code"": ""CN125-1032-8CR"",
    ""ShortDescription"": ""10/32-1/2\"" long/stainless steel"",
    ""Price"": ""4.43""
  },
  {
    ""ProductId"": 63,
    ""IsActive"": 1,
    ""CategoryId"": 70,
    ""CategoryName"": ""CN125"",
    ""Code"": ""CN125-1032-12CR"",
    ""ShortDescription"": ""10/32-3/4\"" long/stainless steel"",
    ""Price"": ""4.43""
  },
  {
    ""ProductId"": 64,
    ""IsActive"": 1,
    ""CategoryId"": 70,
    ""CategoryName"": ""CN125"",
    ""Code"": ""CN125-1032-16CR"",
    ""ShortDescription"": ""10/32-1\"" long/stainless steel"",
    ""Price"": ""4.43""
  },
  {
    ""ProductId"": 166,
    ""IsActive"": 1,
    ""CategoryId"": 70,
    ""CategoryName"": ""CN125"",
    ""Code"": ""CN125-832-8GCR"",
    ""ShortDescription"": ""8/32-1/2\"" long/composite base"",
    ""Price"": ""5.90""
  },
  {
    ""ProductId"": 167,
    ""IsActive"": 1,
    ""CategoryId"": 70,
    ""CategoryName"": ""CN125"",
    ""Code"": ""CN125-1032-8GCR"",
    ""ShortDescription"": ""10/32-1/2\"" long/composite base"",
    ""Price"": ""4.68""
  },
  {
    ""ProductId"": 168,
    ""IsActive"": 1,
    ""CategoryId"": 70,
    ""CategoryName"": ""CN125"",
    ""Code"": ""CN125-1032-12GCR"",
    ""ShortDescription"": ""10/32-3/4\"" long/composite base"",
    ""Price"": ""4.68""
  },
  {
    ""ProductId"": 57,
    ""IsActive"": 1,
    ""CategoryId"": 70,
    ""CategoryName"": ""CN125"",
    ""Code"": ""CN125-632-12CR"",
    ""ShortDescription"": ""6/32-3/4\"" long/stainless steel"",
    ""Price"": ""3.75""
  },
  {
    ""ProductId"": 58,
    ""IsActive"": 1,
    ""CategoryId"": 70,
    ""CategoryName"": ""CN125"",
    ""Code"": ""CN125-632-16CR"",
    ""ShortDescription"": ""6/32-1\"" long/stainless steel"",
    ""Price"": ""3.75""
  },
  {
    ""ProductId"": 165,
    ""IsActive"": 1,
    ""CategoryId"": 70,
    ""CategoryName"": ""CN125"",
    ""Code"": ""CN125-832-5CR"",
    ""ShortDescription"": ""8/32-5/16\"" long/stainless steel"",
    ""Price"": ""3.75""
  },
  {
    ""ProductId"": 59,
    ""IsActive"": 1,
    ""CategoryId"": 70,
    ""CategoryName"": ""CN125"",
    ""Code"": ""CN125-832-8CR"",
    ""ShortDescription"": ""8/32-1/2\"" long/stainless steel"",
    ""Price"": ""3.75""
  },
  {
    ""ProductId"": 60,
    ""IsActive"": 1,
    ""CategoryId"": 70,
    ""CategoryName"": ""CN125"",
    ""Code"": ""CN125-832-12CR"",
    ""ShortDescription"": ""8/32-3/4\"" long/stainless steel"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 61,
    ""IsActive"": 1,
    ""CategoryId"": 70,
    ""CategoryName"": ""CN125"",
    ""Code"": ""CN125-832-16CR"",
    ""ShortDescription"": ""8/32-1\"" long/stainless steel"",
    ""Price"": ""4.98""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 410,
    ""CategoryName"": ""CN125 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 188,
    ""IsActive"": 1,
    ""CategoryId"": 71,
    ""CategoryName"": ""CN200"",
    ""Code"": ""CN200"",
    ""ShortDescription"": ""Standoff (2.00\"" dia), Very Large Base, Locking Thread"",
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 411,
    ""CategoryName"": ""CN200 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 408,
    ""CategoryName"": ""CN305 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 406,
    ""CategoryName"": ""CN505 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 407,
    ""CategoryName"": ""CN555 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 14,
    ""IsActive"": 1,
    ""CategoryId"": 62,
    ""CategoryName"": ""CN609"",
    ""Code"": ""CN609CR08P"",
    ""ShortDescription"": ""8/32\"" stainless steel/primed"",
    ""Price"": ""4.00""
  },
  {
    ""ProductId"": 15,
    ""IsActive"": 1,
    ""CategoryId"": 62,
    ""CategoryName"": ""CN609"",
    ""Code"": ""CN609CR3P"",
    ""ShortDescription"": ""10/32\"" stainless steel primed"",
    ""Price"": ""4.0""
  },
  {
    ""ProductId"": 16,
    ""IsActive"": 1,
    ""CategoryId"": 62,
    ""CategoryName"": ""CN609"",
    ""Code"": ""CN609CR4P"",
    ""ShortDescription"": ""1/4-28\"" stainless steel/primed"",
    ""Price"": ""4.93""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 402,
    ""CategoryName"": ""CN609 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 19,
    ""IsActive"": 1,
    ""CategoryId"": 63,
    ""CategoryName"": ""CN610"",
    ""Code"": ""CN610ACR08P"",
    ""ShortDescription"": ""8/32\"" stainless steel & aluninum"",
    ""Price"": ""7.0""
  },
  {
    ""ProductId"": 120,
    ""IsActive"": 0,
    ""CategoryId"": 63,
    ""CategoryName"": ""CN610"",
    ""Code"": ""CN610CR08P"",
    ""ShortDescription"": ""8/32 stainless steel w/primer"",
    ""Price"": ""4.89""
  },
  {
    ""ProductId"": 20,
    ""IsActive"": 1,
    ""CategoryId"": 63,
    ""CategoryName"": ""CN610"",
    ""Code"": ""CN610ACR3"",
    ""ShortDescription"": ""10/32\"" stainless steel & aluminum"",
    ""Price"": ""6.50""
  },
  {
    ""ProductId"": 121,
    ""IsActive"": 1,
    ""CategoryId"": 63,
    ""CategoryName"": ""CN610"",
    ""Code"": ""CN610CR3P"",
    ""ShortDescription"": ""10/32\"" stainless steel w/primer"",
    ""Price"": ""8.0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 403,
    ""CategoryName"": ""CN610 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 186,
    ""IsActive"": 1,
    ""CategoryId"": 64,
    ""CategoryName"": ""CN611"",
    ""Code"": ""CN611"",
    ""ShortDescription"": ""One Lug, Clip Retained"",
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 404,
    ""CategoryName"": ""CN611 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 8,
    ""IsActive"": 1,
    ""CategoryId"": 65,
    ""CategoryName"": ""CN614"",
    ""Code"": ""CN614CR04P"",
    ""ShortDescription"": ""4/40\"" stainless steel/primed"",
    ""Price"": ""3.2""
  },
  {
    ""ProductId"": 9,
    ""IsActive"": 1,
    ""CategoryId"": 65,
    ""CategoryName"": ""CN614"",
    ""Code"": ""CN614CR06P"",
    ""ShortDescription"": ""6/32\"" stainless steel/primed"",
    ""Price"": ""3.1""
  },
  {
    ""ProductId"": 194,
    ""IsActive"": 1,
    ""CategoryId"": 65,
    ""CategoryName"": ""CN614"",
    ""Code"": ""KA100E2CN614CR06P"",
    ""ShortDescription"": ""Kit, Expoxy Adhesive, Special\r\n6/32\"" stainless steel/primed"",
    ""Price"": ""395""
  },
  {
    ""ProductId"": 10,
    ""IsActive"": 1,
    ""CategoryId"": 65,
    ""CategoryName"": ""CN614"",
    ""Code"": ""CN614CR08P"",
    ""ShortDescription"": ""8/32\"" stainless steel/primed"",
    ""Price"": ""3.10""
  },
  {
    ""ProductId"": 11,
    ""IsActive"": 1,
    ""CategoryId"": 65,
    ""CategoryName"": ""CN614"",
    ""Code"": ""CN614CR3P"",
    ""ShortDescription"": ""10/32\"" stainless steel/primed"",
    ""Price"": ""3.1""
  },
  {
    ""ProductId"": 12,
    ""IsActive"": 1,
    ""CategoryId"": 65,
    ""CategoryName"": ""CN614"",
    ""Code"": ""CN614CR4P"",
    ""ShortDescription"": ""1/4-28\"" stainless steel/primed"",
    ""Price"": ""4.25""
  },
  {
    ""ProductId"": 13,
    ""IsActive"": 1,
    ""CategoryId"": 65,
    ""CategoryName"": ""CN614"",
    ""Code"": ""CN614CR5P"",
    ""ShortDescription"": ""5/16-24\"" stainless steel/primed"",
    ""Price"": ""7.65""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 405,
    ""CategoryName"": ""CN614 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 157,
    ""IsActive"": 1,
    ""CategoryId"": 69,
    ""CategoryName"": ""CN62"",
    ""Code"": ""CN62-440-5CR"",
    ""ShortDescription"": ""4/40-5/16\"" stainless steel"",
    ""Price"": ""3""
  },
  {
    ""ProductId"": 158,
    ""IsActive"": 1,
    ""CategoryId"": 69,
    ""CategoryName"": ""CN62"",
    ""Code"": ""CN62-440-6CR"",
    ""ShortDescription"": ""4/40-3/8\"" stainless steel"",
    ""Price"": ""3""
  },
  {
    ""ProductId"": 159,
    ""IsActive"": 1,
    ""CategoryId"": 69,
    ""CategoryName"": ""CN62"",
    ""Code"": ""CN62-440-8CR"",
    ""ShortDescription"": ""4/40-1/2\"" stainless steel"",
    ""Price"": ""3""
  },
  {
    ""ProductId"": 160,
    ""IsActive"": 1,
    ""CategoryId"": 69,
    ""CategoryName"": ""CN62"",
    ""Code"": ""CN62-440-10CR"",
    ""ShortDescription"": ""4/40-5/8\"" stainless steel"",
    ""Price"": ""3""
  },
  {
    ""ProductId"": 55,
    ""IsActive"": 1,
    ""CategoryId"": 69,
    ""CategoryName"": ""CN62"",
    ""Code"": ""CN62-1032-8CR"",
    ""ShortDescription"": ""10/32-1/2\"" stainless steel"",
    ""Price"": ""3.68""
  },
  {
    ""ProductId"": 161,
    ""IsActive"": 1,
    ""CategoryId"": 69,
    ""CategoryName"": ""CN62"",
    ""Code"": ""CN62-440-12CR"",
    ""ShortDescription"": ""4/40-3/4\"" stainless steel"",
    ""Price"": ""3""
  },
  {
    ""ProductId"": 53,
    ""IsActive"": 1,
    ""CategoryId"": 69,
    ""CategoryName"": ""CN62"",
    ""Code"": ""CN62-632-8CR"",
    ""ShortDescription"": ""6/32-1/2\"" stainless steel"",
    ""Price"": ""3""
  },
  {
    ""ProductId"": 162,
    ""IsActive"": 1,
    ""CategoryId"": 69,
    ""CategoryName"": ""CN62"",
    ""Code"": ""CN62-632-10CR"",
    ""ShortDescription"": ""6/32-5/8\"" stainless steel"",
    ""Price"": ""3""
  },
  {
    ""ProductId"": 163,
    ""IsActive"": 1,
    ""CategoryId"": 69,
    ""CategoryName"": ""CN62"",
    ""Code"": ""CN62-632-12CR"",
    ""ShortDescription"": ""6/32-3/4\"" stainless steel"",
    ""Price"": ""3""
  },
  {
    ""ProductId"": 54,
    ""IsActive"": 1,
    ""CategoryId"": 69,
    ""CategoryName"": ""CN62"",
    ""Code"": ""CN62-832-8CR"",
    ""ShortDescription"": ""8/32-1/2\"" stainless steel"",
    ""Price"": ""3""
  },
  {
    ""ProductId"": 164,
    ""IsActive"": 1,
    ""CategoryId"": 69,
    ""CategoryName"": ""CN62"",
    ""Code"": ""CN62-832-10CR"",
    ""ShortDescription"": ""8/32-5/8\"" stainless steel"",
    ""Price"": ""3""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 409,
    ""CategoryName"": ""CN62 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 92,
    ""IsActive"": 1,
    ""CategoryId"": 133,
    ""CategoryName"": ""CP125"",
    ""Code"": ""CP125-000-005-A1A1"",
    ""ShortDescription"": ""Aluminum/Anodized Flat Patch"",
    ""Price"": ""6""
  },
  {
    ""ProductId"": 175,
    ""IsActive"": 1,
    ""CategoryId"": 133,
    ""CategoryName"": ""CP125"",
    ""Code"": ""CP125-000-005-KA1A1"",
    ""ShortDescription"": ""Click Patch Kit (Includes CB92 Adhesive Kit)"",
    ""Price"": ""8.25""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 473,
    ""CategoryName"": ""CP125 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 93,
    ""IsActive"": 1,
    ""CategoryId"": 135,
    ""CategoryName"": ""CP125-375"",
    ""Code"": ""CP125-375-005-A2A1"",
    ""ShortDescription"": ""Aluminum/Anodized Medium Hat Patch"",
    ""Price"": ""7.5""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 475,
    ""CategoryName"": ""CP125-375 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 94,
    ""IsActive"": 1,
    ""CategoryId"": 136,
    ""CategoryName"": ""CP125-625"",
    ""Code"": ""CP125-625-005-A2A1"",
    ""ShortDescription"": ""Aluminum/Anodized Large Hat Patch"",
    ""Price"": ""8""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 476,
    ""CategoryName"": ""CP125-625 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 95,
    ""IsActive"": 1,
    ""CategoryId"": 134,
    ""CategoryName"": ""CP200"",
    ""Code"": ""CP200AA032"",
    ""ShortDescription"": ""Aluminum/Anodized Flat Patch"",
    ""Price"": ""7.5""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 474,
    ""CategoryName"": ""CP200 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 200,
    ""IsActive"": 1,
    ""CategoryId"": 639,
    ""CategoryName"": ""CP62"",
    ""Code"": ""CP62-000-005-A1A1"",
    ""ShortDescription"": """",
    ""Price"": ""4""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 472,
    ""CategoryName"": ""CP62 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 104,
    ""IsActive"": 1,
    ""CategoryId"": 296,
    ""CategoryName"": ""CS120"",
    ""Code"": ""CS120-105-24CR"",
    ""ShortDescription"": ""1 1/2\"" long/Stainless Steel"",
    ""Price"": ""1.5""
  },
  {
    ""ProductId"": 179,
    ""IsActive"": 1,
    ""CategoryId"": 296,
    ""CategoryName"": ""CS120"",
    ""Code"": ""CS120-105-40CR"",
    ""ShortDescription"": ""2 1/2\"" long/Stainless Steel"",
    ""Price"": ""1.5""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 610,
    ""CategoryName"": ""CS120 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 127,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-632-4CR"",
    ""ShortDescription"": ""6/32-1/4\"" long/stainless steel"",
    ""Price"": ""3""
  },
  {
    ""ProductId"": 34,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-632-8CR"",
    ""ShortDescription"": ""6/32-1/2\"" long/stainless steel"",
    ""Price"": ""3.18""
  },
  {
    ""ProductId"": 35,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-632-12CR"",
    ""ShortDescription"": ""6/32-3/4\"" long/stainless steel"",
    ""Price"": ""3""
  },
  {
    ""ProductId"": 128,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-632-12GCR"",
    ""ShortDescription"": ""6/32-3/4\"" long/composite base"",
    ""Price"": ""6.85""
  },
  {
    ""ProductId"": 36,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-632-16CR"",
    ""ShortDescription"": ""6/32-1\"" long/stainless steel"",
    ""Price"": ""3.19""
  },
  {
    ""ProductId"": 146,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1420-24CR"",
    ""ShortDescription"": ""1/4-20-1.5\"" long/stainless steel"",
    ""Price"": ""4.22""
  },
  {
    ""ProductId"": 147,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1428-8CR"",
    ""ShortDescription"": ""1/4-28-1/2\"" long/stainless steel"",
    ""Price"": ""3.88""
  },
  {
    ""ProductId"": 148,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1428-12CR"",
    ""ShortDescription"": ""1/4-28-3/4\"" long/stainless steel"",
    ""Price"": ""3.88""
  },
  {
    ""ProductId"": 149,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1428-16CR"",
    ""ShortDescription"": ""1/4-28-1\"" long/stainless steel"",
    ""Price"": ""3.88""
  },
  {
    ""ProductId"": 150,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1428-16GCR"",
    ""ShortDescription"": ""1/4-28-1\"" long/composite base"",
    ""Price"": ""6.85""
  },
  {
    ""ProductId"": 141,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1032-24GCR"",
    ""ShortDescription"": ""10/32-1.5\"" long/composite base"",
    ""Price"": ""6.85""
  },
  {
    ""ProductId"": 142,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1420-8CR"",
    ""ShortDescription"": ""1/4-20-1/2\"" long/stainless steel"",
    ""Price"": ""3.88""
  },
  {
    ""ProductId"": 143,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1420-10CR"",
    ""ShortDescription"": ""1/4-20-5/8\"" long/stainless steel"",
    ""Price"": ""3.88""
  },
  {
    ""ProductId"": 202,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1032-5GCR"",
    ""ShortDescription"": ""10/32-5/16\"" long/composite base"",
    ""Price"": ""6.85""
  },
  {
    ""ProductId"": 144,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1420-12CR"",
    ""ShortDescription"": ""1/4-20-3/4\"" long/stainless steel"",
    ""Price"": ""3.88""
  },
  {
    ""ProductId"": 145,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1420-16CR"",
    ""ShortDescription"": ""1/4-20-1\"" long/stainless steel"",
    ""Price"": ""3.88""
  },
  {
    ""ProductId"": 136,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1032-12GCR"",
    ""ShortDescription"": ""10/32-3/4\"" long/composite base"",
    ""Price"": ""6.85""
  },
  {
    ""ProductId"": 137,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1032-14CR"",
    ""ShortDescription"": ""10/32-7/8\"" long/stainless steel"",
    ""Price"": ""3.68""
  },
  {
    ""ProductId"": 138,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1032-14GCR"",
    ""ShortDescription"": ""10/32-7/8\"" long/composite base"",
    ""Price"": ""6.85""
  },
  {
    ""ProductId"": 42,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1032-16CR"",
    ""ShortDescription"": ""10/32-1\"" long/stainless steel"",
    ""Price"": ""3.68""
  },
  {
    ""ProductId"": 139,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1032-16GCR"",
    ""ShortDescription"": ""10/32-1\"" long/composite base"",
    ""Price"": ""6.85""
  },
  {
    ""ProductId"": 140,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1032-24CR"",
    ""ShortDescription"": ""10/32-1.5\"" long/stainless steel"",
    ""Price"": ""4.08""
  },
  {
    ""ProductId"": 39,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-832-16CR"",
    ""ShortDescription"": ""8/32-1\"" long/stainless steel"",
    ""Price"": ""3.19""
  },
  {
    ""ProductId"": 133,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1032-6CR"",
    ""ShortDescription"": ""10/32-3/8\"" long/stainless steel"",
    ""Price"": ""3.68""
  },
  {
    ""ProductId"": 40,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1032-8CR"",
    ""ShortDescription"": ""10/32-1/2\"" long/stainless steel"",
    ""Price"": ""3.68""
  },
  {
    ""ProductId"": 134,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1032-8GCR"",
    ""ShortDescription"": ""10/32-1/2\"" long/composite base"",
    ""Price"": ""6.85""
  },
  {
    ""ProductId"": 135,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1032-10CR"",
    ""ShortDescription"": ""10/32-5/8\"" long/stainless steel"",
    ""Price"": ""3.68""
  },
  {
    ""ProductId"": 41,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-1032-12CR"",
    ""ShortDescription"": ""10/32-3/4\"" long/stainless steel"",
    ""Price"": ""3.68""
  },
  {
    ""ProductId"": 129,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-632-24GCR"",
    ""ShortDescription"": ""6/32-1.5\"" long/composite base"",
    ""Price"": ""6.85""
  },
  {
    ""ProductId"": 37,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-832-8CR"",
    ""ShortDescription"": ""8/32-1/2\"" long/stainless steel"",
    ""Price"": ""3""
  },
  {
    ""ProductId"": 130,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-832-8GCR"",
    ""ShortDescription"": ""8/32-1/2\"" long/composite base"",
    ""Price"": ""6.85""
  },
  {
    ""ProductId"": 38,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-832-12CR"",
    ""ShortDescription"": ""8/32-3/4\"" long/stainless steel"",
    ""Price"": ""3""
  },
  {
    ""ProductId"": 131,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-832-12GCR"",
    ""ShortDescription"": ""8/32-3/4\"" long/composite base"",
    ""Price"": ""6.85""
  },
  {
    ""ProductId"": 132,
    ""IsActive"": 1,
    ""CategoryId"": 73,
    ""CategoryName"": ""CS125"",
    ""Code"": ""CS125-832-14GCR"",
    ""ShortDescription"": ""8/32-7/8\"" long/composite base"",
    ""Price"": ""6.85""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 413,
    ""CategoryName"": ""CS125 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 43,
    ""IsActive"": 1,
    ""CategoryId"": 74,
    ""CategoryName"": ""CS200"",
    ""Code"": ""CS200-1032-8CR"",
    ""ShortDescription"": ""10/32-1/2\"" long"",
    ""Price"": ""7.43""
  },
  {
    ""ProductId"": 44,
    ""IsActive"": 1,
    ""CategoryId"": 74,
    ""CategoryName"": ""CS200"",
    ""Code"": ""CS200-1032-12CR"",
    ""ShortDescription"": ""10/32-3/4\"" long"",
    ""Price"": ""7.43""
  },
  {
    ""ProductId"": 45,
    ""IsActive"": 1,
    ""CategoryId"": 74,
    ""CategoryName"": ""CS200"",
    ""Code"": ""CS200-1032-16CR"",
    ""ShortDescription"": ""10/32-1\"" long"",
    ""Price"": ""7.43""
  },
  {
    ""ProductId"": 151,
    ""IsActive"": 1,
    ""CategoryId"": 74,
    ""CategoryName"": ""CS200"",
    ""Code"": ""CS200-1420-8CR"",
    ""ShortDescription"": ""1/4-20-1/2\"" long"",
    ""Price"": ""7.63""
  },
  {
    ""ProductId"": 152,
    ""IsActive"": 1,
    ""CategoryId"": 74,
    ""CategoryName"": ""CS200"",
    ""Code"": ""CS200-1420-10CR"",
    ""ShortDescription"": ""1/4-20-1/2\"" long"",
    ""Price"": ""7.63""
  },
  {
    ""ProductId"": 48,
    ""IsActive"": 1,
    ""CategoryId"": 74,
    ""CategoryName"": ""CS200"",
    ""Code"": ""CS200-1428-16CR"",
    ""ShortDescription"": ""1/4-28-1\"" long"",
    ""Price"": ""7.63""
  },
  {
    ""ProductId"": 49,
    ""IsActive"": 1,
    ""CategoryId"": 74,
    ""CategoryName"": ""CS200"",
    ""Code"": ""CS200-3816-16CR"",
    ""ShortDescription"": ""3/8-16-1\"" long"",
    ""Price"": ""8.57""
  },
  {
    ""ProductId"": 50,
    ""IsActive"": 1,
    ""CategoryId"": 74,
    ""CategoryName"": ""CS200"",
    ""Code"": ""CS200-3816-24CR"",
    ""ShortDescription"": ""3/8-16-1.5\"" long"",
    ""Price"": ""8.57""
  },
  {
    ""ProductId"": 51,
    ""IsActive"": 1,
    ""CategoryId"": 74,
    ""CategoryName"": ""CS200"",
    ""Code"": ""CS200-51618-16CR"",
    ""ShortDescription"": ""5/16-18-1\"" long"",
    ""Price"": ""8.31""
  },
  {
    ""ProductId"": 52,
    ""IsActive"": 1,
    ""CategoryId"": 74,
    ""CategoryName"": ""CS200"",
    ""Code"": ""CS200-51618-24CR"",
    ""ShortDescription"": ""5/16-18-1.5\"" long"",
    ""Price"": ""8.96""
  },
  {
    ""ProductId"": 153,
    ""IsActive"": 1,
    ""CategoryId"": 74,
    ""CategoryName"": ""CS200"",
    ""Code"": ""CS200-1420-12CR"",
    ""ShortDescription"": ""1/4-20-3/4\"" long"",
    ""Price"": ""7.63""
  },
  {
    ""ProductId"": 154,
    ""IsActive"": 1,
    ""CategoryId"": 74,
    ""CategoryName"": ""CS200"",
    ""Code"": ""CS200-1420-24CR"",
    ""ShortDescription"": ""1/4-20-2\"" long"",
    ""Price"": ""7.97""
  },
  {
    ""ProductId"": 155,
    ""IsActive"": 1,
    ""CategoryId"": 74,
    ""CategoryName"": ""CS200"",
    ""Code"": ""CS200-1420-32CR"",
    ""ShortDescription"": ""1/4-20-2\"" long"",
    ""Price"": ""8.29""
  },
  {
    ""ProductId"": 156,
    ""IsActive"": 1,
    ""CategoryId"": 74,
    ""CategoryName"": ""CS200"",
    ""Code"": ""CS200-1420-40CR"",
    ""ShortDescription"": ""1/4-20-2.5\"" long"",
    ""Price"": ""8.97""
  },
  {
    ""ProductId"": 46,
    ""IsActive"": 1,
    ""CategoryId"": 74,
    ""CategoryName"": ""CS200"",
    ""Code"": ""CS200-1428-8CR"",
    ""ShortDescription"": ""1/4-28-1/2\"" long"",
    ""Price"": ""5.5""
  },
  {
    ""ProductId"": 47,
    ""IsActive"": 1,
    ""CategoryId"": 74,
    ""CategoryName"": ""CS200"",
    ""Code"": ""CS200-1428-12CR"",
    ""ShortDescription"": ""1/4-28-3/4\"" long"",
    ""Price"": ""5.5""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 629,
    ""CategoryName"": ""CS200 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 31,
    ""IsActive"": 1,
    ""CategoryId"": 72,
    ""CategoryName"": ""CS62"",
    ""Code"": ""CS62-1032-5CR"",
    ""ShortDescription"": ""10/32-5/16\"" long/stainless steel"",
    ""Price"": ""3.18""
  },
  {
    ""ProductId"": 32,
    ""IsActive"": 1,
    ""CategoryId"": 72,
    ""CategoryName"": ""CS62"",
    ""Code"": ""CS62-1032-8CR"",
    ""ShortDescription"": ""10/32-1/2\"" long/stainless steel"",
    ""Price"": ""3.18""
  },
  {
    ""ProductId"": 33,
    ""IsActive"": 1,
    ""CategoryId"": 72,
    ""CategoryName"": ""CS62"",
    ""Code"": ""CS62-1032-12CR"",
    ""ShortDescription"": ""10/32-3/4\"" long/stainless steel"",
    ""Price"": ""3.18""
  },
  {
    ""ProductId"": 126,
    ""IsActive"": 1,
    ""CategoryId"": 72,
    ""CategoryName"": ""CS62"",
    ""Code"": ""CS62-1032-16CR"",
    ""ShortDescription"": ""10/32-1\"" long/stainless steel"",
    ""Price"": ""3.18""
  },
  {
    ""ProductId"": 27,
    ""IsActive"": 1,
    ""CategoryId"": 72,
    ""CategoryName"": ""CS62"",
    ""Code"": ""CS62-632-12CR"",
    ""ShortDescription"": ""6/32-3/4\"" long/stainless steel"",
    ""Price"": ""2.5""
  },
  {
    ""ProductId"": 113,
    ""IsActive"": 1,
    ""CategoryId"": 72,
    ""CategoryName"": ""CS62"",
    ""Code"": ""CS62-632-16CR"",
    ""ShortDescription"": ""6/32-1\"" long/stainless steel"",
    ""Price"": ""2.69""
  },
  {
    ""ProductId"": 28,
    ""IsActive"": 1,
    ""CategoryId"": 72,
    ""CategoryName"": ""CS62"",
    ""Code"": ""CS62-832-4CR "",
    ""ShortDescription"": ""8/32-1/4\"" long/stainless steel"",
    ""Price"": ""2.5""
  },
  {
    ""ProductId"": 125,
    ""IsActive"": 1,
    ""CategoryId"": 72,
    ""CategoryName"": ""CS62"",
    ""Code"": ""CS62-832-6CR"",
    ""ShortDescription"": ""8/32-3/8\"" long/stainless steel"",
    ""Price"": ""2.5""
  },
  {
    ""ProductId"": 29,
    ""IsActive"": 1,
    ""CategoryId"": 72,
    ""CategoryName"": ""CS62"",
    ""Code"": ""CS62-832-8CR"",
    ""ShortDescription"": ""8/32-1/2\"" long/stainless steel"",
    ""Price"": ""2.5""
  },
  {
    ""ProductId"": 30,
    ""IsActive"": 1,
    ""CategoryId"": 72,
    ""CategoryName"": ""CS62"",
    ""Code"": ""CS62-832-12CR"",
    ""ShortDescription"": ""8/32-3/4\"" long/stainless steel"",
    ""Price"": ""2.5""
  },
  {
    ""ProductId"": 22,
    ""IsActive"": 1,
    ""CategoryId"": 72,
    ""CategoryName"": ""CS62"",
    ""Code"": ""CS62-440-4CR"",
    ""ShortDescription"": ""4/40-1/4\"" long/stainless steel"",
    ""Price"": ""2.5""
  },
  {
    ""ProductId"": 23,
    ""IsActive"": 1,
    ""CategoryId"": 72,
    ""CategoryName"": ""CS62"",
    ""Code"": ""CS62-440-8CR"",
    ""ShortDescription"": ""4/40-1/2\"" long/stainless steel"",
    ""Price"": ""2.5""
  },
  {
    ""ProductId"": 24,
    ""IsActive"": 1,
    ""CategoryId"": 72,
    ""CategoryName"": ""CS62"",
    ""Code"": ""CS62-440-12CR"",
    ""ShortDescription"": ""4/40-3/4\"" long/stainless steel"",
    ""Price"": ""2.5""
  },
  {
    ""ProductId"": 25,
    ""IsActive"": 1,
    ""CategoryId"": 72,
    ""CategoryName"": ""CS62"",
    ""Code"": ""CS62-632-4CR"",
    ""ShortDescription"": ""6/32-1/4\"" long/stainless steel"",
    ""Price"": ""2.5""
  },
  {
    ""ProductId"": 124,
    ""IsActive"": 1,
    ""CategoryId"": 72,
    ""CategoryName"": ""CS62"",
    ""Code"": ""CS62-632-6CR"",
    ""ShortDescription"": ""6/32-3/8\"" long/stainless steel"",
    ""Price"": ""2.5""
  },
  {
    ""ProductId"": 26,
    ""IsActive"": 1,
    ""CategoryId"": 72,
    ""CategoryName"": ""CS62"",
    ""Code"": ""CS62-632-8CR"",
    ""ShortDescription"": ""6/32-1/2\"" long/stainless steel"",
    ""Price"": ""2.5""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 412,
    ""CategoryName"": ""CS62 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 456,
    ""CategoryName"": ""JK5501 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 462,
    ""CategoryName"": ""JK5502 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 467,
    ""CategoryName"": ""JK5506 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 460,
    ""CategoryName"": ""JK5507 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 458,
    ""CategoryName"": ""JK5541 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 463,
    ""CategoryName"": ""JK5542 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 465,
    ""CategoryName"": ""JK5542 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 469,
    ""CategoryName"": ""JK5546 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 461,
    ""CategoryName"": ""JK5547 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 457,
    ""CategoryName"": ""JK5701 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 464,
    ""CategoryName"": ""JK5702 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 468,
    ""CategoryName"": ""JK5706 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 459,
    ""CategoryName"": ""JK5721 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 466,
    ""CategoryName"": ""JK5722 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 470,
    ""CategoryName"": ""JK5726 - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  },
  {
    ""ProductId"": 112,
    ""IsActive"": 1,
    ""CategoryId"": 439,
    ""CategoryName"": ""KA KIT - RFQ"",
    ""Code"": ""Special"",
    ""ShortDescription"": null,
    ""Price"": ""0""
  }
]
";
                var productList = JsonConvert.DeserializeObject<IEnumerable<ProductCategoryMapping>>(productListJson);

                //var categoriesPath = Path.Combine("product-static-files", "json", "dbexport-oct-19", "categories.json");
                //var categorieslistJson = File.ReadAllText(categoriesPath);
                var categorieslistJson = @"
[
  {
    ""uid"": 5,
    ""IsActive"": 1,
    ""ParentID"": 2,
    ""Name"": ""Nutplates""
  },
  {
    ""uid"": 6,
    ""IsActive"": 1,
    ""ParentID"": 5,
    ""Name"": ""Replaceable Element NutPlates""
  },
  {
    ""uid"": 7,
    ""IsActive"": 1,
    ""ParentID"": 5,
    ""Name"": ""Composite Base Nutplates""
  },
  {
    ""uid"": 8,
    ""IsActive"": 1,
    ""ParentID"": 5,
    ""Name"": ""Miniature Nutpates""
  },
  {
    ""uid"": 9,
    ""IsActive"": 1,
    ""ParentID"": 5,
    ""Name"": ""Sealed Nutplates""
  },
  {
    ""uid"": 10,
    ""IsActive"": 1,
    ""ParentID"": 5,
    ""Name"": ""Sleeved Nutplates""
  },
  {
    ""uid"": 11,
    ""IsActive"": 1,
    ""ParentID"": 5,
    ""Name"": ""FLEX (Fatigue LIfe Extension) Cold-Work Nutplates""
  },
  {
    ""uid"": 12,
    ""IsActive"": 1,
    ""ParentID"": 5,
    ""Name"": ""Quarter Turn Receptacle""
  },
  {
    ""uid"": 13,
    ""IsActive"": 1,
    ""ParentID"": 2,
    ""Name"": ""Studs""
  },
  {
    ""uid"": 14,
    ""IsActive"": 1,
    ""ParentID"": 13,
    ""Name"": ""Metallic Base Studs""
  },
  {
    ""uid"": 15,
    ""IsActive"": 1,
    ""ParentID"": 13,
    ""Name"": ""Composite Base Studs""
  },
  {
    ""uid"": 16,
    ""IsActive"": 1,
    ""ParentID"": 13,
    ""Name"": ""Accessories""
  },
  {
    ""uid"": 21,
    ""IsActive"": 1,
    ""ParentID"": 2,
    ""Name"": ""Standoffs""
  },
  {
    ""uid"": 22,
    ""IsActive"": 1,
    ""ParentID"": 21,
    ""Name"": ""Metallic Base Standoffs""
  },
  {
    ""uid"": 23,
    ""IsActive"": 1,
    ""ParentID"": 21,
    ""Name"": ""Composite Base Standoffs""
  },
  {
    ""uid"": 24,
    ""IsActive"": 1,
    ""ParentID"": 21,
    ""Name"": ""Accessories""
  },
  {
    ""uid"": 25,
    ""IsActive"": 1,
    ""ParentID"": 2,
    ""Name"": ""Mounts / Cable Ties""
  },
  {
    ""uid"": 26,
    ""IsActive"": 1,
    ""ParentID"": 25,
    ""Name"": ""Pinch Mounts Threaded Mounts""
  },
  {
    ""uid"": 27,
    ""IsActive"": 1,
    ""ParentID"": 25,
    ""Name"": ""Threaded Mounts""
  },
  {
    ""uid"": 28,
    ""IsActive"": 1,
    ""ParentID"": 25,
    ""Name"": ""Cable Tie Mounts""
  },
  {
    ""uid"": 29,
    ""IsActive"": 1,
    ""ParentID"": 25,
    ""Name"": ""Cable Ties""
  },
  {
    ""uid"": 30,
    ""IsActive"": 1,
    ""ParentID"": 25,
    ""Name"": ""Insulation Blanket Mounts""
  },
  {
    ""uid"": 31,
    ""IsActive"": 1,
    ""ParentID"": 25,
    ""Name"": ""Other Mounts""
  },
  {
    ""uid"": 32,
    ""IsActive"": 1,
    ""ParentID"": 25,
    ""Name"": ""Accessories""
  },
  {
    ""uid"": 33,
    ""IsActive"": 1,
    ""ParentID"": 2,
    ""Name"": ""Bushings / Inserts""
  },
  {
    ""uid"": 34,
    ""IsActive"": 1,
    ""ParentID"": 33,
    ""Name"": ""Bushing""
  },
  {
    ""uid"": 35,
    ""IsActive"": 1,
    ""ParentID"": 33,
    ""Name"": ""Inserts""
  },
  {
    ""uid"": 36,
    ""IsActive"": 1,
    ""ParentID"": 2,
    ""Name"": ""Click Patch""
  },
  {
    ""uid"": 37,
    ""IsActive"": 1,
    ""ParentID"": 36,
    ""Name"": ""Click Patch""
  },
  {
    ""uid"": 38,
    ""IsActive"": 0,
    ""ParentID"": 2,
    ""Name"": ""Sleeves""
  },
  {
    ""uid"": 39,
    ""IsActive"": 1,
    ""ParentID"": 38,
    ""Name"": ""PANDOR Clearance Fit Sleeves for Hole Protection""
  },
  {
    ""uid"": 40,
    ""IsActive"": 1,
    ""ParentID"": 38,
    ""Name"": ""HOLEMOD Interferance Fit Sleeves for Oversize Fastener""
  },
  {
    ""uid"": 41,
    ""IsActive"": 1,
    ""ParentID"": 38,
    ""Name"": ""FLEX (Fatigue Life Extension) Cold-Work Services""
  },
  {
    ""uid"": 42,
    ""IsActive"": 0,
    ""ParentID"": 2,
    ""Name"": ""ACP (Advanced Composite Products)""
  },
  {
    ""uid"": 43,
    ""IsActive"": 1,
    ""ParentID"": 42,
    ""Name"": ""Composite Bolts""
  },
  {
    ""uid"": 44,
    ""IsActive"": 1,
    ""ParentID"": 42,
    ""Name"": ""Composite Screws""
  },
  {
    ""uid"": 45,
    ""IsActive"": 1,
    ""ParentID"": 42,
    ""Name"": ""Composite Nutplate""
  },
  {
    ""uid"": 46,
    ""IsActive"": 1,
    ""ParentID"": 42,
    ""Name"": ""Composite Frangible Nut""
  },
  {
    ""uid"": 47,
    ""IsActive"": 1,
    ""ParentID"": 42,
    ""Name"": ""Composite Nuts""
  },
  {
    ""uid"": 48,
    ""IsActive"": 1,
    ""ParentID"": 2,
    ""Name"": ""Adhesives and Adhesives Kits""
  },
  {
    ""uid"": 49,
    ""IsActive"": 1,
    ""ParentID"": 48,
    ""Name"": ""Adhesive Packets and Cartridges""
  },
  {
    ""uid"": 50,
    ""IsActive"": 1,
    ""ParentID"": 48,
    ""Name"": ""Adhesive Dispensers""
  },
  {
    ""uid"": 51,
    ""IsActive"": 0,
    ""ParentID"": 48,
    ""Name"": ""Adhesive Kits""
  },
  {
    ""uid"": 52,
    ""IsActive"": 1,
    ""ParentID"": 48,
    ""Name"": ""Adhesives Mixing Tips""
  },
  {
    ""uid"": 53,
    ""IsActive"": 1,
    ""ParentID"": 2,
    ""Name"": ""Tools and Accessories""
  },
  {
    ""uid"": 54,
    ""IsActive"": 1,
    ""ParentID"": 53,
    ""Name"": ""Tools and Accessories""
  },
  {
    ""uid"": 55,
    ""IsActive"": 1,
    ""ParentID"": 2,
    ""Name"": ""General Aviation / Industrial / Commercial""
  },
  {
    ""uid"": 56,
    ""IsActive"": 1,
    ""ParentID"": 55,
    ""Name"": ""Bushing""
  },
  {
    ""uid"": 57,
    ""IsActive"": 1,
    ""ParentID"": 55,
    ""Name"": ""Nutplates""
  },
  {
    ""uid"": 58,
    ""IsActive"": 1,
    ""ParentID"": 55,
    ""Name"": ""Standoffs""
  },
  {
    ""uid"": 59,
    ""IsActive"": 1,
    ""ParentID"": 55,
    ""Name"": ""Studs""
  },
  {
    ""uid"": 60,
    ""IsActive"": 1,
    ""ParentID"": 57,
    ""Name"": ""CN109""
  },
  {
    ""uid"": 61,
    ""IsActive"": 1,
    ""ParentID"": 57,
    ""Name"": ""CN111""
  },
  {
    ""uid"": 62,
    ""IsActive"": 1,
    ""ParentID"": 57,
    ""Name"": ""CN609""
  },
  {
    ""uid"": 63,
    ""IsActive"": 1,
    ""ParentID"": 57,
    ""Name"": ""CN610""
  },
  {
    ""uid"": 64,
    ""IsActive"": 1,
    ""ParentID"": 57,
    ""Name"": ""CN611""
  },
  {
    ""uid"": 65,
    ""IsActive"": 1,
    ""ParentID"": 57,
    ""Name"": ""CN614""
  },
  {
    ""uid"": 66,
    ""IsActive"": 1,
    ""ParentID"": 56,
    ""Name"": ""CN505""
  },
  {
    ""uid"": 67,
    ""IsActive"": 1,
    ""ParentID"": 56,
    ""Name"": ""CN555""
  },
  {
    ""uid"": 68,
    ""IsActive"": 1,
    ""ParentID"": 56,
    ""Name"": ""CN305""
  },
  {
    ""uid"": 69,
    ""IsActive"": 1,
    ""ParentID"": 58,
    ""Name"": ""CN62""
  },
  {
    ""uid"": 70,
    ""IsActive"": 1,
    ""ParentID"": 58,
    ""Name"": ""CN125""
  },
  {
    ""uid"": 71,
    ""IsActive"": 1,
    ""ParentID"": 58,
    ""Name"": ""CN200""
  },
  {
    ""uid"": 72,
    ""IsActive"": 1,
    ""ParentID"": 59,
    ""Name"": ""CS62""
  },
  {
    ""uid"": 73,
    ""IsActive"": 1,
    ""ParentID"": 59,
    ""Name"": ""CS125""
  },
  {
    ""uid"": 74,
    ""IsActive"": 1,
    ""ParentID"": 59,
    ""Name"": ""CS200""
  },
  {
    ""uid"": 75,
    ""IsActive"": 1,
    ""ParentID"": 54,
    ""Name"": ""CB124""
  },
  {
    ""uid"": 76,
    ""IsActive"": 1,
    ""ParentID"": 54,
    ""Name"": ""CB602""
  },
  {
    ""uid"": 77,
    ""IsActive"": 1,
    ""ParentID"": 54,
    ""Name"": ""CB603""
  },
  {
    ""uid"": 78,
    ""IsActive"": 1,
    ""ParentID"": 54,
    ""Name"": ""CB607""
  },
  {
    ""uid"": 79,
    ""IsActive"": 1,
    ""ParentID"": 54,
    ""Name"": ""CB609""
  },
  {
    ""uid"": 80,
    ""IsActive"": 1,
    ""ParentID"": 54,
    ""Name"": ""CB625""
  },
  {
    ""uid"": 81,
    ""IsActive"": 1,
    ""ParentID"": 54,
    ""Name"": ""CB629""
  },
  {
    ""uid"": 82,
    ""IsActive"": 1,
    ""ParentID"": 54,
    ""Name"": ""CB750""
  },
  {
    ""uid"": 83,
    ""IsActive"": 1,
    ""ParentID"": 54,
    ""Name"": ""CB751""
  },
  {
    ""uid"": 84,
    ""IsActive"": 1,
    ""ParentID"": 54,
    ""Name"": ""CB752""
  },
  {
    ""uid"": 85,
    ""IsActive"": 1,
    ""ParentID"": 54,
    ""Name"": ""CB753""
  },
  {
    ""uid"": 86,
    ""IsActive"": 1,
    ""ParentID"": 54,
    ""Name"": ""CB9032""
  },
  {
    ""uid"": 87,
    ""IsActive"": 1,
    ""ParentID"": 54,
    ""Name"": ""CB9268""
  },
  {
    ""uid"": 88,
    ""IsActive"": 1,
    ""ParentID"": 49,
    ""Name"": ""CB200-40""
  },
  {
    ""uid"": 89,
    ""IsActive"": 1,
    ""ParentID"": 49,
    ""Name"": ""CB250-50""
  },
  {
    ""uid"": 90,
    ""IsActive"": 1,
    ""ParentID"": 49,
    ""Name"": ""CB309-50""
  },
  {
    ""uid"": 91,
    ""IsActive"": 1,
    ""ParentID"": 49,
    ""Name"": ""CB359-50""
  },
  {
    ""uid"": 92,
    ""IsActive"": 1,
    ""ParentID"": 49,
    ""Name"": ""CB394-43""
  },
  {
    ""uid"": 93,
    ""IsActive"": 1,
    ""ParentID"": 50,
    ""Name"": ""CB100""
  },
  {
    ""uid"": 94,
    ""IsActive"": 1,
    ""ParentID"": 51,
    ""Name"": ""CB91""
  },
  {
    ""uid"": 95,
    ""IsActive"": 1,
    ""ParentID"": 51,
    ""Name"": ""CB92""
  },
  {
    ""uid"": 96,
    ""IsActive"": 1,
    ""ParentID"": 51,
    ""Name"": ""CB93""
  },
  {
    ""uid"": 97,
    ""IsActive"": 0,
    ""ParentID"": 51,
    ""Name"": ""CB94""
  },
  {
    ""uid"": 98,
    ""IsActive"": 1,
    ""ParentID"": 51,
    ""Name"": ""CB96""
  },
  {
    ""uid"": 99,
    ""IsActive"": 1,
    ""ParentID"": 51,
    ""Name"": ""CB911""
  },
  {
    ""uid"": 100,
    ""IsActive"": 1,
    ""ParentID"": 51,
    ""Name"": ""KA KIT""
  },
  {
    ""uid"": 101,
    ""IsActive"": 1,
    ""ParentID"": 52,
    ""Name"": ""CB106""
  },
  {
    ""uid"": 102,
    ""IsActive"": 1,
    ""ParentID"": 43,
    ""Name"": ""CB1906""
  },
  {
    ""uid"": 103,
    ""IsActive"": 1,
    ""ParentID"": 43,
    ""Name"": ""CB9308""
  },
  {
    ""uid"": 104,
    ""IsActive"": 1,
    ""ParentID"": 43,
    ""Name"": ""CB9366""
  },
  {
    ""uid"": 105,
    ""IsActive"": 1,
    ""ParentID"": 44,
    ""Name"": ""CB1901""
  },
  {
    ""uid"": 106,
    ""IsActive"": 1,
    ""ParentID"": 44,
    ""Name"": ""CB1907""
  },
  {
    ""uid"": 108,
    ""IsActive"": 1,
    ""ParentID"": 46,
    ""Name"": ""CB1916""
  },
  {
    ""uid"": 109,
    ""IsActive"": 1,
    ""ParentID"": 47,
    ""Name"": ""CB9080""
  },
  {
    ""uid"": 110,
    ""IsActive"": 1,
    ""ParentID"": 47,
    ""Name"": ""CB9081""
  },
  {
    ""uid"": 111,
    ""IsActive"": 1,
    ""ParentID"": 39,
    ""Name"": ""CB5907""
  },
  {
    ""uid"": 112,
    ""IsActive"": 1,
    ""ParentID"": 39,
    ""Name"": ""CB5947""
  },
  {
    ""uid"": 113,
    ""IsActive"": 1,
    ""ParentID"": 39,
    ""Name"": ""CB5906""
  },
  {
    ""uid"": 114,
    ""IsActive"": 1,
    ""ParentID"": 39,
    ""Name"": ""CB5946""
  },
  {
    ""uid"": 115,
    ""IsActive"": 1,
    ""ParentID"": 39,
    ""Name"": ""CB5908""
  },
  {
    ""uid"": 116,
    ""IsActive"": 1,
    ""ParentID"": 39,
    ""Name"": ""CB5948""
  },
  {
    ""uid"": 117,
    ""IsActive"": 1,
    ""ParentID"": 40,
    ""Name"": ""JK5501""
  },
  {
    ""uid"": 118,
    ""IsActive"": 1,
    ""ParentID"": 40,
    ""Name"": ""JK5701""
  },
  {
    ""uid"": 119,
    ""IsActive"": 1,
    ""ParentID"": 40,
    ""Name"": ""JK5541""
  },
  {
    ""uid"": 120,
    ""IsActive"": 1,
    ""ParentID"": 40,
    ""Name"": ""JK5721""
  },
  {
    ""uid"": 121,
    ""IsActive"": 1,
    ""ParentID"": 40,
    ""Name"": ""JK5507""
  },
  {
    ""uid"": 122,
    ""IsActive"": 1,
    ""ParentID"": 40,
    ""Name"": ""JK5547""
  },
  {
    ""uid"": 123,
    ""IsActive"": 1,
    ""ParentID"": 40,
    ""Name"": ""JK5502""
  },
  {
    ""uid"": 124,
    ""IsActive"": 1,
    ""ParentID"": 40,
    ""Name"": ""JK5702""
  },
  {
    ""uid"": 125,
    ""IsActive"": 1,
    ""ParentID"": 40,
    ""Name"": ""JK5542""
  },
  {
    ""uid"": 126,
    ""IsActive"": 1,
    ""ParentID"": 40,
    ""Name"": ""JK5722""
  },
  {
    ""uid"": 127,
    ""IsActive"": 1,
    ""ParentID"": 40,
    ""Name"": ""JK5506""
  },
  {
    ""uid"": 128,
    ""IsActive"": 1,
    ""ParentID"": 40,
    ""Name"": ""JK5706""
  },
  {
    ""uid"": 129,
    ""IsActive"": 1,
    ""ParentID"": 40,
    ""Name"": ""JK5546""
  },
  {
    ""uid"": 130,
    ""IsActive"": 1,
    ""ParentID"": 40,
    ""Name"": ""JK5726""
  },
  {
    ""uid"": 131,
    ""IsActive"": 1,
    ""ParentID"": 41,
    ""Name"": ""CB9296""
  },
  {
    ""uid"": 132,
    ""IsActive"": 1,
    ""ParentID"": 37,
    ""Name"": ""CP62""
  },
  {
    ""uid"": 133,
    ""IsActive"": 1,
    ""ParentID"": 37,
    ""Name"": ""CP125""
  },
  {
    ""uid"": 134,
    ""IsActive"": 1,
    ""ParentID"": 37,
    ""Name"": ""CP200""
  },
  {
    ""uid"": 135,
    ""IsActive"": 1,
    ""ParentID"": 37,
    ""Name"": ""CP125-375""
  },
  {
    ""uid"": 136,
    ""IsActive"": 1,
    ""ParentID"": 37,
    ""Name"": ""CP125-625""
  },
  {
    ""uid"": 137,
    ""IsActive"": 1,
    ""ParentID"": 34,
    ""Name"": ""CB5007""
  },
  {
    ""uid"": 138,
    ""IsActive"": 1,
    ""ParentID"": 34,
    ""Name"": ""CB9122""
  },
  {
    ""uid"": 139,
    ""IsActive"": 1,
    ""ParentID"": 34,
    ""Name"": ""CB5005""
  },
  {
    ""uid"": 140,
    ""IsActive"": 1,
    ""ParentID"": 34,
    ""Name"": ""CB5006""
  },
  {
    ""uid"": 141,
    ""IsActive"": 1,
    ""ParentID"": 34,
    ""Name"": ""CB3005""
  },
  {
    ""uid"": 142,
    ""IsActive"": 1,
    ""ParentID"": 34,
    ""Name"": ""CB9084""
  },
  {
    ""uid"": 143,
    ""IsActive"": 1,
    ""ParentID"": 34,
    ""Name"": ""CB9085""
  },
  {
    ""uid"": 145,
    ""IsActive"": 1,
    ""ParentID"": 34,
    ""Name"": ""CB9112""
  },
  {
    ""uid"": 146,
    ""IsActive"": 1,
    ""ParentID"": 34,
    ""Name"": ""CB9029""
  },
  {
    ""uid"": 147,
    ""IsActive"": 1,
    ""ParentID"": 34,
    ""Name"": ""CB9077""
  },
  {
    ""uid"": 148,
    ""IsActive"": 1,
    ""ParentID"": 34,
    ""Name"": ""CB9061""
  },
  {
    ""uid"": 152,
    ""IsActive"": 1,
    ""ParentID"": 34,
    ""Name"": ""CB3005""
  },
  {
    ""uid"": 157,
    ""IsActive"": 1,
    ""ParentID"": 9,
    ""Name"": ""CB9176""
  },
  {
    ""uid"": 176,
    ""IsActive"": 1,
    ""ParentID"": 35,
    ""Name"": ""CB9312""
  },
  {
    ""uid"": 177,
    ""IsActive"": 1,
    ""ParentID"": 35,
    ""Name"": ""CB9060""
  },
  {
    ""uid"": 178,
    ""IsActive"": 1,
    ""ParentID"": 35,
    ""Name"": ""CB9101""
  },
  {
    ""uid"": 179,
    ""IsActive"": 1,
    ""ParentID"": 26,
    ""Name"": ""CB4233""
  },
  {
    ""uid"": 180,
    ""IsActive"": 1,
    ""ParentID"": 26,
    ""Name"": ""CB4132""
  },
  {
    ""uid"": 181,
    ""IsActive"": 1,
    ""ParentID"": 7,
    ""Name"": ""CB4022""
  },
  {
    ""uid"": 182,
    ""IsActive"": 1,
    ""ParentID"": 7,
    ""Name"": ""CB4023""
  },
  {
    ""uid"": 183,
    ""IsActive"": 1,
    ""ParentID"": 27,
    ""Name"": ""CB4014""
  },
  {
    ""uid"": 184,
    ""IsActive"": 1,
    ""ParentID"": 28,
    ""Name"": ""CB3019""
  },
  {
    ""uid"": 188,
    ""IsActive"": 1,
    ""ParentID"": 28,
    ""Name"": ""CB4019""
  },
  {
    ""uid"": 189,
    ""IsActive"": 1,
    ""ParentID"": 28,
    ""Name"": ""CB4020""
  },
  {
    ""uid"": 190,
    ""IsActive"": 1,
    ""ParentID"": 28,
    ""Name"": ""CB4021""
  },
  {
    ""uid"": 191,
    ""IsActive"": 1,
    ""ParentID"": 28,
    ""Name"": ""CB4013""
  },
  {
    ""uid"": 192,
    ""IsActive"": 1,
    ""ParentID"": 28,
    ""Name"": ""CB4024""
  },
  {
    ""uid"": 193,
    ""IsActive"": 1,
    ""ParentID"": 28,
    ""Name"": ""CB9120""
  },
  {
    ""uid"": 194,
    ""IsActive"": 1,
    ""ParentID"": 28,
    ""Name"": ""CB9151""
  },
  {
    ""uid"": 195,
    ""IsActive"": 1,
    ""ParentID"": 28,
    ""Name"": ""CB9302""
  },
  {
    ""uid"": 196,
    ""IsActive"": 1,
    ""ParentID"": 28,
    ""Name"": ""CB9257""
  },
  {
    ""uid"": 197,
    ""IsActive"": 1,
    ""ParentID"": 29,
    ""Name"": ""CB9519""
  },
  {
    ""uid"": 198,
    ""IsActive"": 1,
    ""ParentID"": 29,
    ""Name"": ""CB9459""
  },
  {
    ""uid"": 199,
    ""IsActive"": 1,
    ""ParentID"": 29,
    ""Name"": ""CB9513P5""
  },
  {
    ""uid"": 200,
    ""IsActive"": 1,
    ""ParentID"": 30,
    ""Name"": ""CB9170""
  },
  {
    ""uid"": 201,
    ""IsActive"": 1,
    ""ParentID"": 30,
    ""Name"": ""CB9173""
  },
  {
    ""uid"": 202,
    ""IsActive"": 1,
    ""ParentID"": 30,
    ""Name"": ""CB9206""
  },
  {
    ""uid"": 203,
    ""IsActive"": 1,
    ""ParentID"": 30,
    ""Name"": ""CB9210""
  },
  {
    ""uid"": 204,
    ""IsActive"": 1,
    ""ParentID"": 30,
    ""Name"": ""CB9174""
  },
  {
    ""uid"": 205,
    ""IsActive"": 1,
    ""ParentID"": 30,
    ""Name"": ""CB9208""
  },
  {
    ""uid"": 206,
    ""IsActive"": 1,
    ""ParentID"": 30,
    ""Name"": ""CB9322""
  },
  {
    ""uid"": 207,
    ""IsActive"": 1,
    ""ParentID"": 31,
    ""Name"": ""CB4034""
  },
  {
    ""uid"": 208,
    ""IsActive"": 1,
    ""ParentID"": 31,
    ""Name"": ""CB4035""
  },
  {
    ""uid"": 209,
    ""IsActive"": 1,
    ""ParentID"": 31,
    ""Name"": ""CB9201""
  },
  {
    ""uid"": 210,
    ""IsActive"": 1,
    ""ParentID"": 31,
    ""Name"": ""CB9514""
  },
  {
    ""uid"": 211,
    ""IsActive"": 1,
    ""ParentID"": 31,
    ""Name"": ""CB9205""
  },
  {
    ""uid"": 212,
    ""IsActive"": 1,
    ""ParentID"": 31,
    ""Name"": ""CB9297""
  },
  {
    ""uid"": 213,
    ""IsActive"": 1,
    ""ParentID"": 32,
    ""Name"": ""CB9421""
  },
  {
    ""uid"": 214,
    ""IsActive"": 1,
    ""ParentID"": 32,
    ""Name"": ""CB9422""
  },
  {
    ""uid"": 215,
    ""IsActive"": 1,
    ""ParentID"": 32,
    ""Name"": ""CB9532""
  },
  {
    ""uid"": 216,
    ""IsActive"": 1,
    ""ParentID"": 24,
    ""Name"": ""CB9421""
  },
  {
    ""uid"": 217,
    ""IsActive"": 1,
    ""ParentID"": 24,
    ""Name"": ""CB9422""
  },
  {
    ""uid"": 218,
    ""IsActive"": 1,
    ""ParentID"": 23,
    ""Name"": ""CB4002""
  },
  {
    ""uid"": 219,
    ""IsActive"": 1,
    ""ParentID"": 23,
    ""Name"": ""CB4014""
  },
  {
    ""uid"": 220,
    ""IsActive"": 1,
    ""ParentID"": 23,
    ""Name"": ""CB4001""
  },
  {
    ""uid"": 221,
    ""IsActive"": 1,
    ""ParentID"": 23,
    ""Name"": ""CB4004""
  },
  {
    ""uid"": 222,
    ""IsActive"": 1,
    ""ParentID"": 23,
    ""Name"": ""CB4018""
  },
  {
    ""uid"": 223,
    ""IsActive"": 1,
    ""ParentID"": 23,
    ""Name"": ""CB4201""
  },
  {
    ""uid"": 224,
    ""IsActive"": 1,
    ""ParentID"": 23,
    ""Name"": ""CB4033""
  },
  {
    ""uid"": 225,
    ""IsActive"": 1,
    ""ParentID"": 22,
    ""Name"": ""CB5001""
  },
  {
    ""uid"": 226,
    ""IsActive"": 1,
    ""ParentID"": 22,
    ""Name"": ""CB5004""
  },
  {
    ""uid"": 227,
    ""IsActive"": 1,
    ""ParentID"": 22,
    ""Name"": ""CB2001""
  },
  {
    ""uid"": 228,
    ""IsActive"": 1,
    ""ParentID"": 22,
    ""Name"": ""CB3001""
  },
  {
    ""uid"": 229,
    ""IsActive"": 1,
    ""ParentID"": 22,
    ""Name"": ""CB3004""
  },
  {
    ""uid"": 230,
    ""IsActive"": 1,
    ""ParentID"": 22,
    ""Name"": ""CB3201""
  },
  {
    ""uid"": 231,
    ""IsActive"": 1,
    ""ParentID"": 22,
    ""Name"": ""CB3033""
  },
  {
    ""uid"": 232,
    ""IsActive"": 1,
    ""ParentID"": 16,
    ""Name"": ""CB9188""
  },
  {
    ""uid"": 233,
    ""IsActive"": 1,
    ""ParentID"": 16,
    ""Name"": ""CB9212""
  },
  {
    ""uid"": 234,
    ""IsActive"": 1,
    ""ParentID"": 15,
    ""Name"": ""CB4000""
  },
  {
    ""uid"": 235,
    ""IsActive"": 1,
    ""ParentID"": 15,
    ""Name"": ""CB4017""
  },
  {
    ""uid"": 236,
    ""IsActive"": 1,
    ""ParentID"": 15,
    ""Name"": ""CB4200""
  },
  {
    ""uid"": 237,
    ""IsActive"": 1,
    ""ParentID"": 15,
    ""Name"": ""CB9159""
  },
  {
    ""uid"": 238,
    ""IsActive"": 1,
    ""ParentID"": 15,
    ""Name"": ""CB4101""
  },
  {
    ""uid"": 239,
    ""IsActive"": 1,
    ""ParentID"": 15,
    ""Name"": ""CB4003""
  },
  {
    ""uid"": 240,
    ""IsActive"": 1,
    ""ParentID"": 15,
    ""Name"": ""CB4005""
  },
  {
    ""uid"": 241,
    ""IsActive"": 1,
    ""ParentID"": 14,
    ""Name"": ""CB5000""
  },
  {
    ""uid"": 242,
    ""IsActive"": 1,
    ""ParentID"": 14,
    ""Name"": ""CB2000""
  },
  {
    ""uid"": 243,
    ""IsActive"": 1,
    ""ParentID"": 14,
    ""Name"": ""CB3000""
  },
  {
    ""uid"": 244,
    ""IsActive"": 1,
    ""ParentID"": 14,
    ""Name"": ""CB3200""
  },
  {
    ""uid"": 245,
    ""IsActive"": 1,
    ""ParentID"": 14,
    ""Name"": ""CB3021""
  },
  {
    ""uid"": 246,
    ""IsActive"": 1,
    ""ParentID"": 14,
    ""Name"": ""CB5003""
  },
  {
    ""uid"": 247,
    ""IsActive"": 1,
    ""ParentID"": 14,
    ""Name"": ""CB3003""
  },
  {
    ""uid"": 249,
    ""IsActive"": 1,
    ""ParentID"": 11,
    ""Name"": ""CB6307""
  },
  {
    ""uid"": 250,
    ""IsActive"": 1,
    ""ParentID"": 11,
    ""Name"": ""CB6347""
  },
  {
    ""uid"": 251,
    ""IsActive"": 1,
    ""ParentID"": 11,
    ""Name"": ""CB6311""
  },
  {
    ""uid"": 252,
    ""IsActive"": 1,
    ""ParentID"": 11,
    ""Name"": ""CB6309""
  },
  {
    ""uid"": 253,
    ""IsActive"": 1,
    ""ParentID"": 11,
    ""Name"": ""CB6349""
  },
  {
    ""uid"": 254,
    ""IsActive"": 1,
    ""ParentID"": 11,
    ""Name"": ""CB9382""
  },
  {
    ""uid"": 255,
    ""IsActive"": 1,
    ""ParentID"": 11,
    ""Name"": ""CB9392""
  },
  {
    ""uid"": 256,
    ""IsActive"": 1,
    ""ParentID"": 11,
    ""Name"": ""CB6310""
  },
  {
    ""uid"": 257,
    ""IsActive"": 1,
    ""ParentID"": 10,
    ""Name"": ""CB6109""
  },
  {
    ""uid"": 258,
    ""IsActive"": 1,
    ""ParentID"": 10,
    ""Name"": ""CB6209""
  },
  {
    ""uid"": 259,
    ""IsActive"": 1,
    ""ParentID"": 10,
    ""Name"": ""CB9530""
  },
  {
    ""uid"": 260,
    ""IsActive"": 1,
    ""ParentID"": 6,
    ""Name"": ""CB6007""
  },
  {
    ""uid"": 261,
    ""IsActive"": 1,
    ""ParentID"": 6,
    ""Name"": ""CB6009""
  },
  {
    ""uid"": 262,
    ""IsActive"": 1,
    ""ParentID"": 6,
    ""Name"": ""CB8009""
  },
  {
    ""uid"": 263,
    ""IsActive"": 1,
    ""ParentID"": 6,
    ""Name"": ""CB6011""
  },
  {
    ""uid"": 264,
    ""IsActive"": 1,
    ""ParentID"": 6,
    ""Name"": ""CB8011""
  },
  {
    ""uid"": 265,
    ""IsActive"": 1,
    ""ParentID"": 6,
    ""Name"": ""CB6012""
  },
  {
    ""uid"": 266,
    ""IsActive"": 1,
    ""ParentID"": 6,
    ""Name"": ""CB6013""
  },
  {
    ""uid"": 267,
    ""IsActive"": 1,
    ""ParentID"": 6,
    ""Name"": ""CB9099""
  },
  {
    ""uid"": 268,
    ""IsActive"": 1,
    ""ParentID"": 6,
    ""Name"": ""CB9595""
  },
  {
    ""uid"": 269,
    ""IsActive"": 1,
    ""ParentID"": 6,
    ""Name"": ""CB9102""
  },
  {
    ""uid"": 270,
    ""IsActive"": 1,
    ""ParentID"": 6,
    ""Name"": ""CB9186""
  },
  {
    ""uid"": 271,
    ""IsActive"": 1,
    ""ParentID"": 6,
    ""Name"": ""CB2009""
  },
  {
    ""uid"": 272,
    ""IsActive"": 1,
    ""ParentID"": 6,
    ""Name"": ""CB3009""
  },
  {
    ""uid"": 273,
    ""IsActive"": 1,
    ""ParentID"": 6,
    ""Name"": ""CB2011""
  },
  {
    ""uid"": 274,
    ""IsActive"": 1,
    ""ParentID"": 6,
    ""Name"": ""CB3011""
  },
  {
    ""uid"": 275,
    ""IsActive"": 1,
    ""ParentID"": 9,
    ""Name"": ""CB2031""
  },
  {
    ""uid"": 276,
    ""IsActive"": 1,
    ""ParentID"": 9,
    ""Name"": ""CB9121""
  },
  {
    ""uid"": 277,
    ""IsActive"": 1,
    ""ParentID"": 9,
    ""Name"": ""CB6008""
  },
  {
    ""uid"": 278,
    ""IsActive"": 1,
    ""ParentID"": 9,
    ""Name"": ""CB8008""
  },
  {
    ""uid"": 279,
    ""IsActive"": 1,
    ""ParentID"": 9,
    ""Name"": ""CB6010""
  },
  {
    ""uid"": 280,
    ""IsActive"": 1,
    ""ParentID"": 9,
    ""Name"": ""CB8010""
  },
  {
    ""uid"": 281,
    ""IsActive"": 1,
    ""ParentID"": 9,
    ""Name"": ""CB9356""
  },
  {
    ""uid"": 282,
    ""IsActive"": 1,
    ""ParentID"": 9,
    ""Name"": ""CB9530""
  },
  {
    ""uid"": 283,
    ""IsActive"": 1,
    ""ParentID"": 8,
    ""Name"": ""CB6014""
  },
  {
    ""uid"": 284,
    ""IsActive"": 1,
    ""ParentID"": 8,
    ""Name"": ""CB9197""
  },
  {
    ""uid"": 285,
    ""IsActive"": 1,
    ""ParentID"": 8,
    ""Name"": ""CB9121""
  },
  {
    ""uid"": 286,
    ""IsActive"": 1,
    ""ParentID"": 7,
    ""Name"": ""CB4009""
  },
  {
    ""uid"": 287,
    ""IsActive"": 1,
    ""ParentID"": 7,
    ""Name"": ""CB4011""
  },
  {
    ""uid"": 288,
    ""IsActive"": 1,
    ""ParentID"": 7,
    ""Name"": ""CB1509""
  },
  {
    ""uid"": 292,
    ""IsActive"": 1,
    ""ParentID"": 4,
    ""Name"": ""CB4002""
  },
  {
    ""uid"": 293,
    ""IsActive"": 1,
    ""ParentID"": 4,
    ""Name"": ""CB4132""
  },
  {
    ""uid"": 294,
    ""IsActive"": 1,
    ""ParentID"": 4,
    ""Name"": ""CB4233""
  },
  {
    ""uid"": 295,
    ""IsActive"": 1,
    ""ParentID"": 25,
    ""Name"": ""Pin, Insulation Mount""
  },
  {
    ""uid"": 296,
    ""IsActive"": 1,
    ""ParentID"": 295,
    ""Name"": ""CS120""
  },
  {
    ""uid"": 297,
    ""IsActive"": 1,
    ""ParentID"": 55,
    ""Name"": ""Grommets""
  },
  {
    ""uid"": 298,
    ""IsActive"": 1,
    ""ParentID"": 297,
    ""Name"": ""CG596C""
  },
  {
    ""uid"": 299,
    ""IsActive"": 1,
    ""ParentID"": 49,
    ""Name"": ""CB200""
  },
  {
    ""uid"": 300,
    ""IsActive"": 1,
    ""ParentID"": 50,
    ""Name"": ""CB100-81""
  },
  {
    ""uid"": 301,
    ""IsActive"": 1,
    ""ParentID"": 50,
    ""Name"": ""CB100-21""
  },
  {
    ""uid"": 302,
    ""IsActive"": 1,
    ""ParentID"": 9,
    ""Name"": ""CB6080""
  },
  {
    ""uid"": 303,
    ""IsActive"": 1,
    ""ParentID"": 28,
    ""Name"": ""CB4062""
  },
  {
    ""uid"": 304,
    ""IsActive"": 1,
    ""ParentID"": 28,
    ""Name"": ""CB4063""
  },
  {
    ""uid"": 305,
    ""IsActive"": 1,
    ""ParentID"": 28,
    ""Name"": ""CB4064""
  }
]
";
                var categoryInfos = JsonConvert.DeserializeObject<IEnumerable<CategoryMapping>>(categorieslistJson);

                var categories = new Dictionary<int, Category>();
                var products = new Dictionary<int, Product>();
                var parts = new List<Part>();

                foreach (var categoryInfo in categoryInfos.OrderBy(c => c.uid).Where(c => c.uid >= 5 && c.uid <= 305))
                {
                    // is category
                    if (categoryInfo.uid >= 5 && categoryInfo.uid <= 59)
                    {
                        var parentCategory = (categoryInfo.ParentID == 2) ? null : categories[categoryInfo.ParentID];

                        var category = new Category
                        {
                            Id = Guid.NewGuid(),
                            IsActive = categoryInfo.IsActive,
                            CategoryId = (categoryInfo.ParentID == 2) ? (Guid?)null : parentCategory.Id,
                            Name = categoryInfo.Name
                        };
                        categories.Add(categoryInfo.uid, category);
                        Categories.Add(category);
                    }
                    // is product
                    else if (categoryInfo.uid >= 60 && categoryInfo.uid <= 305 && categoryInfo.ParentID >= 5 && categoryInfo.ParentID <= 59)
                    {
                        var subCategory = categories[categoryInfo.ParentID];
                        var parentCategory = categories.Values.FirstOrDefault(c => c.Id == subCategory.CategoryId);
                        if (parentCategory != null)
                        {
                            var product = new Product
                            {
                                Id = Guid.NewGuid(),
                                IsActive = categoryInfo.IsActive,
                                CategoryId = parentCategory.Id,
                                SubCategoryId = subCategory.Id,
                                Code = categoryInfo.Name
                            };
                            products.Add(categoryInfo.uid, product);
                            Products.Add(product);
                        }
                    }
                }

                SaveChanges();

                foreach (var partInfo in productList.Where(p => p.CategoryId >= 60 && p.CategoryId <= 305))
                {
                    if (products.ContainsKey(partInfo.CategoryId))
                    {
                        var productId = products[partInfo.CategoryId].Id;
                        var part = new Part
                        {
                            Id = Guid.NewGuid(),
                            IsActive = partInfo.IsActive,
                            PartNumber = partInfo.Code,
                            Description = partInfo.ShortDescription,
                            Price = partInfo.Price,
                            ProductId = productId
                        };

                        Parts.Add(part);
                    }
                }

                SaveChanges();
            }
            else
            {
                throw new Exception("Data exists!! Thus, database cannot be seeded.");
            }
        }

        public IEnumerable<Category> GetCategories()
        {
            return GetParentCategories();
        }

        public IEnumerable<Category> GetSubCategories()
        {
            return Categories.Where(category => category.IsActive && category.CategoryId.HasValue);
        }

        public IEnumerable<Product> GetProducts()
        {
            var partsByProductId = Parts.GroupBy(part => part.ProductId).ToDictionary(group => group.Key);
            foreach (var product in Products)
            {
                if (partsByProductId.ContainsKey(product.Id))
                {
                    product.Parts = partsByProductId[product.Id];
                }
            }

            return Products;
        }

        public Product GetProduct(Guid id)
        {
            return Products.FirstOrDefault(product => product.Id == id);
        }

        public IEnumerable<Part> GetParts()
        {
            return Parts;
        }

        public ProductsViewModel GetProductCategories()
        {
            var parentCategories = GetParentCategories();
            return new ProductsViewModel { Categories = parentCategories };
        }

        private IEnumerable<Category> GetParentCategories()
        {
            return Categories.Where(category => category.IsActive && !category.CategoryId.HasValue);;
        }

        public ProductCategoryViewModel GetProducts(Guid categoryId)
        {
            var products = Products.Where(product => product.IsActive && product.CategoryId == categoryId).AsEnumerable();
            var category = Categories.FirstOrDefault(c => c.Id == categoryId);
            var subCategoryIds = products.Select(product => product.SubCategoryId).ToList().Distinct();
            var subCategories = Categories.Where(c => subCategoryIds.Contains(c.Id)).ToList();
            return new ProductCategoryViewModel
            {
                CategoryName = category?.Name ?? "N/A",
                SubCategories = subCategories.Select(c => c.Name).ToList(),
                Products = products.Select(product => new ProductViewModel
                {
                    Id = product.Id,
                    Code = product.Code,
                    ShortDescription = product.ShortDescription,
                    IsMostPopular = product.MostPopular,
                    HasPricing = Parts.Any(part => part.ProductId == product.Id),
                    SubCategory = subCategories.First(c => c.Id == product.SubCategoryId).Name,
                    ImageFilename = product.ImageFilename
                })
            };
        }

        public ProductDetailViewModel GetProductView(Guid productId)
        {
            var product = Products.FirstOrDefault(p => p.Id == productId);
            var viewModel = (ProductDetailViewModel)null;
            if (product != null)
            {
                var category = Categories.First(c => c.Id == product.CategoryId);
                var parts = Parts.Where(p => p.ProductId == product.Id && p.IsActive).ToList();
                parts.Sort((a, b) => CompareParts(a, b));

                viewModel = new ProductDetailViewModel
                {
                    ProductId = productId,
                    Category = category.Name,
                    CategoryId = category.Id,
                    ProductCode = product.Code,
                    ShortDescription = product.ShortDescription,
                    LongDescription = product.LongDescription,
                    NumberOfInstallationExamples = product.NumberOfInstallationExamples,
                    InstallationExamplesPath = GetInstallationExamplesPath(product.Code),
                    ImageFilename = product.ImageFilename,
                    DrawingFilename = product.DrawingFilename,
                    Parts = parts.Select(p => new PartViewModel
                    {
                        PartNumber = p.PartNumber,
                        Description = p.Description,
                        Price = p.Price
                    })
                };
            }
            return viewModel;
        }

        private int CompareParts(Part a, Part b)
        {
            var result = 0;
            var tokensA = Regex.Split(a.PartNumber, NON_WORD_CHARACTER_PATTERN);
            var tokensB = Regex.Split(b.PartNumber, NON_WORD_CHARACTER_PATTERN);

            for (int i = 0; result == 0 && i < tokensA.Length && i < tokensB.Length; i++)
            {
                int numberA;
                int numberB;
                var hasNumberA = int.TryParse(tokensA[i], out numberA);
                var hasNumberB = int.TryParse(tokensB[i], out numberB);

                if (hasNumberA && hasNumberB)
                {
                    result = numberA - numberB;
                }
                else
                {
                    result = tokensA[i].CompareTo(tokensB[i]);
                }
            }

            if (result == 0)
            {
                if (tokensA.Length < tokensB.Length)
                {
                    result = -1;
                } 
                else if (tokensB.Length < tokensA.Length)
                {
                    result = 1;
                }
            }

            return result;
        }

        public IEnumerable<SearchResult> SearchParts(string query)
        {
            var results = new List<SearchResult>();
            if (!string.IsNullOrEmpty(query))
            {
                var formattedQuery = query.ToLower().Trim();
                var matchingParts = Parts.Where(part => part.IsActive && MatchesQuery(part, formattedQuery));
                if (matchingParts.Any())
                {
                    foreach (var part in matchingParts)
                    {
                        var product = Products.FirstOrDefault(prdct => prdct.Id == part.ProductId);
                        if (product == null)
                        {
                            // todo: log product not found and move on
                        }
                        else
                        {
                            var result = GetNewSearchResult(product, part);
                            results.Add(result);
                        }
                    }
                }
                else
                {
                    var matchingProducts = Products.Where(product => product.IsActive && MatchesQuery(product, formattedQuery));
                    foreach (var product in matchingProducts)
                    {
                        var result = GetNewSearchResult(product);
                        results.Add(result);
                    }
                }
            }
            return results;
        }

        public void CreateOrUpdateProduct(Product product)
        {
            var existingProduct = Products.FirstOrDefault(prdct => prdct.Id == product.Id);
            if (existingProduct == null)
            {
                Products.Add(product);
            }
            else
            {
                existingProduct.Code = product.Code;
                existingProduct.ShortDescription = product.ShortDescription;
                existingProduct.LongDescription = product.LongDescription;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.SubCategoryId = product.SubCategoryId;
                existingProduct.MostPopular = product.MostPopular;
                if (!string.IsNullOrEmpty(product.ImageFilename))
                {
                    existingProduct.ImageFilename = product.ImageFilename;
                }
                if (!string.IsNullOrEmpty(product.DrawingFilename))
                {
                    existingProduct.DrawingFilename = product.DrawingFilename;
                }
            }

            SaveChanges();
        }

        public void DeleteProduct(Guid productId)
        {
            var product = Products.FirstOrDefault(prdct => prdct.Id == productId);
            if (product != null)
            {
                Products.Remove(product);
                SaveChanges();
            }
        }

        private SearchResult GetNewSearchResult(Product product, Part part = null)
        {
            var category = GetCategoryById(product.CategoryId);
            var subCategory = GetCategoryById(product.SubCategoryId);
            var name = part == null ? product.Code : part.PartNumber;
            var description = part == null ? product.ShortDescription : part.Description;
            return new SearchResult(name, product.Id, description, category, subCategory, product.ImageFilename);
        }

        private string GetCategoryById(Guid categoryId)
        {
            return Categories.FirstOrDefault(category => category.Id == categoryId)?.Name ?? null;
        }

        private bool MatchesQuery(Part part, string query)
        {
            return (part.PartNumber?.ToLower().Contains(query) ?? false) || (part.Description?.ToLower().Contains(query) ?? false);
        }

        private bool MatchesQuery(Product product, string query)
        {
            return (product.Code?.ToLower().Contains(query) ?? false) || (product.ShortDescription?.ToLower().Contains(query) ?? false);
        }
        
        private string GetInstallationExamplesPath(string productCode)
        {
            return "/products/installation-examples/" + productCode + "/";
        }

        public void CreateOrUpdateCategory(Category category)
        {
            var existingCategory = Categories.FirstOrDefault(ctgry => ctgry.Id == category.Id);
            if (existingCategory == null)
            {
                Categories.Add(category);
            }
            else
            {
                existingCategory.CategoryId = category.CategoryId;
                existingCategory.Name = category.Name;
            }

            SaveChanges();
        }

        public void DeleteCategoryAndProducts(Guid categoryId)
        {            
            var categoryToDelete = Categories.FirstOrDefault(category => category.Id == categoryId);
            if (categoryToDelete != null)
            {
                var productsToDelete = Products.Where(product => product.CategoryId == categoryId);
                var subCategoriesToDelete = Categories.Where(category => category.CategoryId == categoryId);
                Products.RemoveRange(productsToDelete);
                SaveChanges();
                Categories.RemoveRange(subCategoriesToDelete);
                SaveChanges();
                Categories.Remove(categoryToDelete);
                SaveChanges();
            }
        }

        public void DeleteSubCategoryAndProducts(Guid subCategoryId)
        {
            var subCategory = Categories.FirstOrDefault(category => category.Id == subCategoryId);
            if (subCategory != null)
            {
                var productsToDelete = Products.Where(product => product.SubCategoryId == subCategoryId);
                Products.RemoveRange(productsToDelete);
                SaveChanges();
                Categories.Remove(subCategory);
                SaveChanges();
            }
        }

        public void CreateOrUpdatePart(Part part)
        {
            var existingPart = Parts.FirstOrDefault(prt => prt.Id == part.Id);
            if (existingPart == null)
            {
                Parts.Add(part);
            }
            else
            {
                existingPart.PartNumber = part.PartNumber;
                existingPart.Description = part.Description;
                existingPart.Price = part.Price;
            }

            SaveChanges();
        }

        public void DeletePart(Guid id)
        {
            var part = Parts.FirstOrDefault(prt => prt.Id == id);
            if (part != null)
            {
                Parts.Remove(part);
                SaveChanges();
            }
        }

        public IEnumerable<Product> GetProductsByCategoryOrSubCategoryId(Guid categoryOrSubCategoryId)
        {
            return Products.Where(product => product.CategoryId == categoryOrSubCategoryId || product.SubCategoryId == categoryOrSubCategoryId);
        }

        public Category GetCategory(Guid id)
        {
            return Categories.FirstOrDefault(category => category.Id == id);
        }
    }
}
