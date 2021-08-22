using System.Linq;
using System.Reflection.Metadata;
using AntiClownBotApi.DTO.Responses.MiniGames.BlackJack;
using AntiClownBotApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AntiClownBotApi.Controllers.MiniGames
{
    /*[Controller, Route("api/minigames/blackjack/{userId:ulong}")]
    public class BlackJackController : Controller
    {
        [HttpPost, Route("double")]
        public string Double(ulong userId)
        {
            return "";
        }

        [HttpPost, Route("hit")]
        public string Hit(ulong userId)
        {
            return "";
        }

        [HttpPost, Route("join")]
        public EnumResponses.JoinResult Join(ulong userId)
        {
            if (GlobalState.CurrentBlackJack.Players.Any(player => player.UserId == userId))
            {
                return EnumResponses.JoinResult.AlreadyInGame;
            }

            if (GlobalState.CurrentBlackJack.IsActive)
            {
                return EnumResponses.JoinResult.GameHasAlreadyStarted;
            }

            return GlobalState.CurrentBlackJack.Players.Count > 4
                ? EnumResponses.JoinResult.TooMuchPlayers
                : GlobalState.CurrentBlackJack.Join(userId);
        }

        [HttpPost, Route("leave")]
        public BlackJackLeaveResponseDto Leave(ulong userId)
        {
            if (GlobalState.CurrentBlackJack.Players.All(player => player.UserId != userId))
            {
                return new BlackJackLeaveResponseDto()
                {
                    Result = EnumResponses.LeaveResult.AlreadyNotInGame
                };
            }

            var response = new BlackJackLeaveResponseDto();

            if (GlobalState.CurrentBlackJack.Players.First().UserId == userId)
            {
                GlobalState.CurrentBlackJack.StopTimer();
            }

            var result = GlobalState.CurrentBlackJack.Leave(userId);
            if (GlobalState.CurrentBlackJack.Players.First().IsDealer && GlobalState.CurrentBlackJack.IsActive)
            {
                GlobalState.CurrentBlackJack.StopTimer();
                // TODO make result stored somewhere
                // message += GlobalState.CurrentBlackJack.MakeResult();
                response.AfterLeaveAction = EnumResponses.AfterLeaveAction.GameFinished;
            }
            else if (GlobalState.CurrentBlackJack.IsActive)
            {
                // message += $"{GlobalState.CurrentBlackJack.Players.First().Name}, твоя очередь";
                response.AfterLeaveAction = EnumResponses.AfterLeaveAction.NextPlayerShouldPlay;
            }

            return response;
        }

        [HttpPost, Route("stand")]
        public string Stand(ulong userId)
        {
            return "";
        }

        [HttpPost, Route("start")]
        public BlackJackStartResponseDto Start(ulong userId)
        {
            if (GlobalState.CurrentBlackJack.IsActive)
            {
                return new BlackJackStartResponseDto() {Result = EnumResponses.StartResult.AlreadyStarted};
            }

            if (GlobalState.CurrentBlackJack.Players.All(player => player.UserId != userId))
            {
                return new BlackJackStartResponseDto() {Result = EnumResponses.StartResult.PlayerIsNotInGame};
            }

            var message = GlobalState.CurrentBlackJack.StartRound();
            if (GlobalState.CurrentBlackJack.IsActive)
                GlobalState.CurrentBlackJack.StartTimer();
            return new BlackJackStartResponseDto()
            {
                Result = EnumResponses.StartResult.Success
            };
        }
    }*/
}