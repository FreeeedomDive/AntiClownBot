﻿using AntiClown.Api.Core.Shops.Domain;
using AutoMapper;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Api.Core.Shops.Repositories.Shops;

public class ShopsRepository : IShopsRepository
{
    public ShopsRepository(
        IVersionedSqlRepository<ShopStorageElement> sqlRepository,
        IMapper mapper
    )
    {
        this.sqlRepository = sqlRepository;
        this.mapper = mapper;
    }

    public async Task<Shop> ReadAsync(Guid id)
    {
        var result = await sqlRepository.ReadAsync(id);
        return mapper.Map<Shop>(result);
    }

    public async Task CreateAsync(Shop shop)
    {
        var storageElement = mapper.Map<ShopStorageElement>(shop);
        await sqlRepository.CreateAsync(storageElement);
    }

    public async Task UpdateAsync(Shop shop)
    {
        await sqlRepository.ConcurrentUpdateAsync(shop.Id, x =>
        {
            x.ReRollPrice = shop.ReRollPrice;
            x.FreeReveals = shop.FreeReveals;
        });
    }

    private readonly IVersionedSqlRepository<ShopStorageElement> sqlRepository;
    private readonly IMapper mapper;
}