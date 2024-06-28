/* Generated file */

using AntiClown.Data.Api.Client.Rights;
using AntiClown.Data.Api.Client.Settings;
using AntiClown.Data.Api.Client.Tokens;

namespace AntiClown.Data.Api.Client;

public class AntiClownDataApiClient : IAntiClownDataApiClient
{
    public AntiClownDataApiClient(RestSharp.RestClient client)
    {
        Rights = new RightsClient(client);
        Settings = new SettingsClient(client);
        Tokens = new TokensClient(client);
    }

    public IRightsClient Rights { get; }
    public ISettingsClient Settings { get; }
    public ITokensClient Tokens { get; }
}
