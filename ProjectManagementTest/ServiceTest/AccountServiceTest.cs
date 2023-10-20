using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using ProductManagement.Models.ViewModel;
using ProductManagement.Data.Repositories.Account;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ProjectManagementTest.ServiceTest
{
    public class AccountServiceTest
    {
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<SignInManager<IdentityUser>> _signInManagerMock;
        private readonly AccountService _accountService;

        public AccountServiceTest()
        {
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(),
                It.IsAny<IOptions<IdentityOptions>>(),
                It.IsAny<IPasswordHasher<IdentityUser>>(),
                It.IsAny<IEnumerable<IUserValidator<IdentityUser>>>(),
                It.IsAny<IEnumerable<IPasswordValidator<IdentityUser>>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                It.IsAny<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<IdentityUser>>>()
            );

            _signInManagerMock = new Mock<SignInManager<IdentityUser>>(
                _userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<IdentityUser>>(),
                null,
                null,
                null,
                null
            );

            _accountService = new AccountService(_signInManagerMock.Object, _userManagerMock.Object);
        }

        [Fact]
        public async Task LoginAsync_InvalidUserName_ReturnsInvalidUsernameStatus()
        {
            // Arrange
            var loginModel = new LoginViewModel
            {
                Email = "nonexistentuser",
                Password = "password123",
                RememberMe = false
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(loginModel.Email))
                           .ReturnsAsync((IdentityUser?)null);

            // Act
            var result = await _accountService.LoginAsync(loginModel.Email, loginModel);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Empty(result.Roles);
        }

        [Fact]
        public async Task LoginAsync_SuccessfulLogin_ReturnsLoggedInStatus()
        {
            // Arrange
            var user = new IdentityUser
            {
                UserName = "existinguser",
                Email = "existinguser@example.com"
            };

            var loginModel = new LoginViewModel
            {
                Email = user.Email,
                Password = "validpassword",
                RememberMe = false
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(loginModel.Email))
                       .ReturnsAsync(user);

            _userManagerMock.Setup(um => um.CheckPasswordAsync(user, loginModel.Password))
                       .ReturnsAsync(true);

            _signInManagerMock.Setup(s => s.PasswordSignInAsync(user.Email, loginModel.Password, false, false))
             .ReturnsAsync(SignInResult.Success);

            var userRoles = new List<string> { "role1", "role2" };
            _userManagerMock.Setup(um => um.GetRolesAsync(user))
                       .ReturnsAsync(userRoles);

            // Act
            var result = await _accountService.LoginAsync(loginModel.Email, loginModel);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(userRoles, result.Roles);
        }

        [Fact]
        public async Task RegisterAsync_UserCreationFailed_ReturnsUserCreationFailedStatus()
        {
            // Arrange
            var registrationModel = new RegisterViewModel
            {
                Email = "newuser@example.com",
                Password = "validpassword",
                ConfirmPassword = "validpassword"
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(registrationModel.Email))
                           .ReturnsAsync((IdentityUser?)null);

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), registrationModel.Password))
                           .ReturnsAsync(IdentityResult.Failed()); // Simulating user creation failure

            // Act
            var result = await _accountService.RegisterAsync(registrationModel);

            // Assert
            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task GetRolesAsync_ReturnsUserRoles()
        {
            // Arrange
            var user = new IdentityUser { UserName = "testuser@example.com", Email = "testuser@example.com" };
            var roles = new List<string> { "Role1", "Role2" };

            _userManagerMock.Setup(um => um.GetRolesAsync(user))
                .ReturnsAsync(roles);

            // Act
            var resultRoles = await _accountService.GetRolesAsync(user);

            // Assert
            Assert.NotNull(resultRoles);
            Assert.Equal(roles, resultRoles);
        }

        [Fact]
        public async Task LoginAsync_UserNotFound_ReturnsUserNotFoundStatus()
        {
            // Arrange
            var loginModel = new LoginViewModel
            {
                Email = "nonexistentuser",
                Password = "password123",
                RememberMe = false
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(loginModel.Email))
                           .ReturnsAsync((IdentityUser?)null);

            // Act
            var result = await _accountService.LoginAsync(loginModel.Email, loginModel);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Empty(result.Roles);
        }

        [Fact]
        public async Task RegisterAsync_UserCreationSucceeded_ReturnsUserCreatedStatus()
        {
            // Arrange
            var registrationModel = new RegisterViewModel
            {
                Email = "newuser@example.com",
                Password = "validpassword",
                ConfirmPassword = "validpassword"
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(registrationModel.Email))
                           .ReturnsAsync((IdentityUser?)null);

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), registrationModel.Password))
                           .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _accountService.RegisterAsync(registrationModel);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task SignOutAsync_SignsOutUser()
        {
            // Act
            await _accountService.SignOutAsync();

            // Assert
            _signInManagerMock.Verify(sm => sm.SignOutAsync(), Times.Once);
        }

        [Fact]
        public async Task GetUserByEmailAsync_UserExists_ReturnsUser()
        {
            // Arrange
            var userEmail = "testuser@example.com";
            var expectedUser = new IdentityUser { UserName = userEmail, Email = userEmail };

            _userManagerMock.Setup(um => um.FindByEmailAsync(userEmail))
                           .ReturnsAsync(expectedUser);

            // Act
            var resultUser = await _accountService.GetUserByEmailAsync(userEmail);

            // Assert
            Assert.NotNull(resultUser);
            Assert.Equal(expectedUser, resultUser);
        }

        [Fact]
        public async Task GetUserByEmailAsync_UserNotFound_ReturnsNull()
        {
            // Arrange
            var userEmail = "nonexistentuser@example.com";

            _userManagerMock.Setup(um => um.FindByEmailAsync(userEmail))
                           .ReturnsAsync((IdentityUser?)null);

            // Act
            var resultUser = await _accountService.GetUserByEmailAsync(userEmail);

            // Assert
            Assert.Null(resultUser);
        }
    }
}
