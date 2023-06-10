using AntiClown.Core.Dto.Exceptions;

namespace AntiClown.Api.Dto.Exceptions.Items;

public class LootBoxNotFoundException : AntiClownNotFoundException
{
    public LootBoxNotFoundException(Guid userId)
        : base($"User {userId} has no lootboxes")
    {
        UserId = userId;
    }

    public Guid UserId { get; set; }
}