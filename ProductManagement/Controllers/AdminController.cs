using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Data.Repositories.UserCrud;
using ProductManagement.Models.ViewModel;
using System.Threading.Tasks;

namespace ProductManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }


        public IActionResult ViewUser()
        {
            var adminUserViewModels = _userService.GetAdminUsers();
            return View(adminUserViewModels);
        }

 

        public IActionResult AdminDashboard()
        {
            var adminViewModel = new AdminDashboardViewModel();
            return View("AdminDashboard", adminViewModel);
        }
        public IActionResult AddUser()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddUser(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                _userService.AddUser(model);
                return RedirectToAction("ViewUser");
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult EditUser(string id)
        {
            var model = _userService.GetUserById(id);
            if (model != null)
            {
                return View(model);
            }
            return RedirectToAction("ViewUser");
        }

        [HttpPost]
        public IActionResult EditUser(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                _userService.EditUser(model);
                return RedirectToAction("ViewUser");
            }
            return View(model);
        }





        public IActionResult DeleteUser(string id)
        {
            _userService.DeleteUser(id);
            return RedirectToAction("ViewUser");
        }
    }
}
