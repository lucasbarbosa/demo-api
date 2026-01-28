namespace DemoApi.Domain.Interfaces;

using DemoApi.Domain.Entities;

public interface IRepository<TEntity> where TEntity : Entity
{
    Task<IList<TEntity>> GetAll() => Task.FromResult<IList<TEntity>>(Array.Empty<TEntity>());

    Task<TEntity?> Create(TEntity entity) => Task.FromResult<TEntity?>(default);

    Task<bool> Update(TEntity entity) => Task.FromResult(false);
}