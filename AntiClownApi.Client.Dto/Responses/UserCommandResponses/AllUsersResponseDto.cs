using AntiClownApiClient.Dto.Responses;

namespace AntiClownApiClient.Dto.Responses.UserCommandResponses
{
    public class AllUsersResponseDto: BaseResponseDto
    {
        public List<ulong> Users { get; set; }
    }
}