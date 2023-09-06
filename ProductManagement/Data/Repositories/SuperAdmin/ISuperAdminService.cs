using Microsoft.AspNetCore.Identity;
using ProductManagement.Models.ViewModel;

public interface ISuperAdminService
{
    List<SuperAdminDashboardViewModel> GetAdminUsers();
    IdentityResult CreateAdminUser(string email, string password);
    EditAdminViewModel GetAdminUser(string id);
    IdentityResult UpdateAdminUser(EditAdminViewModel model);
    IdentityResult DeleteAdminUser(string id);
}
