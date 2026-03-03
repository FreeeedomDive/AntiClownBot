using Xdd.HttpHelpers.Models.Exceptions;

namespace AntiClown.Api.Dto.Exceptions.Tribute;

public class TributeIsOnCooldownException : ConflictException
{
    public TributeIsOnCooldownException(Guid userId)
        : base($"Tribute for user {userId} is still on cooldown")
    {
        UserId = userId;
    }
    
    public Guid UserId { get; set; }
}