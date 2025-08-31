using VBorsa_API.Application.Repositories.Holding;
using VBorsa_API.Persistence.Context;

namespace VBorsa_API.Persistence.Repositories.Holding;

public class HoldingReadRepository : ReadRepository<Core.Entities.Holding>, IHoldingReadRepository
{
    public HoldingReadRepository(VBorsaDbContext context) : base(context)
    {
    }
}