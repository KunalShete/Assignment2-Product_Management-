using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ProductManagement.Data.Repositories.UserCrud;
using ProductManagement.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementTest.ServiceTest
{
    public class UserServiceTest
    {
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly UserService _userService;

        public UserServiceTest()
        {
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(),
                null, null, null, null, null, null, null, null
            );

            _userService = new UserService(_userManagerMock.Object);

        }


        private Mock<UserManager<IdentityUser>> CreateUserManagerMock()
        {
            var userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(),
                null, null, null, null, null, null, null, null
            );

            userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<IdentityUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            return userManagerMock;
        }


        [Fact]
        public void GetAdminUsers_ReturnsAdminUserViewModels()
        {
            // Arrange
            var users = new List<IdentityUser>
            {
                new IdentityUser { Id = "1", UserName = "user1", Email = "user1@example.com" },
                new IdentityUser { Id = "2", UserName = "user2", Email = "user2@example.com" }
            };

            _userManagerMock.Setup(u => u.GetUsersInRoleAsync("User"))
                           .ReturnsAsync(users);

            // Act
            var result = _userService.GetAdminUsers();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(users.Count, result.Count);

            for (var i = 0; i < users.Count; i++)
            {
                Assert.Equal(users[i].Id, result[i].Id);
                Assert.Equal(users[i].UserName, result[i].Name);
                Assert.Equal(users[i].Email, result[i].Email);
            }
        }

     
        [Fact]
        public void GetUserById_ExistingUser_ReturnsAddUserViewModel()
        {
            // Arrange
            var userId = "1";
            var userEmail = "user1@example.com";
            var identityUser = new IdentityUser { Id = userId, Email = userEmail };

            _userManagerMock.Setup(u => u.FindByIdAsync(userId))
                           .ReturnsAsync(identityUser);

            // Act
            var result = _userService.GetUserById(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal(userEmail, result.Email);
        }

        [Fact]
        public void GetUserById_NonExistingUser_ReturnsNull()
        {
            // Arrange
            var userId = "nonexistentid";

            _userManagerMock.Setup(u => u.FindByIdAsync(userId))
                           .ReturnsAsync((IdentityUser)null);

            // Act
            var result = _userService.GetUserById(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DeleteUser_ExistingUser_DeletesUser()
        {
            // Arrange
            var userId = "1";
            var identityUser = new IdentityUser { Id = userId };

            _userManagerMock.Setup(u => u.FindByIdAsync(userId))
                           .ReturnsAsync(identityUser);

            // Act
            _userService.DeleteUser(userId);

            // Assert
            _userManagerMock.Verify(u => u.DeleteAsync(identityUser), Times.Once);
        }

        [Fact]
        public void DeleteUser_NonExistingUser_DoesNotDeleteUser()
        {
            // Arrange
            var userId = "nonexistentid";

            _userManagerMock.Setup(u => u.FindByIdAsync(userId))
                           .ReturnsAsync((IdentityUser)null);

            // Act
            _userService.DeleteUser(userId);

            // Assert
            _userManagerMock.Verify(u => u.DeleteAsync(It.IsAny<IdentityUser>()), Times.Never);
        }


        [Fact]
        public void GetAdminUsers_NoAdminUsers_ReturnsEmptyList()
        {
            // Arrange
            var users = new List<IdentityUser>();

            _userManagerMock.Setup(u => u.GetUsersInRoleAsync("User"))
                           .ReturnsAsync(users);

            // Act
            var result = _userService.GetAdminUsers();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
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
            var userService = new UserService(userManagerMock.Object);

            // Assert
            var userManagerField = typeof(UserService).GetField("_userManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(userManagerField);
            var userManagerValue = userManagerField.GetValue(userService);
            Assert.Same(userManagerMock.Object, userManagerValue);
        }

        [Fact]
        public void AddUser_SuccessfulUserCreation_AddsUserToRole()
        {
            // Arrange
            var user = new AddUserViewModel
            {
                Email = "testuser@example.com",
                Password = "password123"
            };

            var userManagerMock = CreateUserManagerMock();
            var userService = new UserService(userManagerMock.Object);

            userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<IdentityUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            userService.AddUser(user);

            // Assert
            userManagerMock.Verify(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<IdentityUser>(), "User"), Times.Once);
        }

        [Fact]
        public void AddUser_FailedUserCreation_DoesNotAddUserToRole()
        {
            // Arrange
            var user = new AddUserViewModel
            {
                Email = "testuser@example.com",
                Password = "password123"
            };

            var userManagerMock = CreateUserManagerMock();
            var userService = new UserService(userManagerMock.Object);

            userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Creation failed" }));

            // Act
            userService.AddUser(user);

            // Assert
            userManagerMock.Verify(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<IdentityUser>(), "User"), Times.Never);
        }




        [Fact]
        public void AddUser_UserCreationFailure_DoesNotAddToRole()
        {
            // Arrange
            var user = new AddUserViewModel
            {
                Email = "testuser@example.com",
                Password = "invalid" // Invalid password that will cause user creation to fail
            };

            var userManagerMock = CreateUserManagerMock();
            var userService = new UserService(userManagerMock.Object);

            userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Creation failed" }));

            // Act
            userService.AddUser(user);

            // Assert
            userManagerMock.Verify(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<IdentityUser>(), "User"), Times.Never);
        }


        [Fact]
        public void EditUser_NonExistingUser_DoesNotEditUser()
        {
            // Arrange
            var nonExistingUserId = "nonexistentid";
            var newEmail = "newemail@example.com";

            var userManagerMock = CreateUserManagerMock();
            var userService = new UserService(userManagerMock.Object);

            userManagerMock.Setup(u => u.Users).Returns(new List<IdentityUser>().AsQueryable());

            var userToUpdate = new AddUserViewModel
            {
                Id = nonExistingUserId,
                Email = newEmail
            };

            // Act
            userService.EditUser(userToUpdate);

            // Assert
            userManagerMock.Verify(um => um.UpdateAsync(It.IsAny<IdentityUser>()), Times.Never);
        }


        [Fact]
        public void EditUser_ExistingUser_EditsUserEmail()
        {
            // Arrange
            var userId = "1";
            var userEmail = "user1@example.com";
            var newEmail = "newemail@example.com";
            var identityUser = new IdentityUser { Id = userId, Email = userEmail };

            var userManagerMock = CreateUserManagerMock();
            var userService = new UserService(userManagerMock.Object);

            userManagerMock.Setup(u => u.Users).Returns(new List<IdentityUser> { identityUser }.AsQueryable());

            var userToUpdate = new AddUserViewModel
            {
                Id = userId,
                Email = newEmail
            };

            // Act
            userService.EditUser(userToUpdate);

            // Assert
            Assert.Equal(newEmail, identityUser.Email);
            userManagerMock.Verify(um => um.UpdateAsync(identityUser), Times.Once);
        }






      

    }
}
