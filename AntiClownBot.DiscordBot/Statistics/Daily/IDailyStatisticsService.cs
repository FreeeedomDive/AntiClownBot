using AntiClownDiscordBotVersion2.Models.DailyStatistics;

namespace AntiClownDiscordBotVersion2.Statistics.Daily;

public interface IDailyStatisticsService
{
    void ClearDayStatistics();
    void ChangeUserCredits(ulong id, int diff);
    void Save();
    DailyStatistics DailyStatistics { get; set; }
}