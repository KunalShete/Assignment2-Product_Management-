using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Data.Repositories.SuperAdmin;
using ProductManagement.Models.ViewModel;

namespace ProductManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private readonly ISuperAdminService _superAdminService;

        public SuperAdminController(ISuperAdminService superAdminService)
        {
            _superAdminService = superAdminService;
        }

        public IActionResult SuperAdminDashboard()
        {
            var adminUserViewModels = _superAdminService.GetAdminUsers();
            return View(adminUserViewModels);
        }

        public IActionResult AddAdmin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddAdmin(AddAdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _superAdminService.CreateAdminUser(model.Email, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("SuperAdminDashboard");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        public IActionResult EditAdmin(string id)
        {
            var model = _superAdminService.GetAdminUser(id);
            if (model != null)
            {
                return View(model);
            }
            return RedirectToAction("SuperAdminDashboard");
        }

        [HttpPost]
        public IActionResult EditAdmin(EditAdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _superAdminService.UpdateAdminUser(model);
                if (result.Succeeded)
                {
                    return RedirectToAction("SuperAdminDashboard");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        public IActionResult DeleteAdmin(string id)
        {
            var result = _superAdminService.DeleteAdminUser(id);
            if (result != null && result.Succeeded)
            {
                return RedirectToAction("SuperAdminDashboard");
            }
            ModelState.AddModelError(string.Empty, "Error deleting admin user.");
            return RedirectToAction("SuperAdminDashboard");
        }






        [HttpGet]
        public async Task<IActionResult> UserDashboard()
        {
            var users = await _superAdminService.GetUsersAsync();
            return View(users);
        }
        

        [HttpPost]
        public async Task<IActionResult> PromoteToAdmin(string userId)
        {
            var result = await _superAdminService.PromoteUserToAdminAsync(userId);
            if (result)
            {
                return RedirectToAction("SuperAdminDashboard");
            }

            ModelState.AddModelError(string.Empty, "Failed to promote user to admin.");
            return RedirectToAction("SuperAdminDashboard");
        }




    }


}
