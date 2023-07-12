using AntiClown.Core.Dto.Exceptions;

namespace AntiClown.Api.Dto.Exceptions.Economy;

public class LohotronAlreadyUsedException : AntiClownConflictException
{
    public LohotronAlreadyUsedException(Guid userId) : base($"User {userId} already used lohotron")
    {
        UserId = userId;
    }

    public Guid UserId { get; }
}