namespace AntiClownBotApi.DTO.Responses.MiniGames.BlackJack
{
    public class BlackJackLeaveResponseDto: BaseResponseDto
    {
        public EnumResponses.LeaveResult Result { get; set; }
        public EnumResponses.AfterLeaveAction AfterLeaveAction { get; set; }
    }
}