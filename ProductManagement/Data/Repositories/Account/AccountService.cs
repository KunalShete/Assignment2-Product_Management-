using Microsoft.AspNetCore.Identity;
using ProductManagement.Models.ViewModel;

namespace ProductManagement.Data.Repositories.Account
{
        public class AccountService : IAccountService
        {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;



        public AccountService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public async Task<(bool Succeeded, IEnumerable<string> Roles)> LoginAsync(string email, LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(email);



            if (user == null)
            {
                // User not found
                return (false, Enumerable.Empty<string>());
            }



            var result = await _signInManager.PasswordSignInAsync(email, model.Password, model.RememberMe, lockoutOnFailure: false);



            if (result.Succeeded)
            {
                var roles = await GetRolesAsync(user);
                return (true, roles);
            }



            return (false, Enumerable.Empty<string>());
        }

        public async Task<IdentityUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IEnumerable<string>> GetRolesAsync(IdentityUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

      

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel registerModel)
        {
            var user = new IdentityUser { UserName = registerModel.Email, Email = registerModel.Email };
            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (result.Succeeded)
            {
                var role = "User";
                await _userManager.AddToRoleAsync(user, role);
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return result;
        }

        public async Task SignOutAsync()
        {
            // Your logout logic here
            await _signInManager.SignOutAsync();
        }

    }
}
