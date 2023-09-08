using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data.Repositories.ProductCrud;

using ProductManagement.Models.ViewModel;
using System.Data;

namespace ProductManagement.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly IProductService _productService;

        public UserController(IProductService productService)
        {

            _productService = productService;
        }

        public IActionResult ViewProducts()
        {
            var products = _productService.AllProducts();

            return View(products);
        }
        public IActionResult UserDashboard()
        {
            var userViewModel = new UserDashboardViewModel();
            return View("UserDashboard", userViewModel);
        }
    }
}
