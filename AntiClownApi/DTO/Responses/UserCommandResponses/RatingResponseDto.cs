using System.Collections.Generic;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.DTO.Responses.UserCommandResponses
{
    public class RatingResponseDto: BaseResponseDto
    {
        public int ScamCoins { get; set; }
        
        public List<BaseItem> Inventory { get; set; }
    }
}