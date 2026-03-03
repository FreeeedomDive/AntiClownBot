using AntiClown.Api.Dto.Achievements;
using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Api.Client.Achievements;

public class AchievementsClient : IAchievementsClient
{
    public AchievementsClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AchievementDto[]> ReadAllAsync()
    {
        var requestBuilder = new RequestBuilder("api/achievements/", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AchievementDto[]>(requestBuilder.Build());
    }

    public async Task<AchievementDto> ReadAsync(Guid achievementId)
    {
        var requestBuilder = new RequestBuilder($"api/achievements/{achievementId}", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AchievementDto>(requestBuilder.Build());
    }

    public async Task<Guid> CreateAsync(NewAchievementDto newAchievement)
    {
        var requestBuilder = new RequestBuilder("api/achievements/", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(newAchievement);
        return await client.MakeRequestAsync<Guid>(requestBuilder.Build());
    }

    public async Task DeleteAsync(Guid achievementId)
    {
        var requestBuilder = new RequestBuilder($"api/achievements/{achievementId}", HttpRequestMethod.DELETE);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task<UserAchievementDto[]> GetUsersByAchievementAsync(Guid achievementId)
    {
        var requestBuilder = new RequestBuilder($"api/achievements/{achievementId}/users", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<UserAchievementDto[]>(requestBuilder.Build());
    }

    public async Task GrantAsync(Guid achievementId, GrantAchievementDto grant)
    {
        var requestBuilder = new RequestBuilder($"api/achievements/{achievementId}/grant", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(grant);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task<UserAchievementDto[]> GetAchievementsByUserAsync(Guid userId)
    {
        var requestBuilder = new RequestBuilder($"api/achievements/users/{userId}", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<UserAchievementDto[]>(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
