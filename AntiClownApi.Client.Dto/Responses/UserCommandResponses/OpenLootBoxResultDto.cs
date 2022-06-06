using AntiClownApiClient.Dto.Models;
using AntiClownApiClient.Dto.Responses;

namespace AntiClownApiClient.Dto.Responses.UserCommandResponses
{
    public class OpenLootBoxResultDto: BaseResponseDto
    {
        public bool IsSuccessful { get; set; }
        public LootBoxReward Reward { get; set; }
    }
}