using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Bingo;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services.Bingo;
using AntiClown.Entertainment.Api.Dto.F1Predictions.Bingo;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.F1;

[Route("entertainmentApi/f1Bingo")]
public class F1BingoController(
    IF1BingoCardsService f1BingoCardsService,
    IF1BingoBoardsService f1BingoBoardsService,
    IMapper mapper
) : ControllerBase
{
    [HttpGet("cards")]
    public async Task<ActionResult<F1BingoCardDto[]>> ReadCards([FromQuery] int season)
    {
        var result = await f1BingoCardsService.FindAsync(season);
        return mapper.Map<F1BingoCardDto[]>(result);
    }

    [HttpPost("cards")]
    public async Task<ActionResult> CreateCard([FromBody] CreateF1BingoCardDto dto)
    {
        await f1BingoCardsService.CreateCardAsync(mapper.Map<CreateF1BingoCard>(dto));
        return NoContent();
    }

    [HttpPatch("cards/{cardId:guid}")]
    public async Task<ActionResult> UpdateCard([FromRoute] Guid cardId, [FromBody] F1BingoCardDto dto)
    {
        await f1BingoCardsService.UpdateCardAsync(cardId, mapper.Map<UpdateF1BingoCard>(dto));
        return NoContent();
    }

    [HttpGet("boards/{userId:guid}")]
    public async Task<ActionResult<Guid[]>> GetBoard([FromRoute] Guid userId, [FromQuery] int season)
    {
        return await f1BingoBoardsService.GetOrCreateBingoBoard(userId, season);
    }
}