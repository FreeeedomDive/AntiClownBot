using AntiClown.Core.Dto.Exceptions;

namespace AntiClown.Api.Dto.Exceptions.Items;

public class TooManyActiveItemsCountException : AntiClownConflictException
{
    public TooManyActiveItemsCountException(Guid userId, string itemType)
        : base($"User {userId} already has maximum amount of active items of type {itemType}")
    {
        UserId = userId;
        ItemType = itemType;
    }
    
    public Guid UserId { get; set; }
    public string ItemType { get; set; }
}