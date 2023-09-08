using ProductManagement.Models.ViewModel;
using System;
using System.Collections.Generic;

namespace ProductManagement.Data.Repositories.UserCrud
{
    public interface IUserService
    {
        void AddUser(AddUserViewModel user);
        AddUserViewModel GetUserById(string id);
        void EditUser(AddUserViewModel user);
        void DeleteUser(string id);
        List<AdminDashboardViewModel> GetAdminUsers();
    }
}