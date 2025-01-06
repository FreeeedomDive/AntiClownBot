namespace AntiClown.DiscordBot.Ai.Client;

public interface IGeminiAiClient
{
    Task<string> GetResponseAsync(string request);
}