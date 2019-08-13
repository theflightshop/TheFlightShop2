using System;
using System.Collections.Generic;
using TheFlightShop.Models;

namespace TheFlightShop.DAL
{
    public interface IProductReadDAL
    {
        IEnumerable<ProductCategoryCount> GetProductCategoryCounts();
        ProductCategoryCount GetProductCategoryCount(Guid categoryId);
        SubCategoryView GetSubCategoryView(Guid subCategoryId);
        ProductView GetProductView(Guid productId);
    }
}
