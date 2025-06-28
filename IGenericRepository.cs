using Microsoft.EntityFrameworkCore;

namespace Small_Library;

public interface IGenericRepository<T> where T : class
{
    Task Update(T entity);
    Task Delete(T entity);
    Task<List<T>> GetAll();
    Task Add(T entity);
    Task<T> GetById(object id);
}