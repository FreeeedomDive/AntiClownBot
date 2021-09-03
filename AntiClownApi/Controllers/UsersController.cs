using System;
using System.Collections.Generic;
using System.Linq;
using AntiClownBotApi.Commands;
using AntiClownBotApi.Commands.UserCommands;
using AntiClownBotApi.Database.DBControllers;
using AntiClownBotApi.DTO.Requests;
using AntiClownBotApi.DTO.Responses;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;
using Microsoft.AspNetCore.Mvc;

namespace AntiClownBotApi.Controllers
{
    [ApiController]
    [Route("/api/users")]
    public class UsersController : Controller
    {
        private ShopRepository ShopRepository { get; }
        private UserRepository UserRepository { get; }
        private IEnumerable<ICommand> Commands { get; }

        public UsersController(
            ShopRepository shopRepository, 
            UserRepository userRepository, 
            IEnumerable<ICommand> commands
            )
        {
            ShopRepository = shopRepository;
            UserRepository = userRepository;
            Commands = commands;
        }
        
        private TResponse ExecuteCommand<TCommand, TResponse>(BaseRequestDto requestDto)
            where TCommand : ICommand where TResponse : BaseResponseDto
            => Commands.OfType<TCommand>().First().Execute(requestDto) as TResponse;

        [HttpGet("")]
        public AllUsersResponseDto AllUsers()
            => ExecuteCommand<AllUsersCommand, AllUsersResponseDto>(new BaseRequestDto());

        [HttpPost("{id}/tribute")]
        public TributeResponseDto Tribute(ulong id) =>
            ExecuteCommand<TributeCommand, TributeResponseDto>(new BaseRequestDto {UserId = id});

        [HttpGet("{id}/tribute/when")]
        public WhenNextTributeResponseDto WhenNextTribute(ulong id) =>
            ExecuteCommand<WhenCommand, WhenNextTributeResponseDto>(new BaseRequestDto {UserId = id});

        [HttpPost("removeCooldowns")]
        public BaseResponseDto RemoveCooldowns() =>
            ExecuteCommand<RemoveCooldownsCommand, BaseResponseDto>(new BaseRequestDto());

        [HttpGet("{userId}/items/{itemId}")]
        public ItemResponseDto GetUserItemById(ulong userId, Guid itemId)
        {
            var result = UserRepository.GetItemById(userId, itemId, out var item);
            return new ItemResponseDto()
            {
                UserId = userId, Result = result, Item = item
            };
        }

        [HttpGet("{id}/rating")]
        public RatingResponseDto Rating(ulong id) =>
            ExecuteCommand<RatingCommand, RatingResponseDto>(new BaseRequestDto {UserId = id});

        [HttpPost("changeBalance")]
        public ChangeUserBalanceResponseDto ChangeBalance(ChangeUserBalanceRequestDto dto) =>
            ExecuteCommand<ChangeUserBalanceCommand, ChangeUserBalanceResponseDto>(dto);

        [HttpPost("bulkChangeBalance")]
        public BulkChangeUserRatingResponseDto BulkChangeBalance(BulkChangeUserBalanceRequestDto dto) =>
            ExecuteCommand<BulkChangeUserBalanceCommand, BulkChangeUserRatingResponseDto>(dto);

        [HttpGet("mostRich")]
        public ulong GetRichestUser() => UserRepository.GetRichestUser();

        [HttpPost("dailyReset")]
        public void DailyReset()
        {
            ShopRepository.ResetFreeRevealsForAllUsers();
            ShopRepository.ResetReRollPriceForAllUsers();
        }
    }
}