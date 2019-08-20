﻿using System;
using System.Collections.Generic;
using TheFlightShop.Models;

namespace TheFlightShop.DAL
{
    public class DemoProductReadDAL : IProductReadDAL
    {
        // general aviation
        private readonly Guid NUTPLATES_CATEGORY_ID = Guid.Parse("959dbb29-f979-4f5e-a9b7-2a3949c57fe6");
        private readonly Guid CB2009_ID = Guid.Parse("3f2a40d0-33f0-486b-98dd-d9cfaf4faee2");

        public ProductsViewModel GetProductCategories()
        {
            return new ProductsViewModel
            {
                Categories = new List<ProductCategory>
                {
                    new ProductCategory
                    {
                        Id = NUTPLATES_CATEGORY_ID,
                        Name = "Nutplates"
                    },
                    new ProductCategory
                    {
                        Id = Guid.NewGuid(),
                        Name = "Studs & Standoffs"
                    },
                    new ProductCategory
                    {
                        Id = Guid.NewGuid(),
                        Name = "Mounts"
                    },
                    new ProductCategory
                    {
                        Id = Guid.NewGuid(),
                        Name = "Acres® Sleeves"
                    },
                    new ProductCategory
                    {
                        Id = Guid.NewGuid(),
                        Name = "Bushings"
                    },
                    new ProductCategory
                    {
                        Id = Guid.NewGuid(),
                        Name = "Click Path®"
                    },
                    new ProductCategory
                    {
                        Id = Guid.NewGuid(),
                        Name = "Lomas® Screws"
                    },
                    new ProductCategory
                    {
                        Id = Guid.NewGuid(),
                        Name = "Adhesives"
                    }
                }
            };
        }

        public ProductCategoryViewModel GetProducts(Guid categoryId)
        {
            if (categoryId == NUTPLATES_CATEGORY_ID)
            {
                var viewModel = new ProductCategoryViewModel
                {
                    CategoryName = "Nutplates",
                    SubCategories = new List<string>
                    {
                        "Standard", "Sealed", "Sleeved"
                    },
                    Products = new List<Product>
                    {
                        new Product
                        {
                            Id = CB2009_ID,
                            Code = "CB2009",
                            SubCategory = "Standard",
                            ShortDescription = "Two-Lug Bracket-Retained Nutplate",
                            HasPricing = false,
                            IsMostPopular = false
                        },
                        new Product
                        {
                            Id = CB2009_ID,
                            Code = "CB2011",
                            SubCategory = "Standard",
                            ShortDescription = "One-Lug Bracket-Retained Nutplate",
                            HasPricing = false,
                            IsMostPopular = false
                        },
                        new Product
                        {
                            Id = CB2009_ID,
                            Code = "CB3009",
                            SubCategory = "Standard",
                            ShortDescription = "Two-Lug Bracket-Retained Nutplate",
                            HasPricing = false,
                            IsMostPopular = false
                        },
                        new Product
                        {
                            Id = CB2009_ID,
                            Code = "CB3011",
                            SubCategory = "Standard",
                            ShortDescription = "One-Lug Bracket-Retained Nutplate",
                            HasPricing = false,
                            IsMostPopular = false
                        },
                        new Product
                        {
                            Id = CB2009_ID,
                            Code = "CB4009",
                            SubCategory = "Standard",
                            ShortDescription = "Two-Lug Bracket-Retained Nutplate",
                            HasPricing = false,
                            IsMostPopular = false
                        },
                        new Product
                        {
                            Id = CB2009_ID,
                            Code = "CB4011",
                            SubCategory = "Standard",
                            ShortDescription = "One-Lug Bracket-Retained Nutplate",
                            HasPricing = false,
                            IsMostPopular = false
                        },
                        new Product
                        {
                            Id = CB2009_ID,
                            Code = "CB6003",
                            SubCategory = "Standard",
                            ShortDescription = "No-Lug Clip-Retained Nutplate",
                            HasPricing = false,
                            IsMostPopular = false
                        },
                        new Product
                        {
                            Id = CB2009_ID,
                            Code = "CB6009",
                            SubCategory = "Standard",
                            ShortDescription = "Two-Lug Clip-Retained Nutplate",
                            HasPricing = false,
                            IsMostPopular = false
                        },
                        new Product
                        {
                            Id = CB2009_ID,
                            Code = "CB6008",
                            SubCategory = "Sealed",
                            ShortDescription = "One-Lug Sealed Nutplate",
                            HasPricing = false,
                            IsMostPopular = false
                        },
                        new Product
                        {
                            Id = CB2009_ID,
                            Code = "CB6010",
                            SubCategory = "Sealed",
                            ShortDescription = "Two-Lug Sealed Nutplate",
                            HasPricing = false,
                            IsMostPopular = false
                        },
                        new Product
                        {
                            Id = CB2009_ID,
                            Code = "CB6109",
                            SubCategory = "Sleeved",
                            ShortDescription = "Two-Lug Flared Sleeved Nutplate",
                            HasPricing = false,
                            IsMostPopular = false
                        },
                        new Product
                        {
                            Id = CB2009_ID,
                            Code = "CB6209",
                            SubCategory = "Sleeved",
                            ShortDescription = "Two-Lug Straight-Sleeved Nutplate",
                            HasPricing = false,
                            IsMostPopular = false
                        }
                    }
                };
                foreach (var product in viewModel.Products)
                {
                    product.ImageSource = GetImageSource(viewModel.CategoryName, product.Code);
                }
                return viewModel;
            }
            throw new NotImplementedException("This is just demo data!");
        }

        public ProductView GetProductView(Guid productId)
        {
            if (productId == CB2009_ID)
            {
                var viewModel = new ProductView
                {
                    CategoryId = NUTPLATES_CATEGORY_ID,
                    Category = "Nutplates",
                    ProductCode = "CB2009",
                    ShortDescription = "Two-Lug Bracket-Retained Nutplate",
                    LongDescription = "Two-lug nutplate with bracket-retained nut element for FOD-critical applications where nut replaceability is desired but nut element retainer clips are not permitted. Supplied with integral elastic installation fixture. Fixture simplifies nutplate installation by providing bondline clamp-up and maintenance of concentricity with the fastener hole, ensuring full availability of nut float and protection of threads from adhesive.",
                    DrawingUrl = "https://www.clickbond.com/downloadfile.ashx?path=Uploads/ProductPDF/636268296532963658.pdf",
                    ComparisonTableUrl = "https://www.clickbond.com/downloadfile.ashx?path=Uploads/ProductPDF/636342787605560043.pdf",
                    Parts = new List<Part>
                    {
                        new Part
                        {
                            PartNumber = "CN609CR08P",
                            Description = "8/32\" stainless steel/primed",
                            Price = 4
                        },
                        new Part
                        {
                            PartNumber = "CN609CR3P",
                            Description = "10/32\" stainless steel primed",
                            Price = 4
                        },
                        new Part
                        {
                            PartNumber = "CN609CR4P",
                            Description = "1/4-28\" stainless steel/primed",
                            Price = 4.93m
                        }
                    }
                };
                viewModel.ImageSource = GetImageSource(viewModel.Category, viewModel.ProductCode);
                return viewModel;
            }
            throw new NotImplementedException("This is just demo data!");
        }

        private string GetImageSource(string category, string productCode)
        {
            return "/products/product-images/" + category.ToLower() + "/" + productCode + ".jpg";
        }
    }
}
