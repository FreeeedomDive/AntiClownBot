/* Generated file */

using AntiClown.DiscordBot.Client.DiscordMembers;

namespace AntiClown.DiscordBot.Client;

public class AntiClownDiscordBotClient : IAntiClownDiscordBotClient
{
    public AntiClownDiscordBotClient(RestSharp.RestClient restClient)
    {
        DiscordMembers = new DiscordMembersClient(restClient);
    }

    public IDiscordMembersClient DiscordMembers { get; }
}
