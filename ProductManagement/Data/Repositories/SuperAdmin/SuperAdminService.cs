using Microsoft.AspNetCore.Identity;
using ProductManagement.Models.ViewModel;
using System.Collections.Generic;

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
}
