using AntiClown.Api.Core.Transactions.Domain;
using AntiClown.Api.Core.Transactions.Repositories;

namespace AntiClown.Api.Core.Transactions.Services;

public class TransactionsService : ITransactionsService
{
    public TransactionsService(ITransactionsRepository transactionsRepository)
    {
        this.transactionsRepository = transactionsRepository;
    }

    public async Task<Transaction[]> ReadManyAsync(Guid userId, int skip = 0, int take = 50)
    {
        return await transactionsRepository.ReadManyAsync(userId, skip, take);
    }

    public async Task<Transaction[]> FindAsync(TransactionsFilter filter)
    {
        return await transactionsRepository.FindAsync(filter);
    }

    public async Task CreateAsync(Transaction transaction)
    {
        await transactionsRepository.CreateAsync(transaction);
    }

    private readonly ITransactionsRepository transactionsRepository;
}