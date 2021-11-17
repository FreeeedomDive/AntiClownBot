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
        private UserRepository UserRepository { get; }
        
        public RatingCommand(UserRepository userRepository)
        {
            UserRepository = userRepository;
        }
        
        public BaseResponseDto Execute(BaseRequestDto dto)
        {
            var user = UserRepository.GetUserWithEconomyAndItems(dto.UserId);
            var response = new RatingResponseDto()
            {
                UserId = dto.UserId,
                ScamCoins = user.Economy.ScamCoins,
                NetWorth = user.Economy.ScamCoins + user.Items.Where(item => item.ItemType == ItemType.Positive)
                                           .Select(item => item.Price).Sum(),
                LootBoxes = user.Economy.LootBoxes,
                Inventory = user.Items.Where(item => item.IsActive).Select(BaseItem.FromDbItem).ToList()
            };
            return response;
        }
    }
}