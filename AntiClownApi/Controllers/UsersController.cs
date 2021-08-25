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
        private readonly List<ICommand> _commands = new()
        {
            new AllUsersCommand(),
            new TributeCommand(),
            new RatingCommand(),
            new ChangeUserBalanceCommand(),
            new BulkChangeUserBalanceCommand(),
            new WhenCommand()
        };

        private TResponse ExecuteCommand<TCommand, TResponse>(BaseRequestDto requestDto)
            where TCommand : ICommand where TResponse : BaseResponseDto
            => _commands.OfType<TCommand>().First().Execute(requestDto) as TResponse;

        [HttpGet, Route("")]
        public AllUsersResponseDto AllUsers()
            => ExecuteCommand<AllUsersCommand, AllUsersResponseDto>(new BaseRequestDto());

        [HttpPost, Route("{id}/tribute")]
        public TributeResponseDto Tribute(ulong id) =>
            ExecuteCommand<TributeCommand, TributeResponseDto>(new BaseRequestDto {UserId = id});

        [HttpGet, Route("{id}/tribute/when")]
        public WhenNextTributeResponseDto WhenNextTribute(ulong id) =>
            ExecuteCommand<WhenCommand, WhenNextTributeResponseDto>(new BaseRequestDto {UserId = id});

        [HttpPost, Route("removeCooldowns")]
        public BaseResponseDto RemoveCooldowns() =>
            ExecuteCommand<RemoveCooldownsCommand, BaseResponseDto>(new BaseRequestDto());

        [HttpGet, Route("{userId}/items/{itemId}")]
        public ItemResponseDto GetUserItemById(ulong userId, Guid itemId)
        {
            var result = UserDbController.GetItemById(userId, itemId, out var item);
            return new ItemResponseDto()
            {
                UserId = userId, Result = result, Item = item
            };
        }

        [HttpGet, Route("{id}/rating")]
        public RatingResponseDto Rating(ulong id) =>
            ExecuteCommand<RatingCommand, RatingResponseDto>(new BaseRequestDto {UserId = id});

        [HttpPost, Route("changeBalance")]
        public ChangeUserBalanceResponseDto ChangeBalance(ChangeUserBalanceRequestDto dto) =>
            ExecuteCommand<ChangeUserBalanceCommand, ChangeUserBalanceResponseDto>(dto);

        [HttpPost, Route("bulkChangeBalance")]
        public BulkChangeUserRatingResponseDto BulkChangeBalance(BulkChangeUserBalanceRequestDto dto) =>
            ExecuteCommand<BulkChangeUserBalanceCommand, BulkChangeUserRatingResponseDto>(dto);

        [HttpGet, Route("richest")]
        public ulong GetRichestUser() => UserDbController.GetRichestUser();

        [HttpPost, Route("dailyReset")]
        public void DailyReset()
        {
            ShopDbController.ResetFreeRevealsForAllUsers();
            ShopDbController.ResetReRollPriceForAllUsers();
        }
    }
}