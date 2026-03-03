/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Data.Api.Client.Settings;

public class SettingsClient : ISettingsClient
{
    public SettingsClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Data.Api.Dto.Settings.SettingDto[]> ReadAllAsync()
    {
        var requestBuilder = new RequestBuilder($"dataApi/settings/", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Data.Api.Dto.Settings.SettingDto[]>(requestBuilder.Build());
    }

    public async Task<AntiClown.Data.Api.Dto.Settings.SettingDto> ReadAsync(string category, string key)
    {
        var requestBuilder = new RequestBuilder($"dataApi/settings/categories/{category}/keys/{key}", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Data.Api.Dto.Settings.SettingDto>(requestBuilder.Build());
    }

    public async Task<AntiClown.Data.Api.Dto.Settings.SettingDto[]> FindAsync(string category)
    {
        var requestBuilder = new RequestBuilder($"dataApi/settings/categories/{category}", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Data.Api.Dto.Settings.SettingDto[]>(requestBuilder.Build());
    }

    public async Task CreateOrUpdateAsync(AntiClown.Data.Api.Dto.Settings.SettingDto settingDto)
    {
        var requestBuilder = new RequestBuilder($"dataApi/settings/", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(settingDto);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
