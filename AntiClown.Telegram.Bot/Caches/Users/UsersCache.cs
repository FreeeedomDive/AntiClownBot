using System.Collections.Concurrent;
using AntiClown.Api.Client;
using AntiClown.Api.Dto.Users;
using AntiClown.DiscordBot.Client;
using AntiClown.DiscordBot.Dto.Members;

namespace AntiClown.Telegram.Bot.Caches.Users;

public class UsersCache : IUsersCache
{
    public UsersCache(
        IAntiClownApiClient antiClownApiClient,
        IAntiClownDiscordBotClient antiClownDiscordBotClient,
        ILogger<UsersCache> logger
    )
    {
        this.antiClownApiClient = antiClownApiClient;
        this.antiClownDiscordBotClient = antiClownDiscordBotClient;
        this.logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var allUsers = await antiClownApiClient.Users.ReadAllAsync();
            users = new ConcurrentDictionary<Guid, UserDto>(allUsers.ToDictionary(x => x.Id));
            usersByTelegramId = new ConcurrentDictionary<long, UserDto>(
                allUsers.Where(x => x.TelegramId is not null).ToDictionary(x => x.TelegramId!.Value)
            );
            logger.LogInformation("Users cache initialized with {count} users, {telegramUsersCount} users has bound telegram", users.Count, usersByTelegramId.Count);
            var members = await antiClownDiscordBotClient.DiscordMembers.GetDiscordMembersAsync(allUsers.Select(x => x.Id).ToArray());
            discordMembers = new ConcurrentDictionary<Guid, DiscordMemberDto?>(
                members.Where(x => x is not null).ToDictionary(x => x!.UserId)
            );
            logger.LogInformation("Members cache initialized with {count} members", discordMembers.Count);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to initialize users cache");
        }
    }

    public async Task BindTelegramAsync(long telegramUserId, Guid userId)
    {
        if (!users.TryGetValue(userId, out var user))
        {
            return;
        }

        user.TelegramId = telegramUserId;
        usersByTelegramId[telegramUserId] = user;
        var discordMember = await antiClownDiscordBotClient.DiscordMembers.GetDiscordMemberAsync(userId);
        discordMembers.TryAdd(userId, discordMember);
    }

    public UserDto? TryGetUser(Guid userId)
    {
        return users.GetValueOrDefault(userId);
    }

    public UserDto? TryGetUser(long telegramUserId)
    {
        return usersByTelegramId.GetValueOrDefault(telegramUserId);
    }

    public DiscordMemberDto? TryGetDiscordMember(Guid userId)
    {
        logger.LogInformation("DiscordMembers count: {count}, trying to get userId {userId}", discordMembers.Count, userId);
        return discordMembers.GetValueOrDefault(userId);
    }

    private ConcurrentDictionary<Guid, UserDto> users;
    private ConcurrentDictionary<long, UserDto> usersByTelegramId;
    private ConcurrentDictionary<Guid, DiscordMemberDto?> discordMembers;

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IAntiClownDiscordBotClient antiClownDiscordBotClient;
    private readonly ILogger<UsersCache> logger;
}