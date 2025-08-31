using Microsoft.EntityFrameworkCore;
using VBorsa_API.Core.Entities.Common;

namespace VBorsa_API.Application.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    public DbSet<T> Table { get; }
}