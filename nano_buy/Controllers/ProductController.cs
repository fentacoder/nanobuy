using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NanoShop.Core.Entities;
using NanoShop.Core.Repositories.Products;
using nano_buy.Models.Products;

namespace NanoShop.Web.Controllers
{
    [Authorize]
    public class ProductController : BaseController
    {
        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IProductRepository productRepository, IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("product/create")]
        public JsonResult CreateProduct([FromBody]ProductModel input)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(input.Base64Image);

                //download the System.Drawing.Common nuget package
                Image image;
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    image = Image.FromStream(ms);
                }
                var fileName = $"Product-{DateTime.Now.Ticks}.png";
                image.Save(_webHostEnvironment.WebRootPath + "//product-images//" + fileName);

                _productRepository.Create(new Product
                {
                    Name = input.Name,
                    Price = input.Price,
                    FilePath = fileName,
                    CreatedDateTime = DateTime.Now
                });
                return new JsonResult("Product Created");
            }
            catch (Exception)
            {
                return new JsonResult("Error occured while creating product");
            }
        }
    }
}