using ApiWrapper.Models;

namespace ApiWrapper.Responses.UserCommandResponses
{
    public class OpenLootBoxResultDto: BaseResponseDto
    {
        public bool IsSuccessful { get; set; }
        public LootBoxReward Reward { get; set; }
    }
}