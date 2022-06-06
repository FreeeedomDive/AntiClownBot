using System;

namespace AntiClownApiClient.Dto.Responses.UserCommandResponses
{
    public class WhenNextTributeResponseDto: BaseResponseDto
    {
        public DateTime NextTribute { get; set; }
    }
}