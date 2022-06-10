using AntiClownDiscordBotVersion2.Models.DailyStatistics;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using Newtonsoft.Json;

namespace AntiClownDiscordBotVersion2.Statistics.Daily;

public class DailyStatisticsService : IDailyStatisticsService
{
    public DailyStatisticsService()
    {
        DailyStatistics = TryRead(out var stats) ? stats : new DailyStatistics();
    }

    public void ClearDayStatistics()
    {
        DailyStatistics = new DailyStatistics();
    }

    public void ChangeUserCredits(ulong id, int count)
    {
        DailyStatistics.CreditsById.AddRecord(id, count);
        DailyStatistics.CreditsCollected += count;
        Save();
    }

    public void Save()
    {
        var json = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(FileName, json);
    }

    private static bool TryRead(out DailyStatistics dict)
    {
        dict = null;
        if (!File.Exists(FileName))
        {
            return false;
        }
        dict = JsonConvert.DeserializeObject<DailyStatistics>(File.ReadAllText(FileName));
        return dict != null;
    }
    
    public DateTime TodayDate { get; set; }
    public DailyStatistics DailyStatistics { get; set; }

    private const string FileName = "StatisticsFiles/daily.json";
}