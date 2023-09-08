using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagement.Controllers;
using ProductManagement.Data.Repositories.Account;
using ProductManagement.Data.Repositories.ProductCrud;
using ProductManagement.Models.DomainModel;
using ProductManagement.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementTest
{
    public class UserControllerTest
    {


        [Fact]
        public void ViewProducts_ReturnsViewWithProducts()
        {
            // Arrange
            var productRepositoryMock = new Mock<IProductService>();
            var controller = new UserController(productRepositoryMock.Object);
            var products = new List<ProductModel>
            {
                new ProductModel { Id = Guid.NewGuid(), Name = "Product 1", Price = 10 },
                new ProductModel { Id = Guid.NewGuid(), Name = "Product 2", Price = 20 }
            };
            productRepositoryMock.Setup(repo => repo.AllProducts()).Returns(products);

            // Act
            var result = controller.ViewProducts() as ViewResult;
            var model = result.ViewData.Model as List<ProductModel>;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Equal(products.Count, model.Count); // Assert the number of products
            productRepositoryMock.Verify(repo => repo.AllProducts(), Times.Once);
        }

        [Fact]
        public void UserDashboard_ReturnsViewWithViewModel()
        {

            // Arrange
            var mockAccountService = new Mock<IProductService>();
            var controller = new UserController(mockAccountService.Object);


            // Act
            var result = controller.UserDashboard() as ViewResult;
            var model = result.ViewData.Model as UserDashboardViewModel;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Equal("UserDashboard", result.ViewName);
        }
    }
}
