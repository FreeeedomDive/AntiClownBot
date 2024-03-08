using AntiClown.Core.Dto.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using RestSharp;

namespace AntiClown.Data.Api.Client.Settings;

public class SettingsClient : ISettingsClient
{
    public SettingsClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<SettingDto[]> ReadAllAsync()
    {
        var request = new RestRequest("settings");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<SettingDto[]>();
    }

    public async Task<SettingDto> ReadAsync(string category, string key)
    {
        var request = new RestRequest($"settings/categories/{category}/keys/{key}");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<SettingDto>();
    }

    public async Task<SettingDto[]> FindAsync(string category)
    {
        var request = new RestRequest($"settings/categories/{category}");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<SettingDto[]>();
    }

    public async Task CreateOrUpdateAsync(string category, string key, string value)
    {
        var request = new RestRequest("settings");
        request.AddJsonBody(
            new SettingDto
            {
                Category = category,
                Name = key,
                Value = value,
            }
        );
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestClient restClient;
}