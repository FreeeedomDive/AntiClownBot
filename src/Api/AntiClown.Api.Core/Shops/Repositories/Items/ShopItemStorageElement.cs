using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Api.Core.Shops.Repositories.Items;

[Index(nameof(ShopId))]
public class ShopItemStorageElement : SqlStorageElement
{
    public Guid ShopId { get; set; }
    public string Name { get; set; }
    public string Rarity { get; set; }
    public int Price { get; set; }
    public bool IsRevealed { get; set; }
    public bool IsOwned { get; set; }
}