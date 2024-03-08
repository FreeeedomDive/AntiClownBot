using AntiClown.DiscordApi.Client.Members;
using RestSharp;

namespace AntiClown.DiscordApi.Client;

public class AntiClownDiscordApiClient : IAntiClownDiscordApiClient
{
    public AntiClownDiscordApiClient(RestClient restClient)
    {
        MembersClient = new MembersClient(restClient);
    }

    public IMembersClient MembersClient { get; }
}