using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data.Repositories;
using ProductManagement.Models.ViewModel;
using System.Data;

namespace ProductManagement.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly IProductService _productRepository;

        public UserController(IProductService productRepository)
        {

            _productRepository = productRepository;
        }

        public IActionResult ViewProducts()
        {
            var products = _productRepository.AllProducts();

            return View(products);
        }
        public IActionResult UserDashboard()
        {
            var userViewModel = new UserDashboardViewModel();
            return View("UserDashboard", userViewModel);
        }
    }
}
