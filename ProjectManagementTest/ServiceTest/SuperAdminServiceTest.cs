using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ProductManagement.Data.Repositories.SuperAdmin;
using ProductManagement.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ProjectManagementTest.ServiceTest
{
    public class SuperAdminServiceTest
    {
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly SuperAdminService _superAdminService;


        public SuperAdminServiceTest()
        {
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            var identityOptionsMock = new Mock<IOptions<IdentityOptions>>();
            var passwordHasherMock = new Mock<IPasswordHasher<IdentityUser>>();
            var userValidators = new List<IUserValidator<IdentityUser>>();
            var passwordValidators = new List<IPasswordValidator<IdentityUser>>();
            var lookupNormalizerMock = new Mock<ILookupNormalizer>();
            var errorDescriberMock = new Mock<IdentityErrorDescriber>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            var loggerMock = new Mock<ILogger<UserManager<IdentityUser>>>();

            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                userStoreMock.Object,
                identityOptionsMock.Object,
                passwordHasherMock.Object,
                userValidators,
                passwordValidators,
                lookupNormalizerMock.Object,
                errorDescriberMock.Object,
                serviceProviderMock.Object,
                loggerMock.Object);

            _superAdminService = new SuperAdminService(_userManagerMock.Object);
        }


        [Fact]
        public void GetAdminUsers_Returns_AdminUserViewModels()
        {
            // Arrange
            var adminUsers = new List<IdentityUser>
            {
                new IdentityUser { Id = "1", Email = "admin1@example.com" },
                new IdentityUser { Id = "2", Email = "admin2@example.com" },
            };

            _userManagerMock.Setup(um => um.GetUsersInRoleAsync("Admin"))
                .ReturnsAsync(adminUsers);

            // Act
            var result = _superAdminService.GetAdminUsers();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<SuperAdminDashboardViewModel>>(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task UpdateUserAsync_UserExists_ReturnsTrue()
        {
            // Arrange
            var existingUser = new IdentityUser { Id = "1", Email = "user@example.com" };
            _userManagerMock.Setup(um => um.FindByIdAsync(existingUser.Id))
                .ReturnsAsync(existingUser);

            _userManagerMock.Setup(um => um.UpdateAsync(existingUser))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var userToUpdate = new IdentityUser { Id = "1", Email = "updated@example.com" };
            var result = await _superAdminService.UpdateUserAsync(userToUpdate);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetUserByIdAsync_UserExists_ReturnsUser()
        {
            // Arrange
            var existingUser = new IdentityUser { Id = "1", Email = "user@example.com" };
            _userManagerMock.Setup(um => um.FindByIdAsync(existingUser.Id))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _superAdminService.GetUserByIdAsync(existingUser.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingUser.Id, result.Id);
            Assert.Equal(existingUser.Email, result.Email);
        }

        [Fact]
        public async Task GetUserByIdAsync_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((IdentityUser)null);

            // Act
            var result = await _superAdminService.GetUserByIdAsync("nonExistentUserId");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateUserAsync_UserUpdateFails_ReturnsFalse()
        {
            // Arrange
            var existingUser = new IdentityUser { Id = "1", Email = "user@example.com" };
            _userManagerMock.Setup(um => um.FindByIdAsync(existingUser.Id))
                .ReturnsAsync(existingUser);

            _userManagerMock.Setup(um => um.UpdateAsync(existingUser))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Update failed" }));

            // Act
            var userToUpdate = new IdentityUser { Id = "1", Email = "updated@example.com" };
            var result = await _superAdminService.UpdateUserAsync(userToUpdate);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CreateUserAsync_SuccessfulCreation_ReturnsTrue()
        {
            // Arrange
            var model = new SuperAdminDashboardViewModel
            {
                Email = "newuser@example.com",
                Password = "password123"
            };

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<IdentityUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _superAdminService.CreateUserAsync(model);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CreateUserAsync_CreationFails_ReturnsFalse()
        {
            // Arrange
            var model = new SuperAdminDashboardViewModel
            {
                Email = "newuser@example.com",
                Password = "password123"
            };

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Creation failed" }));

            // Act
            var result = await _superAdminService.CreateUserAsync(model);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetAdminUsers_NoAdminUsers_ReturnsEmptyList()
        {
            // Arrange
            var adminUsers = new List<IdentityUser>();

            _userManagerMock.Setup(um => um.GetUsersInRoleAsync("Admin"))
                .ReturnsAsync(adminUsers);

            // Act
            var result = _superAdminService.GetAdminUsers();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<SuperAdminDashboardViewModel>>(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUserByIdAsync_NullUserId_ReturnsNull()
        {
            // Arrange
            string userId = null;

            // Act
            var result = await _superAdminService.GetUserByIdAsync(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateUserAsync_UserDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var nonExistentUserId = "nonExistentUserId";
            _userManagerMock.Setup(um => um.FindByIdAsync(nonExistentUserId))
                .ReturnsAsync((IdentityUser)null);

            // Act
            var userToUpdate = new IdentityUser { Id = nonExistentUserId, Email = "updated@example.com" };
            var result = await _superAdminService.UpdateUserAsync(userToUpdate);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateUserAsync_SuccessfulUpdate_ReturnsTrue()
        {
            // Arrange
            var existingUser = new IdentityUser { Id = "1", Email = "user@example.com" };
            _userManagerMock.Setup(um => um.FindByIdAsync(existingUser.Id))
                .ReturnsAsync(existingUser);

            _userManagerMock.Setup(um => um.UpdateAsync(existingUser))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var userToUpdate = new IdentityUser { Id = "1", Email = "updated@example.com" };
            var result = await _superAdminService.UpdateUserAsync(userToUpdate);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Constructor_WithUserManager_InitializesUserManagerField()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(),
                null, null, null, null, null, null, null, null
            );

            // Act
            var superAdminService = new SuperAdminService(userManagerMock.Object);

            // Assert
            var userManagerField = typeof(SuperAdminService).GetField("_userManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(userManagerField);
            var userManagerValue = userManagerField.GetValue(superAdminService);
            Assert.Same(userManagerMock.Object, userManagerValue);
        }

        [Fact]
        public async Task GetUsersAsync_ReturnsListOfUsersInRoleUser()
        {
            // Arrange
            var usersInRoleUser = new List<IdentityUser>
            {
                new IdentityUser { Id = "1", Email = "user1@example.com" },
                new IdentityUser { Id = "2", Email = "user2@example.com" },
            };

            _userManagerMock.Setup(um => um.GetUsersInRoleAsync("User"))
                .ReturnsAsync(usersInRoleUser);

            // Act
            var result = await _superAdminService.GetUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<IdentityUser>>(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void DeleteAdminUser_DeletesAdminUser_ReturnsSuccessResult()
        {
            // Arrange
            var userId = "1";
            var adminUser = new IdentityUser { Id = userId, Email = "admin@example.com" };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(adminUser);

            _userManagerMock.Setup(um => um.DeleteAsync(adminUser))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = _superAdminService.DeleteAdminUser(userId);

            // Assert
            _userManagerMock.Verify(um => um.FindByIdAsync(userId), Times.Once);
            _userManagerMock.Verify(um => um.DeleteAsync(adminUser), Times.Once);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
        }

        [Fact]
        public void UpdateAdminUser_UpdatesAdminUser_ReturnsSuccessResult()
        {
            // Arrange
            var userId = "1";
            var adminUser = new IdentityUser { Id = userId, Email = "admin@example.com" };
            var editModel = new EditAdminViewModel { Id = userId, Email = "updated@example.com" };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(adminUser);

            _userManagerMock.Setup(um => um.UpdateAsync(adminUser))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = _superAdminService.UpdateAdminUser(editModel);

            // Assert
            _userManagerMock.Verify(um => um.FindByIdAsync(userId), Times.Once);
            _userManagerMock.Verify(um => um.UpdateAsync(adminUser), Times.Once);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal("updated@example.com", adminUser.Email);
        }

        [Fact]
        public void GetAdminUser_UserExists_ReturnsEditAdminViewModel()
        {
            // Arrange
            var userId = "1";
            var adminUser = new IdentityUser { Id = userId, Email = "admin@example.com" };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(adminUser);

            // Act
            var result = _superAdminService.GetAdminUser(userId);

            // Assert
            _userManagerMock.Verify(um => um.FindByIdAsync(userId), Times.Once);

            Assert.NotNull(result);
            Assert.IsType<EditAdminViewModel>(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal(adminUser.Email, result.Email);
        }

        [Fact]
        public void GetAdminUser_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var userId = "nonExistentUserId";

            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync((IdentityUser)null);

            // Act
            var result = _superAdminService.GetAdminUser(userId);

            // Assert
            _userManagerMock.Verify(um => um.FindByIdAsync(userId), Times.Once);

            Assert.Null(result);
        }

    }
}