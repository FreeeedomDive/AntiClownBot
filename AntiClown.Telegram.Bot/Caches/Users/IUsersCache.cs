using AntiClown.Api.Dto.Users;
using AntiClown.DiscordBot.Dto.Members;

namespace AntiClown.Telegram.Bot.Caches.Users;

public interface IUsersCache
{
    Task InitializeAsync();
    Task BindTelegramAsync(long telegramUserId, Guid userId);
    UserDto? TryGetUser(Guid userId);
    UserDto? TryGetUser(long telegramUserId);
    DiscordMemberDto? TryGetDiscordMember(Guid userId);
}