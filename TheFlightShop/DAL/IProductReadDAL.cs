using System;
using System.Collections.Generic;
using TheFlightShop.DAL.Schemas;
using TheFlightShop.Models;

namespace TheFlightShop.DAL
{
    public interface IProductReadDAL
    {
        IEnumerable<Category> GetCategories();
        IEnumerable<Category> GetSubCategories();
        void InitializeFrom();

        IEnumerable<Product> GetProducts();
        ProductsViewModel GetProductCategories();
        ProductCategoryViewModel GetProducts(Guid categoryId);
        ProductDetailViewModel GetProductView(Guid productId);
        IEnumerable<SearchResult> SearchParts(string query);

        void CreateOrUpdateProduct(Product product);
        void DeleteProduct(Guid productId);
        void CreateOrUpdateCategory(Category category);
        void DeleteCategoryAndProducts(Guid categoryId);
        void DeleteSubCategoryAndProducts(Guid subCategoryId);
        void CreateOrUpdatePart(Part part);
        void DeletePart(Guid id);
    }
}
