using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TheFlightShop.DAL.Schemas;
using TheFlightShop.Logging;
using TheFlightShop.Models;

namespace TheFlightShop.DAL
{
    public class ProductDAL : IProductDAL
    {
        private const string NON_WORD_CHARACTER_PATTERN = @"[\W]+";
        private const string NUMBER_PATTERN = @"[\d]+";
        private const string LETTER_PATTERN = @"[A-Za-z]+";

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

        private readonly string _connectionString;
        private readonly ILogger _logger;

        public string MaintenanceSubdirectory { get; }

        public ProductDAL(string connectionString, string maintenanceSubdirectory, ILogger logger)
        {
            _connectionString = connectionString;
            _logger = logger;
            MaintenanceSubdirectory = maintenanceSubdirectory;
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            try
            {
                return await GetParentCategories();
            }
            catch (Exception ex)
            {
                var error = ex.InnerException ?? ex;
                throw new FlightShopActionException($"{nameof(ProductDAL)}.{nameof(GetCategories)}", error);
            }
        }

        public async Task<IEnumerable<Category>> GetSubCategories()
        {
            try
            {
                using (var db = new ProductContext(_connectionString))
                {
                    return await db.Categories.Where(category => category.IsActive && category.CategoryId.HasValue).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                var error = ex.InnerException ?? ex;
                throw new FlightShopActionException($"{nameof(ProductDAL)}.{nameof(GetSubCategories)}", error);
            }
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            try
            {
                using (var db = new ProductContext(_connectionString))
                {
                    var stopwatch = Stopwatch.StartNew();

                    var getParts = db.Parts.ToListAsync();
                    var getProducts = db.Products.ToListAsync();
                    await Task.WhenAll(getParts, getProducts);
                    
                    var products = getProducts.Result;
                    var partsByProductId = getParts.Result.GroupBy(part => part.ProductId).ToDictionary(group => group.Key);
                    foreach (var product in products)
                    {
                        if (partsByProductId.ContainsKey(product.Id))
                        {
                            product.Parts = partsByProductId[product.Id];
                        }
                    }

                    stopwatch.Stop();
                    _logger.LogInformation($"{nameof(ProductDAL)}.{nameof(GetProducts)}-{products?.Count ?? 0} products in {stopwatch.ElapsedMilliseconds}ms");

                    return products;
                }
            }
            catch (Exception ex)
            {
                var error = ex.InnerException ?? ex;
                throw new FlightShopActionException($"{nameof(ProductDAL)}.{nameof(GetProducts)}", error);
            }
        }

        public async Task<Product> GetProduct(Guid id)
        {
            try
            {
                using (var db = new ProductContext(_connectionString))
                {
                    return await db.Products.FindAsync(id);
                }
            }
            catch (Exception ex)
            {
                var error = ex.InnerException ?? ex;
                _logger.LogError(error, $"{nameof(ProductDAL)}.{nameof(GetProduct)}");
                throw;
            }
        }

        public async Task<IEnumerable<Part>> GetParts()
        {
            try
            {
                using (var db = new ProductContext(_connectionString))
                {
                    return await db.Parts.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                var error = ex.InnerException ?? ex;
                _logger.LogError(error, $"{nameof(ProductDAL)}.{nameof(GetParts)}");
                throw;
            }
        }

        public async Task<ProductsViewModel> GetProductCategories()
        {
            try
            {
                var parentCategories = await GetParentCategories();
                return new ProductsViewModel { Categories = parentCategories };
            }
            catch (Exception ex)
            {
                var error = ex.InnerException ?? ex;
                _logger.LogError(error, $"{nameof(ProductDAL)}.{nameof(GetProductCategories)}");
                throw;
            }
        }

        private async Task<IEnumerable<Category>> GetParentCategories()
        {
            using (var db = new ProductContext(_connectionString))
            {
                return await db.Categories.Where(category => category.IsActive && !category.CategoryId.HasValue).ToListAsync();
            }
        }

        public async Task<ProductCategoryViewModel> GetProductsByCategory(Guid categoryId)
        {
            try
            {
                using (var db = new ProductContext(_connectionString))
                {
                    var stopwatch = Stopwatch.StartNew();

                    var getProducts = db.Products.Where(product => product.IsActive && product.CategoryId == categoryId).ToListAsync();
                    var getCategory = db.Categories.FindAsync(categoryId);
                    var getSubCategories = db.Categories.Where(cat => cat.CategoryId == categoryId).ToListAsync();
                    await Task.WhenAll(getProducts, getCategory, getSubCategories);

                    var products = getProducts.Result;
                    var category = getCategory.Result;
                    var subCategories = getSubCategories.Result;
                    var viewModel = new ProductCategoryViewModel
                    {
                        CategoryName = category?.Name ?? "N/A",
                        SubCategories = subCategories.Select(c => c.Name).ToList(),
                        Products = products.Select(product => new ProductViewModel
                        {
                            Id = product.Id,
                            Code = product.Code,
                            ShortDescription = product.ShortDescription,
                            IsMostPopular = product.MostPopular,
                            SubCategory = subCategories.First(c => c.Id == product.SubCategoryId).Name,
                            ImageFilename = product.ImageFilename
                        })
                    };

                    stopwatch.Stop();
                    _logger.LogInformation($"{nameof(ProductDAL)}.{nameof(GetProductsByCategory)}-{products.Count} products in {stopwatch.ElapsedMilliseconds}ms");
                    return viewModel;
                }
            }
            catch (Exception ex)
            {
                var error = ex.InnerException ?? ex;
                _logger.LogError(error, $"{nameof(ProductDAL)}.{nameof(GetProductsByCategory)},categoryId={categoryId}");
                throw;
            }
        }

        public async Task<ProductDetailViewModel> GetProductView(Guid productId)
        {
            try
            {
                using (var db = new ProductContext(_connectionString))
                {
                    var stopwatch = Stopwatch.StartNew();

                    var product = await db.Products.FindAsync(productId);
                    var viewModel = (ProductDetailViewModel)null;
                    if (product != null)
                    {
                        var getCategory = db.Categories.FindAsync(product.CategoryId);
                        var getParts = db.Parts.Where(p => p.ProductId == product.Id && p.IsActive).ToListAsync();
                        await Task.WhenAll(getCategory, getParts);
                        var category = getCategory.Result;
                        var parts = getParts.Result;

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
                    else
                    {
                        _logger.LogWarning($"{nameof(ProductDAL)}.{nameof(GetProductView)}-no product found for productId={productId}");
                    }

                    stopwatch.Stop();
                    _logger.LogInformation($"{nameof(ProductDAL)}.{nameof(GetProductView)}-productId={productId} in {stopwatch.ElapsedMilliseconds}ms");
                    return viewModel;
                }
            }
            catch (Exception ex)
            {
                var error = ex.InnerException ?? ex;
                _logger.LogError(error, $"{nameof(ProductDAL)}.{nameof(GetProductView)},productId={productId}");
                throw;
            }
        }

        private int CompareParts(Part a, Part b)
        {
            var result = 0;
            var tokensA = Regex.Split(a.PartNumber, NON_WORD_CHARACTER_PATTERN);
            var tokensB = Regex.Split(b.PartNumber, NON_WORD_CHARACTER_PATTERN);

            for (int i = 0; result == 0 && i < tokensA.Length && i < tokensB.Length; i++)
            {
                var wordMatchA = Regex.Match(tokensA[i], LETTER_PATTERN, RegexOptions.IgnoreCase);
                var wordMatchB = Regex.Match(tokensB[i], LETTER_PATTERN, RegexOptions.IgnoreCase);
                var numberMatchA = Regex.Match(tokensA[i], NUMBER_PATTERN, RegexOptions.IgnoreCase);
                var numberMatchB = Regex.Match(tokensB[i], NUMBER_PATTERN, RegexOptions.IgnoreCase);
                var wordResult = 0;
                var numberResult = 0;

                if (wordMatchA.Success && wordMatchB.Success)
                {
                    wordResult = wordMatchA.Value.CompareTo(wordMatchB.Value);
                }
                if (numberMatchA.Success && numberMatchB.Success)
                {
                    var numberA = long.Parse(numberMatchA.Value);
                    var numberB = long.Parse(numberMatchB.Value);
                    numberResult = numberA.CompareTo(numberB);
                }

                result = wordResult == 0 ? numberResult : wordResult;
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

        public async Task<IEnumerable<SearchResult>> SearchParts(string query)
        {
            try
            {
                using (var db = new ProductContext(_connectionString))
                {
                    var stopwatch = Stopwatch.StartNew();

                    var results = new List<SearchResult>();
                    if (!string.IsNullOrEmpty(query))
                    {
                        var formattedQuery = query.ToLower().Trim();
                        var searchParts = SearchPartsOrProducts(formattedQuery, db);
                        var searchMaintenance = SearchMaintenanceItems(formattedQuery, db);
                        await Task.WhenAll(searchParts, searchMaintenance);
                        results.AddRange(searchParts.Result);
                        results.AddRange(searchMaintenance.Result);
                    }
                    else
                    {
                        _logger.LogWarning($"{nameof(ProductDAL)}.{nameof(SearchParts)}-query is null or empty");
                    }

                    stopwatch.Stop();
                    _logger.LogInformation($"{nameof(ProductDAL)}.{nameof(SearchParts)}-{results.Count} results in {stopwatch.ElapsedMilliseconds}ms, query={query}");
                    return results;
                }
            }
            catch (Exception ex)
            {
                var error = ex.InnerException ?? ex;
                _logger.LogError(error, $"{nameof(ProductDAL)}.{nameof(SearchParts)},query={query}");
                throw;
            }
        }

        private async Task<IEnumerable<PartSearchResult>> SearchPartsOrProducts(string trimmedLowercaseQuery, ProductContext db)
        {
            var results = new List<PartSearchResult>();

            var matchingParts = await db.Parts.Where(part => part.IsActive && MatchesQuery(part, trimmedLowercaseQuery)).ToListAsync();
            if (matchingParts.Any())
            {
                foreach (var part in matchingParts)
                {
                    var product = await db.Products.FindAsync(part.ProductId);
                    if (product == null)
                    {
                        _logger.LogWarning($"{nameof(ProductDAL)}.{nameof(SearchPartsOrProducts)}-part exists with partId={part.Id} and has productId={part.ProductId}, but product wasn't found with that productId.");
                    }
                    else
                    {
                        var result = await GetNewSearchResult(db, product, part);
                        results.Add(result);
                    }
                }
            }
            else
            {
                var matchingProducts = await db.Products.Where(product => product.IsActive && MatchesQuery(product, trimmedLowercaseQuery)).ToListAsync();
                foreach (var product in matchingProducts)
                {
                    var result = await GetNewSearchResult(db, product);
                    results.Add(result);
                }
            }

            return results;
        }

        private async Task<IEnumerable<MaintenanceSearchResult>> SearchMaintenanceItems(string trimmedLowercaseQuery, ProductContext db)
        {
            var results = new List<MaintenanceSearchResult>();
            var matchingItems = await db.MaintenanceItems.Where(item => item.IsActive && MatchesQuery(item, trimmedLowercaseQuery)).ToListAsync();
            foreach (var item in matchingItems)
            {
                var maintenanceFilename = $"{MaintenanceSubdirectory}/{item.ImageFilename}";
                var result = new MaintenanceSearchResult(item.Name, item.Description, item.Controller, item.Action, maintenanceFilename);
                results.Add(result);
            }
            return results;
        }

        public async Task CreateOrUpdateProduct(Product product)
        {
            try
            {
                using (var db = new ProductContext(_connectionString))
                {
                    var stopwatch = Stopwatch.StartNew();

                    var existingProduct = await db.Products.FindAsync(product.Id);
                    if (existingProduct == null)
                    {
                        db.Products.Add(product);
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

                    await db.SaveChangesAsync();

                    stopwatch.Stop();
                    _logger.LogInformation($"{nameof(ProductDAL)}.{nameof(CreateOrUpdateProduct)}-productId={product.Id} saved in {stopwatch.ElapsedMilliseconds}ms");
                }
            }
            catch (Exception ex)
            {
                var error = ex.InnerException ?? ex;
                _logger.LogError(error, $"{nameof(ProductDAL)}.{nameof(CreateOrUpdateProduct)},productId={product?.Id},categoryId={product?.CategoryId}");
                throw;
            }
        }

        public async Task DeleteProduct(Guid productId)
        {
            try
            {
                using (var db = new ProductContext(_connectionString))
                {
                    var product = await db.Products.FindAsync(productId);
                    if (product != null)
                    {
                        db.Products.Remove(product);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                var error = ex.InnerException ?? ex;
                _logger.LogError(error, $"{nameof(ProductDAL)}.{nameof(DeleteProduct)},productId={productId}");
                throw;
            }
        }

        private async Task<PartSearchResult> GetNewSearchResult(ProductContext db, Product product, Part part = null)
        {
            var getCategoryName = GetCategoryById(db, product.CategoryId);
            var getSubCategoryName = GetCategoryById(db, product.SubCategoryId);
            await Task.WhenAll(getCategoryName, getSubCategoryName);
            var name = part == null ? product.Code : part.PartNumber;
            var description = part == null ? product.ShortDescription : part.Description;
            return new PartSearchResult(name, product.Id, description, getCategoryName.Result, getSubCategoryName.Result, product.ImageFilename);
        }

        private async Task<string> GetCategoryById(ProductContext db, Guid categoryId)
        {
            var category = await db.Categories.FindAsync(categoryId);
            return category?.Name ?? null;
        }

        private bool MatchesQuery(Part part, string trimmedLowercaseQuery)
        {
            return (part.PartNumber?.ToLower().Contains(trimmedLowercaseQuery) ?? false) || (part.Description?.ToLower().Contains(trimmedLowercaseQuery) ?? false);
        }

        private bool MatchesQuery(Product product, string trimmedLowercaseQuery)
        {
            return (product.Code?.ToLower().Contains(trimmedLowercaseQuery) ?? false) || (product.ShortDescription?.ToLower().Contains(trimmedLowercaseQuery) ?? false);
        }

        private bool MatchesQuery(MaintenanceItem item, string trimmedLowercaseQuery)
        {
            return (item.Name?.ToLower().Contains(trimmedLowercaseQuery) ?? false) || (item.Keywords?.ToLower().Contains(trimmedLowercaseQuery) ?? false);
        }

        private string GetInstallationExamplesPath(string productCode)
        {
            return "/products/installation-examples/" + productCode + "/";
        }

        public async Task CreateOrUpdateCategory(Category category)
        {
            try
            {
                using (var db = new ProductContext(_connectionString))
                {
                    var existingCategory = await db.Categories.FindAsync(category.Id);
                    if (existingCategory == null)
                    {
                        db.Categories.Add(category);
                    }
                    else
                    {
                        existingCategory.CategoryId = category.CategoryId;
                        existingCategory.Name = category.Name;
                    }

                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var error = ex.InnerException ?? ex;
                _logger.LogError(error, $"{nameof(ProductDAL)}.{nameof(CreateOrUpdateCategory)},categoryId={category?.Id}");
                throw;
            }
        }

        public async Task DeleteCategoryAndProducts(Guid categoryId)
        {
            try
            {
                using (var db = new ProductContext(_connectionString))
                {
                    var stopwatch = Stopwatch.StartNew();

                    var categoryToDelete = await db.Categories.FindAsync(categoryId);
                    if (categoryToDelete != null)
                    {
                        var getDeleteProducts = db.Products.Where(product => product.CategoryId == categoryId).ToListAsync();
                        var getDeleteSubCategories = db.Categories.Where(category => category.CategoryId == categoryId).ToListAsync();
                        await Task.WhenAll(getDeleteProducts, getDeleteSubCategories);
                        db.Products.RemoveRange(getDeleteProducts.Result);
                        await db.SaveChangesAsync();
                        db.Categories.RemoveRange(getDeleteSubCategories.Result);
                        await db.SaveChangesAsync();
                        db.Categories.Remove(categoryToDelete);
                        await db.SaveChangesAsync();
                    }

                    stopwatch.Stop();
                    _logger.LogInformation($"{nameof(ProductDAL)}.{nameof(DeleteCategoryAndProducts)}-categoryId={categoryId} deleted in {stopwatch.ElapsedMilliseconds}ms");
                }
            }
            catch (Exception ex)
            {
                var error = ex.InnerException ?? ex;
                _logger.LogError(error, $"{nameof(ProductDAL)}.{nameof(DeleteCategoryAndProducts)},categoryId={categoryId}");
                throw;
            }
        }

        public async Task DeleteSubCategoryAndProducts(Guid subCategoryId)
        {
            try
            {
                using (var db = new ProductContext(_connectionString))
                {
                    var subCategory = await db.Categories.FindAsync(subCategoryId);
                    if (subCategory != null)
                    {
                        var productsToDelete = await db.Products.Where(product => product.SubCategoryId == subCategoryId).ToListAsync();
                        db.Products.RemoveRange(productsToDelete);
                        await db.SaveChangesAsync();
                        db.Categories.Remove(subCategory);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                var error = ex.InnerException ?? ex;
                _logger.LogError(error, $"{nameof(ProductDAL)}.{nameof(DeleteSubCategoryAndProducts)},subCategoryId={subCategoryId}");
                throw;
            }
        }

        public async Task CreateOrUpdatePart(Part part)
        {
            try
            {
                using (var db = new ProductContext(_connectionString))
                {
                    var existingPart = await db.Parts.FindAsync(part.Id);
                    if (existingPart == null)
                    {
                        db.Parts.Add(part);
                    }
                    else
                    {
                        existingPart.PartNumber = part.PartNumber;
                        existingPart.Description = part.Description;
                        existingPart.Price = part.Price;
                    }

                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var error = ex.InnerException ?? ex;
                _logger.LogError(error, $"{nameof(ProductDAL)}.{nameof(CreateOrUpdatePart)},partId={part?.Id}");
                throw;
            }
        }

        public async Task DeletePart(Guid id)
        {
            try
            {
                using (var db = new ProductContext(_connectionString))
                {
                    var part = await db.Parts.FindAsync(id);
                    if (part != null)
                    {
                        db.Parts.Remove(part);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                var error = ex.InnerException ?? ex;
                _logger.LogError(error, $"{nameof(ProductDAL)}.{nameof(DeletePart)},partId={id}");
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryOrSubCategoryId(Guid categoryOrSubCategoryId)
        {
            try
            {
                using (var db = new ProductContext(_connectionString))
                {
                    var stopwatch = Stopwatch.StartNew();
                    var result = await db.Products.Where(product => product.CategoryId == categoryOrSubCategoryId || product.SubCategoryId == categoryOrSubCategoryId).ToListAsync();
                    stopwatch.Stop();
                    _logger.LogInformation($"{nameof(ProductDAL)}.{nameof(GetProductsByCategoryOrSubCategoryId)}-{result?.Count ?? 0} products in {stopwatch.ElapsedMilliseconds}ms");
                    return result;
                }
            }
            catch (Exception ex)
            {
                var error = ex.InnerException ?? ex;
                _logger.LogError(error, $"{nameof(ProductDAL)}.{nameof(GetProductsByCategoryOrSubCategoryId)},id={categoryOrSubCategoryId}");
                throw;
            }
        }

        public async Task<Category> GetCategory(Guid id)
        {
            try
            {
                using (var db = new ProductContext(_connectionString))
                {
                    return await db.Categories.FindAsync(id);
                }
            }
            catch (Exception ex)
            {
                var error = ex.InnerException ?? ex;
                _logger.LogError(error, $"{nameof(ProductDAL)}.{nameof(GetCategory)},categoryId={id}");
                throw;
            }
        }
    }
}
