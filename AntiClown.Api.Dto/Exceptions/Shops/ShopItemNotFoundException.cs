using AntiClown.Core.Dto.Exceptions;
using Xdd.HttpHelpers.Models.Exceptions;

namespace AntiClown.Api.Dto.Exceptions.Shops;

public class ShopItemNotFoundException : NotFoundException
{
    public ShopItemNotFoundException(Guid shopId, Guid itemId)
        : base($"Item {itemId} not found in shop {shopId}")
    {
        ShopId = shopId;
        ItemId = itemId;
    }
    
    public Guid ShopId { get; set; }
    public Guid ItemId { get; set; }
}