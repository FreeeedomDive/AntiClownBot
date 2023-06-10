using AntiClown.Core.Dto.Exceptions;

namespace AntiClown.Api.Dto.Exceptions.Items;

public class ForbiddenInactiveStatusForNegativeItemException : AntiClownBadRequestException
{
    public ForbiddenInactiveStatusForNegativeItemException(Guid itemId)
        : base($"Negative item {itemId} can't be inactive")
    {
        ItemId = itemId;
    }
    
    public Guid ItemId { get; set; }
}