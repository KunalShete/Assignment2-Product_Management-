using Microsoft.AspNetCore.Mvc;
using ProductManagement.Data.Repositories.Account;
using ProductManagement.Models.ViewModel;
using ProductManagement.Models.ViewModel;




namespace ProductManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;



        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var (result, roles) = await _accountService.LoginAsync(loginModel.Email, loginModel);
                if (result)
                {

                    if (roles.Contains("SuperAdmin"))
                    {
                        return RedirectToAction("SuperAdminDashboard", "SuperAdmin");
                    }
                    else if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("AdminDashboard", "Admin");
                    }
                    else if (roles.Contains("User"))
                    {
                        return RedirectToAction("UserDashboard", "User");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Please enter valid details");
                }
            }
            return View(loginModel);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.RegisterAsync(registerModel);
                if (result.Succeeded)
                {
                    // Handle successful registration (e.g., redirect to a success page)
                    return RedirectToAction("Login"); // Change to the appropriate action
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(registerModel);
        }



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _accountService.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        public IActionResult AccessDenied()
        {

            return View();

        }

        public IActionResult Index()
        {
            return View();
        }
    }

}