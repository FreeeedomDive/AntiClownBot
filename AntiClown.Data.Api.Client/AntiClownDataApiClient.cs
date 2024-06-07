/* Generated file */

using AntiClown.Data.Api.Client.Rights;
using AntiClown.Data.Api.Client.Settings;
using AntiClown.Data.Api.Client.Tokens;

namespace AntiClown.Data.Api.Client;

public class AntiClownDataApiClient : IAntiClownDataApiClient
{
    public AntiClownDataApiClient(RestSharp.RestClient restClient)
    {
        Rights = new RightsClient(restClient);
        Settings = new SettingsClient(restClient);
        Tokens = new TokensClient(restClient);
    }

    public IRightsClient Rights { get; }
    public ISettingsClient Settings { get; }
    public ITokensClient Tokens { get; }
}
