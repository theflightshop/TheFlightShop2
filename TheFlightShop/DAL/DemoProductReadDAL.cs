using System;
using System.Collections.Generic;
using TheFlightShop.Models;

namespace TheFlightShop.DAL
{
    public class DemoProductReadDAL : IProductReadDAL
    {
        // general aviation
        private readonly Guid GEN_AVIATION_CATEGORY_ID = Guid.Parse("959dbb29-f979-4f5e-a9b7-2a3949c57fe6");
        private readonly Guid NUTPLATES_ID = Guid.Parse("3a6f5cf5-e364-4bb6-8be6-fa4aa477d051");
        private readonly Guid CN609_ID = Guid.Parse("3f2a40d0-33f0-486b-98dd-d9cfaf4faee2");

        public IEnumerable<ProductCategoryCount> GetProductCategoryCounts()
        {
            return new List<ProductCategoryCount>
            {
                new ProductCategoryCount
                {
                    CategoryId = GEN_AVIATION_CATEGORY_ID,
                    CategoryName = "General Aviation / Industrial / Commercial",
                    Counts = new List<ProductSubCategoryCount>
                    {
                        new ProductSubCategoryCount
                        {
                            SubCategoryId = Guid.NewGuid(),
                            SubCategoryName = "Bushings",
                            Count = 3
                        },
                        new ProductSubCategoryCount
                        {
                            SubCategoryId = NUTPLATES_ID,
                            SubCategoryName = "Nutplates",
                            Count = 6
                        },
                        new ProductSubCategoryCount
                        {
                            SubCategoryId = Guid.NewGuid(),
                            SubCategoryName = "Standoffs",
                            Count = 3
                        },
                        new ProductSubCategoryCount
                        {
                            SubCategoryId = Guid.NewGuid(),
                            SubCategoryName = "Studs",
                            Count = 3
                        },
                        new ProductSubCategoryCount
                        {
                            SubCategoryId = Guid.NewGuid(),
                            SubCategoryName = "Grommets",
                            Count = 1
                        }
                    }
                },
                new ProductCategoryCount
                {
                    CategoryId = Guid.NewGuid(),
                    CategoryName = "Studs",
                    Counts = new List<ProductSubCategoryCount>
                    {
                        new ProductSubCategoryCount
                        {
                            SubCategoryId = Guid.NewGuid(),
                            SubCategoryName = "Metallic Base Studs",
                            Count = 7
                        },
                        new ProductSubCategoryCount
                        {
                            SubCategoryId = Guid.NewGuid(),
                            SubCategoryName = "Composite Base Studs",
                            Count = 7
                        },
                        new ProductSubCategoryCount
                        {
                            SubCategoryId = Guid.NewGuid(),
                            SubCategoryName = "Accessories",
                            Count = 2
                        }
                    }
                }
            };
        }

        public ProductCategoryCount GetProductCategoryCount(Guid categoryId)
        {
            if (categoryId == GEN_AVIATION_CATEGORY_ID)
            {
                return new ProductCategoryCount
                {
                    CategoryId = GEN_AVIATION_CATEGORY_ID,
                    CategoryName = "General Aviation / Industrial / Commercial",
                    Counts = new List<ProductSubCategoryCount>
                    {
                        new ProductSubCategoryCount
                        {
                            SubCategoryId = Guid.NewGuid(),
                            SubCategoryName = "Bushings",
                            Count = 3
                        },
                        new ProductSubCategoryCount
                        {
                            SubCategoryId = NUTPLATES_ID,
                            SubCategoryName = "Nutplates",
                            Count = 6
                        },
                        new ProductSubCategoryCount
                        {
                            SubCategoryId = Guid.NewGuid(),
                            SubCategoryName = "Standoffs",
                            Count = 3
                        },
                        new ProductSubCategoryCount
                        {
                            SubCategoryId = Guid.NewGuid(),
                            SubCategoryName = "Studs",
                            Count = 3
                        },
                        new ProductSubCategoryCount
                        {
                            SubCategoryId = Guid.NewGuid(),
                            SubCategoryName = "Grommets",
                            Count = 1
                        }
                    }
                };
            }
            throw new NotImplementedException("This is just demo data!");
        }

        public SubCategoryView GetSubCategoryView(Guid subCategoryId)
        {
            if (subCategoryId == NUTPLATES_ID)
            {
                return new SubCategoryView
                {
                    CategoryId = GEN_AVIATION_CATEGORY_ID,
                    Category = "General Aviation / Industrial / Commercial",
                    SubCategory = "Nutplates",
                    Products = new List<Product>
                {
                    new Product
                    {
                        Id = Guid.NewGuid(),
                        Code = "CN611",
                        ShortDescription = "One Lug, Clip Retained",
                        IsMostPopular = false,
                        HasPricing = false
                    },
                    new Product
                    {
                        Id = CN609_ID,
                        Code = "CN609",
                        ShortDescription = "Two Lug, Clip Retained",
                        IsMostPopular = true,
                        HasPricing = true
                    },
                    new Product
                    {
                        Id = Guid.NewGuid(),
                        Code = "CN111",
                        ShortDescription = "Nutplate, Bracket Retained",
                        IsMostPopular = false,
                        HasPricing = false
                    },
                    new Product
                    {
                        Id = Guid.NewGuid(),
                        Code = "CN109",
                        ShortDescription = "Two Lug, Bracket Retained",
                        IsMostPopular = true,
                        HasPricing = true
                    },
                    new Product
                    {
                        Id = Guid.NewGuid(),
                        Code = "CN614",
                        ShortDescription = "Two Lug, Miniature",
                        IsMostPopular = true,
                        HasPricing = true
                    },
                    new Product
                    {
                        Id = Guid.NewGuid(),
                        Code = "CN610",
                        ShortDescription = "Two Lug, Sealed Dome, floating",
                        IsMostPopular = true,
                        HasPricing = true
                    }
                }
                };
            }
            throw new NotImplementedException("This is just demo data!");
        }

        public ProductView GetProductView(Guid productId)
        {
            if (productId == CN609_ID)
            {
                return new ProductView
                {
                    CategoryId = GEN_AVIATION_CATEGORY_ID,
                    Category = "General Aviation / Industrial / Commercial",
                    SubCategoryId = NUTPLATES_ID,
                    SubCategory = "Nutplates",
                    ProductCode = "CN609",
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
