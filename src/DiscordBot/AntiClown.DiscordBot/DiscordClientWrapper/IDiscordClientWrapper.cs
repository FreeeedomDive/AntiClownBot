using AntiClown.DiscordBot.DiscordClientWrapper.Channels;
using AntiClown.DiscordBot.DiscordClientWrapper.Emotes;
using AntiClown.DiscordBot.DiscordClientWrapper.Guilds;
using AntiClown.DiscordBot.DiscordClientWrapper.Members;
using AntiClown.DiscordBot.DiscordClientWrapper.Messages;
using AntiClown.DiscordBot.DiscordClientWrapper.Roles;

namespace AntiClown.DiscordBot.DiscordClientWrapper;

public interface IDiscordClientWrapper
{
    Task StartDiscordAsync();

    IEmotesClient Emotes { get; }
    IGuildsClient Guilds { get; }
    IMembersClient Members { get; }
    IMessagesClient Messages { get; }
    IRolesClient Roles { get; }
    IChannelsClient Channels { get; }
}