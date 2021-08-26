using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBControllers;
using AntiClownBotApi.Database.DBModels;
using AntiClownBotApi.DTO.Requests;
using AntiClownBotApi.DTO.Responses;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;
using AntiClownBotApi.Models;
using AntiClownBotApi.Models.Items;
using AntiClownBotApi.Services;
using Hangfire;

namespace AntiClownBotApi.Commands.UserCommands
{
    public class TributeCommand : ICommand
    {
        public BaseResponseDto Execute(BaseRequestDto dto)
        {
            return TributeService.MakeTribute(dto.UserId, false);
        }
    }
}