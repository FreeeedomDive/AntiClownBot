using System.Collections.Generic;
using System.Linq;
using AntiClownBotApi.Commands;
using AntiClownBotApi.Commands.SocialRatingCommands;
using AntiClownBotApi.DTO.Requests;
using AntiClownBotApi.DTO.Responses;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;
using Microsoft.AspNetCore.Mvc;

namespace AntiClownBotApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UsersController : Controller
    {
        private readonly List<ICommand> _commands = new()
        {
            new TributeCommand(),
            new RatingCommand()
        };

        [HttpGet]
        public string Index()
        {
            return "index";
        }

        private TResponse ExecuteCommand<TCommand, TResponse>(BaseRequestDto requestDto)
            where TCommand : ICommand where TResponse : BaseResponseDto
            => _commands.OfType<TCommand>().First().Execute(requestDto) as TResponse;

        [HttpPost, Route("{id}/tribute")]
        public TributeResponseDto Tribute(ulong id) =>
            ExecuteCommand<TributeCommand, TributeResponseDto>(new BaseRequestDto(id));

        [HttpGet, Route("{id}/rating")]
        public RatingResponseDto Rating(ulong id) =>
            ExecuteCommand<RatingCommand, RatingResponseDto>(new BaseRequestDto(id));
        
        
    }
}