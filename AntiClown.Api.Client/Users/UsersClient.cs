/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Api.Client.Users;

public class UsersClient : IUsersClient
{
    public UsersClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Api.Dto.Users.UserDto[]> ReadAllAsync()
    {
        var requestBuilder = new RequestBuilder($"api/users/", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Api.Dto.Users.UserDto[]>(requestBuilder.Build());
    }

    public async Task<AntiClown.Api.Dto.Users.UserDto> ReadAsync(System.Guid userId)
    {
        var requestBuilder = new RequestBuilder($"api/users/{userId}", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Api.Dto.Users.UserDto>(requestBuilder.Build());
    }

    public async Task<AntiClown.Api.Dto.Users.UserDto[]> FindAsync(AntiClown.Api.Dto.Users.UserFilterDto filter)
    {
        var requestBuilder = new RequestBuilder($"api/users/find", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(filter);
        return await client.MakeRequestAsync<AntiClown.Api.Dto.Users.UserDto[]>(requestBuilder.Build());
    }

    public async Task<AntiClown.Api.Dto.Users.FindByIntegrationResultDto> FindByIntegrationAsync(AntiClown.Api.Dto.Users.UserIntegrationFilterDto filter)
    {
        var requestBuilder = new RequestBuilder($"api/users/findByIntegration", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(filter);
        return await client.MakeRequestAsync<AntiClown.Api.Dto.Users.FindByIntegrationResultDto>(requestBuilder.Build());
    }

    public async Task<System.Guid> CreateAsync(AntiClown.Api.Dto.Users.NewUserDto newUser)
    {
        var requestBuilder = new RequestBuilder($"api/users/", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(newUser);
        return await client.MakeRequestAsync<System.Guid>(requestBuilder.Build());
    }

    public async Task BindTelegramAsync(System.Guid userId, long telegramId)
    {
        var requestBuilder = new RequestBuilder($"api/users/{userId}/bindTelegram", HttpRequestMethod.PATCH);
        requestBuilder.WithQueryParameter("telegramId", telegramId);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
