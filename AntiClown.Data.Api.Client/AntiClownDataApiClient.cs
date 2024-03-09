using AntiClown.Data.Api.Client.Rights;
using AntiClown.Data.Api.Client.Settings;
using AntiClown.Data.Api.Client.Tokens;
using RestSharp;

namespace AntiClown.Data.Api.Client;

public class AntiClownDataApiClient : IAntiClownDataApiClient
{
    public AntiClownDataApiClient(RestClient restClient)
    {
        Settings = new SettingsClient(restClient);
        Tokens = new TokensClient(restClient);
        Rights = new RightsClient(restClient);
    }
    
    public ISettingsClient Settings { get; }
    public ITokensClient Tokens { get; }
    public IRightsClient Rights { get; }
}