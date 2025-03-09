using AntiClown.Entertainment.Api.Core.F1Predictions.Services.Bingo;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.F1;

[Route("entertainmentApi/f1Bingo")]
public class F1BingoController(IF1BingoCardsService f1BingoCardsService, IF1BingoBoardsService f1BingoBoardsService) : ControllerBase
{
    
}