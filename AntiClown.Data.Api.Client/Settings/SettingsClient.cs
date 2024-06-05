/* Generated file */
using RestSharp;
using Xdd.HttpHelpers.Models.Extensions;

namespace AntiClown.Data.Api.Client.Settings;

public class SettingsClient : ISettingsClient
{
    public SettingsClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<AntiClown.Data.Api.Dto.Settings.SettingDto[]> ReadAllAsync()
    {
        var request = new RestRequest("dataApi/settings/", Method.Get);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Data.Api.Dto.Settings.SettingDto[]>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Data.Api.Dto.Settings.SettingDto> ReadAsync(System.String category, System.String key)
    {
        var request = new RestRequest("dataApi/settings/categories/{category}/keys/{key}", Method.Get);
        request.AddUrlSegment("category", category);
        request.AddUrlSegment("key", key);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Data.Api.Dto.Settings.SettingDto>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Data.Api.Dto.Settings.SettingDto[]> FindAsync(System.String category)
    {
        var request = new RestRequest("dataApi/settings/categories/{category}", Method.Get);
        request.AddUrlSegment("category", category);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Data.Api.Dto.Settings.SettingDto[]>();
    }

    public async System.Threading.Tasks.Task CreateOrUpdateAsync(AntiClown.Data.Api.Dto.Settings.SettingDto settingDto)
    {
        var request = new RestRequest("dataApi/settings/", Method.Post);
        request.AddJsonBody(settingDto);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestSharp.RestClient restClient;
}
