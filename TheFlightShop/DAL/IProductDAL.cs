using System;
using System.Collections.Generic;
using TheFlightShop.DAL.Schemas;
using TheFlightShop.Models;

namespace TheFlightShop.DAL
{
    public interface IProductDAL
    {
        IEnumerable<Category> GetCategories();
        IEnumerable<Category> GetSubCategories();
        void InitializeFrom();

        IEnumerable<Product> GetProducts();
        Product GetProduct(Guid id);
        ProductsViewModel GetProductCategories();
        ProductCategoryViewModel GetProducts(Guid categoryId);
        ProductDetailViewModel GetProductView(Guid productId);
        IEnumerable<SearchResult> SearchParts(string query);
        IEnumerable<Part> GetParts();
        IEnumerable<Product> GetProductsByCategoryOrSubCategoryId(Guid categoryOrSubCategoryId);
        Category GetCategory(Guid id);

        void CreateOrUpdateProduct(Product product);
        void DeleteProduct(Guid productId);
        void CreateOrUpdateCategory(Category category);
        void DeleteCategoryAndProducts(Guid categoryId);
        void DeleteSubCategoryAndProducts(Guid subCategoryId);
        void CreateOrUpdatePart(Part part);
        void DeletePart(Guid id);
    }
}
