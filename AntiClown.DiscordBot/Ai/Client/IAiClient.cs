namespace AntiClown.DiscordBot.Ai.Client;

public interface IAiClient
{
    Task<string> GetResponseAsync(string request);
}