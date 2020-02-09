using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TheFlightShop.Auth;
using TheFlightShop.DAL;
using TheFlightShop.DAL.Schemas;
using TheFlightShop.IO;
using TheFlightShop.Models;

namespace TheFlightShop.Controllers
{    
    public class AdminController : Controller
    {
        private readonly IHash _hash;
        
        private readonly IProductReadDAL _productReadDal;
        private readonly IFileManager _fileManager;

        public AdminController(IHash hash, IProductReadDAL productReadDal, IFileManager fileManager)
        {
            _hash = hash;
            _productReadDal = productReadDal;
            _fileManager = fileManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        //[TokenAuthorize(Roles = new string[] { RequestRole.ADMIN })]
        //public IActionResult Encrypt(string value)
        //{
        //    var hashAndSalt = _hash.GenerateHashAndSalt(value);
        //    return new JsonResult(new { hash = hashAndSalt.Item1, salt = hashAndSalt.Item2 });
        //}        

        [TokenAuthorize(Roles = new string[] { RequestRole.ADMIN })]
        public IActionResult Products()
        {
            var products = _productReadDal.GetProducts();
            var categories = _productReadDal.GetCategories();
            var subCategories = _productReadDal.GetSubCategories();
            return new JsonResult(new { products, categories, subCategories });
        }

        [TokenAuthorize(Roles = new string[] { RequestRole.ADMIN })]
        public async Task<IActionResult> CreateOrUpdateProduct([FromForm]Guid id, [FromForm]string code, [FromForm]string shortDescription,
             [FromForm]string longDescription,  [FromForm]Guid categoryId, [FromForm]Guid subCategoryId,
            [FromForm]bool mostPopular, [FromForm]IFormFile image, [FromForm]IFormFile drawing)
        {
            var product = new Product
            {
                Id = id,
                Code = code,
                ShortDescription = shortDescription,
                LongDescription = longDescription,
                CategoryId = categoryId,
                SubCategoryId = subCategoryId,
                MostPopular = mostPopular,
                ImageFilename = image?.FileName,
                DrawingFilename = drawing?.FileName,
                IsActive = true
            };
            _productReadDal.CreateOrUpdateProduct(product);
            var tasks = new List<Task>();
            if (image != null && image.Length > 0)
            {
                var imageTask = _fileManager.OverwriteProductImage(image);
                tasks.Add(imageTask);
            } 
            if (drawing != null && drawing.Length > 0)
            {
                var drawingTask = _fileManager.OverwriteProductDrawing(drawing);
                tasks.Add(drawingTask);
            }
            if (tasks.Any())
            {
                await Task.WhenAll(tasks);
            }

            return new OkResult();
        }

        [TokenAuthorize(Roles = new string[] { RequestRole.ADMIN })]
        [HttpDelete]
        [Route("~/Admin/Product/{id:Guid}")]
        public IActionResult DeleteProduct(Guid id)
        {
            _productReadDal.DeleteProduct(id);
            return new OkResult();
        }

        [TokenAuthorize(Roles = new string[] { RequestRole.ADMIN })]
        public IActionResult CreateOrUpdateCategory([FromForm]Guid Id, [FromForm]string Name, [FromForm]Guid? CategoryId)
        {
            var category = new Category
            {
                Id = Id,
                Name = Name,
                CategoryId = CategoryId,
                IsActive = true
            };
            _productReadDal.CreateOrUpdateCategory(category);

            return new OkResult();
        }

        [TokenAuthorize(Roles = new string[] { RequestRole.ADMIN })]
        [HttpDelete]
        [Route("~/Admin/Category/{id:Guid}")]
        public IActionResult DeleteCategory(Guid id)
        {
            _productReadDal.DeleteCategoryAndProducts(id);
            return new OkResult();
        }

        [TokenAuthorize(Roles = new string[] { RequestRole.ADMIN })]
        [HttpDelete]
        [Route("~/Admin/SubCategory/{id:Guid}")]
        public IActionResult DeleteSubCategory(Guid id)
        {
            _productReadDal.DeleteSubCategoryAndProducts(id);
            return new OkResult();
        }

        [TokenAuthorize(Roles = new string[] { RequestRole.ADMIN })]
        public IActionResult CreateOrUpdatePart([FromForm]Guid id, [FromForm]string partNumber, [FromForm]Guid productId, [FromForm]string description, [FromForm]decimal price)
        {
            var part = new Part
            {
                Id = id,
                PartNumber = partNumber,
                ProductId = productId,
                Description = description,
                Price = price,
                IsActive = true
            };
            _productReadDal.CreateOrUpdatePart(part);

            return new OkResult();
        }

        [TokenAuthorize(Roles = new string[] { RequestRole.ADMIN })]
        [HttpDelete]
        [Route("~/Admin/Part/{id:Guid}")]
        public IActionResult DeletePart(Guid id)
        {
            _productReadDal.DeletePart(id);
            return new OkResult();
        }
    }
}