using DSharpPlus.Net.Models;

namespace AntiClownDiscordBotVersion2.DiscordClientWrapper.Channels;

public interface IChannelsClient
{
    Task ModifyChannelAsync(ulong channelId, Action<ChannelEditModel> action);
}