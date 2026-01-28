namespace DemoApi.Domain.Interfaces;

using DemoApi.Domain.Entities;

public interface IProductRepository
{
    Task<IList<Product>> GetAll();

    Task<Product?> GetById(uint id);

    Task<Product?> GetByName(string name);

    Task<Product> Create(Product product);

    Task<bool> Update(Product product);

    Task<bool> DeleteById(uint id);
}