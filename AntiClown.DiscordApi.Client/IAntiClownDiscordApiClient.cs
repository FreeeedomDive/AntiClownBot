using AntiClown.DiscordApi.Client.Members;

namespace AntiClown.DiscordApi.Client;

public interface IAntiClownDiscordApiClient
{
    IMembersClient MembersClient { get; }
}