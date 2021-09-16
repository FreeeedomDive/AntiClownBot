using System;
using AntiClownBotApi.DTO.Requests;
using AntiClownBotApi.DTO.Responses;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;

namespace AntiClownBotApi.Commands.UserCommands
{
    public class CoinsTransactionCommand : ICommand
    {
        private GlobalState GlobalState { get; }

        public CoinsTransactionCommand(GlobalState globalState)
        {
            GlobalState = globalState;
        }
        
        public BaseResponseDto Execute(BaseRequestDto dto)
        {
            if (dto is not CoinsTransactionRequestDto request)
            {
                return new CoinsTransactionResponseDto
                {
                    UserId = dto.UserId,
                    Result = CoinsTransactionResult.Error,
                };
            }

            var transactionId = Guid.NewGuid();
            GlobalState.ChangeUserBalance(request.SenderUserId, -request.Count, $"Финансовая транзакция {transactionId}");
            GlobalState.ChangeUserBalance(request.RecipientUserId, request.Count, $"Финансовая транзакция {transactionId}");
            return new CoinsTransactionResponseDto
            {
                UserId = dto.UserId,
                Result = CoinsTransactionResult.Success,
            };
        }
    }
}