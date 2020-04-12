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
        private readonly IProductDAL _productReadDal;
        private readonly IFileManager _fileManager;

        public AdminController(IProductDAL productReadDal, IFileManager fileManager)
        {
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
        public async Task<IActionResult> Products()
        {
            var getProducts = _productReadDal.GetProducts();
            var getCategories = _productReadDal.GetCategories();
            var getSubCategories = _productReadDal.GetSubCategories();
            await Task.WhenAll(getProducts, getCategories, getSubCategories);
            return new JsonResult(new { products=getProducts.Result, categories=getCategories.Result, subCategories=getSubCategories.Result });
        }

        [TokenAuthorize(Roles = new string[] { RequestRole.ADMIN })]
        public async Task<IActionResult> CreateOrUpdateProduct([FromForm]Guid id, [FromForm]string code, [FromForm]string shortDescription,
             [FromForm]string longDescription,  [FromForm]Guid categoryId, [FromForm]Guid subCategoryId,
            [FromForm]bool mostPopular, [FromForm]IFormFile image, [FromForm]IFormFile drawing)
        {
            var product = new Product
            {
                Id = id,
                Code = code?.Trim(),
                ShortDescription = shortDescription,
                LongDescription = longDescription,
                CategoryId = categoryId,
                SubCategoryId = subCategoryId,
                MostPopular = mostPopular,
                ImageFilename = image?.FileName,
                DrawingFilename = drawing?.FileName,
                IsActive = true
            };
            
            var tasks = new List<Task>();
            var saveProduct = _productReadDal.CreateOrUpdateProduct(product);
            tasks.Add(saveProduct);
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
            await Task.WhenAll(tasks);

            return new OkResult();
        }

        [TokenAuthorize(Roles = new string[] { RequestRole.ADMIN })]
        [HttpDelete]
        [Route("~/Admin/Product/{id:Guid}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {            
            var product = await _productReadDal.GetProduct(id);
            var deleteFiles = _fileManager.DeleteProductFiles(new List<Product> { product });
            var deleteProduct = _productReadDal.DeleteProduct(id);
            await Task.WhenAll(deleteFiles, deleteProduct);

            return new OkResult();
        }

        [TokenAuthorize(Roles = new string[] { RequestRole.ADMIN })]
        public async Task<IActionResult> CreateOrUpdateCategory([FromForm]Guid id, [FromForm]string name, [FromForm]IFormFile image)
        {
            var category = new Category
            {
                Id = id,
                Name = name,
                CategoryId = null,
                ImageFilename = image?.FileName,
                IsActive = true
            };

            var tasks = new List<Task>();
            var saveCategory = _productReadDal.CreateOrUpdateCategory(category);
            tasks.Add(saveCategory);
            
            if (image != null && image.Length > 0)
            {
                var saveImage = _fileManager.OverwriteCategoryImage(image);
                tasks.Add(saveImage);
            }
            await Task.WhenAll(tasks);

            return new OkResult();
        }

        [TokenAuthorize(Roles = new string[] { RequestRole.ADMIN })]
        public async Task<IActionResult> CreateOrUpdateSubCategory([FromForm]Guid Id, [FromForm]string Name, [FromForm]Guid CategoryId)
        {
            var category = new Category
            {
                Id = Id,
                Name = Name?.Trim(),
                CategoryId = CategoryId,
                ImageFilename = null,
                IsActive = true
            };
            await _productReadDal.CreateOrUpdateCategory(category);

            return new OkResult();
        }

        [TokenAuthorize(Roles = new string[] { RequestRole.ADMIN })]
        [HttpDelete]
        [Route("~/Admin/Category/{id:Guid}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var getCategory = _productReadDal.GetCategory(id);
            var getProducts = _productReadDal.GetProductsByCategoryOrSubCategoryId(id);
            await Task.WhenAll(getCategory, getProducts);

            var categoryDelTask = _fileManager.DeleteCategoryImage(getCategory.Result.ImageFilename);
            var productFileDelTask = _fileManager.DeleteProductFiles(getProducts.Result);
            var deleteEntities = _productReadDal.DeleteCategoryAndProducts(id);
            await Task.WhenAll(categoryDelTask, productFileDelTask, deleteEntities);
            
            return new OkResult();
        }

        [TokenAuthorize(Roles = new string[] { RequestRole.ADMIN })]
        [HttpDelete]
        [Route("~/Admin/SubCategory/{id:Guid}")]
        public async Task<IActionResult> DeleteSubCategory(Guid id)
        {
            var products = await _productReadDal.GetProductsByCategoryOrSubCategoryId(id);
            var deleteFiles = _fileManager.DeleteProductFiles(products);
            var deleteEntities = _productReadDal.DeleteSubCategoryAndProducts(id);
            await Task.WhenAll(deleteFiles, deleteEntities);

            return new OkResult();
        }

        [TokenAuthorize(Roles = new string[] { RequestRole.ADMIN })]
        public async Task<IActionResult> CreateOrUpdatePart([FromForm]Guid id, [FromForm]string partNumber, [FromForm]Guid productId, [FromForm]string description, [FromForm]decimal price)
        {
            var part = new Part
            {
                Id = id,
                PartNumber = partNumber?.Trim(),
                ProductId = productId,
                Description = description,
                Price = price,
                IsActive = true
            };
            await _productReadDal.CreateOrUpdatePart(part);

            return new OkResult();
        }

        [TokenAuthorize(Roles = new string[] { RequestRole.ADMIN })]
        [HttpDelete]
        [Route("~/Admin/Part/{id:Guid}")]
        public async Task<IActionResult> DeletePart(Guid id)
        {
            await _productReadDal.DeletePart(id);
            return new OkResult();
        }
    }
}