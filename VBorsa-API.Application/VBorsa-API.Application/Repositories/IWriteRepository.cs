using VBorsa_API.Core.Entities.Common;

namespace VBorsa_API.Application.Repositories;

public interface IWriteRepository<T> : IRepository<T> where T : BaseEntity
{
    Task<bool> AddAsync(T model);
    Task<bool> AddRangeAsync(List<T> models);
    bool Remove(T model);
    bool RemoveRange(List<T> models);
    bool UpdateAsync(T model);
    Task<int> SaveAsync();
}