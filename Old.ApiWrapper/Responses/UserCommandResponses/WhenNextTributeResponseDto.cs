using System;

namespace ApiWrapper.Responses.UserCommandResponses
{
    public class WhenNextTributeResponseDto: BaseResponseDto
    {
        public DateTime NextTribute { get; set; }
    }
}