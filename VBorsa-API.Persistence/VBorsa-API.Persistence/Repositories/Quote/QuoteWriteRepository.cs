using VBorsa_API.Application.Repositories.Quote;
using VBorsa_API.Persistence.Context;

namespace VBorsa_API.Persistence.Repositories.Quote;

public class QuoteWriteRepository : WriteRepository<Core.Entities.Quote>, IQuoteWriteRepository
{
    public QuoteWriteRepository(VBorsaDbContext context) : base(context)
    {
    }
}