using VBorsa_API.Application.Repositories.Holding;
using VBorsa_API.Persistence.Context;

namespace VBorsa_API.Persistence.Repositories.Holding;

public class HoldingWriteRepository : WriteRepository<Core.Entities.Holding>, IHoldingWriteRepository
{
    public HoldingWriteRepository(VBorsaDbContext context) : base(context)
    {
    }
}