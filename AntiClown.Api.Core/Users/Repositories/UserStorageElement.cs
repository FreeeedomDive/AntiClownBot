using System.ComponentModel.DataAnnotations.Schema;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Api.Core.Users.Repositories;

public class UserStorageElement : SqlStorageElement
{
    [Column] public ulong DiscordId { get; set; }
    [Column] public long? TelegramId { get; set; }
}