using AntiClown.Data.Api.Client;
using AntiClown.DiscordBot.DiscordClientWrapper.Channels;
using AntiClown.DiscordBot.DiscordClientWrapper.Emotes;
using AntiClown.DiscordBot.DiscordClientWrapper.Guilds;
using AntiClown.DiscordBot.DiscordClientWrapper.Members;
using AntiClown.DiscordBot.DiscordClientWrapper.Messages;
using AntiClown.DiscordBot.DiscordClientWrapper.Roles;
using DSharpPlus;

namespace AntiClown.DiscordBot.DiscordClientWrapper;

public class DiscordClientWrapper : IDiscordClientWrapper
{
    public DiscordClientWrapper(
        DiscordClient discordClient,
        IAntiClownDataApiClient antiClownDataApiClient
    )
    {
        this.discordClient = discordClient;
        Emotes = new EmotesClient(discordClient);
        Guilds = new GuildsClient(discordClient, antiClownDataApiClient);
        Members = new MembersClient(discordClient, antiClownDataApiClient);
        Messages = new MessagesClient(discordClient, antiClownDataApiClient);
        Roles = new RolesClient(discordClient, Guilds, Members, antiClownDataApiClient);
        Channels = new ChannelsClient(Guilds);
    }

    public async Task StartDiscordAsync()
    {
        await discordClient.ConnectAsync();
    }

    public IEmotesClient Emotes { get; }
    public IGuildsClient Guilds { get; }
    public IMembersClient Members { get; }
    public IMessagesClient Messages { get; }
    public IRolesClient Roles { get; }
    public IChannelsClient Channels { get; }

    private readonly DiscordClient discordClient;
}