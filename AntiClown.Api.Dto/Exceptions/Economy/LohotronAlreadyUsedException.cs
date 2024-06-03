using Xdd.HttpHelpers.Models.Exceptions;

namespace AntiClown.Api.Dto.Exceptions.Economy;

public class LohotronAlreadyUsedException : ConflictException
{
    public LohotronAlreadyUsedException(Guid userId) : base($"User {userId} already used lohotron")
    {
        UserId = userId;
    }

    public Guid UserId { get; }
}