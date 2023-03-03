using AntiClown.Api.Dto.Exceptions.Base;

namespace AntiClown.Api.Dto.Exceptions.Items;

public class LootBoxNotFoundException : AntiClownApiNotFoundException
{
    public LootBoxNotFoundException(Guid userId)
        : base($"User {userId} has no lootboxes")
    {
        UserId = userId;
    }

    public Guid UserId { get; set; }
}