using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using Newtonsoft.Json;

namespace AntiClownDiscordBotVersion2.Statistics.Emotes;

public class EmoteStatsService : IEmoteStatsService
{
    public EmoteStatsService(IDiscordClientWrapper discordClientWrapper)
    {
        this.discordClientWrapper = discordClientWrapper;
        emoteStatistics = TryRead(out var dict) ? dict : new Dictionary<string, int>();
    }
    
    public async Task<string> GetStats()
    {
        return await emoteStatistics.GetStats(async key => await discordClientWrapper.Emotes.FindEmoteAsync(key));
    }

    public void AddStats(string emote)
    {
        emoteStatistics.AddRecord(emote);
        Save();
    }

    public void RemoveStats(string emote)
    {
        emoteStatistics.RemoveRecord(emote);
        Save();
    }

    private void Save()
    {
        var json = JsonConvert.SerializeObject(emoteStatistics, Formatting.Indented);
        File.WriteAllText(FileName, json);
    }

    private static bool TryRead(out Dictionary<string, int> dict)
    {
        dict = null;
        if (!File.Exists(FileName))
        {
            return false;
        }
        dict = JsonConvert.DeserializeObject<Dictionary<string, int>>(File.ReadAllText(FileName));
        return dict != null;
    }

    private const string FileName = "StatisticsFiles/emotes.json";

    private readonly Dictionary<string, int> emoteStatistics;
    private readonly IDiscordClientWrapper discordClientWrapper;
}