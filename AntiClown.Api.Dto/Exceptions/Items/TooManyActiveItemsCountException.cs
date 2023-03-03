using AntiClown.Api.Dto.Exceptions.Base;

namespace AntiClown.Api.Dto.Exceptions.Items;

public class TooManyActiveItemsCountException : AntiClownApiConflictException
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