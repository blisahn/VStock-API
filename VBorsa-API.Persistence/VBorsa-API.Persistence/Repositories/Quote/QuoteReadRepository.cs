using VBorsa_API.Application.Repositories.Quote;
using VBorsa_API.Persistence.Context;

namespace VBorsa_API.Persistence.Repositories.Quote;

public class QuoteReadRepository : ReadRepository<Core.Entities.Quote>, IQuoteReadRepository
{
    public QuoteReadRepository(VBorsaDbContext context) : base(context)
    {
    }
}