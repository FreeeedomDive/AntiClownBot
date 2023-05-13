using AntiClown.Api.Dto.Exceptions.Base;

namespace AntiClown.Api.Dto.Exceptions.Tribute;

public class AutoTributeWasCancelledByEarlierTributeException : AntiClownApiConflictException
{
    public AutoTributeWasCancelledByEarlierTributeException(Guid userId)
        : base($"Auto tribute for user {userId} was cancelled by earlier tribute")
    {
        UserId = userId;
    }
    
    public Guid UserId { get; set; }
}