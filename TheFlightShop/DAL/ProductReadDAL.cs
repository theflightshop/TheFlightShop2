using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TheFlightShop.DAL.Schemas;
using TheFlightShop.Models;

namespace TheFlightShop.DAL
{
    public class ProductReadDAL : DbContext, IProductReadDAL
    {
        private class ProductCategoryMapping
        {
            public int ProductId { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public string Code { get; set; }
            public string ShortDescription { get; set; }
            public decimal Price { get; set; }
        }

        private class CategoryMapping
        {
            public int uid { get; set; }
            public int ParentID { get; set; }
            public string Name { get; set; }
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Part> Parts { get; set; }

        private readonly string _connectionString;

        public ProductReadDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().Ignore(product => product.DrawingUrl);
            base.OnModelCreating(modelBuilder);
        }

        public void InitializeFrom()
        {
            var productsPath = Path.Combine(Directory.GetCurrentDirectory(), "product-static-files", "json", "dbexport-oct-19", "productInfo.json");
            var productListJson = File.ReadAllText(productsPath);
            var productList = JsonConvert.DeserializeObject<IEnumerable<ProductCategoryMapping>>(productListJson);

            var categoriesPath = Path.Combine(Directory.GetCurrentDirectory(), "product-static-files", "json", "dbexport-oct-19", "categories.json");
            var categorieslistJson = File.ReadAllText(categoriesPath);
            var categoryInfos = JsonConvert.DeserializeObject<IEnumerable<CategoryMapping>>(categorieslistJson);

            var categories = new Dictionary<int, Category>();
            var products = new Dictionary<int, Product>();
            var parts = new List<Part>();

            foreach (var categoryInfo in categoryInfos.OrderBy(c => c.uid).Where(c => c.uid >= 5 && c.uid <= 305))
            {
                // is category
                if (categoryInfo.uid >=5 && categoryInfo.uid <= 59)
                {
                    var parentCategory = (categoryInfo.ParentID == 2) ? null : categories[categoryInfo.ParentID];

                    var category = new Category
                    {
                        Id = Guid.NewGuid(),
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
                            CategoryId = parentCategory.Id,
                            SubCategoryId = subCategory.Id,
                            Code = categoryInfo.Name
                        };
                        products.Add(categoryInfo.uid, product);
                        Products.Add(product);
                    }
                }
            }

            foreach (var partInfo in productList.Where(p => p.CategoryId >= 60 && p.CategoryId <= 305))
            {
                if (products.ContainsKey(partInfo.CategoryId))
                {
                    var productId = products[partInfo.CategoryId].Id;
                    var part = new Part
                    {
                        Id = Guid.NewGuid(),
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

        public ProductsViewModel GetProductCategories()
        {
            var parentCategories = Categories.Where(category => !category.CategoryId.HasValue);
            return new ProductsViewModel { Categories = parentCategories };
        }

        public ProductCategoryViewModel GetProducts(Guid categoryId)
        {
            var products = Products.Where(product => product.CategoryId == categoryId).AsEnumerable();
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
                    ImageSource = GetImageSource(product.Code)
                })
            };
        }

        public ProductDetailViewModel GetProductView(Guid productId)
        {
            throw new NotImplementedException();
        }

        private string GetImageSource(string productCode)
        {
            return "/products/product-images/" + productCode.ToLower() + ".gif";
        }
    }
}
