using SqlRepositoryBase.Core.Models;

namespace AntiClown.Api.Core.Shops.Repositories.Shops;

public class ShopStorageElement : VersionedSqlStorageElement
{
    public int ReRollPrice { get; set; }
    public int FreeReveals { get; set; }
}