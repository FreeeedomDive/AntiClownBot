using AntiClownDiscordBotVersion2.DiscordClientWrapper.Channels;
using AntiClownDiscordBotVersion2.DiscordClientWrapper.Emotes;
using AntiClownDiscordBotVersion2.DiscordClientWrapper.Guilds;
using AntiClownDiscordBotVersion2.DiscordClientWrapper.Members;
using AntiClownDiscordBotVersion2.DiscordClientWrapper.Messages;
using AntiClownDiscordBotVersion2.DiscordClientWrapper.Roles;
using AntiClownDiscordBotVersion2.DiscordClientWrapper.Voice;

namespace AntiClownDiscordBotVersion2.DiscordClientWrapper;

public interface IDiscordClientWrapper
{
    Task StartDiscord();

    IEmotesClient Emotes { get; }
    IGuildsClient Guilds { get; }
    IMembersClient Members { get; }
    IMessagesClient Messages { get; }
    IRolesClient Roles { get; }
    IVoiceClient Voice { get; }
    IChannelsClient Channels { get; }
}