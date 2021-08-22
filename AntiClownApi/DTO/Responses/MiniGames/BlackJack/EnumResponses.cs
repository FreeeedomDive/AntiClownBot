namespace AntiClownBotApi.DTO.Responses.MiniGames.BlackJack
{
    public static class EnumResponses
    {
        public enum JoinResult
        {
            Success,
            AlreadyInGame,
            GameHasAlreadyStarted,
            TooMuchPlayers
        }

        public enum LeaveResult
        {
            Success,
            AlreadyNotInGame
        }

        public enum AfterLeaveAction
        {
            NextPlayerShouldPlay,
            GameFinished
        }

        public enum StartResult
        {
            Success,
            AlreadyStarted,
            PlayerIsNotInGame
        }

        public enum AfterStartAction
        {
            Success,
            DealerIsAlone,
            AllUsersKicked
        }
    }
}