using ProductManagement.Models.ViewModel;
using System;
using System.Collections.Generic;
    public interface IUserService
    {
        void AddUser(AddUserViewModel model);
        AddUserViewModel GetUserById(string id);
        void EditUser(AddUserViewModel user);
        void DeleteUser(string id);
        List<AdminDashboardViewModel> GetAdminUsers();
    }
