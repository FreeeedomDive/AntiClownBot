using AntiClown.Data.Api.Client.Settings;
using RestSharp;

namespace AntiClown.Data.Api.Client;

public class AntiClownDataApiClient : IAntiClownDataApiClient
{
    public AntiClownDataApiClient(RestClient restClient)
    {
        Settings = new SettingsClient(restClient);
    }
    
    public ISettingsClient Settings { get; set; }
}