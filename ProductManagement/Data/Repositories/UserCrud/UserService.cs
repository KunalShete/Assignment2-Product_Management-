﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using ProductManagement.Models.DomainModel;
using ProductManagement.Models.ViewModel;

namespace ProductManagement.Data.Repositories.UserCrud
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;


        public UserService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public List<AdminDashboardViewModel> GetAdminUsers()
        {
            var adminUsers = _userManager.GetUsersInRoleAsync("User").Result;
            var adminUserViewModels = adminUsers.Select(u => new AdminDashboardViewModel
            {
                Id = u.Id,
                Name = u.UserName,
                Email = u.Email
            }).ToList();

            return adminUserViewModels;
        }

        public void AddUser(AddUserViewModel user)
        {
            var identityUser = new IdentityUser { UserName = user.Email, Email = user.Email };

            var result = _userManager.CreateAsync(identityUser, user.Password).Result;

            if (result.Succeeded)
            {
                _userManager.AddToRoleAsync(identityUser, "User").Wait();
            }
        }
        public AddUserViewModel GetUserById(string id)
        {
            var identityUser = _userManager.FindByIdAsync(id).Result;
            if (identityUser != null)
            {
                return new AddUserViewModel
                {
                    Id = identityUser.Id,
                    Email = identityUser.Email
                };
            }
            return null;
        }


        public void EditUser(AddUserViewModel user)
        {
            var existingUser = _userManager.Users.FirstOrDefault(p => p.Id == user.Id);
            if (existingUser != null)
            {
                existingUser.Email = user.Email;

                // Update other properties as needed
               _userManager.UpdateAsync(existingUser);
            }
        }
        public void DeleteUser(string id)
        {
            var identityUser = _userManager.FindByIdAsync(id).Result;
            if (identityUser != null)
            {
                var result = _userManager.DeleteAsync(identityUser).Result;
                // Handle the delete result as needed
            }
        }
    }
}
