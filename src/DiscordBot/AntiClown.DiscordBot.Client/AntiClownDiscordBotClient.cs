/* Generated file */

using AntiClown.DiscordBot.Client.DiscordMembers;

namespace AntiClown.DiscordBot.Client;

public class AntiClownDiscordBotClient : IAntiClownDiscordBotClient
{
    public AntiClownDiscordBotClient(RestSharp.RestClient client)
    {
        DiscordMembers = new DiscordMembersClient(client);
    }

    public IDiscordMembersClient DiscordMembers { get; }
}
