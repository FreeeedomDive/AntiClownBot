﻿using AntiClown.Api.Core.Transactions.Domain;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

    public async Task CreateAsync(Transaction transaction)
    {
        var storageElement = mapper.Map<TransactionStorageElement>(transaction);
        await sqlRepository.CreateAsync(storageElement);
    }

    private readonly ISqlRepository<TransactionStorageElement> sqlRepository;
    private readonly IMapper mapper;
}