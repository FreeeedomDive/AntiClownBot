namespace AntiClownDiscordBotVersion2.Statistics.Emotes;

public interface IEmoteStatsService
{
    Task<string> GetStats();
    void AddStats(string emote);
    void RemoveStats(string emote);
}