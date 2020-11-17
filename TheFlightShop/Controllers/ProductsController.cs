using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheFlightShop.DAL;
using TheFlightShop.IO;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TheFlightShop.Controllers
{
    public class ProductsController : Controller
    {
        private const int SECONDS_IN_ONE_YEAR = 60 * 60 * 24 * 365;
        private const int SECONDS_IN_ONE_WEEK = 60 * 60 * 24 * 7;

        private IProductDAL _productReadDAL;
        private IFileManager _fileManager;

        public ProductsController(IProductDAL productReadDAL, IFileManager fileManager)
        {
            _productReadDAL = productReadDAL;
            _fileManager = fileManager;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var viewModel = await _productReadDAL.GetProductCategories();
            return View(viewModel);
        }

        public async Task<IActionResult> Category(Guid id)
        {
            var viewModel = await _productReadDAL.GetProductsByCategory(id);
            return View(viewModel);
        }

        public async Task<IActionResult> ProductDetail(Guid id)
        {
            var productView = await _productReadDAL.GetProductView(id);
            return productView == null ? (IActionResult)new StatusCodeResult(404) : View(productView);
        }

        [ResponseCache(Duration = SECONDS_IN_ONE_YEAR)]
        [Route("~/Products/CategoryImage/{fileName}")]
        public async Task<IActionResult> GetCategoryImage(string fileName)
        {
            var fileStream = await _fileManager.GetCategoryImage(fileName);
            return GetImageResult(fileStream, fileName);
        }

        [ResponseCache(Duration = SECONDS_IN_ONE_YEAR)]
        [Route("~/Products/ProductImage/{fileName}")]
        public async Task<IActionResult> GetProductImage(string fileName)
        {
            var fileStream = await _fileManager.GetProductFile(fileName);
            return GetImageResult(fileStream, fileName);
        }

        [ResponseCache(Duration = SECONDS_IN_ONE_YEAR)]
        [Route("~/Products/ProductImage/Maintenance/{fileName}")]
        public async Task<IActionResult> GetMaintenanceItemImage(string fileName)
        {
            var fileStream = await _fileManager.GetProductFile($"{_productReadDAL.MaintenanceSubdirectory}/{fileName}");
            return GetImageResult(fileStream, fileName);
        }

        private IActionResult GetImageResult(Stream fileStream, string fileName)
        {
            IActionResult result;
            if (fileStream != null && fileStream.Length > 0)
            {
                fileStream.Position = 0;
                var fileType = fileName.Split('.')[1];
                var contentType = $"image/{fileType}";
                result = File(fileStream, contentType);
            }
            else
            {
                result = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
            return result;
        }

        [Route("~/Products/ProductDrawing/{fileName}")]
        public async Task<IActionResult> GetProductDrawing(string fileName)
        {
            var fileStream = await _fileManager.GetProductDrawing(fileName);
            return GetFilestreamResponse(fileStream);
        }

        [Route("~/Products/IsoCertificate")]
        public async Task<IActionResult> GetIsoCertificate()
        {
            var fileName = Environment.GetEnvironmentVariable("ISO_CERTIFICATE_FILENAME");
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "AS9120B_Certification.pdf"; 
            }
            var fileStream = await _fileManager.GetProductFile(fileName);
            return GetFilestreamResponse(fileStream);
        }

        private IActionResult GetFilestreamResponse(Stream fileStream)
        {
            IActionResult result;
            if (fileStream != null && fileStream.Length > 0)
            {
                fileStream.Position = 0;
                result = File(fileStream, "application/pdf");
            }
            else
            {
                result = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
            return result;
        }
    }
}
