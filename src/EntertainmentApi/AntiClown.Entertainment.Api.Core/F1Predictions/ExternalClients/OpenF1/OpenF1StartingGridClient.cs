using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.ExternalClients.OpenF1;

public class OpenF1StartingGridClient(HttpClient httpClient, ILogger<OpenF1StartingGridClient> logger) : IStartingGridClient
{
    // Free OpenF1 access allows at most three requests per second. The grid lookup
    // needs four requests, so coordinate calls across client instances in this process.
    private static readonly SemaphoreSlim RequestGate = new(1, 1);
    private static readonly Queue<DateTimeOffset> RequestTimes = new();

    public async Task<string[]?> GetDriverNamesAsync(int season, int raceIndex, bool isSprint)
    {
        var sessions = await GetAsync<Session[]>($"/v1/sessions?year={season}&session_name=Race");
        var raceSession = sessions?.Where(x => !x.IsCancelled).OrderBy(x => x.DateStart).ElementAtOrDefault(raceIndex - 1);
        if (raceSession is null)
        {
            return null;
        }

        // OpenF1 associates the starting grid with qualifying, not with the race/sprint session.
        var qualifyingName = isSprint ? "Sprint%20Qualifying" : "Qualifying";
        var qualifyingSessions = await GetAsync<Session[]>(
            $"/v1/sessions?meeting_key={raceSession.MeetingKey}&session_name={qualifyingName}"
        );
        var sessionKey = qualifyingSessions?.FirstOrDefault(x => !x.IsCancelled)?.SessionKey;
        if (sessionKey is null)
        {
            return null;
        }

        var grid = await GetAsync<GridEntry[]>($"/v1/starting_grid?session_key={sessionKey}");
        if (grid is null || grid.Length == 0)
        {
            return null;
        }

        var drivers = await GetAsync<Driver[]>($"/v1/drivers?session_key={sessionKey}");
        if (drivers is null || drivers.Length == 0)
        {
            return null;
        }

        var namesByNumber = drivers
            .Where(x => !string.IsNullOrWhiteSpace(x.LastName))
            .GroupBy(x => x.DriverNumber)
            .ToDictionary(x => x.Key, x => x.First().LastName);
        if (grid.Any(x => !namesByNumber.ContainsKey(x.DriverNumber)))
        {
            logger.LogWarning("OpenF1 starting grid has drivers missing from session {SessionKey}", sessionKey);
            return null;
        }

        return grid.OrderBy(x => x.Position is > 0 ? x.Position : int.MaxValue)
                   .Select(x => namesByNumber[x.DriverNumber])
                   .ToArray();
    }

    private async Task<T?> GetAsync<T>(string url)
    {
        for (var attempt = 0; attempt < 2; attempt++)
        {
            await WaitForRequestSlotAsync();
            using var response = await httpClient.GetAsync(url);
            if ((int)response.StatusCode == 429 && attempt == 0)
            {
                var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(2);
                await Task.Delay(retryAfter > TimeSpan.Zero ? retryAfter : TimeSpan.FromSeconds(2));
                continue;
            }

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("OpenF1 request {Url} failed with {StatusCode}", url, response.StatusCode);
                return default;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        return default;
    }

    private static async Task WaitForRequestSlotAsync()
    {
        await RequestGate.WaitAsync();
        try
        {
            while (true)
            {
                var now = DateTimeOffset.UtcNow;
                while (RequestTimes.Count > 0 && now - RequestTimes.Peek() >= TimeSpan.FromSeconds(1))
                {
                    RequestTimes.Dequeue();
                }

                if (RequestTimes.Count < 3)
                {
                    RequestTimes.Enqueue(now);
                    return;
                }

                await Task.Delay(RequestTimes.Peek().AddSeconds(1) - now + TimeSpan.FromMilliseconds(50));
            }
        }
        finally
        {
            RequestGate.Release();
        }
    }

    private sealed record Session
    {
        [JsonProperty("session_key")]
        public int SessionKey { get; init; }

        [JsonProperty("meeting_key")]
        public int MeetingKey { get; init; }

        [JsonProperty("date_start")]
        public DateTimeOffset DateStart { get; init; }

        [JsonProperty("is_cancelled")]
        public bool IsCancelled { get; init; }
    }

    private sealed record GridEntry
    {
        [JsonProperty("driver_number")]
        public int DriverNumber { get; init; }

        [JsonProperty("position")]
        public int? Position { get; init; }
    }

    private sealed record Driver
    {
        [JsonProperty("driver_number")]
        public int DriverNumber { get; init; }

        [JsonProperty("last_name")]
        public string LastName { get; init; } = string.Empty;
    }
}
