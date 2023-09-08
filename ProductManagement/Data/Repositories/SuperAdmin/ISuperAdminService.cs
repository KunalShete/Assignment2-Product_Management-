using Microsoft.AspNetCore.Identity;
using ProductManagement.Models.ViewModel;





namespace ProductManagement.Data.Repositories.SuperAdmin
{
    public interface ISuperAdminService
    {
        List<SuperAdminDashboardViewModel> GetAdminUsers();
        IdentityResult CreateAdminUser(string email, string password);
        EditAdminViewModel GetAdminUser(string id);
        IdentityResult UpdateAdminUser(EditAdminViewModel model);
        IdentityResult DeleteAdminUser(string id);




        Task<IEnumerable<IdentityUser>> GetUsersAsync();
        Task<IdentityUser> GetUserByIdAsync(string id);
        Task<bool> UpdateUserAsync(IdentityUser user);
        Task<bool> PromoteUserToAdminAsync(string userId);

    }
}