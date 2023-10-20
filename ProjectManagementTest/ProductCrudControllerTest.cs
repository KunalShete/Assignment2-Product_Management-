using ProductManagement.Controllers;
using ProductManagement.Models.DomainModel;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Data.Repositories.ProductCrud;
using ProductManagement.Data.Repositories.Account;

namespace ProductCrudControllerTest
{
    public class ProductCrudControllerTest
    {
        [Fact]
        public void TestGetAllProducts()
        {
            var productServiceMock = new Mock<IProductService>();
            var ProductCrudController = new ProductCrudController(productServiceMock.Object);

 

            var products = new List<ProductModel>
            {
                new ProductModel
                {
                    Id = Guid.NewGuid(),
                    Name = "iPhone",
                    Price = 10000
                },
                new ProductModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Vivo",
                    Price = 20000
                }
            };

 

            productServiceMock.Setup(service => service.GetAllProducts())
                .Returns(products);

 

            // Act
            var result = ProductCrudController.AllProducts() as ViewResult;

 

            // Assert
            Assert.NotNull(result);
            //Assert.Equal(products, result.Model);
            Assert.Null(result.ViewName);
        }

 

        [Fact]
        public void TestAddProduct()
        {
            var productServiceMock = new Mock<IProductService>();
            var productController = new ProductCrudController(productServiceMock.Object);

 

            var productToAdd = new ProductModel
            {
                Id = Guid.NewGuid(),
                Name = "iPhone",
                Price = 10
            };

 

            productServiceMock.Setup(service => service.AddProduct(It.IsAny<ProductModel>()))
                .Verifiable();

 

            // Act
            var result = productController.AddProduct(productToAdd) as RedirectToActionResult;

 

            // Assert
            Assert.NotNull(result);
            Assert.Equal("AllProducts", result.ActionName);

 

            productServiceMock.Verify(service => service.AddProduct(productToAdd), Times.Once);
        }

 

        [Fact]
        public void TestUpdateProduct()
        {
            var productServiceMock = new Mock<IProductService>();
            var productController = new ProductCrudController(productServiceMock.Object);

 

            var productId = Guid.NewGuid();
            var productToEdit = new ProductModel
            {
                Id = productId,
                Name = "iPhone",
                Price = 15000
            };

 

            productServiceMock.Setup(service => service.GetProductById(productId))
                .Returns(productToEdit);

 

            // Act
            var result = productController.UpdateProduct(productId) as ViewResult;

 

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<ProductModel>(result.Model);
            Assert.Equal(productToEdit, model);
        }

 

        [Fact]
        public void TestDeleteProduct()
        {
            var productServiceMock = new Mock<IProductService>();
            var productController = new ProductCrudController(productServiceMock.Object);
            var productId = Guid.NewGuid();
            productServiceMock.Setup(service => service.DeleteProduct(productId))
                .Verifiable();

              // Act
            var result = productController.DeleteProduct(productId) as RedirectToActionResult;

             // Assert
            //Assert.NotNull(result);
            Assert.Equal("AllProducts", result.ActionName);

            productServiceMock.Verify(service => service.DeleteProduct(productId), Times.Once);
        }


        [Fact]
        public void TestAddProductAction()
        {
            var mockAccountService = new Mock<IProductService>();
            var controller = new ProductCrudController(mockAccountService.Object);



            // Act
            var result = controller.AddProduct() as ViewResult;



            // Assert
            Assert.NotNull(result);

        }


        [Fact]
        public void EditProduct_ValidModelState_RedirectsToAllProducts()
        {
            // Arrange
            var productServiceMock = new Mock<IProductService>();
            var controller = new ProductCrudController(productServiceMock.Object);
            var validProductModel = new ProductModel { Id = Guid.NewGuid(), Name = "Updated Product", Price = 12 };

            // Act
            var result = controller.EditProduct(validProductModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("AllProducts", result.ActionName);
            productServiceMock.Verify(s => s.UpdateProduct(validProductModel), Times.Once);
        }

        [Fact]
        public void EditProductInvalidModelStateReturnsView()
        {
            // Arrange
            var productServiceMock = new Mock<IProductService>();
            var controller = new ProductCrudController(productServiceMock.Object);
            controller.ModelState.AddModelError("Name", "Name is required");
            var invalidProductModel = new ProductModel { Id = Guid.NewGuid(), Name = null, Price = 12 };

            // Act
            var result = controller.EditProduct(invalidProductModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(invalidProductModel, result.Model);
            productServiceMock.Verify(s => s.UpdateProduct(It.IsAny<ProductModel>()), Times.Never);
        }





       



    }
}