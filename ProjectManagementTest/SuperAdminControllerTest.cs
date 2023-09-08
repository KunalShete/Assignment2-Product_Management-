using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagement.Controllers;
using ProductManagement.Models.ViewModel;
using Xunit;
using Microsoft.AspNetCore.Identity;
using ProductManagement.Data.Repositories.SuperAdmin;
using ProductManagement.Data.Repositories.Account;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SuperAdminControllerTest
{
        public class SuperAdminControllerTest
        {




        [Fact]
        public void TestAddAdminAction()
        {
            var mockAccountService = new Mock<ISuperAdminService>();
            var controller = new SuperAdminController(mockAccountService.Object);



            // Act
            var result = controller.AddAdmin() as ViewResult;



            // Assert
            Assert.NotNull(result);

        }


        [Fact]
        public void EditAdmin_UserFound_ReturnsViewResult()
        {
            // Arrange
            var userId = "1";
            var editModel = new EditAdminViewModel { Id = userId, Email = "admin@example.com" };

            var superAdminService = new Mock<ISuperAdminService>();
            superAdminService.Setup(s => s.GetAdminUser(userId)).Returns(editModel);

            var controller = new SuperAdminController(superAdminService.Object);

            // Act
            var result = controller.EditAdmin(userId);

            // Assert
            Assert.IsType<ViewResult>(result);
        }


        [Fact]
        public void EditAdmin_UserNotFound_RedirectsToSuperAdminDashboard()
        {
            // Arrange
            var userId = "nonexistent";
            var superAdminService = new Mock<ISuperAdminService>();
            superAdminService.Setup(s => s.GetAdminUser(userId)).Returns((EditAdminViewModel)null);

            var controller = new SuperAdminController(superAdminService.Object);

            // Act
            var result = controller.EditAdmin(userId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SuperAdminDashboard", result.ActionName);
        }



        [Fact]
        public void DeleteAdmin_SuccessfulDelete_RedirectsToSuperAdminDashboard()
        {
            // Arrange
            var userId = "1";
            var identityResult = IdentityResult.Success;

            var superAdminService = new Mock<ISuperAdminService>();
            superAdminService.Setup(s => s.DeleteAdminUser(userId)).Returns(identityResult);

            var controller = new SuperAdminController(superAdminService.Object);

            // Act
            var result = controller.DeleteAdmin(userId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SuperAdminDashboard", result.ActionName);
        }

        [Fact]
        public void DeleteAdmin_UnsuccessfulDelete_ReturnsErrorModelState()
        {
            // Arrange
            var userId = "2";
            var identityResult = IdentityResult.Failed(new IdentityError { Description = "Delete failed." });

            var superAdminService = new Mock<ISuperAdminService>();
            superAdminService.Setup(s => s.DeleteAdminUser(userId)).Returns(identityResult);

            var controller = new SuperAdminController(superAdminService.Object);

            // Act
            var result = controller.DeleteAdmin(userId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SuperAdminDashboard", result.ActionName);
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey(string.Empty));
            Assert.Equal("Error deleting admin user.", controller.ModelState[string.Empty].Errors[0].ErrorMessage);
        }


    /*    [Fact]
        public async Task UserDashboard_ReturnsViewWithUsers()
        {
            // Arrange
            var users = new List<IdentityUser>
            {
                new IdentityUser { Id = "1", Email = "user1@example.com" },
                new IdentityUser { Id = "2", Email = "user2@example.com" }
            };

            var superAdminServiceMock = new Mock<ISuperAdminService>();
            superAdminServiceMock.Setup(s => s.GetUsersAsync()).ReturnsAsync(users);

            var controller = new SuperAdminController(superAdminServiceMock.Object);

            // Act
            var result = await controller.UserDashboard() as ViewResult;
          //  var model = result.ViewData.Model as List<IdentityUser>;

            // Assert
            Assert.IsType<List<SuperAdminDashboardViewModel>>(result.Model);

            var model = result.Model as List<SuperAdminDashboardViewModel>;
            Assert.NotNull(model);
            Assert.Equal(users.Count, model.Count); // Assert the number of users
            Assert.Equal("UserDashboard", result.ViewName);
        }*/


        [Fact]
        public void SuperAdminDashboard()
        {
            // Arrange
            var mockService = new Mock<ISuperAdminService>();
            var adminUsers = new List<SuperAdminDashboardViewModel>
        {
            new SuperAdminDashboardViewModel { Id = "1", Email = "admin1@example.com" },
            new SuperAdminDashboardViewModel { Id = "2", Email = "admin2@example.com" },
        };
            mockService.Setup(service => service.GetAdminUsers()).Returns(adminUsers);

            var controller = new SuperAdminController(mockService.Object);

            // Act
            var result = controller.SuperAdminDashboard() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as List<SuperAdminDashboardViewModel>;
            Assert.NotNull(model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void AddAdmin()
        {
            // Arrange
            var mockService = new Mock<ISuperAdminService>();
            mockService.Setup(service => service.CreateAdminUser(It.IsAny<string>(), It.IsAny<string>()))
                       .Returns(IdentityResult.Success);

            var controller = new SuperAdminController(mockService.Object);
            var model = new AddAdminViewModel { Email = "admin@example.com", Password = "password" };

            // Act
            var result = controller.AddAdmin(model) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SuperAdminDashboard", result.ActionName);
        }

        [Fact]
        public void EditAdmin()
        {
            // Arrange
            var mockService = new Mock<ISuperAdminService>();
            mockService.Setup(service => service.GetAdminUser(It.IsAny<string>()))
                       .Returns(new EditAdminViewModel { Id = "1", Email = "admin@example.com" });
            mockService.Setup(service => service.UpdateAdminUser(It.IsAny<EditAdminViewModel>()))
                       .Returns(IdentityResult.Success);

            var controller = new SuperAdminController(mockService.Object);
            var model = new EditAdminViewModel { Id = "1", Email = "updated@example.com" };

            // Act
            var result = controller.EditAdmin(model) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SuperAdminDashboard", result.ActionName);
        }

        [Fact]
        public async Task PromoteToAdmin_ValidUserId_ReturnsRedirectToDashboard()
        {
            // Arrange
            var mockService = new Mock<ISuperAdminService>();
            mockService.Setup(service => service.PromoteUserToAdminAsync(It.IsAny<string>()))
                       .ReturnsAsync(true);

            var controller = new SuperAdminController(mockService.Object);
            var userId = "UserId";

            // Act
            var result = await controller.PromoteToAdmin(userId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SuperAdminDashboard", result.ActionName);
        }

        [Fact]
        public async Task PromoteToAdmin_InvalidUserId_ReturnsRedirectToDashboard()
        {
            // Arrange
            var mockService = new Mock<ISuperAdminService>();
            mockService.Setup(service => service.PromoteUserToAdminAsync(It.IsAny<string>()))
                       .ReturnsAsync(false);

            var controller = new SuperAdminController(mockService.Object);
            var userId = "InvalidUserId"; // Non-existent user ID

            // Act
            var result = await controller.PromoteToAdmin(userId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SuperAdminDashboard", result.ActionName);
        }




    }
}
