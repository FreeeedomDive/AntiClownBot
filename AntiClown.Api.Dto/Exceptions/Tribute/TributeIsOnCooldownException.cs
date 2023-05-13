using AntiClown.Api.Dto.Exceptions.Base;

namespace AntiClown.Api.Dto.Exceptions.Tribute;

public class TributeIsOnCooldownException : AntiClownApiConflictException
{
    public TributeIsOnCooldownException(Guid userId)
        : base($"Tribute for user {userId} is still on cooldown")
    {
        UserId = userId;
    }
    
    public Guid UserId { get; set; }
}