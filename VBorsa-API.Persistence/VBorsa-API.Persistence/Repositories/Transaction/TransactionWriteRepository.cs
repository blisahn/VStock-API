using VBorsa_API.Application.Repositories.Transaction;
using VBorsa_API.Persistence.Context;

namespace VBorsa_API.Persistence.Repositories.Transaction;

public class TransactionWriteRepository : WriteRepository<Core.Entities.Transaction>, ITransactionWriteRepository
{
    public TransactionWriteRepository(VBorsaDbContext context) : base(context)
    {
    }
}