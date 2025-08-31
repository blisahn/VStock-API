using VBorsa_API.Application.Repositories.Transaction;
using VBorsa_API.Persistence.Context;

namespace VBorsa_API.Persistence.Repositories.Transaction;

public class TransactionReadRepository : ReadRepository<Core.Entities.Transaction>, ITransactionReadRepository
{
    public TransactionReadRepository(VBorsaDbContext context) : base(context)
    {
    }
}