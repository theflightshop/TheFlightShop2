using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.IO
{
    public class FileManager : IFileManager
    {
        private readonly string _imagesPath;
        private readonly string _drawingsPath;

        public FileManager(string imagesPath = "wwwroot/products/product-images/", string drawingsPath = "wwwroot/products/drawings/")
        {
            _imagesPath = imagesPath;
            _drawingsPath = drawingsPath;
        }

        public async Task OverwriteProductDrawing(IFormFile drawing)
        {
            using (var fileStream = new FileStream(_drawingsPath + drawing.FileName, FileMode.Create))
            {
                await drawing.CopyToAsync(fileStream);
            }
        }

        public async Task OverwriteProductImage(IFormFile image)
        {
            using (var fileStream = new FileStream(_imagesPath + image.FileName, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
        }
    }
}
