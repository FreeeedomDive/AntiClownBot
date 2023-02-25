using AntiClown.Api.Core.Transactions.Domain;

namespace AntiClown.Api.Core.Transactions.Services;

public interface ITransactionsService
{
    Task<Transaction[]> ReadManyAsync(Guid userId, int skip = 0, int take = 50);
    Task CreateAsync(Transaction transaction);
}