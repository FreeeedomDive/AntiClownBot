using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.F1Predictions.Bingo;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Web.Api.Controllers;

[Route("webApi/f1Bingo")]
public class F1BingoController(IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient) : ControllerBase
{
    [HttpGet("cards")]
    public async Task<ActionResult<F1BingoCardDto[]>> ReadCards([FromQuery] int season)
    {
        return await antiClownEntertainmentApiClient.F1Bingo.ReadCardsAsync(season);
    }

    [HttpPatch("cards/{cardId:guid}")]
    public async Task<ActionResult> UpdateCard([FromRoute] Guid cardId, [FromBody] UpdateF1BingoCardDto dto)
    {
        await antiClownEntertainmentApiClient.F1Bingo.UpdateCardAsync(cardId, dto);
        return NoContent();
    }

    [HttpGet("boards/{userId:guid}")]
    public async Task<ActionResult<Guid[]>> GetBoard([FromRoute] Guid userId, [FromQuery] int season)
    {
        return await antiClownEntertainmentApiClient.F1Bingo.GetBoardAsync(userId, season);
    }
}