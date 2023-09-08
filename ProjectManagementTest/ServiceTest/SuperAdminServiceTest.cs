using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ProductManagement.Data.Repositories.SuperAdmin;
using ProductManagement.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementTest.ServiceTest
{
    public class SuperAdminServiceTest
    {

        [Fact]
        public void GetAdminUsers_Returns_AdminUserViewModels()
        {
            // Arrange
            var _userManagerMock = new Mock<UserManager<IdentityUser>>(
                 Mock.Of<IUserStore<IdentityUser>>(),
                 Mock.Of<IOptions<IdentityOptions>>(),
                 Mock.Of<IPasswordHasher<IdentityUser>>(),
                 new List<IUserValidator<IdentityUser>>(),
                 new List<IPasswordValidator<IdentityUser>>(),
                 Mock.Of<ILookupNormalizer>(),
                 Mock.Of<IdentityErrorDescriber>(),
                 Mock.Of<IServiceProvider>(),
                 Mock.Of<ILogger<UserManager<IdentityUser>>>());

            // Mock the GetUsersInRoleAsync method to return a list of users
            var adminUsers = new List<IdentityUser>
        {
            new IdentityUser { Id = "1", Email = "admin1@example.com" },
            new IdentityUser { Id = "2", Email = "admin2@example.com" },
        };

            _userManagerMock.Setup(um => um.GetUsersInRoleAsync("Admin"))
                .ReturnsAsync(adminUsers);

            var yourClass = new SuperAdminService(_userManagerMock.Object);

            // Act
            var result = yourClass.GetAdminUsers();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<SuperAdminDashboardViewModel>>(result);
            Assert.Equal(2, result.Count);

            // You can further assert the properties of the SuperAdminDashboardViewModel objects if needed.
        }


            [Fact]
            public async Task UpdateUserAsync_UserExists_ReturnsTrue()
            {
                // Arrange
                var userManagerMock = new Mock<UserManager<IdentityUser>>(
                    Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

                var existingUser = new IdentityUser { Id = "1", Email = "user@example.com" };

                userManagerMock.Setup(um => um.FindByIdAsync(existingUser.Id))
                    .ReturnsAsync(existingUser);

                userManagerMock.Setup(um => um.UpdateAsync(existingUser))
                    .ReturnsAsync(IdentityResult.Success);

                var yourClass = new SuperAdminService(userManagerMock.Object);

                // Act
                var userToUpdate = new IdentityUser { Id = "1", Email = "updated@example.com" };
                var result = await yourClass.UpdateUserAsync(userToUpdate);

                // Assert
                Assert.True(result);
            }


        [Fact]
        public async Task GetUserByIdAsync_UserExists_ReturnsUser()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            var existingUser = new IdentityUser { Id = "1", Email = "user@example.com" };

            userManagerMock.Setup(um => um.FindByIdAsync(existingUser.Id))
                .ReturnsAsync(existingUser);

            var yourClass = new SuperAdminService(userManagerMock.Object);

            // Act
            var result = await yourClass.GetUserByIdAsync(existingUser.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingUser.Id, result.Id);
            Assert.Equal(existingUser.Email, result.Email);
        }

        [Fact]
        public async Task GetUserByIdAsyncUserDoesNotExistReturnsNull()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((IdentityUser)null); // User does not exist

            var yourClass = new SuperAdminService(userManagerMock.Object);

            // Act
            var result = await yourClass.GetUserByIdAsync("nonExistentUserId");

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task GetUserByIdAsync_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((IdentityUser)null); // User does not exist

            var yourClass = new SuperAdminService(userManagerMock.Object);

            // Act
            var result = await yourClass.GetUserByIdAsync("nonExistentUserId");

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task UpdateUserAsync_UserDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((IdentityUser)null); // User does not exist

            var yourClass = new SuperAdminService(userManagerMock.Object);

            // Act
            var userToUpdate = new IdentityUser { Id = "nonExistentUserId", Email = "updated@example.com" };
            var result = await yourClass.UpdateUserAsync(userToUpdate);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CreateUserAsync_SuccessfulCreation_ReturnsTrue()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            var model = new SuperAdminDashboardViewModel
            {
                Email = "newuser@example.com",
                Password = "password123"
            };

            userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<IdentityUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            var yourClass = new SuperAdminService(userManagerMock.Object);

            // Act
            var result = await yourClass.CreateUserAsync(model);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CreateUserAsync_FailedCreation_ReturnsFalse()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            var model = new SuperAdminDashboardViewModel
            {
                Email = "newuser@example.com",
                Password = "password123"
            };

            userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Creation failed" }));

            var yourClass = new SuperAdminService(userManagerMock.Object);

            // Act
            var result = await yourClass.CreateUserAsync(model);

            // Assert
            Assert.False(result);
        }


    }
}
