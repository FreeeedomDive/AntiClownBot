using SqlRepositoryBase.Core.Models;

namespace AntiClown.Api.Core.Economies.Repositories;

public class EconomyStorageElement : VersionedSqlStorageElement
{
    public int ScamCoins { get; set; }
    public DateTime NextTribute { get; set; }
    public int LootBoxes { get; set; }
    public bool IsLohotronReady { get; set; }
}