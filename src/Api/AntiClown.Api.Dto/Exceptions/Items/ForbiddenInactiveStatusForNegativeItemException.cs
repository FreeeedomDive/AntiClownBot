using Xdd.HttpHelpers.Models.Exceptions;

namespace AntiClown.Api.Dto.Exceptions.Items;

public class ForbiddenInactiveStatusForNegativeItemException : BadRequestException
{
    public ForbiddenInactiveStatusForNegativeItemException(Guid itemId)
        : base($"Negative item {itemId} can't be inactive")
    {
        ItemId = itemId;
    }
    
    public Guid ItemId { get; set; }
}