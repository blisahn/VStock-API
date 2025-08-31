using VBorsa_API.Application.Repositories;
using VBorsa_API.Application.Repositories.Symbol;
using VBorsa_API.Persistence.Context;

namespace VBorsa_API.Persistence.Repositories.Symbol;

public class SymbolReadRepository : ReadRepository<Core.Entities.Symbol>, ISymbolReadRepository
{
    public SymbolReadRepository(VBorsaDbContext context) : base(context)
    {
    }
}