using DSharpPlus.Entities;
using DSharpPlus.Net.Models;

namespace AntiClown.DiscordBot.DiscordClientWrapper.Channels;

public interface IChannelsClient
{
    Task<DiscordChannel> FindDiscordChannel(ulong channelId);
    Task<DiscordChannel[]> GetGuildChannels();
    Task ModifyChannelAsync(ulong channelId, Action<ChannelEditModel> action);
}