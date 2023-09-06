using ProductManagement.Controllers;
using ProductManagement.Models.DomainModel;
using Moq;
using Microsoft.AspNetCore.Mvc;



 

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

    }
}