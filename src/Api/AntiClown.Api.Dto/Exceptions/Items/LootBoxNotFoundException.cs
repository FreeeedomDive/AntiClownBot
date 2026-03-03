using AntiClown.Core.Dto.Exceptions;
using Xdd.HttpHelpers.Models.Exceptions;

namespace AntiClown.Api.Dto.Exceptions.Items;

public class LootBoxNotFoundException : NotFoundException
{
    public LootBoxNotFoundException(Guid userId)
        : base($"User {userId} has no lootboxes")
    {
        UserId = userId;
    }

    public Guid UserId { get; set; }
}