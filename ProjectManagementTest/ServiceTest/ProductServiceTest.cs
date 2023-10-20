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



        [Fact]
        public void AddProduct_AddsProductToDatabase()
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

                var newProduct = new ProductModel { Name = "New Product", Price = (int?)15.99 };

                // Act
                productService.AddProduct(newProduct);

                // Assert
                var addedProduct = productService.GetProductById(newProduct.Id);
                Assert.NotNull(addedProduct);
                Assert.Equal(newProduct.Name, addedProduct.Name);
                Assert.Equal(newProduct.Price, addedProduct.Price);
            }
        }




        [Fact]
        public void GetAllProducts_ReturnsEmptyListForEmptyDatabase()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName: "EmptyDatabase"))
                .BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var productService = new ProductService(dbContext);

                // Act
                var products = productService.GetAllProducts();

                // Assert
                Assert.Empty(products);
            }
        }


        //---------------------------------------------


    
        [Fact]
        public void UpdateProduct_DoesNotUpdateWithInvalidId()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName: "UpdateProductInvalidIdTest"))
                .BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var productService = new ProductService(dbContext);

                // Create an invalid product (one that does not exist in the database)
                var invalidProduct = new ProductModel { Id = Guid.NewGuid(), Name = "Invalid Product", Price = (int?)19.99 };

                // Act
                productService.UpdateProduct(invalidProduct);

                // Assert
                var product = productService.GetProductById(invalidProduct.Id);
                Assert.Null(product);
            }
        }



        [Fact]
        public void DeleteProduct_DoesNotThrowExceptionForNonexistentProduct()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName: "DeleteProductNonexistentTest"))
                .BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var productService = new ProductService(dbContext);

                // Create a non-existent product ID
                var invalidProductId = Guid.NewGuid();

                // Act and Assert
                Assert.Null(productService.GetProductById(invalidProductId));

                // Attempt to delete the non-existent product
                productService.DeleteProduct(invalidProductId);

                // Assert again that the product is still non-existent, and no exception is thrown
                Assert.Null(productService.GetProductById(invalidProductId));
            }
        }



        [Fact]
        public void AddProduct_HandlesLongProductName()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName: "AddProductLongNameTest"))
                .BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var productService = new ProductService(dbContext);

                // Create a product with a very long name
                var longName = new string('X', 1000); // Create a string of 1000 'X' characters
                var product = new ProductModel { Name = longName, Price = (int?)10.99 };

                // Act
                productService.AddProduct(product);

                // Assert
                var addedProduct = productService.GetProductById(product.Id);
                Assert.NotNull(addedProduct);
                Assert.Equal(longName, addedProduct.Name);
            }
        }

        [Fact]
        public void AddProduct_HandlesHighPriceValue()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName: "AddProductHighPriceTest"))
                .BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var productService = new ProductService(dbContext);

                // Create a product with a very high price
                var highPrice = int.MaxValue;
                var product = new ProductModel { Name = "High Price Product", Price = highPrice };

                // Act
                productService.AddProduct(product);

                // Assert
                var addedProduct = productService.GetProductById(product.Id);
                Assert.NotNull(addedProduct);
                Assert.Equal(highPrice, addedProduct.Price);
            }
        }



        [Fact]
        public void AllProducts_ReturnsAllProducts()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName: "AllProductsTest"))
                .BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var productService = new ProductService(dbContext);

                // Add some test products to the in-memory database
                productService.AddProduct(new ProductModel { Name = "Product 1", Price = (int?)10.99 });
                productService.AddProduct(new ProductModel { Name = "Product 2", Price = (int?)20.99 });

                // Act
                var products = productService.AllProducts();

                // Assert
                Assert.Equal(2, products.Count);
                
            }
        }

       

        [Fact]
        public void AllProducts_ReturnsEmptyListForEmptyDatabase()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName: "EmptyDatabase"))
                .BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var productService = new ProductService(dbContext);

                // Act
                var products = productService.AllProducts();

                // Assert
                Assert.Empty(products);
            }
        }







    }
}
