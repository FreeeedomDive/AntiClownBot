/* Generated file */
using RestSharp;
using Xdd.HttpHelpers.Models.Extensions;

namespace AntiClown.Entertainment.Api.Client.F1Predictions;

public class F1PredictionsClient : IF1PredictionsClient
{
    public F1PredictionsClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.F1Predictions.F1RaceDto[]> FindAsync(AntiClown.Entertainment.Api.Dto.F1Predictions.F1RaceFilterDto filter)
    {
        var request = new RestRequest("entertainmentApi/f1Predictions/find", Method.Post);
        request.AddJsonBody(filter);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.F1Predictions.F1RaceDto[]>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.F1Predictions.F1RaceDto> ReadAsync(System.Guid raceId)
    {
        var request = new RestRequest("entertainmentApi/f1Predictions/{raceId}", Method.Get);
        request.AddUrlSegment("raceId", raceId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.F1Predictions.F1RaceDto>();
    }

    public async System.Threading.Tasks.Task<System.Guid> StartNewRaceAsync(System.String name)
    {
        var request = new RestRequest("entertainmentApi/f1Predictions/", Method.Post);
        request.AddQueryParameter("name", name.ToString());
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<System.Guid>();
    }

    public async System.Threading.Tasks.Task AddPredictionAsync(System.Guid raceId, AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionDto prediction)
    {
        var request = new RestRequest("entertainmentApi/f1Predictions/{raceId}/addPrediction", Method.Post);
        request.AddUrlSegment("raceId", raceId);
        request.AddJsonBody(prediction);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task ClosePredictionsAsync(System.Guid raceId)
    {
        var request = new RestRequest("entertainmentApi/f1Predictions/{raceId}/close", Method.Post);
        request.AddUrlSegment("raceId", raceId);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task AddResultAsync(System.Guid raceId, AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionRaceResultDto raceResult)
    {
        var request = new RestRequest("entertainmentApi/f1Predictions/{raceId}/addResult", Method.Post);
        request.AddUrlSegment("raceId", raceId);
        request.AddJsonBody(raceResult);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task AddClassificationsResultAsync(System.Guid raceId, AntiClown.Entertainment.Api.Dto.F1Predictions.F1DriverDto[] f1Drivers)
    {
        var request = new RestRequest("entertainmentApi/f1Predictions/{raceId}/addClassification", Method.Post);
        request.AddUrlSegment("raceId", raceId);
        request.AddJsonBody(f1Drivers);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task AddDnfDriverAsync(System.Guid raceId, AntiClown.Entertainment.Api.Dto.F1Predictions.F1DriverDto dnfDriver)
    {
        var request = new RestRequest("entertainmentApi/f1Predictions/{raceId}/addDnf", Method.Post);
        request.AddUrlSegment("raceId", raceId);
        request.AddQueryParameter("dnfDriver", dnfDriver.ToString());
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task AddSafetyCarAsync(System.Guid raceId)
    {
        var request = new RestRequest("entertainmentApi/f1Predictions/{raceId}/addSafetyCar", Method.Post);
        request.AddUrlSegment("raceId", raceId);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task AddFirstPlaceLeadAsync(System.Guid raceId, System.Decimal firstPlaceLead)
    {
        var request = new RestRequest("entertainmentApi/f1Predictions/{raceId}/addFirstPlaceLead", Method.Post);
        request.AddUrlSegment("raceId", raceId);
        request.AddQueryParameter("firstPlaceLead", firstPlaceLead.ToString());
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionUserResultDto[]> FinishRaceAsync(System.Guid raceId)
    {
        var request = new RestRequest("entertainmentApi/f1Predictions/{raceId}/finish", Method.Post);
        request.AddUrlSegment("raceId", raceId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionUserResultDto[]>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionUserResultDto[]> ReadResultsAsync(System.Guid raceId)
    {
        var request = new RestRequest("entertainmentApi/f1Predictions/{raceId}/results", Method.Get);
        request.AddUrlSegment("raceId", raceId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionUserResultDto[]>();
    }

    public async System.Threading.Tasks.Task<Dictionary<System.Guid, AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionUserResultDto[]>> ReadStandingsAsync(System.Int32? season = null)
    {
        var request = new RestRequest("entertainmentApi/f1Predictions/standings", Method.Get);
        request.AddQueryParameter("season", season.ToString());
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<Dictionary<System.Guid, AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionUserResultDto[]>>();
    }

    private readonly RestSharp.RestClient restClient;
}
