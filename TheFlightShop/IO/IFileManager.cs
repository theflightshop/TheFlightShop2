using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheFlightShop.DAL.Schemas;

namespace TheFlightShop.IO
{
    public interface IFileManager
    {
        Task<bool> OverwriteProductImage(IFormFile image);
        Task<bool> OverwriteProductDrawing(IFormFile drawing);
        Task<bool> OverwriteCategoryImage(IFormFile image);

        Task<Stream> GetProductFile(string fileName);
        Task<Stream> GetProductDrawing(string fileName);
        Task<Stream> GetCategoryImage(string fileName);

        Task DeleteProductFiles(IEnumerable<Product> products);
        Task DeleteCategoryImage(string fileName);
    }
}
