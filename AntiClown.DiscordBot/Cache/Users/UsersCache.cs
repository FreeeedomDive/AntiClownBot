using System.Collections.Concurrent;
using AntiClown.Api.Client;
using AntiClown.Api.Dto.Users;
using AntiClown.DiscordBot.DiscordClientWrapper;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.Cache.Users;

public class UsersCache : IUsersCache
{
    public UsersCache(
        IAntiClownApiClient antiClownApiClient,
        IDiscordClientWrapper discordClientWrapper,
        ILogger<UsersCache> logger
    )
    {
        this.antiClownApiClient = antiClownApiClient;
        this.discordClientWrapper = discordClientWrapper;
        this.logger = logger;
        discordMemberToApiId = new ConcurrentDictionary<ulong, Guid>();
        apiIdToDiscordMember = new ConcurrentDictionary<Guid, DiscordMember>();
    }

    public async Task InitializeAsync()
    {
        try
        {
            var guildMembers = (await discordClientWrapper.Members.GetAllAsync()).ToDictionary(x => x.Id);
            var apiUsers = await antiClownApiClient.Users.ReadAllAsync();
            foreach (var apiUser in apiUsers)
            {
                if (!guildMembers.TryGetValue(apiUser.DiscordId, out var member))
                {
                    continue;
                }

                AddUser(member, apiUser.Id);
            }

            isInitialized = true;
            logger.LogInformation(
                "Users cache initialized:\n"
                + "ApiIdToDiscordMember elements count: {ApiIdToDiscordMemberCount}\n"
                + "DiscordMemberToApiId elements count: {DiscordMemberToApiIdCount}",
                apiIdToDiscordMember.Count,
                discordMemberToApiId.Count
            );
        }
        catch (Exception e)
        {
            logger.LogError(e, "{UsersCache} initialization error", nameof(UsersCache));
        }
    }

    public async Task AddOrUpdate(DiscordMember member)
    {
        var apiId = await GetApiIdByMemberIdAsync(member.Id);
        apiIdToDiscordMember[apiId] = member;
    }

    public async Task<DiscordMember?> GetMemberByApiIdAsync(Guid userId)
    {
        if (!isInitialized)
        {
            await InitializeAsync();
        }

        apiIdToDiscordMember.TryGetValue(userId, out var member);
        logger.LogInformation("Resolve ApiUserId-MemberId binding [{apiId}]-[{memberId}] from cache", userId, member?.Id);
        return member;
    }

    public async Task<Guid> GetApiIdByMemberIdAsync(ulong memberId)
    {
        if (!isInitialized)
        {
            await InitializeAsync();
        }

        if (discordMemberToApiId.TryGetValue(memberId, out var apiId))
        {
            logger.LogInformation("Resolve MemberId-ApiUserId binding [{memberId}]-[{apiId}] from cache", memberId, apiId);
            return apiId;
        }

        var member = await discordClientWrapper.Members.GetAsync(memberId);
        var users = await antiClownApiClient.Users.FindAsync(
            new UserFilterDto
            {
                DiscordId = memberId,
            }
        );

        var user = users.SingleOrDefault();
        if (user is not null)
        {
            logger.LogInformation("Add new MemberId-ApiUserId binding [{memberId}]-[{apiId}] (get user from API)", memberId, user.Id);
            AddUser(member, user.Id);
            return user.Id;
        }

        var newUserId = await antiClownApiClient.Users.CreateAsync(
            new NewUserDto
            {
                DiscordId = memberId,
            }
        );
        AddUser(member, newUserId);
        logger.LogInformation("Add new MemberId-ApiUserId binding [{memberId}]-[{apiId}] (create new user)", memberId, newUserId);

        return newUserId;
    }

    private void AddUser(DiscordMember member, Guid apiUserId)
    {
        apiIdToDiscordMember[apiUserId] = member;
        discordMemberToApiId[member.Id] = apiUserId;
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly ConcurrentDictionary<Guid, DiscordMember> apiIdToDiscordMember;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly ConcurrentDictionary<ulong, Guid> discordMemberToApiId;
    private readonly ILogger<UsersCache> logger;

    private bool isInitialized;
}