using System.Collections.Generic;

namespace AntiClownBotApi.DTO.Responses.UserCommandResponses
{
    public class TributeResponseDto: BaseResponseDto
    {
        public int TributeQuality { get; set; }
        public bool IsTributeAutomatic { get; set; }
        public bool IsNextTributeAutomatic { get; set; }
        public Dictionary<string, int> CooldownModifiers { get; set; }
        public bool IsCommunismActive { get; set; }
        public ulong SharedCommunistUserId { get; set; }
    }
}