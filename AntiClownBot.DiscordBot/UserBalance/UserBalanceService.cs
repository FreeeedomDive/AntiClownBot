using AntiClownApiClient;
using AntiClownDiscordBotVersion2.Statistics.Daily;

namespace AntiClownDiscordBotVersion2.UserBalance;

public class UserBalanceService : IUserBalanceService
{
    public UserBalanceService(
        IApiClient apiClient,
        IDailyStatisticsService dailyStatisticsService
    )
    {
        this.apiClient = apiClient;
        this.dailyStatisticsService = dailyStatisticsService;
    }

    public async Task ChangeUserBalanceWithDailyStatsAsync(ulong userId, int diff, string reason)
    {
        await apiClient.Users.ChangeUserRatingAsync(userId, diff, reason);
        dailyStatisticsService.ChangeUserCredits(userId, diff);
    }
    
    private readonly IApiClient apiClient;
    private readonly IDailyStatisticsService dailyStatisticsService;
}