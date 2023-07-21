using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories;

[Index(nameof(IsActive))]
public class F1RaceStorageElement : VersionedSqlStorageElement
{
    public bool IsActive { get; set; }
    public bool IsOpened { get; set; }
    public string SerializedPredictions { get; set; }
    public string SerializedResults { get; set; }
}