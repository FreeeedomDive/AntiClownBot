using AntiClown.DiscordBot.Ai.Dto;
using AntiClown.DiscordBot.Ai.Settings;
using Microsoft.Extensions.Options;
using RestSharp;

namespace AntiClown.DiscordBot.Ai.Client;

public class GeminiAiClient(IOptions<GeminiAiSettings> options, ILogger<GeminiAiClient> logger) : IAiClient
{
    public async Task<string> GetResponseAsync(string request)
    {
        var client = new RestClient("https://generativelanguage.googleapis.com");
        var restRequest = new RestRequest("v1beta/models/gemini-1.5-flash:generateContent");
        restRequest.AddQueryParameter("key", options.Value.ApiKey);
        restRequest.AddJsonBody(
            new
            {
                contents = new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = request,
                        },
                    },
                },
            }
        );

        var response = await client.ExecutePostAsync<AiResponse>(restRequest);
        if (response.Data?.Candidates is null || response.Data?.Candidates.Length == 0)
        {
            logger.LogError("Failed to get GeminiAi response\n{Data}", response);
            return string.Empty;
        }

        var aiResponse = response.Data.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ?? string.Empty;
        logger.LogInformation("AI request: {Request}\nResponse: {Response}", request, aiResponse);

        return aiResponse;
    }
}