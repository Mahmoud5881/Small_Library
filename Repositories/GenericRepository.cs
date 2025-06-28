using Microsoft.EntityFrameworkCore;
using Small_Library;

namespace Small_Library.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly LibraryDbContext context;

    public GenericRepository(LibraryDbContext context)
    {
        this.context = context;
    }

    public async Task Add(T entity)
    {
        await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task Delete(T entity)
    {
        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<List<T>> GetAll()
    {
        return await context.Set<T>().ToListAsync();
    }

    public async Task Update(T entity)
    {
        context.Set<T>().Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task<T> GetById(object id)
    {
        return await context.Set<T>().FindAsync(id);
    }
}