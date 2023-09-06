using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagement.Controllers;
using ProductManagement.Models.ViewModel;

using Xunit;

namespace UserCrud
{
    public class AdminControllerTest
    {
        [Fact]
        public void TestViewUser()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(service => service.GetAdminUsers())
                .Returns(new List<AdminDashboardViewModel>
                {
                    new AdminDashboardViewModel
                    {
                        Id = "1",
                        Name = "User1",
                        Email = "user1@example.com"
                    },
                    new AdminDashboardViewModel
                    {
                        Id = "2",
                        Name = "User2",
                        Email = "user2@example.com"
                    }
                });
            var adminController = new AdminController(userServiceMock.Object);

            // Act
            var result = adminController.ViewUser() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<List<AdminDashboardViewModel>>(result.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void TestAddUser_ValidModel()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var adminController = new AdminController(userServiceMock.Object);
            var userToAdd = new AddUserViewModel
            {
                Email = "newuser@example.com",
                Password = "Password123",
                ConfirmPassword = "Password123"
            };

            // Act
            var result = adminController.AddUser(userToAdd) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ViewUser", result.ActionName);
            userServiceMock.Verify(service => service.AddUser(userToAdd), Times.Once);
        }

        [Fact]
        public void TestAddUser_InvalidModel()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var adminController = new AdminController(userServiceMock.Object);
            var invalidUser = new AddUserViewModel
            {
                // Invalid model without required fields
            };
            adminController.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = adminController.AddUser(invalidUser) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(adminController.ModelState.IsValid);
        }

        [Fact]
        public void TestEditUser_ValidModel()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(service => service.GetUserById("1"))
                .Returns(new AddUserViewModel
                {
                    Id = "1",
                    Email = "user1@example.com"
                });
            var adminController = new AdminController(userServiceMock.Object);
            var userToEdit = new AddUserViewModel
            {
                Id = "1",
                Email = "edited@example.com"
            };

            // Act
            var result = adminController.EditUser(userToEdit) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ViewUser", result.ActionName);
            userServiceMock.Verify(service => service.EditUser(userToEdit), Times.Once);
        }

        [Fact]
        public void TestDeleteUser()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var adminController = new AdminController(userServiceMock.Object);

            // Act
            var result = adminController.DeleteUser("1") as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ViewUser", result.ActionName);
            userServiceMock.Verify(service => service.DeleteUser("1"), Times.Once);
        }
    }
}
