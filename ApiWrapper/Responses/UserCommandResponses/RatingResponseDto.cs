using System.Collections.Generic;
using ApiWrapper.Models.Items;

namespace ApiWrapper.Responses.UserCommandResponses
{
    public class RatingResponseDto: BaseResponseDto
    {
        public int ScamCoins { get; set; }
        public List<BaseItem> Inventory { get; set; }
    }
}