﻿using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Extensions;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Results;

public class F1PredictionResultsRepository : IF1PredictionResultsRepository
{
    public F1PredictionResultsRepository(ISqlRepository<F1PredictionResultStorageElement> sqlRepository)
    {
        this.sqlRepository = sqlRepository;
    }

    public async Task CreateOrUpdateAsync(F1PredictionResult[] results)
    {
        var f1PredictionResultStorageElements = results.Select(ToStorageElement).ToArray();
        foreach (var storageElement in f1PredictionResultStorageElements)
        {
            var existing = (await InnerFindAsync(
                new F1PredictionResultsFilter
                {
                    RaceId = storageElement.RaceId,
                    UserId = storageElement.UserId,
                }
            )).FirstOrDefault();
            await (
                existing is not null
                    ? sqlRepository.UpdateAsync(
                        existing.Id,
                        x =>
                        {
                            x.TenthPlacePoints = storageElement.TenthPlacePoints;
                            x.DnfPoints = storageElement.DnfPoints;
                            x.SafetyCarsPoints = storageElement.SafetyCarsPoints;
                            x.FirstPlaceLeadPoints = storageElement.FirstPlaceLeadPoints;
                            x.TeamMatesPoints = storageElement.TeamMatesPoints;
                            x.TotalPoints = storageElement.TotalPoints;
                        }
                    )
                    : sqlRepository.CreateAsync(storageElement)
            );
        }
    }

    public async Task<F1PredictionResult[]> FindAsync(F1PredictionResultsFilter filter)
    {
        var result = await InnerFindAsync(filter);
        return result.Select(ToModel).ToArray();
    }

    private async Task<F1PredictionResultStorageElement[]> InnerFindAsync(F1PredictionResultsFilter filter)
    {
        return await sqlRepository
                     .BuildCustomQuery()
                     .WhereIf(filter.UserId is not null, x => x.UserId == filter.UserId!.Value)
                     .WhereIf(filter.RaceId is not null, x => x.RaceId == filter.RaceId!.Value)
                     .ToArrayAsync();
    }

    private static F1PredictionResultStorageElement ToStorageElement(F1PredictionResult predictionResult)
    {
        return new F1PredictionResultStorageElement
        {
            Id = Guid.NewGuid(),
            UserId = predictionResult.UserId,
            RaceId = predictionResult.RaceId,
            TenthPlacePoints = predictionResult.TenthPlacePoints,
            DnfPoints = predictionResult.DnfsPoints,
            SafetyCarsPoints = predictionResult.SafetyCarsPoints,
            FirstPlaceLeadPoints = predictionResult.FirstPlaceLeadPoints,
            TeamMatesPoints = predictionResult.TeamMatesPoints,
            TotalPoints = predictionResult.TotalPoints,
        };
    }

    private static F1PredictionResult ToModel(F1PredictionResultStorageElement storageElement)
    {
        return new F1PredictionResult
        {
            UserId = storageElement.UserId,
            RaceId = storageElement.RaceId,
            TenthPlacePoints = storageElement.TenthPlacePoints,
            DnfsPoints = storageElement.DnfPoints,
            SafetyCarsPoints = storageElement.SafetyCarsPoints,
            FirstPlaceLeadPoints = storageElement.FirstPlaceLeadPoints,
            TeamMatesPoints = storageElement.TeamMatesPoints,
            TotalPoints = storageElement.TotalPoints,
        };
    }

    private readonly ISqlRepository<F1PredictionResultStorageElement> sqlRepository;
}