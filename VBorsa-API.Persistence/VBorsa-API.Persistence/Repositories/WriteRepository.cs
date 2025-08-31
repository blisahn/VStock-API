using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using VBorsa_API.Application.Repositories;
using VBorsa_API.Core.Entities.Common;
using VBorsa_API.Persistence.Context;

namespace VBorsa_API.Persistence.Repositories;

public class WriteRepository<T> : IWriteRepository<T> where T : BaseEntity
{
    private readonly VBorsaDbContext _context;

    public WriteRepository(VBorsaDbContext context)
    {
        _context = context;
    }

    public DbSet<T> Table => _context.Set<T>();


    public async Task<bool> AddRangeAsync(List<T> models)
    {
        await _context.AddRangeAsync(models);
        return true;
    }

    public async Task<bool> AddAsync(T model)
    {
        await _context.AddAsync(model);
        return true;
    }

    public bool Remove(T model)
    {
        _context.Remove(model);
        return true;
    }

    public bool RemoveRange(List<T> models)
    {
        _context.RemoveRange(models);
        return true;
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public bool UpdateAsync(T model)
    {
        EntityEntry entry = Table.Update(model);
        return EntityState.Modified == entry.State;
    }
}