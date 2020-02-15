using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.IO
{
    public interface IFileManager
    {
        Task OverwriteProductImage(IFormFile image);
        Task OverwriteProductDrawing(IFormFile drawing);
        Task OverwriteCategoryImage(IFormFile image);
    }
}
