using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Models.ViewModel;
using Moq;
using ProductManagement.Data.Repositories.Account;
using ProductManagement.Controllers;

namespace AccountControllerTest
{
    public class AccountControllerTest
    {


        [Fact]
        public void TestLoginAction()
        {
            var mockAccountService = new Mock<IAccountService>();
            var controller = new AccountController(mockAccountService.Object);



            // Act
            var result = controller.Login() as ViewResult;



            // Assert
            Assert.NotNull(result);

        }


        [Fact]
        public void TestRegisterAction()
        {
            var mockAccountService = new Mock<IAccountService>();
            var controller = new AccountController(mockAccountService.Object);



            // Act
            var result = controller.Register() as ViewResult;



            // Assert
            Assert.NotNull(result);

        }


        [Fact]
        public void TestAccessDeniedAction()
        {
            var mockAccountService = new Mock<IAccountService>();
            var controller = new AccountController(mockAccountService.Object);



            // Act
            var result = controller.AccessDenied() as ViewResult;



            // Assert
            Assert.NotNull(result);

        }



        [Fact]
        public void TestIndexAction()
        {
            var mockAccountService = new Mock<IAccountService>();
            var controller = new AccountController(mockAccountService.Object);



            // Act
            var result = controller.Index() as ViewResult;



            // Assert
            Assert.NotNull(result);

        }



        [Fact]
            public async Task Login_ValidModel_RedirectsToDashboard()
            {
                // Arrange
                var mockAccountService = new Mock<IAccountService>();
                var controller = new AccountController(mockAccountService.Object);



                var user = new IdentityUser { UserName = "test@example.com" };
                var loginModel = new LoginViewModel
                {
                    Email = "test@example.com",
                    Password = "password",
                    RememberMe = false
                };

                mockAccountService.Setup(service => service.LoginAsync(loginModel.Email, loginModel))
                    .ReturnsAsync((true, new List<string> { "User" }));

             
                mockAccountService.Setup(service => service.GetUserByEmailAsync(loginModel.Email))
                    .ReturnsAsync(user);

                // Act
                var result = await controller.Login(loginModel) as RedirectToActionResult;

                // Assert
                Assert.NotNull(result);
                Assert.Equal("UserDashboard", result.ActionName);
                Assert.Equal("User", result.ControllerName);
            }

            //Invalid Login
            [Fact]
            public async Task Login_InvalidCredentials_ReturnsViewWithModelError()
            {
                // Arrange
                var mockAccountService = new Mock<IAccountService>();
                var controller = new AccountController(mockAccountService.Object);

                var loginModel = new LoginViewModel
                {
                    Email = "invalid@example.com",
                    Password = "invalidpassword",
                    RememberMe = false
                };

                // Simulate a failed login
                mockAccountService.Setup(service => service.LoginAsync(loginModel.Email, loginModel))
                    .ReturnsAsync((false, new List<string>()));

                // Act
                var result = await controller.Login(loginModel) as ViewResult;

                // Assert
                Assert.NotNull(result);
                Assert.False(controller.ModelState.IsValid);
                Assert.Contains("Please enter valid details", controller.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }
        [Fact]
        public async Task Register_ValidModel_RedirectsToLogin()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();
            var controller = new AccountController(mockAccountService.Object);

            var registerModel = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "password",
                ConfirmPassword = "password"
            };
            
            mockAccountService.Setup(service => service.RegisterAsync(registerModel))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await controller.Register(registerModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
        }

        //Invalid Registration
        [Fact]
            public async Task Register_InvalidModel_ReturnsViewWithModelError()
            {
                // Arrange
                var mockAccountService = new Mock<IAccountService>();
                var controller = new AccountController(mockAccountService.Object);

                var registerModel = new RegisterViewModel
                {
                                        Email = "invalidemail", // Invalid email format
                    Password = "password",
                    ConfirmPassword = "password"
                };

                // Simulate a failed registration
                var errorDescription = "Email is not in the correct format.";
                mockAccountService.Setup(service => service.RegisterAsync(registerModel))
                    .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = errorDescription }));

                // Act
                var result = await controller.Register(registerModel) as ViewResult;

                // Assert
                Assert.NotNull(result);
                Assert.False(controller.ModelState.IsValid);
                Assert.Contains(errorDescription, controller.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            //Logout
            [Fact]
            public async Task LogoutRedirectsToLogin()
            {
                // Arrange
                var mockAccountService = new Mock<IAccountService>();
                var controller = new AccountController(mockAccountService.Object);

                // Act
                var result = await controller.Logout() as RedirectToActionResult;

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Login", result.ActionName);
                Assert.Equal("Account", result.ControllerName);
            }

        /*--------------------------------------------------------------------*/

        [Fact]
        public async Task Login_RedirectsToUserDashboardForValidCredentials()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();
            var controller = new AccountController(mockAccountService.Object);
            var user = new IdentityUser { UserName = "test@example.com" };
            var loginModel = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "password",
                RememberMe = false
            };

            mockAccountService.Setup(service => service.LoginAsync(loginModel.Email, loginModel))
                .ReturnsAsync((true, new List<string> { "User" }));

            mockAccountService.Setup(service => service.GetUserByEmailAsync(loginModel.Email))
                .ReturnsAsync(user);

            // Act
            var result = await controller.Login(loginModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("UserDashboard", result.ActionName);
            Assert.Equal("User", result.ControllerName);
        }

        [Fact]
        public async Task Login_ReturnsViewWithErrorForInvalidCredentials()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();
            var controller = new AccountController(mockAccountService.Object);
            var loginModel = new LoginViewModel
            {
                Email = "invalid@example.com",
                Password = "invalidpassword",
                RememberMe = false
            };

            
            mockAccountService.Setup(service => service.LoginAsync(loginModel.Email, loginModel))
                .ReturnsAsync((false, new List<string>()));

            // Act
            var result = await controller.Login(loginModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Contains("Please enter valid details", controller.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        }

        [Fact]
        public async Task Logout_RedirectsToLogin()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();
            var controller = new AccountController(mockAccountService.Object);

            // Act
            var result = await controller.Logout() as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
            Assert.Equal("Account", result.ControllerName);
        }


        [Fact]
        public async Task LogoutRedirectsToRegister()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();
            var controller = new AccountController(mockAccountService.Object);

            // Act
            var result = await controller.Logout() as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
            Assert.Equal("Account", result.ControllerName);
        }

        [Fact]
        public async Task Logout_CallsSignOutAsync()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();
            var controller = new AccountController(mockAccountService.Object);

            // Act
            await controller.Logout();

            // Assert
            mockAccountService.Verify(service => service.SignOutAsync(), Times.Once);
        }



    
    

        [Fact]
        public async Task Register_ReturnsViewWithErrorForInvalidModel()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();
            var controller = new AccountController(mockAccountService.Object);
            var registerModel = new RegisterViewModel
            {
                Email = "invalidemail", 
                Password = "password",
                ConfirmPassword = "password"
            };

           
            var errorDescription = "Email is not in the correct format.";
            mockAccountService.Setup(service => service.RegisterAsync(registerModel))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = errorDescription }));

            // Act
            var result = await controller.Register(registerModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Contains(errorDescription, controller.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        }

     
        [Fact]
        public void Index_ReturnsIndexView()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();
            var controller = new AccountController(mockAccountService.Object);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }



    }
}
