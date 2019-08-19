using System;
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
                return new ProductCategoryViewModel
                {
                    CategoryName = "Nutplates",
                    Products = new List<Product>
                {
                    new Product
                    {
                        Id = CB2009_ID,
                        Code = "CB2009",
                        ShortDescription = "Two-Lug Bracket-Retained Nutplate",
                        HasPricing = false,
                        IsMostPopular = false
                    },
                    new Product
                    {
                        Id = CB2009_ID,
                        Code = "CB2009",
                        ShortDescription = "Two-Lug Bracket-Retained Nutplate",
                        HasPricing = false,
                        IsMostPopular = false
                    },
                    new Product
                    {
                        Id = CB2009_ID,
                        Code = "CB2009",
                        ShortDescription = "Two-Lug Bracket-Retained Nutplate",
                        HasPricing = false,
                        IsMostPopular = false
                    },
                    new Product
                    {
                        Id = CB2009_ID,
                        Code = "CB2009",
                        ShortDescription = "Two-Lug Bracket-Retained Nutplate",
                        HasPricing = false,
                        IsMostPopular = false
                    },
                    new Product
                    {
                        Id = CB2009_ID,
                        Code = "CB2009",
                        ShortDescription = "Two-Lug Bracket-Retained Nutplate",
                        HasPricing = false,
                        IsMostPopular = false
                    },
                    new Product
                    {
                        Id = CB2009_ID,
                        Code = "CB2009",
                        ShortDescription = "Two-Lug Bracket-Retained Nutplate",
                        HasPricing = false,
                        IsMostPopular = false
                    },
                    new Product
                    {
                        Id = CB2009_ID,
                        Code = "CB2009",
                        ShortDescription = "Two-Lug Bracket-Retained Nutplate",
                        HasPricing = false,
                        IsMostPopular = false
                    },
                    new Product
                    {
                        Id = CB2009_ID,
                        Code = "CB2009",
                        ShortDescription = "Two-Lug Bracket-Retained Nutplate",
                        HasPricing = false,
                        IsMostPopular = false
                    }
                }
                };
            }
            throw new NotImplementedException("This is just demo data!");
        }

        public ProductView GetProductView(Guid productId)
        {
            if (productId == CB2009_ID)
            {
                return new ProductView
                {
                    CategoryId = NUTPLATES_CATEGORY_ID,
                    Category = "Nutplates",
                    SubCategoryId = Guid.NewGuid(),
                    SubCategory = "Standard",
                    ProductCode = "CB2009",
                    ShortDescription = "Two Lug, Clip Retained",
                    LongDescription = "CN609 nutplate has a removable clip for nut replacement and can be ordered in sizes ranging from 8/32\" to 5/16-18\" with a nut and baseplate material of A-286 CRES. This CN609 nutplate can be ordered in metric sizes.",
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
            }
            throw new NotImplementedException("This is just demo data!");
        }
    }
}
