﻿using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Extensions;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories;

public class F1PredictionResultsRepository : IF1PredictionResultsRepository
{
    public F1PredictionResultsRepository(ISqlRepository<F1PredictionResultStorageElement> sqlRepository)
    {
        this.sqlRepository = sqlRepository;
    }

    public async Task CreateAsync(F1PredictionResult[] results)
    {
        var f1PredictionResultStorageElements = results.Select(ToStorageElement).ToArray();
        await sqlRepository.CreateAsync(f1PredictionResultStorageElements);
    }

    public async Task<F1PredictionResult[]> FindAsync(F1PredictionResultsFilter filter)
    {
        var result = await sqlRepository
                           .BuildCustomQuery()
                           .WhereIf(filter.UserId is not null, x => x.UserId == filter.UserId!.Value)
                           .WhereIf(filter.RaceId is not null, x => x.RaceId == filter.RaceId!.Value)
                           .ToArrayAsync();
        return result.Select(ToModel).ToArray();
    }

    private static F1PredictionResultStorageElement ToStorageElement(F1PredictionResult predictionResult)
    {
        return new F1PredictionResultStorageElement
        {
            Id = Guid.NewGuid(),
            UserId = predictionResult.UserId,
            RaceId = predictionResult.RaceId,
            FirstDnfPoints = predictionResult.FirstDnfPoints,
            TenthPlacePoints = predictionResult.TenthPlacePoints,
        };
    }

    private static F1PredictionResult ToModel(F1PredictionResultStorageElement storageElement)
    {
        return new F1PredictionResult
        {
            UserId = storageElement.UserId,
            RaceId = storageElement.RaceId,
            FirstDnfPoints = storageElement.FirstDnfPoints,
            TenthPlacePoints = storageElement.TenthPlacePoints,
        };
    }

    private readonly ISqlRepository<F1PredictionResultStorageElement> sqlRepository;
}