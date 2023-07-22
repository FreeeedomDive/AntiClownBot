using AntiClown.Entertainment.Api.Client.Extensions;
using AntiClown.Entertainment.Api.Dto.F1Predictions;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client.F1Predictions;

public class F1PredictionsClient : IF1PredictionsClient
{
    public F1PredictionsClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<F1RaceDto> ReadAsync(Guid raceId)
    {
        var request = new RestRequest($"f1Predictions/{raceId}");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<F1RaceDto>();
    }

    public async Task<Guid> StartNewRaceAsync(string name)
    {
        var request = new RestRequest("f1Predictions");
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<Guid>();
    }

    public async Task AddPredictionAsync(Guid raceId, Guid userId, F1DriverDto tenthPlaceDriver, F1DriverDto firstDnfDriver)
    {
        var request = new RestRequest($"f1Predictions/{raceId}/addPrediction");
        request.AddJsonBody(
            new F1PredictionDto
            {
                RaceId = raceId,
                UserId = userId,
                TenthPlacePickedDriver = tenthPlaceDriver,
                FirstDnfPickedDriver = firstDnfDriver,
            }
        );
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task ClosePredictionsAsync(Guid raceId)
    {
        var request = new RestRequest($"f1Predictions/{raceId}/close");
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task AddFirstDnfResultAsync(Guid raceId, F1DriverDto firstDnfDriver)
    {
        var request = new RestRequest($"f1Predictions/{raceId}/addFirstDnf").AddQueryParameter("firstDnfDriver", firstDnfDriver);
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task AddClassificationsResultAsync(Guid raceId, F1DriverDto[] f1Drivers)
    {
        var request = new RestRequest($"f1Predictions/{raceId}/addClassification");
        request.AddJsonBody(f1Drivers);
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task<F1PredictionResultDto[]> FinishRaceAsync(Guid raceId)
    {
        var request = new RestRequest($"f1Predictions/{raceId}/finish");
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<F1PredictionResultDto[]>();
    }

    public async Task<Dictionary<Guid, F1PredictionResultDto[]>> ReadStandingsAsync()
    {
        var request = new RestRequest("f1Predictions/standings");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<Dictionary<Guid, F1PredictionResultDto[]>>();
    }

    private readonly RestClient restClient;
}