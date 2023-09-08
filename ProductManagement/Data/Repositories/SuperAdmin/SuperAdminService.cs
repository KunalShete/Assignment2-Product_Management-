using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Models.ViewModel;
using System.Collections.Generic;

namespace ProductManagement.Data.Repositories.SuperAdmin
{
    public class SuperAdminService : ISuperAdminService
    {
        private readonly UserManager<IdentityUser> _userManager;


        public SuperAdminService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;

        }

        public List<SuperAdminDashboardViewModel> GetAdminUsers()
        {
            var adminUsers = _userManager.GetUsersInRoleAsync("Admin").Result;
            var adminUserViewModels = adminUsers.Select(u => new SuperAdminDashboardViewModel
            {
                Id = u.Id,
                Email = u.Email
            }).ToList();

            return adminUserViewModels;
        }

        public IdentityResult CreateAdminUser(string email, string password)
        {
            var adminUser = new IdentityUser { UserName = email, Email = email };
            var result = _userManager.CreateAsync(adminUser, password).Result;

            if (result.Succeeded)
            {
                _userManager.AddToRoleAsync(adminUser, "Admin").Wait();
            }

            return result;
        }

        public EditAdminViewModel GetAdminUser(string id)
        {
            var adminUser = _userManager.FindByIdAsync(id).Result;
            if (adminUser != null)
            {
                return new EditAdminViewModel
                {
                    Id = adminUser.Id,
                    Email = adminUser.Email
                };
            }
            return null;
        }

        public IdentityResult UpdateAdminUser(EditAdminViewModel model)
        {
            var adminUser = _userManager.FindByIdAsync(model.Id).Result;
            if (adminUser != null)
            {
                adminUser.Email = model.Email;
                return _userManager.UpdateAsync(adminUser).Result;
            }
            return null;
        }

        public IdentityResult DeleteAdminUser(string id)
        {
            var adminUser = _userManager.FindByIdAsync(id).Result;
            if (adminUser != null)
            {
                return _userManager.DeleteAsync(adminUser).Result;
            }
            return null;
        }





        public async Task<IEnumerable<IdentityUser>> GetUsersAsync()
        {
            return await _userManager.GetUsersInRoleAsync("User");
        }

        public async Task<bool> CreateUserAsync(SuperAdminDashboardViewModel model)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                return true;
            }
            return false;
        }
        public async Task<IdentityUser> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }


        public async Task<bool> UpdateUserAsync(IdentityUser user)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id);
            if (existingUser == null)
            {
                return false;
            }
            existingUser.Email = user.Email;
            existingUser.UserName = user.Email;

            var result = await _userManager.UpdateAsync(existingUser);

            return result.Succeeded;
        }

        public async Task<bool> PromoteUserToAdminAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            var result = await _userManager.AddToRoleAsync(user, "Admin");
            if (result.Succeeded)
            {
                await _userManager.RemoveFromRoleAsync(user, "User");
                return true;
            }
            return false;
        }



    }

}