using ProductManagement.Models.DomainModel;
using System;
using System.Collections.Generic;

public interface IProductService
{
    List<ProductModel> GetAllProducts();
    ProductModel GetProductById(Guid id);
    void AddProduct(ProductModel product);
    void UpdateProduct(ProductModel product);
    void DeleteProduct(Guid id);

    List<ProductModel> AllProducts();
}
