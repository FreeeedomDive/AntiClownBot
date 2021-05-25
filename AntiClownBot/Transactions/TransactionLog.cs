using NLog;

namespace AntiClownBot.Transactions
{
    public static class TransactionLog
    {
        private static readonly Logger Logger = NLogWrapper.GetTransactionsLogger();
        
        public static void AddLog(SocialRatingUser user, string data)
        {
            Logger.Debug($"{user.DiscordUsername} --- {data}");
        }
    }
}