using System.Net;
using AntiClown.F1Predictions.Client.Configuration;
using AntiClown.F1Predictions.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;

namespace AntiClown.F1Predictions.Client;

public sealed class F1PredictionsClient : IF1PredictionsClient, IDisposable
{
    public F1PredictionsClient(F1PredictionsConnectionOptions options)
    {
        if (!Uri.TryCreate(options.ServiceUrl, UriKind.Absolute, out var url)
            || string.IsNullOrWhiteSpace(options.ServiceKey) || options.LeagueId == Guid.Empty)
        {
            throw new InvalidOperationException("F1Predictions ServiceUrl, ServiceKey and LeagueId must be configured");
        }

        client = new RestClient(new RestClientOptions(url) { MaxTimeout = options.TimeoutMilliseconds });
        client.AddDefaultHeader("Authorization", $"Bearer {options.ServiceKey}");
        client.AddDefaultHeader("Accept", "application/json");
        resource = $"internal/v1/leagues/{options.LeagueId}";
    }

    public async Task<RaceDto[]> GetRacesAsync(Guid participantId, int season, CancellationToken cancellationToken = default) =>
        (await RequiredAsync<ListResponseDto<RaceDto>>(Request($"races?season={season}", Method.Get, participantId), cancellationToken)).Data;

    public async Task<UserRaceDto> GetRaceAsync(Guid participantId, Guid raceId, CancellationToken cancellationToken = default)
    {
        var details = await RequiredAsync<RaceDetailsDto>(Request($"races/{raceId}", Method.Get, participantId), cancellationToken);
        var prediction = await SendAsync<PredictionDto>(Request($"races/{raceId}/prediction", Method.Get, participantId), cancellationToken);
        var result = await SendAsync<RaceResultDetailsDto>(Request($"races/{raceId}/result", Method.Get, participantId), cancellationToken);
        return new UserRaceDto(details, prediction, result);
    }

    public Task<PredictionDto> SavePredictionAsync(Guid participantId, Guid raceId, SavePredictionRequestDto prediction,
        CancellationToken cancellationToken = default) =>
        RequiredAsync<PredictionDto>(Request($"races/{raceId}/prediction", Method.Put, participantId, prediction), cancellationToken);

    public Task<SeasonStandingsDto> GetStandingsAsync(int season, CancellationToken cancellationToken = default) =>
        RequiredAsync<SeasonStandingsDto>(Request($"standings?season={season}", Method.Get), cancellationToken);

    public Task<SeasonStatsDto> GetStatsAsync(Guid participantId, int season, CancellationToken cancellationToken = default) =>
        RequiredAsync<SeasonStatsDto>(Request($"stats?season={season}", Method.Get, participantId), cancellationToken);

    public async Task<TeamDto[]> GetTeamsAsync(Guid participantId, int season, CancellationToken cancellationToken = default) =>
        (await RequiredAsync<ListResponseDto<TeamDto>>(Request($"teams?season={season}", Method.Get, participantId), cancellationToken)).Data;

    public Task<ChampionshipDetailsDto> GetChampionshipAsync(Guid participantId, int season, CancellationToken cancellationToken = default) =>
        RequiredAsync<ChampionshipDetailsDto>(Request($"championship/{season}", Method.Get, participantId), cancellationToken);

    public Task<ChampionshipPredictionDto> SaveChampionshipPredictionAsync(Guid participantId, int season,
        ChampionshipStageDto stage, SaveChampionshipPredictionRequestDto prediction, CancellationToken cancellationToken = default) =>
        RequiredAsync<ChampionshipPredictionDto>(
            Request($"championship/{season}/predictions/{stage}", Method.Put, participantId, prediction), cancellationToken);

    public Task<BingoDetailsDto> GetOwnBingoAsync(Guid participantId, int season, CancellationToken cancellationToken = default) =>
        RequiredAsync<BingoDetailsDto>(Request($"bingo/boards/me?season={season}&lang=ru", Method.Get, participantId), cancellationToken);

    public Task<BingoDetailsDto> CreateOwnBingoAsync(Guid participantId, int season, CancellationToken cancellationToken = default) =>
        RequiredAsync<BingoDetailsDto>(Request($"bingo/boards/me?season={season}&lang=ru", Method.Put, participantId), cancellationToken);

    private RestRequest Request(string path, Method method, Guid? participantId = null, object? body = null)
    {
        var request = new RestRequest($"{resource}/{path}", method);
        if (participantId.HasValue)
        {
            request.AddHeader("F1P-ParticipantId", participantId.Value.ToString());
        }
        if (body is not null)
        {
            request.AddStringBody(JsonConvert.SerializeObject(body, JsonSettings), DataFormat.Json);
        }
        return request;
    }

    private async Task<T> RequiredAsync<T>(RestRequest request, CancellationToken cancellationToken) where T : class =>
        await SendAsync<T>(request, cancellationToken)
        ?? throw new F1PredictionsException(502, "Invalid F1Predictions response", "F1Predictions returned an empty response");

    private async Task<T?> SendAsync<T>(RestRequest request, CancellationToken cancellationToken) where T : class
    {
        var response = await client.ExecuteAsync(request, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        var status = (int)response.StatusCode;
        switch (status)
        {
            case 0:
                throw new F1PredictionsException(response.ResponseStatus == ResponseStatus.TimedOut ? 504 : 502,
                    "F1Predictions unavailable", "Не удалось получить ответ F1Predictions");
            case < 200 or >= 300:
            {
                var title = "F1Predictions request failed";
                var detail = $"F1Predictions returned HTTP {status}";
                try
                {
                    var problem = JObject.Parse(response.Content ?? "{}");
                    title = problem.Value<string>("title") ?? title;
                    detail = problem.Value<string>("detail") ?? detail;
                }
                catch (JsonException)
                {
                }
                throw new F1PredictionsException(status, title, detail);
            }
        }

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return null;
        }
        try
        {
            return JsonConvert.DeserializeObject<T>(response.Content ?? "", JsonSettings);
        }
        catch (JsonException)
        {
            throw new F1PredictionsException(502, "Invalid F1Predictions response", "F1Predictions returned invalid JSON");
        }
    }

    public void Dispose() => client.Dispose();

    private readonly RestClient client;
    private readonly string resource;
    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        Converters = { new StringEnumConverter() },
    };
}
