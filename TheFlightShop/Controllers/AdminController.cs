﻿using System;
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

        [TokenAuthorize(Roles = new string[] { RequestRole.ADMIN })]
        public IActionResult Encrypt(string value)
        {
            var hashAndSalt = _hash.GenerateHashAndSalt(value);
            return new JsonResult(new { hash = hashAndSalt.Item1, salt = hashAndSalt.Item2 });
        }        

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
    }
}