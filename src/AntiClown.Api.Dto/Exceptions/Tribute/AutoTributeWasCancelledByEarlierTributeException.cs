using Xdd.HttpHelpers.Models.Exceptions;

namespace AntiClown.Api.Dto.Exceptions.Tribute;

public class AutoTributeWasCancelledByEarlierTributeException : ConflictException
{
    public AutoTributeWasCancelledByEarlierTributeException(Guid userId)
        : base($"Auto tribute for user {userId} was cancelled by earlier tribute")
    {
        UserId = userId;
    }
    
    public Guid UserId { get; set; }
}