using System;

namespace AntiClownBotApi.DTO.Responses.UserCommandResponses
{
    public class WhenNextTributeResponseDto: BaseResponseDto
    {
        public DateTime NextTribute { get; set; }
    }
}