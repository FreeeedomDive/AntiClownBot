using AntiClownApiClient.Dto.Requests;
using AntiClownApiClient.Dto.Responses.UserCommandResponses;
using AntiClownApiClient.Extensions;
using RestSharp;

namespace AntiClownApiClient.UsersClient;

public class UsersClient : IUsersClient
{
    public UsersClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<TributeResponseDto> TributeAsync(ulong userId)
    {
        var request = new RestRequest($"api/users/{userId}/tribute");
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<TributeResponseDto>();
    }

    public async Task<WhenNextTributeResponseDto> WhenNextTributeAsync(ulong userId)
    {
        var request = new RestRequest($"api/users/{userId}/tribute/when");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<WhenNextTributeResponseDto>();
    }

    public async Task<RatingResponseDto> RatingAsync(ulong userId)
    {
        var request = new RestRequest($"api/users/{userId}/rating");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<RatingResponseDto>();
    }

    public async Task<ChangeUserBalanceResponseDto> ChangeUserRatingAsync(ulong userId, int ratingDiff, string reason)
    {
        var request = new RestRequest($"api/users/changeBalance");
        request.AddJsonBody(new ChangeUserRatingRequestDto
        {
            UserId = userId,
            RatingDiff = ratingDiff,
            Reason = reason
        });
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<ChangeUserBalanceResponseDto>();
    }

    public async Task BulkChangeUserBalanceAsync(List<ulong> users, int ratingDiff, string reason)
    {
        var request = new RestRequest($"api/users/bulkChangeBalance");
        request.AddJsonBody(new BulkChangeUserBalanceRequestDto
        {
            Users = users,
            RatingDiff = ratingDiff,
            Reason = reason
        });
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task<AllUsersResponseDto> GetAllUsersAsync()
    {
        var request = new RestRequest($"api/users/");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<AllUsersResponseDto>();
    }

    public async Task RemoveCooldownsAsync()
    {
        var request = new RestRequest($"api/users/removeCooldowns");
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task<ulong> GetRichestUserAsync()
    {
        var request = new RestRequest($"api/users/mostRich");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<ulong>();
    }

    public async Task<ItemResponseDto> GetItemByIdAsync(ulong userId, Guid itemId)
    {
        var request = new RestRequest($"api/users/{userId}/items/{itemId}");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<ItemResponseDto>();
    }

    public async Task DailyResetAsync()
    {
        var request = new RestRequest($"api/users/dailyReset");
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestClient restClient;
}