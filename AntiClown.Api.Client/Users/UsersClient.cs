using AntiClown.Api.Dto.Users;
using AntiClown.Core.Dto.Extensions;
using RestSharp;

namespace AntiClown.Api.Client.Users;

public class UsersClient : IUsersClient
{
    public UsersClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<UserDto[]> ReadAllAsync()
    {
        var request = new RestRequest(BuildApiUrl());
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<UserDto[]>();
    }

    public async Task<UserDto> ReadAsync(Guid userId)
    {
        var request = new RestRequest($"{BuildApiUrl()}/{userId}");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<UserDto>();
    }

    public async Task<UserDto[]> FindAsync(UserFilterDto filter)
    {
        var request = new RestRequest($"{BuildApiUrl()}/find");
        request.AddJsonBody(filter);
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<UserDto[]>();
    }

    public async Task<Guid> CreateAsync(NewUserDto newUserDto)
    {
        var request = new RestRequest(BuildApiUrl());
        request.AddJsonBody(newUserDto);
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<Guid>();
    }

    private static string BuildApiUrl() => "users";

    private readonly RestClient restClient;
}