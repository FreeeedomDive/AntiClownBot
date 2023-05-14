using AntiClown.Tools.Utility.Extensions;
using AntiClownDiscordBotVersion2.Models.DailyStatistics;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using Newtonsoft.Json;

namespace AntiClownDiscordBotVersion2.Statistics.Daily;

public class DailyStatisticsService : IDailyStatisticsService
{
    public DailyStatisticsService()
    {
        var filesDirectory = Environment.GetEnvironmentVariable("AntiClownBotFilesDirectory") ?? throw new Exception("AntiClownBotFilesDirectory env variable was null");
        fileName = $"{filesDirectory}/StatisticsFiles/daily.json";
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
        var json = JsonConvert.SerializeObject(DailyStatistics, Formatting.Indented);
        File.WriteAllText(fileName, json);
    }

    private static bool TryRead(out DailyStatistics dict)
    {
        dict = null!;
        if (!File.Exists(fileName))
        {
            return false;
        }
        dict = JsonConvert.DeserializeObject<DailyStatistics>(File.ReadAllText(fileName))!;
        return dict != null;
    }
    
    public DateTime TodayDate { get; set; }
    public DailyStatistics DailyStatistics { get; set; }

    private static string fileName = "";
}