using AntiClown.Core.Dto.Extensions;
using AntiClown.Entertainment.Api.Dto.F1Predictions;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client.F1Predictions;

public class F1PredictionsClient : IF1PredictionsClient
{
    public F1PredictionsClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<F1RaceDto[]> ReadActiveAsync()
    {
        var request = new RestRequest("f1Predictions/active");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<F1RaceDto[]>();
    }

    public async Task<F1RaceDto> ReadAsync(Guid raceId)
    {
        var request = new RestRequest($"f1Predictions/{raceId}");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<F1RaceDto>();
    }

    public async Task<Guid> StartNewRaceAsync(string name)
    {
        var request = new RestRequest("f1Predictions").AddQueryParameter("name", name);
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<Guid>();
    }

    public async Task ClosePredictionsAsync(Guid raceId)
    {
        var request = new RestRequest($"f1Predictions/{raceId}/close");
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task AddPredictionAsync(Guid raceId, F1PredictionDto f1Prediction)
    {
        var request = new RestRequest($"f1Predictions/{raceId}/addPrediction").AddJsonBody(f1Prediction);
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task AddResultAsync(Guid raceId, F1PredictionRaceResultDto result)
    {
        var request = new RestRequest($"f1Predictions/{raceId}/addResult").AddJsonBody(result);
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task AddClassificationsResultAsync(Guid raceId, F1DriverDto[] f1Drivers)
    {
        var request = new RestRequest($"f1Predictions/{raceId}/addClassification").AddJsonBody(f1Drivers);
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task AddDnfDriverAsync(Guid raceId, F1DriverDto driver)
    {
        var request = new RestRequest($"f1Predictions/{raceId}/addDnf").AddQueryParameter("dnfDriver", driver);
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task AddSafetyCarAsync(Guid raceId)
    {
        var request = new RestRequest($"f1Predictions/{raceId}/addSafetyCar");
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task AddFirstPlaceLeadAsync(Guid raceId, decimal firstPlaceLead)
    {
        var request = new RestRequest($"f1Predictions/{raceId}/addFirstPlaceLead").AddQueryParameter("firstPlaceLead", firstPlaceLead);
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task<F1PredictionUserResultDto[]> FinishAsync(Guid raceId)
    {
        var request = new RestRequest($"f1Predictions/{raceId}/finish");
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<F1PredictionUserResultDto[]>();
    }

    public async Task<F1PredictionUserResultDto[]> ReadResultsAsync(Guid raceId)
    {
        var request = new RestRequest($"f1Predictions/{raceId}/results");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<F1PredictionUserResultDto[]>();
    }

    public async Task<Dictionary<Guid, F1PredictionUserResultDto?[]>> ReadStandingsAsync(int? season = null)
    {
        var request = new RestRequest("f1Predictions/standings");
        if (season.HasValue)
        {
            request.AddQueryParameter("season", season.Value);
        }
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<Dictionary<Guid, F1PredictionUserResultDto?[]>>();
    }

    private readonly RestClient restClient;
}