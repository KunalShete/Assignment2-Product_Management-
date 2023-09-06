using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagement.Controllers;
using ProductManagement.Models.ViewModel;
using Xunit;
using Microsoft.AspNetCore.Identity;

namespace SuperAdminControllerTest
{
        public class SuperAdminControllerTest
        {
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

    }
}
