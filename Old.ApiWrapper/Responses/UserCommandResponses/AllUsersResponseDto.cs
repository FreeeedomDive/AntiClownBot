using System.Collections.Generic;

namespace ApiWrapper.Responses.UserCommandResponses
{
    public class AllUsersResponseDto: BaseResponseDto
    {
        public List<ulong> Users { get; set; }
    }
}