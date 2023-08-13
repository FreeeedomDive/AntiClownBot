using AntiClown.DiscordBot.DiscordClientWrapper.Guilds;
using DSharpPlus.Entities;
using DSharpPlus.Net.Models;

namespace AntiClown.DiscordBot.DiscordClientWrapper.Channels;

public class ChannelsClient : IChannelsClient
{
    public ChannelsClient(IGuildsClient guildsClient)
    {
        this.guildsClient = guildsClient;
    }

    public async Task<DiscordChannel> FindDiscordChannel(ulong channelId)
    {
        var guild = await guildsClient.GetGuildAsync();
        if (!guild.Channels.ContainsKey(channelId))
        {
            throw new ArgumentException($"Channel {channelId} doesn't exist");
        }
        return guild.Channels[channelId];
    }

    public async Task<DiscordChannel[]> GetGuildChannels()
    {
        var guild = await guildsClient.GetGuildAsync();

        return guild.Channels.Values.ToArray();
    }

    public async Task ModifyChannelAsync(ulong channelId, Action<ChannelEditModel> action)
    {
        var botChannel = await FindDiscordChannel(channelId);
        await botChannel.ModifyAsync(action);
    }

    private readonly IGuildsClient guildsClient;
}