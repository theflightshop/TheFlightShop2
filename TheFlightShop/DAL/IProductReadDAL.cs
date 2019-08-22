﻿using System;
using System.Collections.Generic;
using TheFlightShop.Models;

namespace TheFlightShop.DAL
{
    public interface IProductReadDAL
    {
        ProductsViewModel GetProductCategories();
        ProductCategoryViewModel GetProducts(Guid categoryId);
        ProductDetailViewModel GetProductView(Guid productId);
    }
}
