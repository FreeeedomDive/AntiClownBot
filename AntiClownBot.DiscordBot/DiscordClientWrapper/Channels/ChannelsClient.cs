using AntiClownDiscordBotVersion2.DiscordClientWrapper.Guilds;
using DSharpPlus;
using DSharpPlus.Net.Models;

namespace AntiClownDiscordBotVersion2.DiscordClientWrapper.Channels;

public class ChannelsClient : IChannelsClient
{
    public ChannelsClient(
        DiscordClient discordClient,
        IGuildsClient guildsClient
    )
    {
        this.discordClient = discordClient;
        this.guildsClient = guildsClient;
    }

    public async Task ModifyChannelAsync(ulong channelId, Action<ChannelEditModel> action)
    {
        var botChannel = await guildsClient.FindDiscordChannel(channelId);
        await botChannel.ModifyAsync(action);
    }
    
    private readonly DiscordClient discordClient;
    private readonly IGuildsClient guildsClient;
}