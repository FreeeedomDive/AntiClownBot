using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.Exceptions.F1Predictions;
using AntiClown.Entertainment.Api.Dto.F1Predictions;
using AntiClown.Web.Api.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Web.Api.Controllers;

[Route("webApi/f1Predictions")]
public class F1PredictionsController : Controller
{
    public F1PredictionsController(IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
    }

    [HttpGet("active")]
    public async Task<ActionResult<F1RaceDto[]>> ReadActiveAsync()
    {
        return await antiClownEntertainmentApiClient.F1Predictions.ReadActiveAsync();
    }

    [HttpPost("{raceId:guid}/addPrediction")]
    public async Task<ActionResult<AddPredictionResultDto>> AddPredictionAsync([FromRoute] Guid raceId, [FromBody] F1PredictionDto prediction)
    {
        try
        {
            await antiClownEntertainmentApiClient.F1Predictions.AddPredictionAsync(raceId, prediction);
            return AddPredictionResultDto.Success;
        }
        catch (PredictionsAlreadyClosedException)
        {
            return AddPredictionResultDto.PredictionsClosed;
        }
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}