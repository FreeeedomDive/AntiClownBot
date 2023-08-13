using AntiClown.Api.Core.Transactions.Domain;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Extensions;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Api.Core.Transactions.Repositories;

public class TransactionsRepository : ITransactionsRepository
{
    public TransactionsRepository(
        ISqlRepository<TransactionStorageElement> sqlRepository,
        IMapper mapper
    )
    {
        this.sqlRepository = sqlRepository;
        this.mapper = mapper;
    }

    public async Task<Transaction[]> ReadManyAsync(Guid userId, int skip = 0, int take = 50)
    {
        var result = await sqlRepository
                           .BuildCustomQuery()
                           .Where(x => x.UserId == userId)
                           .OrderByDescending(x => x.DateTime)
                           .Skip(skip)
                           .Take(take)
                           .ToArrayAsync();

        return mapper.Map<Transaction[]>(result);
    }

    public async Task<Transaction[]> FindAsync(TransactionsFilter filter)
    {
        var result = await sqlRepository
                           .BuildCustomQuery()
                           .WhereIf(filter.UserId.HasValue, x => x.UserId == filter.UserId!.Value)
                           .WhereIf(filter.DateTimeRange?.From is not null, x => filter.DateTimeRange!.From!.Value <= x.DateTime)
                           .WhereIf(filter.DateTimeRange?.To is not null, x => x.DateTime <= filter.DateTimeRange!.To!.Value)
                           .OrderByDescending(x => x.DateTime)
                           .ToArrayAsync();

        return mapper.Map<Transaction[]>(result);
    }

    public async Task CreateAsync(Transaction transaction)
    {
        var storageElement = mapper.Map<TransactionStorageElement>(transaction);
        await sqlRepository.CreateAsync(storageElement);
    }

    public async Task CreateAsync(Transaction[] transactions)
    {
        var storageElements = mapper.Map<TransactionStorageElement[]>(transactions);
        await sqlRepository.CreateAsync(storageElements);
    }

    private readonly IMapper mapper;

    private readonly ISqlRepository<TransactionStorageElement> sqlRepository;
}