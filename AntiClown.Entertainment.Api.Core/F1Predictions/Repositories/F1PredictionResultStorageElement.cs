﻿using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories;

[Index(nameof(RaceId))]
[Index(nameof(UserId))]
public class F1PredictionResultStorageElement : SqlStorageElement
{
    public Guid RaceId { get; set; }
    public Guid UserId { get; set; }
    public int TenthPlacePoints { get; set; }
    public int FirstDnfPoints { get; set; }
}