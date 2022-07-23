namespace AntiClownDiscordBotVersion2.UserBalance;

public interface IUserBalanceService
{
    Task ChangeUserBalanceWithDailyStatsAsync(ulong userId, int diff, string reason);
    void ChangeDailyStats(ulong userId, int diff);
}