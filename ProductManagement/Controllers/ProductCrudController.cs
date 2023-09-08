using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagement.Data.Repositories;
using ProductManagement.Data.Repositories.ProductCrud;
using ProductManagement.Models.DomainModel;
using ProductManagement.Models.ViewModel;
using System;
using System.Buffers;
using System.Data;
using System.Security.Claims;
using Xunit;

namespace ProductManagement.Controllers
{
    public class ProductCrudController : Controller
    {

        private readonly IProductService _productService;
        public ProductCrudController(IProductService productService)
        {
            

            _productService = productService;
        }


      

        public IActionResult AllProducts()
        {
            var products = _productService.AllProducts();
            return View(products);
        }

       
        [HttpPost]
        public IActionResult AddProduct(ProductModel product)
        {
            if (ModelState.IsValid)
            {
                _productService.AddProduct(product);
                return RedirectToAction("AllProducts");
            }
            return View(product);
        }
        public IActionResult AddProduct()
        {
            return View();
        }


        public IActionResult UpdateProduct(Guid id)
        {
            var product = _productService.GetProductById(id);
            if (product != null)
            {
                return View(product);
            }
            return RedirectToAction("AllProducts");
        }

        [HttpPost]
        public IActionResult EditProduct(ProductModel product)
        {
            if (ModelState.IsValid)
            {
                _productService.UpdateProduct(product);
                return RedirectToAction("AllProducts");
            }
            return View(product);
        }

        public IActionResult DeleteProduct(Guid id)
        {
            try
            {
                _productService.DeleteProduct(id);
                return RedirectToAction("AllProducts");
            }
            catch (Exception)
            {
                return NotFound();
            }
          
        }

    }

}
