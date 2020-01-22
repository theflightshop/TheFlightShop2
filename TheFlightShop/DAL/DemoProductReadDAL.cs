using System;
using System.Collections.Generic;
using TheFlightShop.Models;
using Newtonsoft.Json;
using System.IO;
using TheFlightShop.DAL.Schemas;
using System.Linq;
using System.Text.RegularExpressions;

namespace TheFlightShop.DAL
{
    public class DemoProductReadDAL : IProductReadDAL
    {
        // general aviation
        private readonly Guid NUTPLATES_CATEGORY_ID = Guid.Parse("959dbb29-f979-4f5e-a9b7-2a3949c57fe6");
        private readonly Guid CB2009_ID = Guid.Parse("3f2a40d0-33f0-486b-98dd-d9cfaf4faee2");

        private IEnumerable<Category> _categories;
        private IEnumerable<Product> _products;
        private IEnumerable<Part> _parts;

        public DemoProductReadDAL()
        {
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "product-static-files", "json");

            var categoriesPath = Path.Combine(jsonPath, "categories.json");
            var categoriesText = File.ReadAllText(categoriesPath);
            _categories = JsonConvert.DeserializeObject<IEnumerable<Category>>(categoriesText);

            var subCategoriesPath = Path.Combine(jsonPath, "sub-categories.json");
            var subCategoriesText = File.ReadAllText(subCategoriesPath);
            
            var productsPath = Path.Combine(jsonPath, "products.json");
            var productsText = File.ReadAllText(productsPath);
            _products = JsonConvert.DeserializeObject<IEnumerable<Product>>(productsText);

            var partsPath = Path.Combine(jsonPath, "parts.json");
            var partsText = File.ReadAllText(partsPath);
            _parts = JsonConvert.DeserializeObject<IEnumerable<Part>>(partsText);
        }

        public ProductsViewModel GetProductCategories()
        {
            return new ProductsViewModel
            {
                Categories = _categories
            };
        }

        public ProductCategoryViewModel GetProducts(Guid categoryId)
        {
            var viewModel = (ProductCategoryViewModel)null;
            var category = _categories.FirstOrDefault(ctgry => ctgry.Id == categoryId);

            if (category != null)
            {
                var subCategoryNamesById = new Dictionary<Guid, string>();
                var subCategoryNames = new List<string>();
                foreach (var subCategory in new List<Category>())//_subCategories)
                {
                    if (subCategory.CategoryId == categoryId)
                    {
                        subCategoryNamesById.Add(subCategory.Id, subCategory.Name);
                        subCategoryNames.Add(subCategory.Name);
                    }
                }

                var productViewModels = new List<ProductViewModel>();
                var products = _products.Where(product => product.CategoryId == categoryId);
                foreach (var product in products)
                {
                    var subCategoryName = subCategoryNamesById[product.SubCategoryId];
                    var categoryName = GetSantizedCategoryName(category.Name);
                    var imageSource = GetImageSource(categoryName, product.Code);
                    var hasPricing = _parts.Any(part => part.ProductId == product.Id);

                    var productViewModel = new ProductViewModel
                    {
                        Id = product.Id,
                        Code = product.Code,
                        SubCategory = subCategoryName,
                        ShortDescription = product.ShortDescription,
                        ImageSource = imageSource,
                        HasPricing = hasPricing,
                        IsMostPopular = product.MostPopular
                    };
                    productViewModels.Add(productViewModel);
                }

                viewModel = new ProductCategoryViewModel
                {
                    CategoryName = category.Name,
                    SubCategories = subCategoryNames,
                    Products = productViewModels
                };
            }

            return viewModel;
        }

        public ProductDetailViewModel GetProductView(Guid productId)
        {
            var viewModel = (ProductDetailViewModel)null;
            var product = _products.FirstOrDefault(prdct => prdct.Id == productId);

            if (product != null)
            {
                var parts = _parts.Where(part => part.ProductId == productId);
                var partViewModels = new List<PartViewModel>();
                foreach (var part in parts)
                {
                    var partViewModel = new PartViewModel
                    {
                        PartNumber = part.PartNumber,
                        Description = part.Description,
                        Price = part.Price
                    };
                    partViewModels.Add(partViewModel);
                }

                var category = _categories.First(ctgry => ctgry.Id == product.CategoryId);
                var categoryName = GetSantizedCategoryName(category.Name);
                var drawingUrl = product.DrawingUrl ?? GetLocalDrawingUrl(categoryName, product.Code);
                var imageSource = GetImageSource(categoryName, product.Code);
                var installExamplesPth = GetInstallationExamplesPath(categoryName, product.Code);
                viewModel = new ProductDetailViewModel
                {
                    CategoryId = category.Id,
                    Category = category.Name,
                    ProductCode = product.Code,
                    ShortDescription = product.ShortDescription,
                    LongDescription = product.LongDescription,
                    DrawingUrl = drawingUrl,
                    ImageSource = imageSource,
                    NumberOfInstallationExamples = product.NumberOfInstallationExamples,
                    InstallationExamplesPath = installExamplesPth,
                    Parts = partViewModels
                };
                
            }

            return viewModel;
        }

        private string GetInstallationExamplesPath(string category, string productCode)
        {
            return "/products/installation-examples/" + category + "/" + productCode + "/";
        }

        private string GetImageSource(string category, string productCode)
        {
            return "/products/product-images/" + category + "/" + productCode.ToLower() + ".gif";
        }

        private string GetLocalDrawingUrl(string category, string productCode)
        {
            return "/products/drawings/" + category + "/" + productCode.ToLower() + ".pdf";
        }

        private string GetSantizedCategoryName(string category)
        {
            string categoryWithoughSpaces = category.ToLower().Replace(" ", "-");
            return Regex.Replace(categoryWithoughSpaces, @"\W", "");
        }

        public void InitializeFrom()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Part> SearchParts(string query)
        {
            throw new NotImplementedException();
        }
    }
}
