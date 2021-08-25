using System.Linq;
using AntiClownBotApi.Database.DBControllers;
using AntiClownBotApi.DTO.Requests;
using AntiClownBotApi.DTO.Responses;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.Commands.UserCommands
{
    public class RatingCommand: ICommand
    {
        public BaseResponseDto Execute(BaseRequestDto dto)
        {
            var user = UserDbController.GetUserWithEconomyAndItems(dto.UserId);
            var response = new RatingResponseDto()
            {
                UserId = dto.UserId,
                ScamCoins = user.Economy.ScamCoins,
                Inventory = user.Items.Select(BaseItem.FromDbItem).ToList()
            };
            return response;
        }
    }
}