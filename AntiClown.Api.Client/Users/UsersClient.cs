/* Generated file */
using RestSharp;
using AntiClown.Api.Client.Extensions;

namespace AntiClown.Api.Client.Users;

public class UsersClient : IUsersClient
{
    public UsersClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<AntiClown.Api.Dto.Users.UserDto[]> ReadAllAsync()
    {
        var request = new RestRequest("api/users/", Method.Get);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Api.Dto.Users.UserDto[]>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Api.Dto.Users.UserDto> ReadAsync(System.Guid userId)
    {
        var request = new RestRequest("api/users/{userId}", Method.Get);
        request.AddUrlSegment("userId", userId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Api.Dto.Users.UserDto>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Api.Dto.Users.UserDto[]> FindAsync(AntiClown.Api.Dto.Users.UserFilterDto filter)
    {
        var request = new RestRequest("api/users/find", Method.Post);
        request.AddJsonBody(filter);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Api.Dto.Users.UserDto[]>();
    }

    public async System.Threading.Tasks.Task<System.Guid> CreateAsync(AntiClown.Api.Dto.Users.NewUserDto newUser)
    {
        var request = new RestRequest("api/users/", Method.Post);
        request.AddJsonBody(newUser);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<System.Guid>();
    }

    private readonly RestSharp.RestClient restClient;
}
