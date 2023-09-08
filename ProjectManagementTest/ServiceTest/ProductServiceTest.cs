using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductManagement.Data;
using ProductManagement.Data.Repositories.ProductCrud;
using ProductManagement.Models.DomainModel;
using Xunit;

namespace ProjectManagementTest.ServiceTest
{
    public class ProductServiceTest
    {
        [Fact]
        public void GetAllProducts_ReturnsAllProducts()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName: "Assignment2"))
                .BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var productService = new ProductService(dbContext);

                // Add some test products to the in-memory database
                productService.AddProduct(new ProductModel { Name = "Product 1", Price = (int?)10.99 });
                productService.AddProduct(new ProductModel { Name = "Product 2", Price = (int?)20.99 });

                // Act
                var products = productService.GetAllProducts();

                // Assert
                Assert.Equal(2, products.Count);
            }
        }

        [Fact]
        public void GetProductById_ReturnsNullForNonexistentProduct()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName: "InMemoryDatabase"))
                .BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var productService = new ProductService(dbContext);

                // Act
                var product = productService.GetProductById(Guid.NewGuid());

                // Assert
                Assert.Null(product);
            }
        }

     
        [Fact]
        public void DeleteProduct_RemovesProductFromDatabase()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName: "InMemoryDatabase"))
                .BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var productService = new ProductService(dbContext);

                // Add a test product to the in-memory database
                var productId = Guid.NewGuid();
                productService.AddProduct(new ProductModel { Id = productId, Name = "Product 1", Price = (int?)10.99 });

                // Act
                productService.DeleteProduct(productId);

                // Assert
                var deletedProduct = productService.GetProductById(productId);
                Assert.Null(deletedProduct);
            }
        }

    }
}
