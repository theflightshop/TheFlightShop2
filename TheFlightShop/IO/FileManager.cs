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
        private readonly string _categoryImgPath;

        public FileManager(string imagesPath = "wwwroot/products/product-images/", string drawingsPath = "wwwroot/products/drawings/",
            string categoryImagesPath = "wwwroot/products/category-images/")
        {
            _imagesPath = imagesPath;
            _drawingsPath = drawingsPath;
            _categoryImgPath = categoryImagesPath;
        }

        public async Task OverwriteProductDrawing(IFormFile drawing)
        {
            await OverwriteFile(_drawingsPath, drawing);
        }

        public async Task OverwriteProductImage(IFormFile image)
        {
            await OverwriteFile(_imagesPath, image);
        }

        public async Task OverwriteCategoryImage(IFormFile image)
        {
            await OverwriteFile(_categoryImgPath, image);
        }

        private async Task OverwriteFile(string directory, IFormFile file)
        {
            using (var fileStream = new FileStream(directory + file.FileName, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
        }
    }
}
