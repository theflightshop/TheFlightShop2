﻿using System;
using System.Collections.Generic;

namespace TheFlightShop.Models
{
    public class ProductCategoryViewModel
    {
        public string CategoryName { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
