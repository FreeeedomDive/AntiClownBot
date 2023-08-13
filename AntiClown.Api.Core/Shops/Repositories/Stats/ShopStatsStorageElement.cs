using SqlRepositoryBase.Core.Models;

namespace AntiClown.Api.Core.Shops.Repositories.Stats;

public class ShopStatsStorageElement : VersionedSqlStorageElement
{
    public int TotalReRolls { get; set; }
    public int ItemsBought { get; set; }
    public int TotalReveals { get; set; }
    public int ScamCoinsLostOnReveals { get; set; }
    public int ScamCoinsLostOnReRolls { get; set; }
    public int ScamCoinsLostOnPurchases { get; set; }
}