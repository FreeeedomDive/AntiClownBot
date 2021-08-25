using System.Collections.Generic;

namespace AntiClownBotApi.DTO.Responses.MiniGames.BlackJack
{
    public class BlackJackStartResponseDto: BaseResponseDto
    {
        public EnumResponses.StartResult Result { get; set; }
        public EnumResponses.AfterStartAction Action { get; set; }
        public List<ulong> KickedPlayers { get; set; }
        public List<string> Cards { get; set; }
    }
}