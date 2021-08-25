using System.Collections.Generic;

namespace AntiClownBotApi.DTO.Responses.UserCommandResponses
{
    public class AllUsersResponseDto: BaseResponseDto
    {
        public List<ulong> Users { get; set; }
    }
}