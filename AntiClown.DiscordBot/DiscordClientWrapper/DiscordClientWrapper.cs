using AntiClown.DiscordBot.DiscordClientWrapper.Channels;
using AntiClown.DiscordBot.DiscordClientWrapper.Emotes;
using AntiClown.DiscordBot.DiscordClientWrapper.Guilds;
using AntiClown.DiscordBot.DiscordClientWrapper.Members;
using AntiClown.DiscordBot.DiscordClientWrapper.Messages;
using AntiClown.DiscordBot.DiscordClientWrapper.Roles;
using AntiClown.DiscordBot.Options;
using DSharpPlus;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.DiscordClientWrapper;

public class DiscordClientWrapper : IDiscordClientWrapper
{
    public DiscordClientWrapper(
        DiscordClient discordClient,
        IOptions<DiscordOptions> discordOptions
    )
    {
        this.discordClient = discordClient;
        Emotes = new EmotesClient(discordClient);
        Guilds = new GuildsClient(discordClient, discordOptions);
        Members = new MembersClient(discordClient, discordOptions);
        Messages = new MessagesClient(discordClient, discordOptions);
        Roles = new RolesClient(Guilds, Members);
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