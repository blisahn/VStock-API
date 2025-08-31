using VBorsa_API.Application.Repositories.Symbol;
using VBorsa_API.Persistence.Context;

namespace VBorsa_API.Persistence.Repositories.Symbol;

public class SymbolWriteRepository : WriteRepository<Core.Entities.Symbol>, ISymbolWriteRepository
{
    public SymbolWriteRepository(VBorsaDbContext context) : base(context)
    {
    }
}