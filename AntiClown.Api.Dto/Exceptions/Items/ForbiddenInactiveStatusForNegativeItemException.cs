using AntiClown.Api.Dto.Exceptions.Base;

namespace AntiClown.Api.Dto.Exceptions.Items;

public class ForbiddenInactiveStatusForNegativeItemException : AntiClownApiBadRequestException
{
    public ForbiddenInactiveStatusForNegativeItemException(Guid itemId)
        : base($"Negative item {itemId} can't be inactive")
    {
        ItemId = itemId;
    }
    
    public Guid ItemId { get; set; }
}