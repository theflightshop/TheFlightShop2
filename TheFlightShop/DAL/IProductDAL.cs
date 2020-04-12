using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheFlightShop.DAL.Schemas;
using TheFlightShop.Models;

namespace TheFlightShop.DAL
{
    public interface IProductDAL
    {
        Task<IEnumerable<Category>> GetCategories();
        Task<IEnumerable<Category>> GetSubCategories();

        Task<IEnumerable<Product>> GetProducts();
        Task<Product> GetProduct(Guid id);
        Task<ProductsViewModel> GetProductCategories();
        Task<ProductCategoryViewModel> GetProducts(Guid categoryId);
        Task<ProductDetailViewModel> GetProductView(Guid productId);
        Task<IEnumerable<SearchResult>> SearchParts(string query);
        Task<IEnumerable<Part>> GetParts();
        Task<IEnumerable<Product>> GetProductsByCategoryOrSubCategoryId(Guid categoryOrSubCategoryId);
        Task<Category> GetCategory(Guid id);

        Task CreateOrUpdateProduct(Product product);
        Task DeleteProduct(Guid productId);
        Task CreateOrUpdateCategory(Category category);
        Task DeleteCategoryAndProducts(Guid categoryId);
        Task DeleteSubCategoryAndProducts(Guid subCategoryId);
        Task CreateOrUpdatePart(Part part);
        Task DeletePart(Guid id);
    }
}
